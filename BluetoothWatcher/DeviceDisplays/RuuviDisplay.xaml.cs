using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothWatcher.Units;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static BluetoothDeviceController.BluetoothDefinitionLanguage.AdvertisementDataSectionParser;
using enumUtilities;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothWatcher.DeviceDisplays
{
    public sealed partial class RuuviDisplay : UserControl, INotifyPropertyChanged
    {
        public RuuviDisplay()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        private bool NameIsCustom = false;
        private string _NameStr = "ruuvi";
        public string NameStr { get { return _NameStr; } set { if (_NameStr == value) return; _NameStr = value; NotifyPropertyChanged(); } }

        private string _PressureStr = "---";
        public string PressureStr { get { return _PressureStr; } set { if (_PressureStr == value) return; _PressureStr = value; NotifyPropertyChanged(); } }

        private string _TemperatureStr = "---";
        public string TemperatureStr { get { return _TemperatureStr; } set { if (_TemperatureStr == value) return; _TemperatureStr = value; NotifyPropertyChanged(); } }

        private string _HumidityStr = "---";
        public string HumidityStr { get { return _HumidityStr; } set { if (_HumidityStr == value) return; _HumidityStr = value; NotifyPropertyChanged(); } }
        private string _XStr = "---";
        public string XStr { get { return _XStr; } set { if (_XStr == value) return; _XStr = value; NotifyPropertyChanged(); } }

        private string _YStr = "---";
        public string YStr { get { return _YStr; } set { if (_YStr == value) return; _YStr = value; NotifyPropertyChanged(); } }

        private string _ZStr = "---";
        public string ZStr { get { return _ZStr; } set { if (_ZStr == value) return; _ZStr = value; NotifyPropertyChanged(); } }

        private string _VoltageStr = "---";
        public string VoltageStr { get { return _VoltageStr; } set { if (_VoltageStr == value) return; _VoltageStr = value; NotifyPropertyChanged(); } }

        private string _TXStr = "---";
        public string TXStr { get { return _TXStr; } set { if (_TXStr == value) return; _TXStr = value; NotifyPropertyChanged(); } }

        private string _MovementSequenceStr = "---";
        public string MovementSequenceStr { get { return _MovementSequenceStr; } set { if (_MovementSequenceStr == value) return; _MovementSequenceStr = value; NotifyPropertyChanged(); } }

        private string _MovementCounterStr = "---";
        public string MovementCounterStr { get { return _MovementCounterStr; } set { if (_MovementCounterStr == value) return; _MovementCounterStr = value; NotifyPropertyChanged(); } }


        public UserUnits PreferredUnits { get; set; } = new UserUnits();

        int NAdvertisement { get; set; } = 0;
        Ruuvi_Tag LastTag = null;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetAdvertisement (string macAddress, Ruuvi_Tag ruuvi_tag)
        {
            LastTag = ruuvi_tag;

            NAdvertisement++;
            uiCount.Text = NAdvertisement.ToString();
            if (!NameIsCustom)
            {
                NameStr = macAddress;
            }
            UpdateFromTag(LastTag);
        }

        private void UpdateFromTag(Ruuvi_Tag ruuvi_tag)
        {
            if (ruuvi_tag == null) return;
            TemperatureStr = Units.Temperature.ConvertToString(ruuvi_tag.TemperatureInDegreesC, Temperature.Unit.Celcius, PreferredUnits.Temperature);
            PressureStr = Units.Pressure.ConvertToString(ruuvi_tag.PressureInPascals, Pressure.Unit.Pascal, PreferredUnits.Pressure);
            HumidityStr = $"{ruuvi_tag.HumidityInPercent}%";

            XStr = $"{ruuvi_tag.AccelerationInG[0]:F2} G";
            YStr = $"{ruuvi_tag.AccelerationInG[1]:F2} G";
            ZStr = $"{ruuvi_tag.AccelerationInG[2]:F2} G";

            VoltageStr = $"{ruuvi_tag.BatteryVoltage:F2} volts";
            TXStr = $"{ruuvi_tag.TransmitPowerInDb} db";

            MovementSequenceStr = $"{ruuvi_tag.MovementSequenceCounter}";
            MovementCounterStr = $"{ruuvi_tag.MovementCounter}";
        }

        private async void OnSetting(object sender, RoutedEventArgs e)
        {
            var settings = new RuuviSetting(PreferredUnits);
            var dlg = new ContentDialog()
            {
                Title = "Settings",
                PrimaryButtonText = "OK",
                Content = settings,
            };
            var result = await dlg.ShowAsync();
            UpdateFromTag(LastTag);
        }

        private async void OnPickName(object sender, TappedRoutedEventArgs e)
        {
            var newName = new TextBox()
            {
                MinWidth = 200,
                Text = NameStr,
            };
            var dlg = new ContentDialog()
            {
                Title = "Pick new name",
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
                Content = newName,
            };
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                NameIsCustom = true;
                NameStr = newName.Text;
            }
        }
    }
}
