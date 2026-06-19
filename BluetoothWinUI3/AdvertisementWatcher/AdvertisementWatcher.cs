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
using Microsoft.UI.Xaml.Controls;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWatcher.AdvertismentWatcher
{
    public class AdvertisementWatcher
    {
        public delegate void WatcherEventHandler(BluetoothLEAdvertisementWatcher sender, WatcherData e);
        public event WatcherEventHandler WatcherEvent;
        public double FilterRssiDb = -75; // filter out far away things because they are irritating.

        BluetoothLEAdvertisementWatcher BleAdvertisementWatcher = null;

        public void Start()
        {
            BleAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();
            BleAdvertisementWatcher.AllowExtendedAdvertisements = true;
            BleAdvertisementWatcher.ScanningMode = BluetoothLEScanningMode.Active; // Required for Govee H5074
            BleAdvertisementWatcher.Received += BleAdvertisementWatcher_Received;
            BleAdvertisementWatcher.Start();
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
            bool ignoreRssiDbFilter = false;
            WatcherData watcherData = null;
            bool havePreviousAdvert = false;
            WatcherData previousAdvert = null;
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
                havePreviousAdvert = OriginalAdvertisements.TryGetValue(args.BluetoothAddress, out previousAdvert);
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

                    case AdvertisementDataSectionParser.DataTypeValue.IncompleteListOf16BitServiceUuids: // 0x02
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteListOf16BitServiceUuids: // 0x03
                        {
                            var dr = DataReader.FromBuffer(section.Data);
                            dr.ByteOrder = ByteOrder.LittleEndian;
                            while (dr.UnconsumedBufferLength >= 2)
                            {
                                var addr0 = dr.ReadUInt16();
                                //var service = $"{addr7:X04}{addr6:X04}-{addr5:X04}-{addr4:X04}-{addr3:X04}-{addr2:X04}{addr1:X04}{addr0:X04}";
                                var service = BluetoothServiceUuid16Bit.Encode(addr0);
                                Guid guid;
                                var isguid = Guid.TryParse(service, out guid);
                                if (isguid)
                                {
                                    watcherData.ServiceUuids.Add(guid);
                                }
                            }
                        }
                        break;

                    case AdvertisementDataSectionParser.DataTypeValue.IncompleteListOf128BitServiceUuids: // 0x06
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteListOf128BitServiceUuids: // 0x07
                        {
                            var dr = DataReader.FromBuffer(section.Data);
                            dr.ByteOrder = ByteOrder.LittleEndian;
                            while (dr.UnconsumedBufferLength >= 16)
                            {
                                var addr0 = dr.ReadUInt16();
                                var addr1 = dr.ReadUInt16();
                                var addr2 = dr.ReadUInt16();
                                var addr3 = dr.ReadUInt16();
                                var addr4 = dr.ReadUInt16();
                                var addr5 = dr.ReadUInt16();
                                var addr6 = dr.ReadUInt16();
                                var addr7 = dr.ReadUInt16();
                                var service = $"{addr7:X04}{addr6:X04}-{addr5:X04}-{addr4:X04}-{addr3:X04}-{addr2:X04}{addr1:X04}{addr0:X04}";
                                Guid guid;
                                var isguid = Guid.TryParse(service, out guid);
                                if (isguid)
                                {
                                    watcherData.ServiceUuids.Add(guid);
                                }
                            }
                        }

                        break;

                }
            }
            if (string.IsNullOrEmpty(watcherData.BestName) && havePreviousAdvert)
            {
                watcherData.BestName = previousAdvert.BestName;
            }
            if (watcherData.CompanyId == 18498)
            {
                var n = args.Advertisement.LocalName;
                ignoreRssiDbFilter = true;
            }
            if (args.RawSignalStrengthInDBm < FilterRssiDb && !ignoreRssiDbFilter)
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