using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using System;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocolsDevicesCoreExtensions.TI_SensorTag_Extensions
{
    public class TI_SensorTag
    {
        /// <summary>
        /// Which SensorTag. The enums are ordered by date of release (confusingly, the 2541 is earlier than the 1352).
        /// </summary>
        public enum SensorType { Other, TI_2541, TI_1350, TI_1352, NotThisSensorFamily };
        public SensorType TagType { get; set; } = SensorType.Other;

        /// <summary>
        /// Returns true if the local name OR the original name matches a sensor tag
        /// All of these parsers happen to have an AdvertIsSensorFamily :-)
        /// </summary>
        public static SensorType AdvertIsSensorFamily(WatcherData wrapper)
        {
            var retval = NameToSensorType(wrapper.BestName);
            if (retval == SensorType.NotThisSensorFamily && wrapper.OriginalAdvertisement != null)
            {
                retval = NameToSensorType(wrapper.OriginalAdvertisement.Advertisement.LocalName);
            }
            return retval;
        }

        private static SensorType NameToSensorType(string name)
        {
            SensorType retval = SensorType.NotThisSensorFamily;
            if (name != null)
            {
                if (name.StartsWith("SensorTag")) retval = SensorType.TI_2541;
                if (name.StartsWith("CC1350 SensorTag")) retval = SensorType.TI_1350;
                if (name.StartsWith("Multi-Sensor")) retval = SensorType.TI_1352;
            }
            return retval;
        }
    }
    public class Environment_Data : BTCommonMetaData<Environment_Data> //, IExportDataSource
    {
        private double _Temperature = 0.0;
        /// <summary>
        /// Temperature (F32 C) from Service=Temperature and Characteristic=Temperature Data
        ///</summary>
        public double Temperature
        {
            get { return _Temperature; }
            set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged(); }
        }

        private double _Humidity = 0.0;
        /// <summary>
        /// Humidity (F32 Percent) from Service=Humidity and Characteristic=Humidity Data
        ///</summary>
        public double Humidity
        {
            get { return _Humidity; }
            set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged(); }
        }

        private double _Pressure = 0.0;
        /// <summary>
        /// Pressure (F32 hPa) from Service=Barometer and Characteristic=Barometer Data
        ///</summary>
        public double Pressure
        {
            get { return _Pressure; }
            set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged(); }
        }

        public bool HasPressure { get; set; } = false; // Most sensors don't have pressure sensor

        public override Environment_Data Clone(string name = null)
        {
            var retval = this.MemberwiseClone() as Environment_Data;
            if (name != null)
            {
                retval.Name = name;
            }
            return retval;
        }

        /// <summary>
        /// Copies all of the source fields to the 'this' destination
        /// </summary>
        public override void CopyFrom(Environment_Data source)
        {
            var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = source.Name;
            dest.Temperature = source.Temperature;
            dest.Humidity = source.Humidity;
            dest.Pressure = source.Pressure;
            dest.HasPressure = source.HasPressure;
        }

        /// <summary>
        /// For the 2541: Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
        /// and will set the name to the given name if it's not null or empty.
        /// </summary>

        public static Environment_Data CopyToWithConvertAndCreate(TI_SensorTag_2541.Humidity_Data sourceHumidity, Environment_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                // Funny two-step here to get around the problem of creating the 
                // right kind of object but with the right handling of Name.
                dest = new Environment_Data();
                dest = dest.Clone(name);
            }
            dest.TimestampMostRecent = sourceHumidity.TimestampMostRecent;
            dest.Name = String.IsNullOrEmpty(name) ? sourceHumidity.Name : name;
            dest.Temperature = convert(sourceHumidity.Temp, "C"); 
            dest.Humidity = convert(sourceHumidity.Humidity, "Percent");
            dest.Pressure = 0; // No pressure data in the 2541
            dest.HasPressure = false;
            return dest;
        }
        /// <summary>
        /// For the 1350:Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
        /// and will set the name to the given name if it's not null or empty.
        /// </summary>

        public static Environment_Data CopyToWithConvertAndCreate(TI_SensorTag_1350.Barometer_Data sourceTemperature, TI_SensorTag_1350.Humidity_Data sourceHumidity, Environment_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                // Funny two-step here to get around the problem of creating the 
                // right kind of object but with the right handling of Name.
                dest = new Environment_Data();
                dest = dest.Clone(name);
            }
            dest.TimestampMostRecent = sourceTemperature.TimestampMostRecent;
            dest.Name = String.IsNullOrEmpty(name) ? sourceTemperature.Name : name;
            dest.Temperature = convert(sourceTemperature.Temp, "C");
            dest.Humidity = convert(sourceHumidity.Humidity, "Percent");
            dest.Pressure = convert(sourceTemperature.Pressure, "hPA");
            dest.HasPressure = true;
            return dest;
        }

        /// <summary>
        /// For the 1352:Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
        /// and will set the name to the given name if it's not null or empty.
        /// </summary>

        public static Environment_Data CopyToWithConvertAndCreate(TI_SensorTag_1352.Temperature_Data sourceTemperature, TI_SensorTag_1352.Humidity_Data sourceHumidity, Environment_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                // Funny two-step here to get around the problem of creating the 
                // right kind of object but with the right handling of Name.
                dest = new Environment_Data();
                dest = dest.Clone(name);
            }
            dest.TimestampMostRecent = sourceTemperature.TimestampMostRecent;
            dest.Name = String.IsNullOrEmpty(name) ? sourceTemperature.Name : name;
            dest.Temperature = convert(sourceTemperature.Temperature, "C");
            dest.Humidity = convert(sourceHumidity.Humidity, "Percent");
            dest.Pressure = 0; // No pressure data in the 1352
            dest.HasPressure = false;
            return dest;
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            if (HasPressure) return ["Temperature", "Humidity", "Pressure"]; 
            return ["Temperature", "Humidity", ]; 
        }

        public override void ExportRow(IExportData exporter)
        {
            // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
            // RowEnd and add in the timestamps
            exporter.CellSet(Temperature);
            exporter.CellSet(Humidity);
            if (HasPressure) exporter.CellSet(Pressure);
        }

        public override string ToString()
        {
            if (HasPressure) return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temperature} {Humidity}% {Pressure}");
            return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temperature} {Humidity}");
        }
    }
}
