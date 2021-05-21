using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothWatcher.DeviceDisplays
{
    public sealed partial class RuuviDisplay : UserControl
    {
        public RuuviDisplay()
        {
            this.InitializeComponent();
        }

        int NAdvertisement { get; set; } = 0;

        public void SetAdvertisement (Ruuvi_Tag ruuvi_tag)
        {
            NAdvertisement++;
            uiCount.Text = NAdvertisement.ToString();

            uiTemperature.Text = ruuvi_tag.TemperatureInDegreesC.ToString();
            uiPressure.Text = ruuvi_tag.PressureInPascals.ToString();
        }
    }
}
