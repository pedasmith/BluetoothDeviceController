using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using Windows.UI.WebUI;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    public class Ruuvi_Tag
    {
        public bool IsValid { get; set; } = true;
        public UInt16 CompanyId { get; set; } // will by 0x0499==1177 for standard Ruuvi tags
        public byte TagType { get; set; } // 5==standard data tag
        public double TemperatureInDegreesC { get; set; }
        public int PressureInPascals { get; set; }
        public double HumidityInPercent { get; set; }
        public double[] AccelerationInG { get; set; } = new double[3] { 0, 0, 1.0 };
        public double BatteryVoltage { get; set; }
        public int TransmitPowerInDb { get; set; }
        public byte MovementCounter { get; set; }
        public UInt16 MovementSequenceCounter { get; set; }
        public double PM25 { get; set; } = 0.0;
        public double CO2 { get; set; } = 390;
        public double VOC { get; set; } = 0.0;
        public double NOX { get; set; } = 0.0;
        public double LuminosityRaw { get; set; } = 0.0;

        public SensorDataRecord ToSensorDataRecord(SensorDataRecord dest)
        {
            var source = this;
            if (dest == null)
            {
                dest = new SensorDataRecord();
            }
            // TODO: dest.BatteryInPercent = source.BatteryVoltage; // ???
            dest.Humidity = source.HumidityInPercent;
            dest.Temperature = source.TemperatureInDegreesC;
            dest.Pressure = source.PressureInPascals / 100.0; // convert pascal to hPa (hecto-Pascal)
            dest.PM25 = source.PM25;
            dest.CO2 = source.CO2;
            dest.VOC = source.VOC;
            dest.NOX = source.NOX;
            dest.Luminosity = source.LuminosityRaw; // TODO: 
            return dest;
        }

        public static Ruuvi_Tag FromRuuvi_DataRecord (SensorDataRecord source) // Used by Watacher
        {
            if (source == null) return null;
            var retval = new Ruuvi_Tag()
            {
                IsValid = true,
                CompanyId = 0x0499, // synthetic
                TagType = 1, // https://github.com/ruuvi/ruuvi-sensor-protocols/blob/master/broadcast_formats.md
                TemperatureInDegreesC = source.Temperature,
                PressureInPascals = (int)(source.Pressure * 100), // 1 hPA = 100 Pa
                HumidityInPercent = source.Humidity,
                AccelerationInG = new double[3] { 0, 0, 1.0 },
                BatteryVoltage = 5.0, // synthetic
                TransmitPowerInDb = 0, // synthetic
                MovementCounter = 1, // synthetic
                MovementSequenceCounter = 1, // synthetic

                PM25 = source.PM25,
                CO2 = source.CO2,
                VOC = source.VOC,
                NOX = source.NOX,
                LuminosityRaw = source.Luminosity, // TODO: correct units
            };
            return retval;
        }

        // As of 2026-07-21, look in https://docs.ruuvi.com/ for protocol details
        // Type 5: https://docs.ruuvi.com/communication/bluetooth-advertisements/data-format-5-rawv2
        // Type 6: https://docs.ruuvi.com/communication/bluetooth-advertisements/data-format-6

        // https://github.com/ruuvi/ruuvi-sensor-protocols/blob/master/dataformat_05.md
        public static Ruuvi_Tag Parse(BluetoothLEAdvertisementDataSection section, sbyte RSSI)
        {
            var retval = new Ruuvi_Tag();

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.BigEndian; // bluetooth might default to little endian, but Ruuvi is big-endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 76 == 0x4c but that's explicitly not enforced here
                retval.TagType = dr.ReadByte(); // will be type==5 for new tags 6 for Air 2026
                switch (retval.TagType)
                {
                    case 0x05:
                        {
                            retval.TemperatureInDegreesC = ((double)dr.ReadInt16()) * 0.005;
                            retval.HumidityInPercent = ((double)dr.ReadInt16()) * 0.0025;
                            retval.PressureInPascals = ((int)dr.ReadUInt16()) + 50000;
                            // Already set to an array: retval.AccelerationInG = new double[3];
                            retval.AccelerationInG[0] = ((double)dr.ReadInt16()) / 1000.0;
                            retval.AccelerationInG[1] = ((double)dr.ReadInt16()) / 1000.0;
                            retval.AccelerationInG[2] = ((double)dr.ReadInt16()) / 1000.0;
                            var power = dr.ReadUInt16();
                            retval.BatteryVoltage = ((double)(power >> 5)) / 1000.0 + 1.6;
                            retval.TransmitPowerInDb = ((int)(power & 0x1F)) * 2 - 40;
                            retval.MovementCounter = dr.ReadByte();
                            retval.MovementSequenceCounter = dr.ReadUInt16();
                        }
                        break;
                    case 0x06: // Adding this 2026-07-21 TODO: no complete
                        {
                            retval.TemperatureInDegreesC = ((double)dr.ReadInt16()) * 0.005;
                            retval.HumidityInPercent = ((double)dr.ReadInt16()) * 0.0025;
                            retval.PressureInPascals = ((int)dr.ReadUInt16()) + 50000;
                            double pm25 = ((double)dr.ReadUInt16()) / 10.0;
                            double co2 = dr.ReadUInt16();
                            int voc = dr.ReadByte(); // Needs flag data, too
                            int nox = dr.ReadByte(); // Needs flag data, too
                            int luminosity = dr.ReadByte(); // is lux in a logarithmic scale
                            byte r0 = dr.ReadByte();
                            byte seq = dr.ReadByte();
                            byte flags = dr.ReadByte();
                            var calibrationFlag = (flags & 0x01) == 0 ? 0 : 1;
                            var vocFlag = (flags & 0x40) == 0 ? 0 : 1;
                            var noxFlag = (flags & 0x80) == 0 ? 0 : 1;
                            voc = (voc << 1) + vocFlag;
                            nox = (nox << 1) + noxFlag;
                            int mac = (dr.ReadByte() << 16) + (dr.ReadByte() << 8) + (dr.ReadByte());

                            retval.PM25 = pm25;
                            retval.CO2 = co2;
                            retval.VOC = voc;
                            retval.NOX = nox;
                            retval.LuminosityRaw = luminosity;
                        }
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
