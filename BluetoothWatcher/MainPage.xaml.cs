using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothWatcher.DeviceDisplays;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        BluetoothLEAdvertisementWatcher BleAdvertisementWatcher = null;
        Dictionary<ulong, DeviceDisplays.RuuviDisplay> Displays = new Dictionary<ulong, DeviceDisplays.RuuviDisplay>();

        /// <summary>
        /// Fill in help text + start watcher
        /// </summary>

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/Help/StartupHelp.md"));
            var text = await FileIO.ReadTextAsync(file);
            uiStartupMarkdown.Text = text;

            // Start up the watcher
            BleAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();
            BleAdvertisementWatcher.Received += BleAdvertisementWatcher_Received;
            BleAdvertisementWatcher.Start();
        }

        private async void BleAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            //TODO: filter out non-Ruuvi advertisements :-)
            sbyte transmitPower = 0;
            string indent = "";
            ushort companyId;
            object speciality = null;

            // Now parse out the data
            foreach (var section in args.Advertisement.DataSections)
            {
                var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                string str = "";
                BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType = BluetoothCompanyIdentifier.CommonManufacturerType.Other;

                switch (dtv)
                {
                    case DataTypeValue.ManufacturerData:
                        (str, manufacturerType, companyId, speciality) = BluetoothCompanyIdentifier.ParseManufacturerData(section, transmitPower);
                        if (speciality != null)
                        {
                            ;
                        }
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.TxPowerLevel:
                        transmitPower = AdvertisementDataSectionParser.ParseTxPowerLevel(section);
                        break;
                }
            }
            Ruuvi_Tag ruuvi_tag = speciality as Ruuvi_Tag;
            if (ruuvi_tag == null) return;

           await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
           {
               var addr = args.BluetoothAddress;
               RuuviDisplay display = null;
               bool addedFirst = false;
               if (!Displays.ContainsKey(addr))
               {
                   // If ruuvi then ...
                   // We add the display in two places: a list of displays, and the map
                   display = new RuuviDisplay();
                   Displays.Add(addr, display);
                   addedFirst = Displays.Count == 1;
                   uiDevices.Items.Add(display);
               }
               else
               {
                   display = Displays[addr];
               }
               if (display == null) return;

               display.SetAdvertisement(ruuvi_tag);

               // Got a device; better make it visible.
               if (addedFirst)
               {
                   uiStartup.Visibility = Visibility.Collapsed;
                   uiDevices.Visibility = Visibility.Visible;
               }
               uiNDevice.Text = Displays.Count.ToString();
           });
        }
    }
}
