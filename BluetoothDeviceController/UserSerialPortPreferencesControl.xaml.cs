using enumUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace BluetoothDeviceController
{
    public class TerminalLineEndConverter : EnumValueConverter<UserSerialPortPreferences.TerminalLineEnd> { }
    public sealed partial class UserSerialPortPreferencesControl : UserControl
    {
        public UserSerialPortPreferencesControl()
        {
            this.InitializeComponent();
        }
        public UserSerialPortPreferences SerialPortPreferences { get; set; }
        public void SetPreferences(UserSerialPortPreferences pref)
        {
            SerialPortPreferences = pref;
            this.DataContext = SerialPortPreferences;
        }

        private void OnLineEndChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
