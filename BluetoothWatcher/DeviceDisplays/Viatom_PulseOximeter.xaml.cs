using BluetoothDeviceController.BluetoothProtocolsCustom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothWatcher.DeviceDisplays
{
    /// <summary>
    /// Viatom needst to connect to read data
    /// </summary>
    public sealed partial class Viatom_PulseOximeter : UserControl, INotifyPropertyChanged
    {
        public Viatom_PulseOximeter()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        int NAdvertisement = 0;

        private string _PulseStr = "---";
        public string PulseStr { get { return _PulseStr; } set { if (_PulseStr == value) return; _PulseStr = value; NotifyPropertyChanged(); } }

        private string _OxygenStr = "---";
        public string OxygenStr { get { return _OxygenStr; } set { if (_OxygenStr == value) return; _OxygenStr = value; NotifyPropertyChanged(); } }
        
        private string _PerfusionIndexStr = "---";
        public string PerfusionIndexStr { get { return _PerfusionIndexStr; } set { if (_PerfusionIndexStr == value) return; _PerfusionIndexStr = value; NotifyPropertyChanged(); } }

        public void SetAdvertisement(string macAddress, Viatom_PulseOximeter_PC60FW oximeter)
        {

            NAdvertisement++;
            uiCount.Text = NAdvertisement.ToString();
            uiName.Text = macAddress;

            PulseStr = $"{oximeter.PulsePerMinute} bpm";
            OxygenStr = $"{oximeter.OxygenSaturationInPercent}%";
            PerfusionIndexStr = $"{oximeter.PerfusionIndexInPercent}%";
        }
    }
}
