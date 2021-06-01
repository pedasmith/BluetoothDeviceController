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
            const int filterLevel = -75;
            if (args.RawSignalStrengthInDBm < filterLevel)
            {
                return;
            }

            var wdata = new WatcherData()
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
                            wdata.CompleteLocalName = dr.ReadString(dr.UnconsumedBufferLength);
                        }
                        break;

                    case DataTypeValue.ManufacturerData:
                        (wdata.ParsedCompanyData, wdata.ManufacturerType, wdata.CompanyId, wdata.SpecializedDecodedData) = BluetoothCompanyIdentifier.ParseManufacturerData(section, wdata.TransmitPower);
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.TxPowerLevel:
                        wdata.TransmitPower = AdvertisementDataSectionParser.ParseTxPowerLevel(section);
                        break;
                }
            }

            if (wdata.ManufacturerType != BluetoothCompanyIdentifier.CommonManufacturerType.Apple10)
            {
                // don't bother spiting out apple data; there's much too much of it
                System.Diagnostics.Debug.WriteLine($"Bluetooth Event: rx={args.RawSignalStrengthInDBm} tx={wdata.TransmitPower}  txarg={args.TransmitPowerLevelInDBm} name={wdata.CompleteLocalName} company {wdata.CompanyId}={BluetoothCompanyIdentifier.GetBluetoothCompanyIdentifier(wdata.CompanyId)} data={wdata.ParsedCompanyDataTrim}");
            }

            WatcherEvent?.Invoke(sender, wdata);
        }


    }
}