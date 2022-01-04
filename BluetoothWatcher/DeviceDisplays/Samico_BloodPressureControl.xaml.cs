using BluetoothProtocols;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothWatcher.DeviceDisplays
{
    /// <summary>
    /// Samico needs to connect to read data
    /// </summary>
    public sealed partial class Samico_BloodPressureControl : UserControl, INotifyPropertyChanged
    {
        public Samico_BloodPressureControl()
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

        private string _SystolicStr = "---";
        public string SystolicStr { get { return _SystolicStr; } set { if (_SystolicStr == value) return; _SystolicStr = value; NotifyPropertyChanged(); } }

        private string _DiastolicStr = "---";
        public string DiastolicStr { get { return _DiastolicStr; } set { if (_DiastolicStr == value) return; _DiastolicStr = value; NotifyPropertyChanged(); } }

        public void SetData(string macAddress, Samico_BloodPressure_BG512 bloodPressureCuff)
        {

            NAdvertisement++;
            uiCount.Text = NAdvertisement.ToString();
            uiName.Text = macAddress;

            PulseStr = $"{bloodPressureCuff.Results_PulseInBeatsPerMinute}";
            SystolicStr = $"{bloodPressureCuff.Results_SystolicInMMHg}%";
            DiastolicStr = $"{bloodPressureCuff.Results_DiastolicInMMHg}%";
        }
    }
}

