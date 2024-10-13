using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Configuration;
using Digital_Door_Lock.Views;
using Digital_Door_Lock.ViewModels;

namespace Digital_Door_Lock.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
        private void codeenter(object sender, RoutedEventArgs e)
        {
            if (Display.Text.Length < 4)
            {
                Button clickedButton = sender as Button;

                var viewModel = DataContext as MainWindowViewModel;
                if (viewModel != null)
                {
                    viewModel.TextBoxContent += clickedButton.Content.ToString();
                    Display.Text = viewModel.TextBoxContent;
                }
            }

        }


        private void Display_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

