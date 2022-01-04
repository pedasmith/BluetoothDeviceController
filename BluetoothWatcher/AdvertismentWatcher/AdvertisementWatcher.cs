using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothDeviceController.BluetoothDefinitionLanguage.AdvertisementDataSectionParser;

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
            BleAdvertisementWatcher.Received += BleAdvertisementWatcher_Received;
            BleAdvertisementWatcher.Start();
        }

        private void BleAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            int filterLevel = -75;


            var watcherData = new WatcherData()
            {
                OriginalArgs = args,
            };


            // Convert the raw args to specialized args.
            foreach (var section in args.Advertisement.DataSections)
            {
                var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                switch (dtv)
                {
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteLocalName:
                        {
                            var dr = DataReader.FromBuffer(section.Data);
                            watcherData.CompleteLocalName = dr.ReadString(dr.UnconsumedBufferLength);
                        }
                        break;

                    case DataTypeValue.ManufacturerData:
                        (watcherData.ParsedCompanyData, watcherData.ManufacturerType, watcherData.CompanyId, watcherData.SpecializedDecodedData) = BluetoothCompanyIdentifier.ParseManufacturerData(section, watcherData.TransmitPower);
                        if (watcherData.CompanyId == 18498)
                        {
                            ;
                            (watcherData.ParsedCompanyData, watcherData.ManufacturerType, watcherData.CompanyId, watcherData.SpecializedDecodedData) = BluetoothCompanyIdentifier.ParseManufacturerData(section, watcherData.TransmitPower);
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

            if (watcherData.ManufacturerType != BluetoothCompanyIdentifier.CommonManufacturerType.Apple10)
            {
                // don't bother spiting out apple data; there's much too much of it
                System.Diagnostics.Debug.WriteLine($"Bluetooth Event: addr={BluetoothAddress.AsString(args.BluetoothAddress)} rx={args.RawSignalStrengthInDBm} tx={watcherData.TransmitPower}  txarg={args.TransmitPowerLevelInDBm} name={watcherData.CompleteLocalName} company {watcherData.CompanyId}={BluetoothCompanyIdentifier.GetBluetoothCompanyIdentifier(watcherData.CompanyId)} data={watcherData.ParsedCompanyDataTrim}");
            }

            WatcherEvent?.Invoke(sender, watcherData); // Often the MainPage.BleWatcher_WatcherEvent
        }


    }
}