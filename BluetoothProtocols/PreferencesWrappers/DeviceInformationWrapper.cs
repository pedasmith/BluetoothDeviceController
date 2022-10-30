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
        public enum BleAdvertisementType { None, BluetoothLE, Eddystone, Govee, RuuviTag }
        public BleAdvertisementType AdvertisementType { get; set; } = BleAdvertisementType.None;
        /// <summary>
        /// When an advert is a scan response, we should include the original advert. Among other things,
        /// it's how we know that an scan response is a Govee response.
        /// </summary>
        public BluetoothLEAdvertisementReceivedEventArgs BleOriginalAdvert { get; set; } = null;
        public BluetoothLEAdvertisementReceivedEventArgs BleAdvert { get; set; } = null;
        public string EddystoneUrl { get; set; }

        public delegate void BluetoothLEAdvertisementEvent(BluetoothLEAdvertisementReceivedEventArgs data);
        public delegate void BluetoothLEAdvertisementWrapperEvent(BleAdvertisementWrapper data);
        public event BluetoothLEAdvertisementEvent UpdatedBleAdvertisement = null;
        // All wrappers use the same static Universal advertisement
        public static event BluetoothLEAdvertisementEvent UpdatedUniversalBleAdvertisement = null;

        public event BluetoothLEAdvertisementWrapperEvent UpdatedBleAdvertisementWrapper = null;
        public static event BluetoothLEAdvertisementWrapperEvent UpdatedUniversalBleAdvertisementWrapper = null;

        /// <summary>
        /// There's a matrix of events: the per-wrapper events which either take the wrapper or the raw advertisement,
        /// and the universal events which can take the wrapper or the raw advertisement.
        /// </summary>
        /// <param name="advertisementWrapper"></param>
        public void Event(BleAdvertisementWrapper advertisementWrapper)
        {
            if (UpdatedBleAdvertisement == null && UpdatedBleAdvertisementWrapper == null)
            {
                if (UpdatedUniversalBleAdvertisementWrapper != null)
                {
                    // Best universal (aka static) version uses the wrapper which has more data
                    UpdatedUniversalBleAdvertisementWrapper.Invoke(advertisementWrapper);
                }
                else
                {
                    UpdatedUniversalBleAdvertisement?.Invoke(advertisementWrapper.BleAdvert);
                }
            }
            else
            {
                if (UpdatedBleAdvertisementWrapper != null)
                {
                    UpdatedBleAdvertisementWrapper.Invoke(advertisementWrapper);
                }
                else
                {
                    UpdatedBleAdvertisement.Invoke(advertisementWrapper.BleAdvert);
                }
            }
        }
        public delegate void BluetoothEddystoneAdvertisementEvent(string data);
        public event BluetoothEddystoneAdvertisementEvent UpdatedEddystoneAdvertisement = null;
        public void Event(string results)
        {
            UpdatedEddystoneAdvertisement?.Invoke(results);
        }

        public delegate void RuuviAdvertisementEvent(Ruuvi_Tag_v1_Helper.Ruuvi_DataRecord data);
        public event RuuviAdvertisementEvent UpdatedRuuviAdvertisement = null;
        public void Event(Ruuvi_Tag_v1_Helper.Ruuvi_DataRecord results)
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
        public UserPreferences UserPreferences = null;
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
