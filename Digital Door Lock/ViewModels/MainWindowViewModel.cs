using System;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Digital_Door_Lock.Views;

namespace Digital_Door_Lock.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private string lockImageSource;
        public string LockImageSource
        {
            get => lockImageSource;
            set
            {
                lockImageSource = value;
                OnPropertyChanged(nameof(LockImageSource));
            }
        }

        private string _TextBoxContent;
        public string TextBoxContent
        {
            get { return _TextBoxContent; }
            set
            {
                _TextBoxContent = value;
                OnPropertyChanged("TextBoxContent");
            }
        }


        private bool _isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set
            {
                if (_isButtonEnabled != value)
                {
                    _isButtonEnabled = value;
                    OnPropertyChanged(nameof(IsButtonEnabled));
                }
            }
        }

        private string _CurrentPin;
        public string CurrentPin
        {
            get { return _CurrentPin; }
            set
            {
                _CurrentPin = value;
                OnPropertyChanged("CurrentPin");
            }
        }

        private bool isDoorUnlocked = false;

        public ICommand LockUnlockCommand { get; }
        public ICommand ClearDisplayCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        private IoTHubService _iotHubService;
        public MainWindowViewModel(IoTHubService iotHubService)
        {
            _iotHubService = iotHubService;
            LockUnlockCommand = new RelayCommand(ExecuteLockUnlock);
            ClearDisplayCommand = new RelayCommand(ExecuteClearDisplay);
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);
            DisplayPincode();
            LockImageSource = "../Resource/Images/Locked.png"; 
        }
        private async void DisplayPincode()
        {
            try
            {

                var twin = await _iotHubService.GetDeviceTwinAsync();
                if (twin != null && twin.Properties.Desired.Contains("pinCode"))
                {
                    CurrentPin = $"Pin code is: {twin.Properties.Desired["pinCode"].ToString()}";
                }
                else
                {
                    CurrentPin = "No PIN found";
                }
            }
            catch (Exception ex)
            {
                CurrentPin = "Error retrieving PIN";
            }
        }
        private async void ExecuteLockUnlock(object sender)
        {
            IsButtonEnabled = false;
            var iotService = new IoTHubService(PrimaryConnectionString);

            if (isDoorUnlocked)
            {
                LockImageSource = "../Resource/Images/Locked.png";
                await iotService.ReportDeviceStateAsync("Locked");
                isDoorUnlocked = false;
                TextBoxContent = "door locked";
            }
            else
            {
                bool isPinCorrect = await iotService.ComparePinAsync(TextBoxContent);

                if (isPinCorrect)
                {
                    LockImageSource = "../Resource/Images/Unlocked.png";
                    await iotService.ReportDeviceStateAsync("Unlocked");
                    isDoorUnlocked = true;
                    TextBoxContent = "door unlocked";
                }
                else
                {
                    StartShakingEffect();
                    await iotService.ReportDeviceStateAsync("Locked");
                    TextBoxContent = string.Empty;
                    TextBoxContent = "wrong pincode";
                }
            }
            await Task.Delay(3000);
            IsButtonEnabled = true;
        }

        private void ExecuteClearDisplay(object sender)
        {
            TextBoxContent = string.Empty;
        }


        private void ExecuteOpenSettings(object sender)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private const string PrimaryConnectionString = "HostName=AbdinIOThub.azure-devices.net;DeviceId=cec95419-748a-423a-a9ce-bc9f6c9e6e9a;SharedAccessKey=SeFps8MV4pTBV/5hrcsW28Nlz7/j934q0IiFTBU9XZw=";

        private void StartShakingEffect()
        {
            DispatcherTimer shakeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            int shakeCount = 0;

            LockImageSource = "../Resource/Images/half locked.png";

            shakeTimer.Tick += (s, e) =>
            {
                if (shakeCount < 5)
                {
                    LockImageSource = (shakeCount % 2 == 0)
                        ? "../Resource/Images/locked.png"
                        : "../Resource/Images/half locked - no light.png";

                    shakeCount++;
                }
                else
                {
                    shakeTimer.Stop();
                    LockImageSource = "../Resource/Images/locked.png"; // Final image
                }
            };

            shakeTimer.Start();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
