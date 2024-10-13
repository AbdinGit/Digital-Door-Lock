using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Door_Lock.Models
{
    public class SettingsWindowModel
    {
        private string _primaryConnectionString;

        public SettingsWindowModel()
        {
            LoadConnectionString();
        }

        public string PrimaryConnectionString
        {
            get => _primaryConnectionString;
            set => _primaryConnectionString = value;
        }

        public void LoadConnectionString()
        {
            _primaryConnectionString = ConfigurationManager.AppSettings["PrimaryConnectionString"];
        }

        public async Task<bool> ValidateConnectionStringAsync(string connectionString)
        {
            DeviceClient deviceClient = null;

            try
            {
                deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
                await deviceClient.OpenAsync();  
                return true;  
            }
            catch
            {
                return false; 
            }
            finally
            {
                if (deviceClient != null)
                {
                    await deviceClient.CloseAsync();
                }
            }
        }
    }
}
