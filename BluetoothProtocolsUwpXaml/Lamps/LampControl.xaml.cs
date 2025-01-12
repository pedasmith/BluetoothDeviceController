using BluetoothProtocolsDevices.BluetoothProtocolsCustom;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.Lamps
{
    public sealed partial class LampControl : UserControl
    {
        public Light Light { get; set; } // The abstract class
        public LampControl()
        {
            this.InitializeComponent();
        }

        private async void OnOnOffToggled(object sender, RoutedEventArgs e)
        {
            if (Light == null) return;
            await Light.TurnOnOffAsync((sender as ToggleSwitch).IsOn);
        }

        private async void OnBrightnessChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (Light == null) return;
            double value = (double)e.NewValue / 100.0;
            await Light.SetBrightnessAsync(value);
        }
        private async void OnWarmthChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (Light == null) return;
            double value = (double)e.NewValue / 100.0;
            await Light.SetWarmthAsync(value);
        }
    }
}
