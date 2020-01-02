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
        public delegate void DeviceSettingsHandler(object source, DeviceInformationWrapper di);
        public event DeviceSettingsHandler SettingsClick;

        const string OTHER = "🜹";
        public DeviceInformationWrapper DI;

        public DeviceMenuEntryControl(DeviceInformationWrapper di, string name, Specialization specialization)
        {
            this.InitializeComponent();
            DI = di;

            uiNameBlock.Text = name;
            if (specialization == null)
            {
                uiIconBlock.Text = OTHER;
                uiIconBlock.FontSize = uiIconBlock.FontSize * 0.7; // make it smaller and less eye-catching
                uiDescriptionBlock.Text = di.di.Id.Replace("BluetoothLE#BluetoothLEbc:83:85:22:5a:70-", "Address:");
            }
            else
            {
                uiIconBlock.Text = specialization.Icon;
                uiDescriptionBlock.Text = specialization.ShortDescription;
                uiNameBlock.FontWeight = FontWeights.Bold;

                ToolTipService.SetToolTip(uiNameBlock, specialization.Description);
                ToolTipService.SetToolTip(uiIconBlock, specialization.Description);
                ToolTipService.SetToolTip(uiDescriptionBlock, specialization.Description);
            }
        }

        public void UpdateName(string name)
        {
            uiNameBlock.Text = name;
        }

        private void OnSettingsClick(object sender, RoutedEventArgs e)
        {
            SettingsClick?.Invoke(this, DI);
        }
    }
}
