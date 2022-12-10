using BluetoothProtocols.Beacons;
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
        /// This used to be gross and also trigger events. It doesn't any more. Returns either the retval or an Eddystone or Ruvvi retval.
        /// </summary>
        /// <param retval="deviceWrapper"></param>
        /// <param retval="ble"></param>
        /// <param retval="name"></param>
        /// <returns>updated name of device</returns>
        private static string CustomizeWrapperFromAdvertisement(BleAdvertisementWrapper bleWrapper, string name)
        {
            // Lets's see if it's an Eddystone beacon...
            // https://github.com/google/eddystone
            // https://github.com/google/eddystone/blob/master/protocol-specification.md

            var retval = name;
            var ble = bleWrapper.BleAdvert;

            // 
            // let's see if the advert is one of the known cases.
            //
            var govee = Govee.Parse(bleWrapper);
            if (govee != null && govee.IsValid)
            {
                bleWrapper.GoveeDataRecord = govee; // only set if it's valid.
                bleWrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.Govee;
            }
            if (Govee.AdvertIsGovee(bleWrapper))
            {
                // Set the type to Govee if either this advert is govee OR the original advert is Govee.
                // Otherwise, doesn't set to Govee for the first advert.
                bleWrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.Govee;
            }
            var switchbot = SwitchBot.Parse(bleWrapper);
            if (switchbot != null && switchbot.IsValid)
            {
                bleWrapper.RuuviDataRecord = switchbot; // TODO: change the name over and remove the special caes for Govee.
                bleWrapper.SwitchBotDataRecord = switchbot; 
                bleWrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.SwitchBot;
                retval = $"SwitchBot-{switchbot.TagType}";
            }

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
                            bleWrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.Eddystone;
                            switch (EddystoneFrameType)
                            {
                                default:
                                    bleWrapper.AdvertisementEddystoneSubtype = BleAdvertisementWrapper.BleAdvertisementEddystoneSubtype.None;
                                    break;
                                case TypeUID:
                                    bleWrapper.AdvertisementEddystoneSubtype = BleAdvertisementWrapper.BleAdvertisementEddystoneSubtype.Uid;
                                    //TODO: Fixing: deviceWrapper?.BleAdvert.Event("UID: <data>");
                                    break;
                                case TypeTLM:
                                    bleWrapper.AdvertisementEddystoneSubtype = BleAdvertisementWrapper.BleAdvertisementEddystoneSubtype.Tlm;
                                    //TODO: fixing: deviceWrapper?.BleAdvert.Event("TLM: <data>");
                                    break;
                                case TypeEID:
                                    bleWrapper.AdvertisementEddystoneSubtype = BleAdvertisementWrapper.BleAdvertisementEddystoneSubtype.Eid;
                                    //TODO: Fixing: deviceWrapper?.BleAdvert.Event("EID: <data>");
                                    break;
                                case TypeURL: // 0x10: An Eddystone-URL
                                    // https://github.com/google/eddystone/tree/master/eddystone-url
                                    var result = BluetoothDeviceController.Beacons.Eddystone.ParseEddystoneUrlArgs(section.Data);
                                    retval = result.Success ? result.Url : "Invalid eddystone!";

                                    bleWrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.Eddystone;
                                    bleWrapper.AdvertisementEddystoneSubtype = BleAdvertisementWrapper.BleAdvertisementEddystoneSubtype.Url;
                                    bleWrapper.EddystoneUrl = retval;
                                    if (result.Success && result.Url.StartsWith("https://ruu.vi/#"))
                                    {
                                        var ruuvi = BluetoothDeviceController.Beacons.Ruuvi_Tag_v1_Helper.ParseRuuviTag(result.Url);
                                        ruuvi.Data.EventTime = DateTime.Now;
                                        if (ruuvi.Success)
                                        {
                                            retval = ruuvi.ToString(); // Make a new user-friendly string
                                        }
                                        bleWrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.RuuviTag;
                                        bleWrapper.RuuviDataRecord = ruuvi.Data;
                                    }
                                    else
                                    {
                                        retval = $"Eddystone {result.Url}";
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            return retval;
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

        /// <summary>
        /// This no longer horribly returns some data while also calling the wrapper.Event() calls via CustomizeWrapperFromAdvertisement if the wrapper is not null.
        /// </summary>
        public static (string name, string id, string description) GetBleName(BleAdvertisementWrapper bleAdvertWrapper)
        {
            BluetoothLEAdvertisementReceivedEventArgs bleAdvert = bleAdvertWrapper.BleAdvert;
            string name = "???";
            string id = "??-??";
            string description = "??--??";
            if (bleAdvert == null)
            {
                return (name, id, description);
            }
            name = bleAdvert.Advertisement.LocalName;
            // In the case of a Govee ScanResponse, the LocalName isn't set. But we can still get it from the
            // OriginalAdvert localname.
            if (String.IsNullOrEmpty(name))
            {
                if (bleAdvertWrapper.BleOriginalAdvert != null)
                {
                    name = bleAdvertWrapper.BleOriginalAdvert.Advertisement.LocalName;
                }
            }
            id = BluetoothAddress.AsString(bleAdvert.BluetoothAddress);
            if (EmptyOrAllNull(name))
            {
                // BAD: There's a device where the LocalName is 13 NUL chars!
                name = id;
            }
            name = CustomizeWrapperFromAdvertisement(bleAdvertWrapper, name); // Used to also trigger an event, but doesn't any more.
            description = AsDescription (bleAdvert);
            return (name, id, description);
        }

        public static string AsDescription (BluetoothLEAdvertisementReceivedEventArgs bleAdvert)
        {
            var addr = BluetoothAddress.AsString(bleAdvert.BluetoothAddress);
            var time24 = bleAdvert.Timestamp.ToString("HH:mm:ss.f");

            var description = $"{addr} at {time24} RSS {bleAdvert.RawSignalStrengthInDBm}";
            return description;
        }
    }
}
