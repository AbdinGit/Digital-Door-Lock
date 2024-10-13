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
using Digital_Door_Lock.ViewModels;
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
            DataContext = new SettingsWindowViewModel();
        }

    }
}