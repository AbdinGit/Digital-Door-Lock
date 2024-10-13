using Microsoft.Azure.Devices.Shared;
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
using Microsoft.Azure.Devices.Client;
using System.Configuration;
using System.Xml;
namespace Digital_Door_Lock
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string PrimaryConnectionString = ConfigurationManager.AppSettings["PrimaryConnectionString"];
        private readonly IoTHubService _iotService;
        public SettingsWindow()
        {
            _iotService = new IoTHubService(PrimaryConnectionString);
            InitializeComponent();
            LoadConnectionString();


        }

        private void LoadConnectionString()
        {
            // Retrieve the connection string from the config file
            string PrimaryConnectionString = ConfigurationManager.AppSettings["PrimaryConnectionString"];

            // Display the connection string in the TextBox
            ConnectionStringTextBox.Text = PrimaryConnectionString;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ConnectionStringTextBox.Text);
        }

        private async void SetNewConnectionString(object sender, RoutedEventArgs e)
        {
            string connectionString = ConnectionStringTextBox.Text;
            DeviceClient deviceClient = null;

            try
            {
                deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
                await deviceClient.OpenAsync();  // Asynchronous connection
                MessageBox.Show("Connection successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Ensure the device client is closed if it was opened
                if (deviceClient != null)
                {
                    await deviceClient.CloseAsync();
                }
            }
        }

        private void CopyPrimaryConnectionString(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(PrimaryConnectionStringTextbox.Text);
        }

        private void CopySecondaryConnectionString(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(SecondaryConnectionStringTextbox.Text);
        }
    }
}