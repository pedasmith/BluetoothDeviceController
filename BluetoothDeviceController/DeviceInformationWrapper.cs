using BluetoothDeviceController.Beacons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;

namespace BluetoothDeviceController
{
    public class BleAdvertisementWrapper
    {
        /// <summary>
        /// Which exact type of advertisement is this?
        /// </summary>
        public enum BleAdvertisementType { None, BluetoothLE, Eddystone, RuuviTag }
        public BleAdvertisementType AdvertisementType { get; set; } = BleAdvertisementType.None;
        public BluetoothLEAdvertisementReceivedEventArgs BleAdvert { get; set; } = null;
        public string EddystoneUrl { get; set; }

        public delegate void BluetoothLEAdvertisementEvent(BluetoothLEAdvertisementReceivedEventArgs data);
        public event BluetoothLEAdvertisementEvent UpdatedBleAdvertisement = null;
        // All wrappers use the same static Universal advertisement
        public static event BluetoothLEAdvertisementEvent UpdatedUniversalBleAdvertisement = null;
        public void Event(BluetoothLEAdvertisementReceivedEventArgs results)
        {
            if (UpdatedBleAdvertisement == null)
            {
                UpdatedUniversalBleAdvertisement?.Invoke(results);
            }
            else
            {
                UpdatedBleAdvertisement.Invoke(results);
            }
        }

        public delegate void BluetoothEddystoneAdvertisementEvent(string data);
        public event BluetoothEddystoneAdvertisementEvent UpdatedEddystoneAdvertisement = null;
        public void Event(string results)
        {
            UpdatedEddystoneAdvertisement?.Invoke(results);
        }

        public delegate void RuuviAdvertisementEvent(RuuviTag.Ruuvi_DataRecord data);
        public event RuuviAdvertisementEvent UpdatedRuuviAdvertisement = null;
        public void Event(RuuviTag.Ruuvi_DataRecord results)
        {
            UpdatedRuuviAdvertisement?.Invoke(results);
        }

        public override string ToString()
        {
            return BleAdvertisementFormat.AsDescription(BleAdvert);
        }


    }
    public class DeviceInformationWrapper
    {
        public DeviceInformationWrapper(DeviceInformation deviceInformation)
        {
            di = deviceInformation;
        }
        public DeviceInformationWrapper(BleAdvertisementWrapper bleAdvert)
        {
            BleAdvert = bleAdvert;
        }
        public DeviceInformation di { get; set; } = null;
        public UserSerialPortPreferences SerialPortPreferences = null;
        public UserBeaconPreferences BeaconPreferences = null;
        public BleAdvertisementWrapper BleAdvert = null;

        bool _IsNordicUart = false;
        bool _IsNordicUartChecked = false;
        public BluetoothProtocolsCustom.Nordic_Uart AsNordicUart { get; internal set; } = null;
        public async Task<bool> IsNordicUartAsync()
        {
            if (di == null) return false; // but don't update the IsNordicUartChecked
            if (_IsNordicUartChecked) return _IsNordicUart;
            _IsNordicUartChecked = true;
            AsNordicUart = new BluetoothProtocolsCustom.Nordic_Uart(di);
            var retval = await AsNordicUart.EnsureCharacteristicAsync();
            return retval;
        }
    }
}
