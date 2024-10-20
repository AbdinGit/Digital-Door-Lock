using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Door_Lock.Models
{
    public class MainWindowModel
    {
        private bool _isDoorUnlocked;
        private readonly IoTHubService _iotHubService;

        public MainWindowModel(IoTHubService iotHubService)
        {
            _iotHubService = iotHubService;
            _isDoorUnlocked = false;
        }

        public bool IsDoorUnlocked
        {
            get => _isDoorUnlocked;
            private set => _isDoorUnlocked = value;
        }

        public async Task<string> GetStoredPinAsync()
        {
            var twin = await _iotHubService.GetDeviceTwinAsync();
            if (twin != null && twin.Properties.Desired.Contains("pinCode"))
            {
                return twin.Properties.Desired["pinCode"].ToString();
            }
            return null;
        }

        public async Task<string> GetDoorStateAsync()
        {
            return await _iotHubService.GetDoorStateAsync();
        }

        public async Task<bool> ValidatePinAsync(string enteredPin)
        {
            var storedPin = await GetStoredPinAsync();
            return enteredPin == storedPin;
        }

        public async Task LockDoorAsync()
        {
            IsDoorUnlocked = false;
            await _iotHubService.ReportDeviceStateAsync("Locked");
        }

        public async Task UnlockDoorAsync()
        {
            IsDoorUnlocked = true;
            await _iotHubService.ReportDeviceStateAsync("Unlocked"); 
        }
    }
}
