using System;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.Beacons
{
    public class Apple_IBeacon
    {
        public const short AppleCompanyId = 0x4c; // aka 76
        public const byte IBeaconType = 0x02;
        public const byte ApplePhoneType = 0x10;
        public const byte IBeaconLen = 0x15;
        public ushort CompanyId { get; set; }
        public byte BeaconType { get; set; }
        public byte BeaconLen { get; set; }
        public bool IsValid { get { return BeaconType == IBeaconType && BeaconLen >= IBeaconLen; } }
        public bool IsApple10 { get { return BeaconType == ApplePhoneType; } }
        public Guid BeaconGuid { get; set; }
        public UInt16 Major { get; set; }
        public UInt16 Minor { get; set; }
        public sbyte MeasuredPower { get; set; }
        public int ProvidedRssi { get; set; } // This is something provided by the Bluetooth API
        public double Accuracy
        {
            get
            {
                if (MeasuredPower == 0) return -1.0;
                var value = Math.Pow(12.0, 1.5 * (((double)ProvidedRssi / (double)MeasuredPower) - 1.0));
                return value;
            }
        }
        public enum Proximity { Unknown, Immediate, Near, Far }
        public Proximity MeasuredProximity
        {
            get
            {
                var acc = Accuracy;
                if (acc < 0) return Proximity.Unknown;
                if (acc < 0.5) return Proximity.Immediate;
                if (acc < 4.0) return Proximity.Near;
                return Proximity.Far;
            }
        }

        public static Apple_IBeacon Parse(BluetoothLEAdvertisementDataSection section, sbyte RSSI)
        {
            var retval = new Apple_IBeacon();

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // bluetooth defaults to little endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 76 == 0x4c but that's explicitly not enforced here

                retval.BeaconType = dr.ReadByte();
                retval.BeaconLen = dr.ReadByte();
                switch (retval.BeaconType)
                {
                    case 0x02:
                        // See https://github.com/wrightLin/RaspiJob/blob/6004e46ec28535546add496eff0ed8d7947ed563/Beacons.Helper/Beacons.cs
                        // See https://github.com/blueSense/hub-application/blob/59ab67c17cb71883cf19027bf62050b953644750/lib/bluesense-superhub/monitor/parsers/ibeacon.js
                        if (retval.BeaconLen < 0x15)
                        {
                            return retval;
                        }
                        byte[] guidBytes = new byte[16];
                        dr.ReadBytes(guidBytes);
                        retval.BeaconGuid = new Guid(guidBytes);
                        retval.Major = dr.ReadUInt16();
                        retval.Minor = dr.ReadUInt16();
                        retval.MeasuredPower = (sbyte)dr.ReadByte();
                        retval.MeasuredPower = RSSI;
                        break;
                    case 0x10:
                    case 0x12:
                        // TODO: how to handle a typical phone?
                        break;
                    default:
                        break; //TODO: what value here?
                }
            }
            catch (Exception)
            {
                ;
            }
            return retval; // if there was an exception, the beacon is invalid...
        }


        public override string ToString()
        {
            if (!IsValid) return "Invalid IBeacon";

            return $"{MeasuredProximity.ToString()} {Major}.{Minor} {BeaconGuid.ToString()}";
        }
    }
}
