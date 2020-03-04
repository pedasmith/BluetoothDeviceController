using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.Beacons
{
    public static class BleAdvertisementFormat
    {

        /// <summary>
        /// Warning: in addition to its normal function, this function ALSO triggers events. Yes, it's gross, and yes, I'm sorry. This only happens when wrapper is non-null
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="ble"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string CustomizeWrapperFromAdvertisement(DeviceInformationWrapper wrapper, BluetoothLEAdvertisementReceivedEventArgs ble, string name)
        {
            // Lets's see if it's an Eddystone beacon...
            // https://github.com/google/eddystone
            // https://github.com/google/eddystone/blob/master/protocol-specification.md

            foreach (var section in ble.Advertisement.DataSections)
            {
                switch (section.DataType)
                {
                    case 0x16: // 22=service data
                        var dr = DataReader.FromBuffer(section.Data);
                        dr.ByteOrder = ByteOrder.LittleEndian;
                        var Service = dr.ReadUInt16();
                        // https://github.com/google/eddystone
                        if (Service == 0xFEAA) // An Eddystone type
                        {
                            //EddystoneFrameType = (byte)(0x0F & (dr.ReadByte() >> 4));
                            var EddystoneFrameType = dr.ReadByte();
                            const int TypeUID = 0x00;
                            const int TypeURL = 0x10;
                            const int TypeTLM = 0x20;
                            const int TypeEID = 0x40;
                            switch (EddystoneFrameType)
                            {
                                case TypeUID:
                                    wrapper?.BleAdvert.Event("UID: <data>");
                                    break;
                                case TypeTLM:
                                    wrapper?.BleAdvert.Event("TLM: <data>");
                                    break;
                                case TypeEID:
                                    wrapper?.BleAdvert.Event("EID: <data>");
                                    break;
                                case TypeURL: // 0x10: An Eddystone-URL
                                    // https://github.com/google/eddystone/tree/master/eddystone-url
                                    var result = BluetoothDeviceController.Beacons.Eddystone.ParseEddystoneUrlArgs(section.Data);
                                    name = result.Success ? result.Url : "Invalid eddystone!";
                                    if (wrapper != null) wrapper.BleAdvert.EddystoneUrl = name;
                                    if (result.Success && result.Url.StartsWith("https://ruu.vi/#"))
                                    {
                                        //foundValues.Add(AdvertisementType.RuuviTag);
                                        var ruuvi = BluetoothDeviceController.Beacons.RuuviTag.ParseRuuviTag(result.Url);
                                        ruuvi.Data.EventTime = DateTime.Now;
                                        if (ruuvi.Success)
                                        {
                                            name = ruuvi.ToString(); // Make a new user-friendly string
                                        }
                                        if (wrapper != null) wrapper.BleAdvert.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.RuuviTag;
                                        // TODO: this is actually weird; it should be done somewhere else.
                                        // This function is all about setting up the wrapper, not actually
                                        // triggering events!
                                        wrapper?.BleAdvert.Event(ruuvi.Data);
                                    }
                                    else
                                    {
                                        if (wrapper != null) wrapper.BleAdvert.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.Eddystone;
                                        name = $"Eddystone {result.Url}";
                                        // TODO: this is actually weird; it should be done somewhere else.
                                        // This function is all about setting up the wrapper, not actually
                                        // triggering events!
                                        wrapper?.BleAdvert.Event(result.Url);
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            return name;
        }


        private static bool EmptyOrAllNull(string str)
        {
            if (str == null) return true;
            if (str == "") return true;
            foreach (var ch in str)
            {
                if (ch != '\0') return false;
            }
            return true; ;
        }
        public static (string name, string id, string description) GetBleName(DeviceInformationWrapper wrapper, BluetoothLEAdvertisementReceivedEventArgs bleAdvert)
        {
            string name = "???";
            string id = "??-??";
            string description = "??--??";
            if (bleAdvert == null || (wrapper != null && wrapper.BleAdvert == null))
            {
                return (name, id, description);
            }
            name = bleAdvert.Advertisement.LocalName;
            id = bleAdvert.BluetoothAddress.ToString("X");
            if (EmptyOrAllNull(name))
            {
                // BAD: There's a device where the LocalName is 13 NUL chars!
                name = id;
            }
            name = CustomizeWrapperFromAdvertisement(wrapper, bleAdvert, name);
            description = AsDescription (bleAdvert);

            return (name, id, description);
        }

        public static string AsDescription (BluetoothLEAdvertisementReceivedEventArgs bleAdvert)
        {
            var addr = BluetoothAddress.AsString(bleAdvert.BluetoothAddress);
            var timestamp = bleAdvert.Timestamp.ToString("T");
            var description = $"{addr} RSS {bleAdvert.RawSignalStrengthInDBm} at {timestamp}";
            return description;
        }
    }
}
