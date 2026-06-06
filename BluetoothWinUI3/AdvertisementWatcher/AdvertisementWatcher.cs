using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
//using static BluetoothDeviceController.BluetoothDefinitionLanguage.AdvertisementDataSectionParser;

using BluetoothConversions;
using Utilities;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWatcher.AdvertismentWatcher
{
    public class AdvertisementWatcher
    {
        public delegate void WatcherEventHandler(BluetoothLEAdvertisementWatcher sender, WatcherData e);
        public event WatcherEventHandler WatcherEvent;

        BluetoothLEAdvertisementWatcher BleAdvertisementWatcher = null;

        public void Start()
        {
            BleAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();
            BleAdvertisementWatcher.AllowExtendedAdvertisements = true;
            BleAdvertisementWatcher.ScanningMode = BluetoothLEScanningMode.Active; // Required for Govee H5074
            BleAdvertisementWatcher.Received += BleAdvertisementWatcher_Received;
            BleAdvertisementWatcher.Start();

            // TODO: scanning mode?
        }
        public void Stop()
        {
            if (BleAdvertisementWatcher == null) return;
            BleAdvertisementWatcher.Received -= BleAdvertisementWatcher_Received;
            BleAdvertisementWatcher.Stop();
        }

        Dictionary<ulong, WatcherData> OriginalAdvertisements = new Dictionary<ulong, WatcherData>();

        private void BleAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            int filterLevel = -75;
            WatcherData watcherData = null;
            if (args.IsScanResponse)
            {
                OriginalAdvertisements.TryGetValue(args.BluetoothAddress, out watcherData);
                if (watcherData != null) 
                {
                    if (watcherData.ResponseAdvertisement != null)
                    {
                        Log($"DBG: got a second response advertisement!");
                    }
                    watcherData.ResponseAdvertisement = args;
                }
            }
            else
            {
                watcherData = new WatcherData()
                {
                    OriginalAdvertisement = args,
                    ResponseAdvertisement = null,
                };
                OriginalAdvertisements[args.BluetoothAddress] = watcherData;
            }
            if (watcherData == null) return; // got a rogue ScanResponse!


            // When a system goes to sleep and then wakes up, there's often a flood of old advertisements. Don't print them out;
            // it takes a lot of time for no benefit.
            var now = DateTimeOffset.UtcNow;
            var delta = now.Subtract(watcherData.MostRecentAdvertisement.Timestamp);
            var isOldAdvertisement = delta.TotalMinutes > 1;


            // Convert the raw args to specialized args.
            foreach (var section in args.Advertisement.DataSections)
            {
                var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                switch (dtv)
                {
                    case AdvertisementDataSectionParser.DataTypeValue.ShortenedLocalName:
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteLocalName:
                        {
                            var dr = DataReader.FromBuffer(section.Data);
                            var (str, result) = DataReaderReadStringRobust.ReadStringEntire(dr);
                            watcherData.BestName = str + (dtv == AdvertisementDataSectionParser.DataTypeValue.ShortenedLocalName ? " (shortened)" : "");
                        }
                        break;

                    case AdvertisementDataSectionParser.DataTypeValue.ManufacturerData:
                        var tx = watcherData.TransmitPower;
                        var rssi = watcherData.MostRecentAdvertisement.RawSignalStrengthInDBm;
                        var other = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
                        (watcherData.ParsedCompanyData, watcherData.ManufacturerType, watcherData.CompanyId, watcherData.SpecializedDecodedData) = BluetoothCompanyIdentifier.ParseManufacturerData(section, rssi, tx, other);
                        if (watcherData.CompanyId == 18498)
                        {
                            ; // Handy hook for debugging.
                        }
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.TxPowerLevel:
                        watcherData.TransmitPower = AdvertisementDataSectionParser.ParseTxPowerLevel(section);
                        break;
                }
            }
            if (watcherData.CompanyId == 18498)
            {
                var n = args.Advertisement.LocalName;
                filterLevel = -255; // Always let in the blood pressure cuff, because reasons.
            }
            if (args.RawSignalStrengthInDBm < filterLevel)
            {
                return;
            }

            var ignoreForPrinting = isOldAdvertisement || watcherData.ManufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.Apple10;
            if (!ignoreForPrinting)
            {
                // don't bother spiting out apple data; there's much too much of it
                System.Diagnostics.Debug.WriteLine($"Bluetooth Event: addr={BluetoothAddress.AsString(args.BluetoothAddress)} rx={args.RawSignalStrengthInDBm} tx={watcherData.TransmitPower}  txarg={args.TransmitPowerLevelInDBm} name={watcherData.BestName} company {watcherData.CompanyId}={BluetoothCompanyIdentifier.GetBluetoothCompanyIdentifier(watcherData.CompanyId)} data={watcherData.ParsedCompanyDataTrim}");
            }

            WatcherEvent?.Invoke(sender, watcherData); // Often the MainPage.BleWatcher_WatcherEvent
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }
    } // end of class AdvertisementWatcher
}