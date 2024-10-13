using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Digital_Door_Lock.Models;

namespace Digital_Door_Lock.ViewModels
{
    public class SettingsWindowViewModel
    {
        private readonly SettingsWindowModel _settingsWindowModel;
        private string _connectionString;

        public SettingsWindowViewModel()
        {
            _settingsWindowModel = new SettingsWindowModel();
            ConnectionString = _settingsWindowModel.PrimaryConnectionString;

            CopyConnectionStringCommand = new RelayCommand(CopyConnectionString);
            SetNewConnectionStringCommand = new RelayCommand(async param => await SetNewConnectionString());
        }

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                OnPropertyChanged(nameof(ConnectionString));
            }
        }


        public ICommand CopyConnectionStringCommand { get; }
        public ICommand SetNewConnectionStringCommand { get; }

        private void CopyConnectionString(object parameter)
        {
            Clipboard.SetText(ConnectionString);
        }

        private async Task SetNewConnectionString()
        {
            bool isValid = await _settingsWindowModel.ValidateConnectionStringAsync(ConnectionString);
            if (isValid)
            {
                MessageBox.Show("Connection successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Connection failed. Please check your connection string.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
