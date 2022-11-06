using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController
{
    public sealed partial class DeviceMenuEntryControl : UserControl
    {
        public delegate void DeviceSettingsHandler(object source, DeviceInformationWrapper wrapper);
        public event DeviceSettingsHandler SettingsClick;

        public DeviceInformationWrapper Wrapper;

        public DeviceMenuEntryControl(DeviceInformationWrapper wrapper, string name, Specialization specialization, string icon)
        {
            this.InitializeComponent();
            Wrapper = wrapper;
            Update(wrapper, name, specialization, icon);
        }
        int NUpdates = 0;
        public void Update(DeviceInformationWrapper wrapper, string name, Specialization specialization, string icon)
        {
            NUpdates++;
            uiNameBlock.Text = name;
            if (specialization == null)
            {
                uiIconBlock.Text = icon;
                if (NUpdates == 1)
                {
                    // Make it smaller and less appealing, but only the first time.
                    uiIconBlock.FontSize = uiIconBlock.FontSize * 0.7; 
                }
                // BluetoothLE and Bluetooth -- replace the useless bits about the device type
                string description = "??--??";
                if (wrapper.di != null)
                {
                    description = GuidGetCommon.NiceId(wrapper.di.Id, "Address:");
                    description = description.Replace(":00000000:{00001101-0000-1000-8000-00805f9b34fb}", "");
                }
                else if (wrapper.BleAdvert != null)
                {
                    var ble = wrapper.BleAdvert.BleAdvert;
                    var addr = BluetoothAddress.AsString(ble.BluetoothAddress);
                    var timestamp = ble.Timestamp.ToString("T");
                    description = $"{addr} RSS {ble.RawSignalStrengthInDBm} at {timestamp}";
                }
                uiDescriptionBlock.Text = description;
            }
            else
            {
                uiIconBlock.Text = specialization.Icon;
                if (string.IsNullOrEmpty(specialization.ShortDescription))
                {
                    ;
                }
                uiDescriptionBlock.Text = specialization.ShortDescription;
                uiNameBlock.FontWeight = FontWeights.Bold;

                ToolTipService.SetToolTip(uiNameBlock, specialization.Description);
                ToolTipService.SetToolTip(uiIconBlock, specialization.Description);
                ToolTipService.SetToolTip(uiDescriptionBlock, specialization.Description);
            }

            // Hide the settings button when we know it won't have any effect.
            var noUsefulSettings = Wrapper == null || Wrapper.di == null;
            uiSettings.Visibility = noUsefulSettings ? Visibility.Collapsed : Visibility.Visible;
        }

        public void UpdateName(string text)
        {
            uiNameBlock.Text = text;
        }
        public void UpdateDescription(string text)
        {
            uiDescriptionBlock.Text = text;
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsClick?.Invoke(this, Wrapper);
        }
    }
}
