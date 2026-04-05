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
using System.Diagnostics.CodeAnalysis;
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

public interface IDeviceControl
{
    /// <summary>
    /// Returns the KnownDevice associated with the device
    /// </summary>
    /// <returns></returns>
    KnownDevice GetKnownDevice();

    /// <summary>
    /// updates the device control user interface base on the user preferences in SaveData.
    /// As of 2026-04-05 this includes just the name
    /// </summary>
    /// <param name="saveData"></param>
    void UpdateUX(SaveData saveData);

}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTNordic_ThingyControl : UserControl, IDeviceControl
{
    /// <summary>
    /// Used for logging only
    /// </summary>
    private string InternalDeviceType = "Nordic_Thingy";
    Nordic_Thingy Device;
    public BTNordic_ThingyControl()
    {
        InitializeComponent();
        this.DataContextChanged += BTNordic_ThingyControl_DataContextChanged;
    }

    KnownDevice KnownDeviceFromDataContext { get { return DataContext as KnownDevice; } }
    public KnownDevice GetKnownDevice()
    {
        return DataContext as KnownDevice;
    }

    /// <summary>
    /// This is a two-way street. Setting the DataContest to the KnownDevice will update some UX and will
    /// trigger looking up the SaveData and change more things. And it will actually connect to the device.
    /// AND this will update the KnownDevice with, e.g., the DeviceId and the BluetoothLEDevice which will be
    /// used by other bits of the system.
    /// </summary>
    private async void BTNordic_ThingyControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // FYI: by the time this method is called, the DataContext is already set

        if (args.NewValue == null) return; // just bogus; ignore.

        // Must have been set as a KnownDevice; otherwise we're in a very weird state.
        if (KnownDeviceFromDataContext == null)
        {
            Log($"Impossible Error: {InternalDeviceType}: Data context change, but it's not a KnownDevice. Type is {args.NewValue.GetType()}");
            return;
        }

        uiAddress.Text = BluetoothAddress.AsString(KnownDeviceFromDataContext.Advertisement.Addr);

        Device = new Nordic_Thingy();

        Device.ble = await BluetoothLEDevice.FromBluetoothAddressAsync(KnownDeviceFromDataContext.Advertisement.Addr);
        if (Device.ble == null)
        {
            Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(KnownDeviceFromDataContext.Advertisement.Addr)}");
            return;
        }
        // It's critical to set these!
        KnownDeviceFromDataContext.Id = Device.ble.DeviceId ?? ""; // never null :-)
        KnownDeviceFromDataContext.BTLEDevice = Device.ble;


        var saveData = AllSaveData.FindWithId(KnownDeviceFromDataContext.Id);
        if (saveData != null)
        {
            UpdateUX(saveData);
        }


        Device.PropertyChanged += Device_PropertyChanged;
        await Device.NotifyTemperature_cAsync();
        await Device.NotifyPressure_hpaAsync();
        await Device.NotifyHumidityAsync();
        await Device.NotifyTVOCAsync(); // both TVOC and eCOS
        await Device.NotifyColorAsync();
    }

    public void UpdateUX(SaveData saveData)
    {
        if (saveData != null)
        {
            var name = saveData.GetUserName();
            uiDeviceName.Text = name;
        }
    }

    private void Log(string str)
    {
        System.Diagnostics.Debug.WriteLine(str);
        Console.WriteLine(str);
    }

    private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        UIThreadHelper.CallOnUIThread(() =>
        {
            if (this.IsLoaded) // Won't be loaded when we exit the app!
            {
                UpdateUI(e.PropertyName);
            }
        });
    }

    private void UpdateUI(string name)
    {
        switch (name) // from e.PropertyName when the Device does a PropertyChanged.
        {
            case "Temperature_c":
                uiTemperature_c.Text = Device.Temperature_c.ToString("0.0") + " °C";
                break;
            case "Pressure_hpa":
                uiPressure_hpa.Text = Device.Pressure_hpa.ToString("0.0");
                break;
            case "Humidity":
                uiHumidity.Text = Device.Humidity.ToString("0.0") + "%";
                break;
            case "TVOC": // sets both TVOC and eCOS
                uieCOS.Text = Device.eCOS.ToString("0.0");
                uiTVOC.Text = Device.TVOC.ToString("0.0");
                break;
            case "Color":
                {
                    var RGB = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Device.Red, (byte)Device.Green, (byte)Device.Blue));
                    uiColor.Background = RGB; //TODO: and the 'clear' value?
                }
                break;
        }

    }
}
