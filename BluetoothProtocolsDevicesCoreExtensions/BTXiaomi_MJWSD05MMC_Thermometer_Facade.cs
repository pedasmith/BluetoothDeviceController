using BluetoothProtocols;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocolsDevicesCoreExtensions
{
    public static class Xiaomi_MJWSD05MMC_Thermometer_Extensions
    {
        public enum TimeMode {  H24=0, H12=1 };
        public enum TemperatureUnits { C = 0, F = 1 };
        public static async Task<GattCommunicationStatus?> SetTimeModeAsync(this Xiaomi_MJWSD05MMC_Thermometer therm, TimeMode timeMode)
        {
            var data = new byte[] { (byte)timeMode };
            var result = await therm.WriteTimeMode(data);
            return result;
        }

        public static async Task<GattCommunicationStatus?> SetTemperatureUnitsAsync(this Xiaomi_MJWSD05MMC_Thermometer therm, TemperatureUnits temperatureUnits)
        {
            var data = new byte[] { (byte)temperatureUnits };
            var result = await therm.WriteTemperatureUnits(data);
            return result;
        }

        public static async Task<GattCommunicationStatus?> WriteClockTime(this Xiaomi_MJWSD05MMC_Thermometer therm, UInt32 unixTimestamp, Int16 timeZone)
        {
            var dw = new DataWriter();
            dw.ByteOrder = ByteOrder.LittleEndian; // Bluetooth is little endian by default
            dw.WriteUInt32(unixTimestamp);
            if (timeZone < 0) timeZone += 256; // https://github.com/esphome/feature-requests/issues/643
            dw.WriteInt16(timeZone);
            //await dw.StoreAsync(); will throw, which is silly
            var buffer = dw.DetachBuffer();
            var data = buffer.ToArray();
            var result = await therm.WriteClockTime(data);

            // Read the value back.
            var result2 = await therm.ReadClockTime(Windows.Devices.Bluetooth.BluetoothCacheMode.Uncached);
            var readTimestamp = result2.UnixTimestamp;
            var delta = unixTimestamp - readTimestamp;
            return result;
        }

    }
    /// <summary>
    /// Simple (and arguably not needed) facade over the Xiaomi_MJWSD05MMC_Thermometer.Environment_Data
    /// class. That class is a huge class that covers a ton of features that are mostly not needed.
    /// </summary>
    public class BTXiaomi_MJWSD05MMC_Thermometer_Facade : BTCommonMetaData<BTXiaomi_MJWSD05MMC_Thermometer_Facade>
    {
        #region From_Xiaomi_Environment_Data
        private double _Temperature = 0;
        /// <summary>
        /// Temperature (U16 C) from Service=Environment and Characteristic=TemperatureHumidityBattery
        ///</summary>
        public double Temperature
        {
            get { return _Temperature; }
            set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged(); }
        }
        private double _Humidity = 0;
        /// <summary>
        /// Humidity (U16 %) from Service=Environment and Characteristic=TemperatureHumidityBattery
        ///</summary>
        public double Humidity
        {
            get { return _Humidity; }
            set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged(); }
        }
        private double _BatteryLevel = 0;
        /// <summary>
        /// BatteryLevel (U8 ) from Service=Environment and Characteristic=TemperatureHumidityBattery
        ///</summary>
        public double BatteryLevel
        {
            get { return _BatteryLevel; }
            set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged(); }
        }
        public override BTXiaomi_MJWSD05MMC_Thermometer_Facade Clone(string name = null)
        {
            var retval = this.MemberwiseClone() as BTXiaomi_MJWSD05MMC_Thermometer_Facade;
            if (name != null)
            {
                retval.Name = name;
            }
            return retval;
        }
        public override void CopyFrom(BTXiaomi_MJWSD05MMC_Thermometer_Facade source)
        {
            var dest = this;

            // All of the facade-specific values.


            // From the original class
            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = source.Name;
            dest.Temperature = source.Temperature;
            dest.Humidity = source.Humidity;
            dest.BatteryLevel = source.BatteryLevel;
        }

        // Like CopyFrom, but convert the doubles as appropriate + sets name
        public static BTXiaomi_MJWSD05MMC_Thermometer_Facade CopyToWithConvertAndCreate(Xiaomi_MJWSD05MMC_Thermometer.Environment_Data source, BTXiaomi_MJWSD05MMC_Thermometer_Facade dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                dest = new();
            }

            // From the original class
            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
            dest.Temperature = convert(source.Temperature, "C");
            dest.Humidity = convert(source.Humidity, "");
            dest.BatteryLevel = convert(source.BatteryLevel, "");

            // Facade-specific values are set up in the UpdateData() method.
            dest.SetFacadeDataFromOriginalData(); // will update the RPS, Speed, Distance, etc.

            return dest;
        }

        /// <summary>
        /// Does something in BTStandard_Cycling_Speed_Cadence but nothing in particular here.
        /// </summary>
        private void SetFacadeDataFromOriginalData()
        {
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            return ["Temperature", "Humidity", "BatteryLevel"];
        }

        public override void ExportRow(IExportData exporter)
        {
            // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
            // RowEnd and add in the timestamps
            exporter.CellSet(Temperature);
            exporter.CellSet(Humidity);
            exporter.CellSet(BatteryLevel);
        }

        public override string ToString()
        {
            return String.Format($"{TimestampMostRecentDT.ToString("HH:mm:ss")} {Temperature}C {Humidity}% {BatteryLevel}");
        }
#endregion
    }
}
