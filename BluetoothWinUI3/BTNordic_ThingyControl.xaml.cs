using BluetoothProtocols;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilities;
using Windows.Devices.Bluetooth;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif


public sealed partial class BTNordic_ThingyControl : UserControl
{
    Nordic_Thingy Device;
    public BTNordic_ThingyControl()
    {
        InitializeComponent();
        this.DataContextChanged += BTNordic_ThingyControl_DataContextChanged;
    }

    KnownDevice kd = null;
    private async void BTNordic_ThingyControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // Will be a KnownDevice
        kd = args.NewValue as KnownDevice;
        if (kd == null) return;

        uiAddress.Text = BluetoothAddress.AsString(kd.Advertisement.Addr);

        Device = new Nordic_Thingy();

        Device.ble = await BluetoothLEDevice.FromBluetoothAddressAsync(kd.Advertisement.Addr);
        Device.PropertyChanged += Device_PropertyChanged;
        await Device.NotifyTemperature_cAsync();
    }

    private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        UIThreadHelper.CallOnUIThread(() =>
        {
            UpdateUI(e.PropertyName);
        });
    }

    private void UpdateUI(string name)
    {
        switch (name) // from e.PropertyName when the Device does a PropertyChanged.
        {
            case "Temperature_c":
                uiTemperature_c.Text = Device.Temperature_c.ToString("0.0") + " °C";
                break;
        }

    }
}
