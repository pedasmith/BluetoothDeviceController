using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Windows.Storage.Streams;
using static BluetoothProtocols.TI_SensorTag_1352;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocolsDevicesCoreExtensions.TI_SensorTag_1352_Extensions
{
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
        }

        // Like CopyFrom, but convert the doubles as appropriate + sets name
        /// <summary>
        /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
        /// and will set the name to the given name if it's not null or empty.
        /// </summary>

        public static Environment_Data CopyToWithConvertAndCreate(Temperature_Data sourceTemperature, Humidity_Data sourceHumidity, Environment_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
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
            return dest;
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            return ["Temperature", "Humidity", ];
        }

        public override void ExportRow(IExportData exporter)
        {
            // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
            // RowEnd and add in the timestamps
            exporter.CellSet(Temperature);
        }

        public override string ToString()
        {
            return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temperature} {Humidity}");
        }

    }
}
