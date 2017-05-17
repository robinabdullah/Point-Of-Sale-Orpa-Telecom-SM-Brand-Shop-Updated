using System;
using System.Collections;
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

namespace Point_Of_Sale.PL
{
    /// <summary>
    /// Interaction logic for Gift_Add.xaml
    /// </summary>
    public partial class Gift_Add : Window
    {
        public static bool saveButtonPressed =false ;
        private ArrayList imeis;

        public bool isEditing { get; private set; }

        public Gift_Add()
        {
            InitializeComponent();

        }
        
        
        public Gift_Add(ArrayList imeis, bool isEditing) : this()
        {
            this.imeis = imeis;
            this.isEditing = isEditing;
            loadWindowComponentDG();
        }

        private void loadWindowComponentDG()
        {
            List<GiftCodeElements> tempGiftCodeElements = new List<GiftCodeElements>();
            GiftCodeElements dg = null;
            int windowSizeHeight = 154, gridSizeHeight = 68;

            if (isEditing == false)
                foreach (var item in imeis)
                {
                    dg = new GiftCodeElements(item.ToString(), "", "", 0);
                    tempGiftCodeElements.Add(dg);
                    gridSizeHeight = gridSizeHeight + 15;
                    windowSizeHeight = windowSizeHeight + 15;
                }
            else
                foreach (var item in Billing.giftCodeElements)
                {
                    dg = new GiftCodeElements(item.IMEI, item.SL, item.GiftCode, item.Discount);
                    tempGiftCodeElements.Add(dg);
                    gridSizeHeight = gridSizeHeight + 15;
                    windowSizeHeight = windowSizeHeight + 15;
                }

            dataGrid.ItemsSource = tempGiftCodeElements;
            dataGrid.Height = gridSizeHeight;
            Application.Current.MainWindow = this;
            Application.Current.MainWindow.Height = windowSizeHeight;
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            Billing.giftCodeElements = (List<GiftCodeElements>)dataGrid.ItemsSource;
            saveButtonPressed = true;
            this.Close();
            //foreach (var item in Billing.giftCodeElements)
            //   Console.WriteLine(item.IMEI + item.SL + item.GiftCode + item.Discount);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            saveButtonPressed = false;
            this.Close();
            imeis.Clear();
        }
        /*private void loadWindowComponentDG()
        {
            int windowSizeHeight = 154, gridSizeHeight = 63;

            for (int i = 1; i <= 6; i++)
            {
                Label label = new Label();
                label.Content = "359475075120692";
                

                TextBox sl = new TextBox();
                sl.Name = "sl" + i;
                

                TextBox giftCode = new TextBox();
                giftCode.Name = "gift" + i;
                
                TextBox discount = new TextBox();
                discount.Name = "discount" + i;

                DGElements dg = new DGElements(label, sl, giftCode, discount);
                dataGrid.Items.Add(dg);

                gridSizeHeight = gridSizeHeight + 30;
                windowSizeHeight = windowSizeHeight + 30;
            }
            dataGrid.Height = gridSizeHeight;
            Application.Current.MainWindow.Height = windowSizeHeight;
        }
        
        private class DGElements
        {
            Label imei { get; set; }
            TextBox sl{ get; set; }
            TextBox giftcode { get; set; }
            TextBox discount { get; set; }

            public DGElements(Label imei, TextBox sl, TextBox giftcode, TextBox discount)
            {
                this.imei = imei;
                this.sl = sl;
                this.giftcode = giftcode;
                this.discount = discount;
            }
        }
        */





        /*private void loadWindowComponent()
        {
            Label label = new Label();
            label.Content = "IMEIs";
            label.VerticalAlignment = VerticalAlignment.Top; ;
            label.Margin = new Thickness(50, 0, 0, 0);
            innerGrid.Children.Add(label);

            label = new Label();
            label.Content = "SL Number";
            label.VerticalAlignment = VerticalAlignment.Top; ;
            label.Margin = new Thickness(190, 0, 0, 0);
            innerGrid.Children.Add(label);

            label = new Label();
            label.Content = "Gift Code";
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Margin = new Thickness(365, 0, 0, 0);
            innerGrid.Children.Add(label);

            label = new Label();
            label.Content = "Offered Discount";
            label.Margin = new Thickness(520, 0, 0, 0);
            label.VerticalAlignment = VerticalAlignment.Top;
            innerGrid.Children.Add(label);

            int windowSizeHeight = 154, gridSizeHeight = 63;
            int left = 10, top = 28;
            for (int i = 1; i <= 6; i++)
            {
                label = new Label();
                label.Content = "359475075120692";
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                label.Margin = new Thickness(left, top, 0, 0);
                innerGrid.Children.Add(label);

                left += 130;

                TextBox textbox = new TextBox();
                textbox.Name = "sl" + i;
                textbox.Height = 26;
                textbox.Width = 160;
                textbox.FontSize = 16;
                textbox.HorizontalAlignment = HorizontalAlignment.Left;
                textbox.VerticalAlignment = VerticalAlignment.Top;
                textbox.Margin = new Thickness(left, top, 0, 0);
                innerGrid.Children.Add(textbox);

                left += 180;

                textbox = new TextBox();
                textbox.Name = "gift" + i;
                textbox.Height = 26;
                textbox.Width = 160;
                textbox.FontSize = 16;
                textbox.HorizontalAlignment = HorizontalAlignment.Left;
                textbox.VerticalAlignment = VerticalAlignment.Top;
                textbox.Margin = new Thickness(left, top, 0, 0);
                innerGrid.Children.Add(textbox);

                left += 180;

                textbox = new TextBox();
                textbox.Name = "discount" + i;
                textbox.Height = 26;
                textbox.Width = 150;
                textbox.FontSize = 16;
                textbox.HorizontalAlignment = HorizontalAlignment.Left;
                textbox.VerticalAlignment = VerticalAlignment.Top;
                textbox.Margin = new Thickness(left, top, 0, 0);
                innerGrid.Children.Add(textbox);

                left = 10;
                top += 35;
                
                gridSizeHeight = gridSizeHeight + 30;
                windowSizeHeight = windowSizeHeight + 30;
            }
            innerGrid.Height = gridSizeHeight;
            Application.Current.MainWindow.Height =  windowSizeHeight;
            Console.WriteLine("hello world");
            Console.WriteLine("{0} {1} {2}", innerGrid.Height, innerGrid.MinHeight, innerGrid.MaxHeight);
            Console.WriteLine("{0} {1} {2}", innerGrid.Height, Application.Current.MainWindow.Height, Application.Current.MainWindow.MinHeight);

        }
        */
        /*
        <Window x:Class="Point_Of_Sale.PL.Gift_Add"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:Point_Of_Sale.PL"
        mc:Ignorable="d"
        Title="Gift_Add" Height="154" Width="706.622" WindowStartupLocation="CenterScreen">
    <Grid x:Name="mainGrid">
        <Grid x:Name="innerGrid" HorizontalAlignment="Left" Height="63" Margin="10,10,0,0" VerticalAlignment="Top" Width="677" />
        <Button x:Name="button" Content="Save" HorizontalAlignment="Right" Margin="0,0,10,10" VerticalAlignment="Bottom" Width="75" Height="30"/>

    </Grid>
</Window>

             */
        /*
        private void loadWindowComponent()
        {
            controls controlss;
            double kendroPage = 1.0; // to get the value 1 1 2 2 3 3 for kendro pdf
            int left = 5, top = 2, right = 0, bottom = 0;
            for (int i = 0; i < Info.PdfInformationServiceArraylist.Count; i++)
            {
                Label label = new Label();
                label.Content = (i + 1) + ".";
                label.Margin = new Thickness(left, top - 3, right, bottom);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                innerGrid.Children.Add(label);

                left = 33;

                CheckBox checkBox = new CheckBox();
                checkBox.IsChecked = true;
                checkBox.HorizontalAlignment = HorizontalAlignment.Left;
                checkBox.VerticalAlignment = VerticalAlignment.Top;
                checkBox.Margin = new Thickness(left, top + 3, right, bottom);
                checkBox.IsTabStop = false;
                innerGrid.Children.Add(checkBox);

                left = 65;

                PdfInformationService pdf = (PdfInformationService)Info.PdfInformationServiceArraylist[i];
                TextBox textbox = new TextBox();
                textbox.HorizontalAlignment = HorizontalAlignment.Left;
                textbox.VerticalAlignment = VerticalAlignment.Top;
                textbox.Height = 20;
                textbox.Width = 600;
                textbox.Margin = new Thickness(left, top, right, bottom);
                textbox.TextWrapping = TextWrapping.Wrap;
                textbox.IsReadOnly = true;
                textbox.Text = pdf.FileName;
                textbox.IsTabStop = false;
                textbox.ToolTip = pdf.FilePath;
                textbox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5C5C9"));
                innerGrid.Children.Add(textbox);

                left = 675;

                TextBox textbox2 = new TextBox();
                textbox2.HorizontalAlignment = HorizontalAlignment.Left;
                textbox2.VerticalAlignment = VerticalAlignment.Top;
                textbox2.Height = 20;
                textbox2.Width = 50;
                textbox2.Margin = new Thickness(left, top, right, bottom);
                textbox2.TextWrapping = TextWrapping.Wrap;
                textbox2.Text = pdf.TotalVotersWithMigrate.ToString();
                textbox2.IsTabStop = false;
                textbox2.IsEnabled = false;
                textbox2.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5C5C9"));
                innerGrid.Children.Add(textbox2);

                left = 738;

                TextBox textBox3 = new TextBox();
                textBox3.HorizontalAlignment = HorizontalAlignment.Left;
                textBox3.VerticalAlignment = VerticalAlignment.Top;
                textBox3.Height = 20;
                textBox3.Width = 25;
                textBox3.Margin = new Thickness(left, top, right, bottom);
                textBox3.TextWrapping = TextWrapping.Wrap;
                textBox3.Text = Math.Floor(kendroPage).ToString();
                textBox3.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5C5C9"));
                //textBox3.TextChanged += textbox3_ValueChanged;
                textBox3.LostKeyboardFocus += textBox3_LostKeyboardFocus;
                textBox3.GotKeyboardFocus += TextBox3_GotKeyboardFocus;
                //textBox3.KeyDown += textbox3_KeyDown;
                innerGrid.Children.Add(textBox3);

                left = 775;
                kendroPage += 0.5; // to get the value series 1 1 2 2 3 3  for kendro pdf

                Button button = new Button();
                button.Content = "Center";
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Height = 20;
                button.Width = 53;
                //button.Visibility = Visibility.Hidden;
                button.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCCCC"));
                button.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF7F7F7"));
                button.IsTabStop = false;
                button.Margin = new Thickness(left, top, right, bottom);
                button.Click += center_button_clicked;
                innerGrid.Children.Add(button);

                left = 5;
                top += 30;

                controlss = new controls(label, checkBox, textbox, textbox2, textBox3, button);
                controlsArrayList.Add(controlss);
            }
            scrollViewer.Content = innerGrid;


            //innerGrid.Children.Add(button);
        }
        */


    }
}
