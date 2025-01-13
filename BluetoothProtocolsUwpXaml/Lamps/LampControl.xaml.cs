using BluetoothProtocols;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.Lamps
{
    public sealed partial class LampControl : UserControl
    {
        Light.Capability DeviceCapability = 0;
        Light _Light = null;
        public Light Light 
        { 
            get { return _Light; }
            set
            {
                _Light = value;
                DeviceCapability = Light.GetDeviceCapability();
                SetVisibility(uiColor, Light.Capability.SetColorRGB);
            }
        } // The abstract class

        private void SetVisibility(FrameworkElement fe, Light.Capability flag)
        {
            var vis = DeviceCapability.HasFlag(flag) ? Visibility.Visible : Visibility.Collapsed;
            fe.Visibility = vis;
        }

        public LampControl()
        {
            this.InitializeComponent();
        }

        private async void OnOnOffToggled(object sender, RoutedEventArgs e)
        {
            if (Light == null) return;
            if (!DeviceCapability.HasFlag(Light.Capability.OnOff)) return;
            await Light.TurnOnOffAsync((sender as ToggleSwitch).IsOn);
        }

        private async void OnBrightnessChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (Light == null) return;
            if (!DeviceCapability.HasFlag(Light.Capability.SetBrightness)) return;
            double value = (double)e.NewValue / 100.0;
            await Light.SetBrightnessAsync(value);
        }
        private async void OnWarmthChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (Light == null) return;
            if (!DeviceCapability.HasFlag(Light.Capability.SetWarm)) return;
            double value = (double)e.NewValue / 100.0;
            await Light.SetWarmthAsync(value);
        }

        private async void OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (Light == null) return;
            if (!DeviceCapability.HasFlag(Light.Capability.SetColorRGB)) return;

            byte r = args.NewColor.R;
            byte g = args.NewColor.G;
            byte b = args.NewColor.B;
            await Light.SetRGBAsync(r, g, b);
        }
    }
}
