using BluetoothWatcher.Units;
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

namespace BluetoothWatcher.DeviceDisplays
{
    public class TemperatureUnitConverter : EnumValueConverter<Temperature.Unit> { }
    public class PressureUnitConverter : EnumValueConverter<Pressure.Unit> { }
    public sealed partial class RuuviSetting : UserControl
    {
        public RuuviSetting(UserUnits userUnits)
        {
            this.DataContext = userUnits;
            this.InitializeComponent();
        }
    }
}
