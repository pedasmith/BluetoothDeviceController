using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.Names;
using BluetoothProtocols;
using SampleServerXaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Thingy52Controller
{
    public sealed partial class Thingy52Control : UserControl
    {
        public Thingy52Control()
        {
            this.InitializeComponent();
            this.DataContextChanged += Thingy52Control_DataContextChanged; // Called when we get a Thingy52
        }

        private string BTAddress = "";
        private async void Thingy52Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext == null) return; // this happens at start-up.

            var addr = (ulong)this.DataContext;
            var ble = await BluetoothLEDevice.FromBluetoothAddressAsync(addr);
            BTAddress = BluetoothAddress.AsString(ble.BluetoothAddress);
            SetStatus($"Connecting to {BTAddress}");

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            bleDevice.Color_RGB_ClearEvent += BleDevice_Color_RGB_ClearEvent;
            bleDevice.HumidityEvent += BleDevice_HumidityEvent;
            bleDevice.Temperature_cEvent += BleDevice_Temperature_cEvent;
            bleDevice.Pressure_hpaEvent += BleDevice_Pressure_hpaEvent;
            bleDevice.Air_Quality_eCOS_TVOCEvent += BleDevice_Air_Quality_eCOS_TVOCEvent;


            var notifyStatus = await bleDevice.NotifyColor_RGB_ClearAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if (notifyStatus)
            {
                SetStatus($"Connected to {BTAddress}");
            }
            else
            {
                SetStatus($"Error: unable to connected to {BTAddress}");
            }
            await bleDevice.NotifyHumidityAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            await bleDevice.NotifyTemperature_cAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            await bleDevice.NotifyPressure_hpaAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            await bleDevice.NotifyAir_Quality_eCOS_TVOCAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
        }

        private void BleDevice_Air_Quality_eCOS_TVOCEvent(BluetoothDeviceController.BleEditor.ValueParserResult data)
        {
            UIThreadHelper.CallOnUIThread(() => {
                var str = $"eCOS={bleDevice.Air_Quality_eCOS_TVOC_eCOS} TV={bleDevice.Air_Quality_eCOS_TVOC_TVOC}";
                uiAirQuality.Text = str;
            });
        }

        private void BleDevice_Pressure_hpaEvent(BluetoothDeviceController.BleEditor.ValueParserResult data)
        {
            UIThreadHelper.CallOnUIThread(() => {
                var hpa = bleDevice.Pressure_hpa;
                var psi = UnitConvert.ConvertPressure(hpa, BtUnits.Barometer.hpa, BtUnits.Barometer.psi);
                var inHg = UnitConvert.ConvertPressure(hpa, BtUnits.Barometer.hpa, BtUnits.Barometer.inHg);
                var str = $"inHg={inHg:F3} hpa={hpa:F3} psi={psi:F2}";
                uiPressure.Text = str;
            });
        }

        private void BleDevice_Temperature_cEvent(BluetoothDeviceController.BleEditor.ValueParserResult data)
        {
            UIThreadHelper.CallOnUIThread(() => {
                var c = bleDevice.Temperature_c;
                var f = UnitConvert.ConvertTemperature(c, BtUnits.Temperature.celsius, BtUnits.Temperature.fahrenheit);
                var str = $"c={c:F2} f={f:F2}";
                uiTemperature.Text = str;
            });
        }

        private void BleDevice_HumidityEvent(BluetoothDeviceController.BleEditor.ValueParserResult data)
        {
            UIThreadHelper.CallOnUIThread(() => {
                uiHumidity.Text = bleDevice.Humidity.ToString("F1") + "%";
            });
        }

        private void BleDevice_Color_RGB_ClearEvent(BluetoothDeviceController.BleEditor.ValueParserResult data)
        {
            UIThreadHelper.CallOnUIThread(() => {
                var str = $"r={(int)bleDevice.Color_RGB_Clear_Red:X4} g={(int)bleDevice.Color_RGB_Clear_Green:X4} b={(int)bleDevice.Color_RGB_Clear_Blue:X4} clear={(int)bleDevice.Color_RGB_Clear_Clear:X4}";
                uiRGBClear.Text = str; 
            });
        }

        private void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            Log($"DBG: Connection Status={status.AsStatusString}");
            switch (status.Status)
            {
                case GattCommunicationStatus.Success:
                    // Success is assumed by the rest of the system; don't updated just because something didn't fail.
                    break;
                case GattCommunicationStatus.AccessDenied:
                    SetStatus($"Error: access denied for {BTAddress}");
                    break;
                case GattCommunicationStatus.ProtocolError:
                    SetStatus($"Error: protocol error for {BTAddress}");
                    break;
                case GattCommunicationStatus.Unreachable:
                    SetStatus($"Error: unreachable notifyStatus for {BTAddress}");
                    break;
                default:
                    SetStatus($"Error: other error for {BTAddress}");
                    break;
            }
        }

        Nordic_Thingy bleDevice = new Nordic_Thingy();


        private void Log(string str)
        {
            UIThreadHelper.CallOnUIThread(() => { uiLog.Text += str + "\n"; });
        }
        private void SetStatus(string str)
        {
            UIThreadHelper.CallOnUIThread(() => { uiStatus.Text = str; });
        }

        private async Task LogAdapterInfo()
        {
            Log("Log all adapters");
            var defaultAdapter = await BluetoothAdapter.GetDefaultAsync();

            var selector = BluetoothAdapter.GetDeviceSelector();
            var adapterList = await DeviceInformation.FindAllAsync(selector);
            foreach (var di in adapterList)
            {
                var adapter = await BluetoothAdapter.FromIdAsync(di.Id);
                var isDefault = defaultAdapter.BluetoothAddress == adapter.BluetoothAddress;
                var radio = await adapter.GetRadioAsync();
                var (diname, hasdiname) = GetDeviceInformationName(di);
                Log($"    Adapter: {diname} {adapter.DeviceId} default?={isDefault} address={BluetoothAddress.AsString(adapter.BluetoothAddress)}");
                Log($"        MaxAdvertisementDataLength={adapter.MaxAdvertisementDataLength}");
                Log($"        IsExtendedAdvertisingSupported={adapter.IsExtendedAdvertisingSupported}");
                Log($"        IsAdvertisementOffloadSupported={adapter.IsAdvertisementOffloadSupported}");
                Log($"        IsLowEnergySupported={adapter.IsLowEnergySupported}");
                Log($"        IsClassicSupported={adapter.IsClassicSupported}");
                Log($"        IsCentralRoleSupported={adapter.IsCentralRoleSupported}");
                Log($"        IsPeripheralRoleSupported={adapter.IsPeripheralRoleSupported}");
                Log($"        Radio Name={radio.Name}");
                Log($"        Radio State={radio.State}");
                Log($"        Radio Kind={radio.Kind}");
                Log("");
            }
        }

        /// <summary>
        /// Smart routine that returns the name of the device. Might return NULL if the device is an un-named one. 
        /// Uses the UserNameMapping as appropriate.
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        private static (string name, bool hasName) GetDeviceInformationName(DeviceInformation di)
        {
            // Preferred names, in order, are the mapping name, the navigateTo.Name and the navigateTo.Id.
            //var mapping = UserNameMappings.Get(di.Id);
            string name = null; //DBG: mapping?.Name ?? null;
            var hasName = true;
            if (String.IsNullOrEmpty(name)) name = di.Name;
            if (String.IsNullOrEmpty(name)) name = di.Id;

            if (name.StartsWith("BluetoothLE#BluetoothLE"))
            {
                hasName = false;
                name = "Address:" + GuidGetCommon.NiceId(name);
            }
            return (name, hasName);
        }
    }
}
