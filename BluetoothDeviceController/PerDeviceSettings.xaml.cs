using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
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

namespace BluetoothDeviceController
{
    public sealed partial class PerDeviceSettings : UserControl
    {
        public PerDeviceSettings(DeviceInformation di, string oldName)
        {
            this.InitializeComponent();
            this.Loaded += (s, e) =>
                {
                    uiName.Text = oldName;
                    uiDeviceName.Text = di.Name;
                    uiDeviceId.Text = di.Id;
                };
        }
        public string UserName {  get { return uiName.Text; } }
        public bool NameChanged { get; internal set; } = false;

        private void UiName_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            NameChanged = true;
        }
    }
}
