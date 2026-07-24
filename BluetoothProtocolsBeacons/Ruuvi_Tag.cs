using BluetoothProtocols;
using BluetoothWatcher.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public double PM10 { get; set; } = 0.0;
        public double PM25 { get; set; } = 0.0;
        public double PM40 { get; set; } = 0.0;
        public double PM100 { get; set; } = 0.0;
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
            dest.PM10 = source.PM10;
            dest.PM25 = source.PM25;
            dest.PM40 = source.PM40;
            dest.PM100 = source.PM100;
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

                PM10 = source.PM10,
                PM25 = source.PM25,
                PM40 = source.PM40,
                PM100 = source.PM100,
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
        // Type E1: https://docs.ruuvi.com/communication/bluetooth-advertisements/data-format-e1
        // The Air produces both the type 6 and type E1. E1 has more data.
        // Forum: https://f.ruuvi.com/

        // https://github.com/ruuvi/ruuvi-sensor-protocols/blob/master/dataformat_05.md
        public static Ruuvi_Tag Parse(BluetoothLEAdvertisementDataSection section, sbyte RSSI)
        {
            Ruuvi_Tag retval = null;

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                retval = ParseFromDataReader(dr);
            }
            catch (Exception)
            {
                if (retval == null) retval = new Ruuvi_Tag();
                retval.IsValid = false;
            }
            return retval; // if there was an exception, the beacon is invalid...
        }

        public static Ruuvi_Tag ParseFromTestVector(string data)
        {
            // Test vectors from e.g., https://docs.ruuvi.com/communication/bluetooth-advertisements/data-format-e1
            // - have a starting 0x (optional here)
            // - don't include the starting company id
            // - have XX for don't care; replace with 00
            if (data.StartsWith("0x"))
            {
                data = data.Substring(2);
            }
            data = "0499" + data; // Add in the company ID 
            data = data.Replace("XX", "00");
            byte[] vector = new byte[(data.Length / 2)];
            for (int i=0; i<data.Length; i+=2)
            {
                var vectorIndex = i / 2;
                var value = (HexToValue(data[i]) * 16) + HexToValue(data[i + 1]);
                vector[vectorIndex] = (byte)value;
            }
            var dr = DataReader.FromBuffer(vector.AsBuffer());
            var retval = ParseFromDataReader(dr);
            return retval;
        }




    private static int HexToValue(char value)
        {
            var retval = 0;
            if (value >= '0' && value <= '9') retval = value - '0';
            else if (value >= 'A' && value <= 'F') retval = value - 'A' + 10;
            else if (value >= 'a' && value <= 'f') retval = value - 'A' + 10;
            return retval;
        }

        private static int TestOne(string testVector, 
            double expectedTemperature, double expectedPressure, double expectedHumidity,
            double expectedPM10, double expectedPM25, double expectedPM40, double expectedPM100,
            double expectedCO2, double expectedVOC, double expectedNOX, double expectedLuminosity)
        {
            int nerror = 0;
            var ruuvi = ParseFromTestVector(testVector);
            if (!DoubleApprox.Approx(ruuvi.TemperatureInDegreesC, expectedTemperature))
            {
                nerror += 1;
                Log($"Error: Ruuvi: temperature expected {expectedTemperature} actual {ruuvi.TemperatureInDegreesC}");
            }
            if (!DoubleApprox.Approx(ruuvi.PressureInPascals,expectedPressure))
            {
                nerror += 1;
                Log($"Error: Ruuvi: pressure expected {expectedPressure} actual {ruuvi.PressureInPascals}");
            }
            if (!DoubleApprox.Approx(ruuvi.HumidityInPercent, expectedHumidity, 0.0000001))
            {
                nerror += 1;
                Log($"Error: Ruuvi: humidity expected {expectedHumidity} actual {ruuvi.HumidityInPercent}");
            }
            if (!DoubleApprox.Approx(ruuvi.PM10, expectedPM10))
            {
                nerror += 1;
                Log($"Error: Ruuvi: pm10 expected {expectedPM10} actual {ruuvi.PM10}");
            }
            if (!DoubleApprox.Approx(ruuvi.PM25, expectedPM25))
            {
                nerror += 1;
                Log($"Error: Ruuvi: pm25 expected {expectedPM25} actual {ruuvi.PM25}");
            }
            if (!DoubleApprox.Approx(ruuvi.PM40, expectedPM40))
            {
                nerror += 1;
                Log($"Error: Ruuvi: PM40 expected {expectedPM40} actual {ruuvi.PM40}");
            }
            if (!DoubleApprox.Approx(ruuvi.PM100, expectedPM100))
            {
                nerror += 1;
                Log($"Error: Ruuvi: PM100 expected {expectedPM100} actual {ruuvi.PM100}");
            }
            if (!DoubleApprox.Approx(ruuvi.CO2, expectedCO2))
            {
                nerror += 1;
                Log($"Error: Ruuvi: CO2 expected {expectedCO2} actual {ruuvi.CO2}");
            }
            if (!DoubleApprox.Approx(ruuvi.VOC, expectedVOC))
            {
                nerror += 1;
                Log($"Error: Ruuvi: VOC expected {expectedVOC} actual {ruuvi.VOC}");
            }
            if (!DoubleApprox.Approx(ruuvi.NOX, expectedNOX))
            {
                nerror += 1;
                Log($"Error: Ruuvi: NOX expected {expectedNOX} actual {ruuvi.NOX}");
            }
            if (!DoubleApprox.Approx(ruuvi.LuminosityRaw, expectedLuminosity))
            {
                nerror += 1;
                Log($"Error: Ruuvi: Luminosity expected {expectedLuminosity} actual {ruuvi.LuminosityRaw}");
            }
            return nerror;
        }



        public static int Test()
        {
            int nerror = 0;
            // https://docs.ruuvi.com/communication/bluetooth-advertisements/data-format-e1
            // Case: Valid data
            nerror += TestOne("0xE1170C5668C79E0065007004BD11CA00C90A0213E0ACXXXXXXDECDEE01XXXXXXXXXXCBB8334C884F",
                29.5, 101102, 55.3, 10.1, 11.2, 121.3, 455.4, 201, 20, 4, 13027);
            // Case: maximum values
            nerror += TestOne("0xE17FFF9C40FFFE27102710271027109C40FAFADC28F0XXXXXXFFFFFE3FXXXXXXXXXXCBB8334C884F",
                163.835, 115534, 100, 1000, 1000, 1000, 1000, 40000, 500, 500, 144284);
            // Case: minimum values
            nerror += TestOne("0xE1800100000000000000000000000000000000000000XXXXXX0000000XXXXXXXXXXXCBB8334C884F",
                -163.835, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            // https://docs.ruuvi.com/communication/bluetooth-advertisements/data-format-6
            // Case: valid data
            nerror += TestOne("0x06170C5668C79E007000C90501D9XXCD004C884F",
                29.5, 101102, 55.3, 0, 11.2, 0, 0, 201, 10, 2, 13026.67); // Type 6 only include PM25 not the other PM values
            // Case: maximum values
            nerror += TestOne("0x067FFF9C40FFFE27109C40FAFAFEXXFF074C8F4F",
                163.835, 115534, 100, 0, 1000, 0, 0, 40000, 500, 500, 65535);
            // Case: minimum values
            nerror += TestOne("0x0680010000000000000000000000XX00004C884F",
                -163.835, 50000, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            return nerror;
        }
        private static void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }
        public static Ruuvi_Tag ParseFromDataReader(DataReader dr)
        {
            var retval = new Ruuvi_Tag();

            try
            {
                dr.ByteOrder = ByteOrder.BigEndian; // bluetooth might default to little endian, but Ruuvi is big-endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 1177 == 0x0499 but that's explicitly not enforced here
                retval.TagType = dr.ReadByte(); // will be type==5 for new tags type==6  or type==E1 for Air 2026
                switch (retval.TagType)
                {
                    case 0x05:
                        {
                            retval.TemperatureInDegreesC = ((double)dr.ReadInt16()) * 0.005;
                            retval.HumidityInPercent = ((double)dr.ReadUInt16()) * 0.0025;
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
                            retval.IsValid = true;
                        }
                        break;
                    case 0x06: // Adding this 2026-07-21 TODO: not complete
                        {
                            retval.TemperatureInDegreesC = ((double)dr.ReadInt16()) * 0.005;
                            retval.HumidityInPercent = ((double)dr.ReadUInt16()) * 0.0025;
                            retval.PressureInPascals = ((int)dr.ReadUInt16()) + 50000;
                            retval.PM25 = ((double)dr.ReadUInt16()) / 10.0;
                            retval.CO2 = dr.ReadUInt16();
                            int voc = dr.ReadByte(); // Needs flag data, too
                            int nox = dr.ReadByte(); // Needs flag data, too
                            int luminosityCode = dr.ReadByte(); // is lux in a logarithmic scale
                            const double LUX_MAX_VALUE = 65535;
                            const double LUX_MAX_CODE = 254;
                            double LUX_DELTA = Math.Log(LUX_MAX_VALUE + 1) / LUX_MAX_CODE;
                            //double lux_code = Math.Round(Math.Log(luminosity + 1) / LUX_DELTA);
                            retval.LuminosityRaw = Math.Exp((double)luminosityCode * LUX_DELTA) - 1;
                            byte r0 = dr.ReadByte();
                            byte seq = dr.ReadByte();
                            byte flags = dr.ReadByte();
                            var calibrationFlag = (flags & 0x01) == 0 ? 0 : 1;
                            var vocFlag = (flags & 0x40) == 0 ? 0 : 1;
                            var noxFlag = (flags & 0x80) == 0 ? 0 : 1;
                            voc = (voc << 1) + vocFlag;
                            nox = (nox << 1) + noxFlag;
                            int mac = (dr.ReadByte() << 16) + (dr.ReadByte() << 8) + (dr.ReadByte());

                            retval.VOC = voc;
                            retval.NOX = nox;
                            retval.IsValid = true;
                        }
                        break;
                    case 0xE1: // Adding this 2026-07-21 TODO: not complete
                        {
                            retval.TemperatureInDegreesC = ((double)dr.ReadInt16()) * 0.005;
                            retval.HumidityInPercent = ((double)dr.ReadUInt16()) * 0.0025;
                            retval.PressureInPascals = ((int)dr.ReadUInt16()) + 50000;
                            retval.PM10 = ((double)dr.ReadUInt16()) / 10.0;
                            retval.PM25 = ((double)dr.ReadUInt16()) / 10.0;
                            retval.PM40 = ((double)dr.ReadUInt16()) / 10.0;
                            retval.PM100 = ((double)dr.ReadUInt16()) / 10.0;
                            retval.CO2 = dr.ReadUInt16();
                            int voc = dr.ReadByte(); // Needs flag data, too
                            int nox = dr.ReadByte(); // Needs flag data, too
                            //int luminosity = dr.ReadByte(); // is lux in a logarithmic scale
                            retval.LuminosityRaw = (double)((dr.ReadByte() << 16) + (dr.ReadByte() << 8) + (dr.ReadByte())) / 100.0; // In LUX
                            byte r0 = dr.ReadByte();
                            byte r1 = dr.ReadByte();
                            byte r2 = dr.ReadByte();
                            var seq = (dr.ReadByte() << 16) + (dr.ReadByte() << 8) + (dr.ReadByte());
                            byte flags = dr.ReadByte();
                            var calibrationFlag = (flags & 0x01) == 0 ? 0 : 1;
                            var vocFlag = (flags & 0x40) == 0 ? 0 : 1;
                            var noxFlag = (flags & 0x80) == 0 ? 0 : 1;
                            voc = (voc << 1) + vocFlag;
                            nox = (nox << 1) + noxFlag;
                            byte r10 = dr.ReadByte();
                            byte r11 = dr.ReadByte();
                            byte r12 = dr.ReadByte();
                            byte r13 = dr.ReadByte();
                            byte r14 = dr.ReadByte();
                            int mac1 = (dr.ReadByte() << 16) + (dr.ReadByte() << 8) + (dr.ReadByte());
                            int mac2 = (dr.ReadByte() << 16) + (dr.ReadByte() << 8) + (dr.ReadByte());

                            retval.VOC = voc;
                            retval.NOX = nox;
                            retval.IsValid = true;
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
