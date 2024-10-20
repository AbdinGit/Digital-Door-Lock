using System;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Digital_Door_Lock.Models;
using Digital_Door_Lock.Views;

namespace Digital_Door_Lock.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string lockImageSource;
        private string _textBoxContent;
        private bool _isButtonEnabled = true;
        private string _currentPin;
        private MainWindowModel _MainWindowModel;
        private readonly IoTHubService _iotHubService;
        public MainWindowViewModel(IoTHubService iotHubService)
        {
            _MainWindowModel = new MainWindowModel(iotHubService);
            LockUnlockCommand = new RelayCommand(ExecuteLockUnlock);
            ClearDisplayCommand = new RelayCommand(ExecuteClearDisplay);
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);
            RemovePropertiesCommand = new RelayCommand(ExecuteRemoveProperties);
            DisplayPincode();
            LockImageSource = "../Resource/Images/Locked.png";
            _iotHubService = iotHubService;
            _ = ListenForDoorStateUpdatesAsync();
        }

        public string LockImageSource
        {
            get => lockImageSource;
            set
            {
                lockImageSource = value;
                OnPropertyChanged(nameof(LockImageSource));
            }
        }

        public string TextBoxContent
        {
            get => _textBoxContent;
            set
            {
                _textBoxContent = value;
                OnPropertyChanged(nameof(TextBoxContent));
            }
        }

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

        public string CurrentPin
        {
            get => _currentPin;
            set
            {
                _currentPin = value;
                OnPropertyChanged(nameof(CurrentPin));
            }
        }

        public ICommand LockUnlockCommand { get; }
        public ICommand ClearDisplayCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand RemovePropertiesCommand { get; }

        private async void DisplayPincode()
        {
            try
            {
                var pinCode = await _MainWindowModel.GetStoredPinAsync();
                CurrentPin = pinCode != null ? $"Pin code is: {pinCode}" : "No PIN found";
            }
            catch
            {
                CurrentPin = "Error retrieving PIN";
            }
        }

        private bool _isShaking = false;

        private async Task ListenForDoorStateUpdatesAsync()
        {
            while (true)
            {
                if (!_isShaking)
                {
                    var doorState = await _MainWindowModel.GetDoorStateAsync();

                    if (doorState == "Unlocked")
                    {
                        LockImageSource = "../Resource/Images/Unlocked.png";
                    }
                    else if (doorState == "Locked")
                    {
                        LockImageSource = "../Resource/Images/Locked.png";
                    }
                }
                await Task.Delay(2000);
            }
        }

        private async void ExecuteLockUnlock(object sender)
        {
            IsButtonEnabled = false;
            var doorState = await _MainWindowModel.GetDoorStateAsync();

            if (doorState == "Unlocked")
            {
                await _MainWindowModel.LockDoorAsync();
            }
            else
            {
                bool isPinCorrect = await _MainWindowModel.ValidatePinAsync(TextBoxContent);
                if (isPinCorrect)
                {
                    TextBoxContent = "";
                    await _MainWindowModel.UnlockDoorAsync();
                }
                else
                {
                    TextBoxContent = "";
                    await _MainWindowModel.LockDoorAsync();
                    await TriggerShakingEffectAsync();
                }
            }
            await Task.Delay(3000);
            IsButtonEnabled = true;
        }

        private async Task TriggerShakingEffectAsync()
        {
            _isShaking = true; 

            StartShakingEffect(); 

            await Task.Delay(2000); 

            _isShaking = false; 
        }

        private async void ExecuteRemoveProperties(object sender)
        {
            await _iotHubService.RemoveReportedPropertiesAsync("lockState", "pinCode");
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
                    LockImageSource = "../Resource/Images/locked.png";
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
