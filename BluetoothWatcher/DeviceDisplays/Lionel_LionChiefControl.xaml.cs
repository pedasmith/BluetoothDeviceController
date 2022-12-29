using BluetoothDeviceController;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static BluetoothProtocols.Lionel_LionChief_Custom;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothWatcher.DeviceDisplays
{
    public interface IHandleStatus
    {
        void SetStatusText(string text);
        void SetStatusActive(bool isActive);
    }

    public interface ISetHandleStatus
    {
        void SetHandleStatus(IHandleStatus handleStatus);
    }
    public partial class Lionel_LionChiefControl : UserControl, INotifyPropertyChanged
    {
        public Lionel_LionChiefControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += Lionel_LionChiefControl_Loaded;
        }

        private void Lionel_LionChiefControl_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            //TODO: await DoReadDevice_Name();
        }

        public async Task SetBluetooth(ulong btAddr)
        {
            SetStatusActive(true);
            var device = new Lionel_LionChief_Custom();
            var ble = await BluetoothLEDevice.FromBluetoothAddressAsync(btAddr);
            device.ble = ble;
            SetStatusActive(false);
            bleDevice = device;

        }

        //private string DeviceName = "Lionel_LionChief";
        //private string DeviceNameUser = "LC-0-1-0494-B69B";
        int ncommand = 0;
        Lionel_LionChief_Custom bleDevice = new Lionel_LionChief_Custom();


        private IHandleStatus ParentStatusHandler = null;

        public void SetHandleStatus(IHandleStatus handleStatus)
        {
            ParentStatusHandler = handleStatus;
        }
        private void SetStatus(string status)
        {
            uiStatus.Text = status;
            ParentStatusHandler?.SetStatusText(status);
        }
        private void SetStatusActive(bool isActive)
        {
            uiProgress.IsActive = isActive;
            ParentStatusHandler?.SetStatusActive(isActive);
        }


        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(Lionel_LionChief.CharacteristicsEnum.All_enum, true);
            SetStatusActive(false);
        }

        private async void OnWriteLionelBell(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                var tb = sender as ToggleButton;
                bool turnOn = tb.IsChecked.Value;
                await bleDevice.WriteLionelBell(turnOn);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }
        private async void OnWriteLionelHorn(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                var tb = sender as ToggleButton;
                bool turnOn = tb.IsChecked.Value;
                await bleDevice.WriteLionelHorn(turnOn);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }
        private async void OnWriteLionelLights(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                var tb = sender as ToggleButton;
                bool turnOn = tb.IsChecked.Value;
                await bleDevice.WriteLionelLights(turnOn);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteLionelSpeed(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                byte speed = (byte)((sender as Slider).Value);
                await bleDevice.WriteLionelSpeed(speed);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteLionelDisconnect(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                await bleDevice.WriteLionelDisconnect();
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteLionelSteamVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                byte volume = (byte)(uiVolumeSlider.Value);
                await bleDevice.WriteLionelOverallVolume(volume);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteLionelBellPitch(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                sbyte pitch = (sbyte)((sender as Slider).Value);
                byte volume = (byte)(uiVolumeSlider.Value);
                await bleDevice.WriteLionelVolumePitch(SoundSource.Bell, volume, pitch);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteLionelHornPitch(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                sbyte pitch = (sbyte)((sender as Slider).Value);
                byte volume = (byte)(uiVolumeSlider.Value);
                await bleDevice.WriteLionelVolumePitch(SoundSource.Horn, volume, pitch);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private SpeakMessage _CurrSpeakMessage = SpeakMessage.Random_Message;
        public SpeakMessage CurrSpeakMessage
        {
            get { return _CurrSpeakMessage; }
            set
            {
                if (value == _CurrSpeakMessage) return;
                _CurrSpeakMessage = value;
                OnPropertyChanged();
            }
        }
        private async void OnWriteLionelSpeak(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                await bleDevice.WriteLionelSpeak(CurrSpeakMessage);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnDirectionToggled(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                bool isForward = (sender as ToggleSwitch).IsOn;
                await bleDevice.WriteLionelDirection(isForward);

                // When the direction changes, also update the speed to zero.
                // This is what the train actually does; the UI should reflect it.
                uiSpeed.Value = 0;
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

    }
}
