using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.Beacons
{
    public class Ruuvi_Tag
    {
        public bool IsValid { get; set; } = true;
        public UInt16 CompanyId { get; set; } // will by 0x0499==1177 for standard Ruuvi tags
        public byte TagType { get; set; } // 5==standard data tag
        public double TemperatureInDegreesC { get; set; }
        public int PressureInPascals { get; set; }
        public double HumidityInPercent { get; set; }
        public double[] AccelerationInG { get; set; }
        public double BatteryVoltage { get; set; }
        public int TransmitPowerInDb { get; set; }
        public byte MovementCounter { get; set; }
        public UInt16 MovementSequenceCounter { get; set; }

        public static Ruuvi_Tag FromRuuvi_DataRecord (Ruuvi_Tag_v1_Helper.Ruuvi_DataRecord dr)
        {
            if (dr == null) return null;
            var retval = new Ruuvi_Tag()
            {
                IsValid = true,
                CompanyId = 0x0499, // synthetic
                TagType = 1, // https://github.com/ruuvi/ruuvi-sensor-protocols/blob/master/broadcast_formats.md
                TemperatureInDegreesC = dr.Temperature,
                PressureInPascals = (int)(dr.Pressure * 100), // 1 hPA = 100 Pa
                HumidityInPercent = dr.Humidity,
                AccelerationInG = new double[3] { 0, 0, 1.0},
                BatteryVoltage = 5.0, // synthetic
                TransmitPowerInDb = 0, // synthetic
                MovementCounter = 1, // synthetic
                MovementSequenceCounter = 1, // synthetic
            };
            return retval;
        }

        // https://github.com/ruuvi/ruuvi-sensor-protocols/blob/master/dataformat_05.md
        public static Ruuvi_Tag Parse(BluetoothLEAdvertisementDataSection section, sbyte RSSI)
        {
            var retval = new Ruuvi_Tag();

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.BigEndian; // bluetooth might default to little endian, but Ruuvi is big-endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 76 == 0x4c but that's explicitly not enforced here
                retval.TagType = dr.ReadByte(); // will be type==5 for new tags
                switch (retval.TagType)
                {
                    case 0x05:
                        retval.TemperatureInDegreesC = ((double)dr.ReadInt16()) * 0.005;
                        retval.HumidityInPercent = ((double)dr.ReadInt16()) * 0.0025;
                        retval.PressureInPascals = ((int)dr.ReadUInt16()) + 50000;
                        retval.AccelerationInG = new double[3];
                        retval.AccelerationInG[0] = ((double)dr.ReadInt16()) / 1000.0;
                        retval.AccelerationInG[1] = ((double)dr.ReadInt16()) / 1000.0;
                        retval.AccelerationInG[2] = ((double)dr.ReadInt16()) / 1000.0;
                        var power = dr.ReadUInt16();
                        retval.BatteryVoltage = ((double)(power >> 5)) / 1000.0 + 1.6;
                        retval.TransmitPowerInDb = ((int)(power & 0x1F)) * 2 - 40;
                        retval.MovementCounter = dr.ReadByte();
                        retval.MovementSequenceCounter = dr.ReadUInt16();
                        break;
                    default:
                        retval.IsValid = false;
                        break;
                }
            }
            catch (Exception)
            {
                retval.IsValid = false;
            }
            return retval; // if there was an exception, the beacon is invalid...
        }
        public override string ToString()
        {
            return $"Temperaure={TemperatureInDegreesC} Pressure={PressureInPascals} Humidity={HumidityInPercent}% "
                + $"Acceleration=[{AccelerationInG[0]},{AccelerationInG[1]},{AccelerationInG[2]}]G Movement={MovementCounter},{MovementSequenceCounter} "
                + $"Battery={BatteryVoltage}V Tx={TransmitPowerInDb}db";
        }
    }
}
