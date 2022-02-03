using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothDeviceController.BluetoothProtocolsCustom;
using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWatcher.DeviceDisplays;
using BluetoothWatcher.Units;
using enumUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static BluetoothDeviceController.BluetoothDefinitionLanguage.AdvertisementDataSectionParser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }
        // // // BluetoothLEAdvertisementWatcher BleAdvertisementWatcher = null;
        AdvertisementWatcher BleWatcher = new AdvertisementWatcher();
        Dictionary<ulong, DeviceDisplays.RuuviDisplay> RuuviDisplays = new Dictionary<ulong, DeviceDisplays.RuuviDisplay>();
        Dictionary<ulong, DeviceDisplays.Viatom_PulseOximeter> ViatomDisplays = new Dictionary<ulong, Viatom_PulseOximeter>();
        Dictionary<ulong, DeviceDisplays.Samico_BloodPressureControl> SamicoDisplays = new Dictionary<ulong, Samico_BloodPressureControl>();
        Dictionary<ulong, DeviceDisplays.Lionel_LionChiefControl> LionelDisplays = new Dictionary<ulong, Lionel_LionChiefControl>();

        Dictionary<ulong, Viatom_PulseOximeter_PC60FW_Factory> ViatomFactories = new Dictionary<ulong, Viatom_PulseOximeter_PC60FW_Factory>();
        Dictionary<ushort, ulong> CharacteristicHandleToBTAddr = new Dictionary<ushort, ulong>();

        /// <summary>
        /// Fill in help text + start watcher
        /// </summary>

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/Help/StartupHelp.md"));
            var text = await FileIO.ReadTextAsync(file);
            uiStartupMarkdown.Text = text;

            // Start up the watcher
            BleWatcher.WatcherEvent += BleWatcher_WatcherEvent;
            BleWatcher.Start();


            // Unit tests!
            int nerror = 0;
            nerror += DoubleApprox.Test();
            nerror += Temperature.Test();
            nerror += Pressure.Test();
            nerror += BufferList.BufferList.Test();
            if (nerror > 0)
            {
                uiNDevice.Text = $"STARTUP ERROR: {nerror}";
            }
        }

        private async Task<bool> StartLionel(WatcherData wdata)
        {
            Lionel_LionChiefControl display = null;
            if (!LionelDisplays.ContainsKey(wdata.Addr))
            {
                display = new Lionel_LionChiefControl();
                LionelDisplays[wdata.Addr] = display;
                AddDisplay(display);

                await display.SetBluetooth(wdata.Addr);
            }
            else
            {
                display = LionelDisplays[wdata.Addr];
            }

            return false;
        }

        private async Task<bool> StartSamico(WatcherData wdata)
        {
            Samico_BloodPressureControl display = null;
            if (!SamicoDisplays.ContainsKey(wdata.Addr))
            {
                display = new Samico_BloodPressureControl();
                SamicoDisplays[wdata.Addr] = display;
            }
            else
            {
                display = SamicoDisplays[wdata.Addr];
            }
            var device = new Samico_BloodPressure_BG512();
            var ble = await BluetoothLEDevice.FromBluetoothAddressAsync(wdata.Addr);
            device.ble = ble;
            device.ResultsEvent += (data) =>
            {
                var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var addr = BluetoothAddress.AsString(ble.BluetoothAddress);
                    display.SetData(addr, device);
                });
            };
            await device.NotifyResultsAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            return false;
        }
        private async Task<bool> StartViatom(WatcherData wdata)
        {
            //TODO: this prevents us from restarting a pulse.
            // e.g., use a pulse oximeter, then stop for a bit, then start again.

            bool mustStart = !ViatomFactories.ContainsKey(wdata.Addr);
            var ble = await BluetoothLEDevice.FromBluetoothAddressAsync(wdata.Addr);
            if (ble.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                mustStart = true;
            }

            if (mustStart)
            {
                // Turn on notifications.
                var xmitterResult = await ble.GetGattServicesForUuidAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                if (xmitterResult.Status != GattCommunicationStatus.Success) return false;
                if (xmitterResult.Services.Count < 1)
                {
                    ;
                    return false;
                }
                var receiveResult = await xmitterResult.Services[0].GetCharacteristicsForUuidAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
                if (receiveResult.Status != GattCommunicationStatus.Success) return false;
                if (receiveResult.Characteristics.Count < 1)
                {
                    ;
                    return false;
                }
                var receive = receiveResult.Characteristics[0];
                var notifyStatus = await receive.WriteClientCharacteristicConfigurationDescriptorAsync(Windows.Devices.Bluetooth.GenericAttributeProfile.GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (notifyStatus != GattCommunicationStatus.Success) return false;

                if (!CharacteristicHandleToBTAddr.ContainsKey(receive.AttributeHandle))
                {
                    CharacteristicHandleToBTAddr.Add(receive.AttributeHandle, wdata.Addr);
                }
                var added = ViatomFactories.TryAdd(wdata.Addr, new Viatom_PulseOximeter_PC60FW_Factory());
                receive.ValueChanged += Viatom_PulseOximeter_PC60FW_Receive_ValueChanged;
                return true;
            }
            return false;
        }
        private async void BleWatcher_WatcherEvent(BluetoothLEAdvertisementWatcher sender, WatcherData wdata)
        {
            Ruuvi_Tag ruuvi_tag = null;
            bool added_Viatom_PulseOximeter = false;

            var addr = BluetoothAddress.AsString(wdata.Addr);
            if (addr.StartsWith ("78:a5"))
            {
                ;
            }

            if (wdata.CompleteLocalName.StartsWith("PC-60F"))
            {
                added_Viatom_PulseOximeter = await StartViatom(wdata);
            }
            else if (wdata.CompleteLocalName.StartsWith("Samico BP"))
            {
                bool did_Samico = await StartSamico(wdata);
            }
            else if (wdata.CompleteLocalName.StartsWith("LC-"))
            {
                bool did_Lionel = await StartLionel(wdata);
            }
            else if (wdata.SpecializedDecodedData is Ruuvi_Tag)
            {
                // Maybe it's a Ruuvi Tag announcement?
                ruuvi_tag = wdata.SpecializedDecodedData as Ruuvi_Tag;
                if (ruuvi_tag == null)
                {
                    // Maybe it's a type 1 ruuvi tag?
                    var v1 = Ruuvi_Tag_v1_Helper.GetRuuviUrl(wdata.OriginalArgs);
                    if (v1.Success && v1.Data != null)
                    {
                        ruuvi_tag = Ruuvi_Tag.FromRuuvi_DataRecord(v1.Data);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Device seen: {wdata.CompleteLocalName}");
            }

            if (ruuvi_tag == null && !added_Viatom_PulseOximeter)
            {
                return;
            }

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (ruuvi_tag != null) UpdateRuuviDisplay(wdata.Addr, ruuvi_tag);
                if (added_Viatom_PulseOximeter)
                {
                    if (!ViatomDisplays.ContainsKey(wdata.Addr))
                    {
                        var display = new Viatom_PulseOximeter();
                        ViatomDisplays.Add(wdata.Addr, display);
                        AddDisplay(display);
                    }
                }
            });
        }

        private void AddDisplay(Control display)
        {
            uiDevices.Items.Add(display);

            // Got a device; better make it visible.
            if (uiDevices.Items.Count == 1)
            {
                uiStartup.Visibility = Visibility.Collapsed;
                uiDevices.Visibility = Visibility.Visible;
            }
            uiNDevice.Text = uiDevices.Items.Count.ToString();
        }


        private void UpdateRuuviDisplay(ulong addr, Ruuvi_Tag ruuvi_tag)
        {
            RuuviDisplay display = null;
            if (!RuuviDisplays.ContainsKey(addr))
            {
                // If ruuvi then ...
                // We add the display in two places: a list of displays, and the map
                display = new RuuviDisplay();
                RuuviDisplays.Add(addr, display);
                AddDisplay(display);
            }
            else
            {
                display = RuuviDisplays[addr];
            }
            if (display == null) return;
            var macAddress = BluetoothAddress.AsString(addr);
            display.SetAdvertisement(macAddress, ruuvi_tag);
        }

        /// <summary>
        /// Called when a pulse oximeter is updated.
        /// </summary>
        private void Viatom_PulseOximeter_PC60FW_Receive_ValueChanged(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic sender, Windows.Devices.Bluetooth.GenericAttributeProfile.GattValueChangedEventArgs args)
        {
            var addr = CharacteristicHandleToBTAddr[sender.AttributeHandle];
            var factory = ViatomFactories[addr];
            factory.AddNotification(sender, args);
            var value = factory.GetNext();
            if (value != null)
            {
               var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
               {
                   if (ViatomDisplays.ContainsKey(addr))
                   {
                       var display = ViatomDisplays[addr];
                       display.SetAdvertisement("", value);
                   }
               });
            }
        }
    }
}
