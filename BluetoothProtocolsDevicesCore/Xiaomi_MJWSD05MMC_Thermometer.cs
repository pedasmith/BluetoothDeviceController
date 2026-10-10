//From template: Protocol_Core_Body v2026-04-17 11:43
using System;
using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Needed for CallerMemberNameAttribute
using System.Runtime.InteropServices.WindowsRuntime; // Needed for IBuffer.ToArray extension method
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    /// <summary>
    /// Thermometer / Hygrometer / Clock with large LCD display..
    /// This class was automatically generated 2026-10-09::20:00
    /// </summary>

    public  class Xiaomi_MJWSD05MMC_Thermometer : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://www.mi.com/global/product/xiaomi-smart-temperature-and-humidity-monitor-3/specs/
        // Link: https://github.com/keks51/lywsd03mmc-client
        // Link: https://github.com/pvvx/ATC_MiThermometer
        // Link: https://community.home-assistant.io/t/how-to-properly-integrate-xiaomi-thermometer-3-mjwsd05mmc/836078

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "MJWSD05MMC";
        public string Description { get; } = "Thermometer / Hygrometer / Clock with large LCD display.";

        /* Service and Characteristics summary for the device MJWSD05MMC

        Environment service Guid=ebe0ccb0-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_Data (DataGroup record)
                ClockTime characteristic has UnixTimestamp (UInt32-->double) TimezoneOffset (UInt16-->double)  Guid=ebe0ccb7-7a0a-4b0c-8a1a-6ff2997da3a6
                StoredEntries characteristic has StoredEntries (Bytes-->string)  Guid=ebe0ccb9-7a0a-4b0c-8a1a-6ff2997da3a6
                SingleRecord characteristic has SingleRecord (Bytes-->string)  Guid=ebe0ccba-7a0a-4b0c-8a1a-6ff2997da3a6
                LastHourData characteristic has LastHourData (Bytes-->string)  Guid=ebe0ccbb-7a0a-4b0c-8a1a-6ff2997da3a6
                FirstHistoryRecordIndex characteristic has FirstHistoryRecordIndex (Bytes-->string)  Guid=ebe0ccbc-7a0a-4b0c-8a1a-6ff2997da3a6
                TemperatureUnits characteristic has TemperatureUnits (Byte-->double)  Guid=ebe0ccbe-7a0a-4b0c-8a1a-6ff2997da3a6
                TempHumidity characteristic has Temperature (UInt16-->double) Humidity (UInt16-->double) BatteryLevel (Byte-->double)  Guid=ebe0ccc1-7a0a-4b0c-8a1a-6ff2997da3a6
                TemperatureCalibration characteristic has CalibrationData (Bytes-->string)  Guid=ebe0ccc2-7a0a-4b0c-8a1a-6ff2997da3a6
                Battery characteristic has param0 (Byte-->double)  Guid=ebe0ccc4-7a0a-4b0c-8a1a-6ff2997da3a6
                Disconnect characteristic has DisconnectDAta (Bytes-->string)  Guid=ebe0ccc8-7a0a-4b0c-8a1a-6ff2997da3a6
                MiHomeCommand characteristic has MiHomeCommand (Bytes-->string)  Guid=ebe0ccd4-7a0a-4b0c-8a1a-6ff2997da3a6
                TimeMode characteristic has TimeMode (Byte-->double)  Guid=ebe0cce4-7a0a-4b0c-8a1a-6ff2997da3a6


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Connection Parameter characteristic has Interval_Min (UInt16-->double) Interval_Max (UInt16-->double) Latency (UInt16-->double) Timeout (UInt16-->double)  Guid=2a04


        Generic Service service Guid=1801
            Generic Service_Data (DataGroup record)
                Service Changes characteristic has StartRange (UInt16-->double) EndRange (UInt16-->double)  Guid=2a05


        Device Info service Guid=180a
            Device Info_Data (DataGroup record)
                PnP ID characteristic has VendorIDSource (Byte-->double) VendorID (UInt16-->double) ProductID (UInt16-->double) ProductVersion (UInt16-->double)  Guid=2a50
                System ID characteristic has SystemId (String-->string)  Guid=2a23
                Model Number characteristic has ModelNumber (String-->string)  Guid=2a24
                Serial Number characteristic has SerialNumber (String-->string)  Guid=2a25
                Firmware Revision characteristic has FirmwareRevision (String-->string)  Guid=2a26
                Hardware Revision characteristic has HardwareRevision (String-->string)  Guid=2a27
                Software Revision characteristic has SoftwareRevision (String-->string)  Guid=2a28


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                BatteryLevel characteristic has BatteryLevel (SByte-->double)  Guid=2a19


        TELink OTA service Guid=00010203-0405-0607-0809-0a0b0c0d1912
            TELink_OTA_Data (DataGroup record)
                OTABytes characteristic has OTABytes (Bytes-->string)  Guid=00010203-0405-0607-0809-0a0b0c0d2b12


        Unknown9 service Guid=00000100-0065-6c62-2e74-6f696d2e696d
            Unknown9_Data (DataGroup record)
                Unknown0 characteristic has Unknown0 (Bytes-->string)  Guid=00000101-0065-6c62-2e74-6f696d2e696d
                Unknown1 characteristic has Unknown1 (Bytes-->string)  Guid=00000102-0065-6c62-2e74-6f696d2e696d
        */

        public const string ClockTimePropertyChangedName = "ClockTime";
        public const string StoredEntriesPropertyChangedName = "StoredEntries";
        public const string SingleRecordPropertyChangedName = "SingleRecord";
        public const string LastHourDataPropertyChangedName = "LastHourData";
        public const string FirstHistoryRecordIndexPropertyChangedName = "FirstHistoryRecordIndex";
        public const string TemperatureUnitsPropertyChangedName = "TemperatureUnits";
        public const string TempHumidityPropertyChangedName = "TempHumidity";
        public const string TemperatureCalibrationPropertyChangedName = "TemperatureCalibration";
        public const string BatteryPropertyChangedName = "Battery";
        public const string DisconnectPropertyChangedName = "Disconnect";
        public const string MiHomeCommandPropertyChangedName = "MiHomeCommand";
        public const string TimeModePropertyChangedName = "TimeMode";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Connection_ParameterPropertyChangedName = "Connection_Parameter";
        public const string Service_ChangesPropertyChangedName = "Service_Changes";
        public const string PnP_IDPropertyChangedName = "PnP_ID";
        public const string System_IDPropertyChangedName = "System_ID";
        public const string Model_NumberPropertyChangedName = "Model_Number";
        public const string Serial_NumberPropertyChangedName = "Serial_Number";
        public const string Firmware_RevisionPropertyChangedName = "Firmware_Revision";
        public const string Hardware_RevisionPropertyChangedName = "Hardware_Revision";
        public const string Software_RevisionPropertyChangedName = "Software_Revision";
        public const string BatteryLevelPropertyChangedName = "BatteryLevel";
        public const string OTABytesPropertyChangedName = "OTABytes";
        public const string Unknown0PropertyChangedName = "Unknown0";
        public const string Unknown1PropertyChangedName = "Unknown1";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the Environment Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Environment_Data :BTCommonMetaData<Environment_Data> //, IExportDataSource
        {
            private double _UnixTimestamp = 0;
            /// <summary>
            /// UnixTimestamp (U32 ) from Service=Environment and Characteristic=ClockTime
            ///</summary>
            public double UnixTimestamp 
            { 
                get { return _UnixTimestamp; }
                set { if (value == _UnixTimestamp) return; _UnixTimestamp = value; OnPropertyChanged();}
            }
            private double _TimezoneOffset = 0;
            /// <summary>
            /// TimezoneOffset (U16 ) from Service=Environment and Characteristic=ClockTime
            ///</summary>
            public double TimezoneOffset 
            { 
                get { return _TimezoneOffset; }
                set { if (value == _TimezoneOffset) return; _TimezoneOffset = value; OnPropertyChanged();}
            }

            private byte[] _StoredEntries = null;
            /// <summary>
            /// StoredEntries (BYTES ) from Service=Environment and Characteristic=StoredEntries
            ///</summary>
            public byte[] StoredEntries 
            { 
                get { return _StoredEntries; }
                set { if (value == _StoredEntries) return; _StoredEntries = value; OnPropertyChanged();}
            }

            private byte[] _SingleRecord = null;
            /// <summary>
            /// SingleRecord (BYTES ) from Service=Environment and Characteristic=SingleRecord
            ///</summary>
            public byte[] SingleRecord 
            { 
                get { return _SingleRecord; }
                set { if (value == _SingleRecord) return; _SingleRecord = value; OnPropertyChanged();}
            }

            private byte[] _LastHourData = null;
            /// <summary>
            /// LastHourData (BYTES ) from Service=Environment and Characteristic=LastHourData
            ///</summary>
            public byte[] LastHourData 
            { 
                get { return _LastHourData; }
                set { if (value == _LastHourData) return; _LastHourData = value; OnPropertyChanged();}
            }

            private byte[] _FirstHistoryRecordIndex = null;
            /// <summary>
            /// FirstHistoryRecordIndex (BYTES ) from Service=Environment and Characteristic=FirstHistoryRecordIndex
            ///</summary>
            public byte[] FirstHistoryRecordIndex 
            { 
                get { return _FirstHistoryRecordIndex; }
                set { if (value == _FirstHistoryRecordIndex) return; _FirstHistoryRecordIndex = value; OnPropertyChanged();}
            }

            private double _TemperatureUnits = 0;
            /// <summary>
            /// TemperatureUnits (U8 ) from Service=Environment and Characteristic=TemperatureUnits
            ///</summary>
            public double TemperatureUnits 
            { 
                get { return _TemperatureUnits; }
                set { if (value == _TemperatureUnits) return; _TemperatureUnits = value; OnPropertyChanged();}
            }

            private double _Temperature = 0;
            /// <summary>
            /// Temperature (U16 C) from Service=Environment and Characteristic=TempHumidity
            ///</summary>
            public double Temperature 
            { 
                get { return _Temperature; }
                set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged();}
            }
            private double _Humidity = 0;
            /// <summary>
            /// Humidity (U16 %) from Service=Environment and Characteristic=TempHumidity
            ///</summary>
            public double Humidity 
            { 
                get { return _Humidity; }
                set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged();}
            }
            private double _BatteryLevel = 0;
            /// <summary>
            /// BatteryLevel (U8 ) from Service=Environment and Characteristic=TempHumidity
            ///</summary>
            public double BatteryLevel 
            { 
                get { return _BatteryLevel; }
                set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged();}
            }

            private byte[] _CalibrationData = null;
            /// <summary>
            /// CalibrationData (BYTES ) from Service=Environment and Characteristic=TemperatureCalibration
            ///</summary>
            public byte[] CalibrationData 
            { 
                get { return _CalibrationData; }
                set { if (value == _CalibrationData) return; _CalibrationData = value; OnPropertyChanged();}
            }

            private double _param0 = 0;
            /// <summary>
            /// param0 (U8 ) from Service=Environment and Characteristic=Battery
            ///</summary>
            public double param0 
            { 
                get { return _param0; }
                set { if (value == _param0) return; _param0 = value; OnPropertyChanged();}
            }

            private byte[] _DisconnectDAta = null;
            /// <summary>
            /// DisconnectDAta (BYTES ) from Service=Environment and Characteristic=Disconnect
            ///</summary>
            public byte[] DisconnectDAta 
            { 
                get { return _DisconnectDAta; }
                set { if (value == _DisconnectDAta) return; _DisconnectDAta = value; OnPropertyChanged();}
            }

            private byte[] _MiHomeCommand = null;
            /// <summary>
            /// MiHomeCommand (BYTES ) from Service=Environment and Characteristic=MiHomeCommand
            ///</summary>
            public byte[] MiHomeCommand 
            { 
                get { return _MiHomeCommand; }
                set { if (value == _MiHomeCommand) return; _MiHomeCommand = value; OnPropertyChanged();}
            }

            private double _TimeMode = 0;
            /// <summary>
            /// TimeMode (U8 ) from Service=Environment and Characteristic=TimeMode
            ///</summary>
            public double TimeMode 
            { 
                get { return _TimeMode; }
                set { if (value == _TimeMode) return; _TimeMode = value; OnPropertyChanged();}
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
                dest.UnixTimestamp = source.UnixTimestamp;
                dest.TimezoneOffset = source.TimezoneOffset;
                dest.StoredEntries = source.StoredEntries;
                dest.SingleRecord = source.SingleRecord;
                dest.LastHourData = source.LastHourData;
                dest.FirstHistoryRecordIndex = source.FirstHistoryRecordIndex;
                dest.TemperatureUnits = source.TemperatureUnits;
                dest.Temperature = source.Temperature;
                dest.Humidity = source.Humidity;
                dest.BatteryLevel = source.BatteryLevel;
                dest.CalibrationData = source.CalibrationData;
                dest.param0 = source.param0;
                dest.DisconnectDAta = source.DisconnectDAta;
                dest.MiHomeCommand = source.MiHomeCommand;
                dest.TimeMode = source.TimeMode;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Environment_Data CopyToWithConvertAndCreate(Environment_Data source, Environment_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.UnixTimestamp = convert(source.UnixTimestamp, "");
                dest.TimezoneOffset = convert(source.TimezoneOffset, "");
                dest.StoredEntries = source.StoredEntries;
                dest.SingleRecord = source.SingleRecord;
                dest.LastHourData = source.LastHourData;
                dest.FirstHistoryRecordIndex = source.FirstHistoryRecordIndex;
                dest.TemperatureUnits = convert(source.TemperatureUnits, "");
                dest.Temperature = convert(source.Temperature, "C");
                dest.Humidity = convert(source.Humidity, "%");
                dest.BatteryLevel = convert(source.BatteryLevel, "");
                dest.CalibrationData = source.CalibrationData;
                dest.param0 = convert(source.param0, "");
                dest.DisconnectDAta = source.DisconnectDAta;
                dest.MiHomeCommand = source.MiHomeCommand;
                dest.TimeMode = convert(source.TimeMode, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["UnixTimestamp", "TimezoneOffset", "StoredEntries", "SingleRecord", "LastHourData", "FirstHistoryRecordIndex", "TemperatureUnits", "Temperature", "Humidity", "BatteryLevel", "CalibrationData", "param0", "DisconnectDAta", "MiHomeCommand", "TimeMode"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(UnixTimestamp);
                exporter.CellSet(TimezoneOffset);
                exporter.CellSet(StoredEntries);
                exporter.CellSet(SingleRecord);
                exporter.CellSet(LastHourData);
                exporter.CellSet(FirstHistoryRecordIndex);
                exporter.CellSet(TemperatureUnits);
                exporter.CellSet(Temperature);
                exporter.CellSet(Humidity);
                exporter.CellSet(BatteryLevel);
                exporter.CellSet(CalibrationData);
                exporter.CellSet(param0);
                exporter.CellSet(DisconnectDAta);
                exporter.CellSet(MiHomeCommand);
                exporter.CellSet(TimeMode);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {UnixTimestamp} {TimezoneOffset} {StoredEntries} {SingleRecord} {LastHourData} {FirstHistoryRecordIndex} {TemperatureUnits} {Temperature} {Humidity} {BatteryLevel} {CalibrationData} {param0} {DisconnectDAta} {MiHomeCommand} {TimeMode}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Common Configuration Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Common_Configuration_Data :BTCommonMetaData<Common_Configuration_Data> //, IExportDataSource
        {
            private string _Device_Name = "";
            /// <summary>
            /// Device_Name (STRING ) from Service=Common Configuration and Characteristic=Device Name
            ///</summary>
            public string Device_Name 
            { 
                get { return _Device_Name; }
                set { if (value == _Device_Name) return; _Device_Name = value; OnPropertyChanged();}
            }

            private double _Appearance = 0;
            /// <summary>
            /// Appearance (U16 ) from Service=Common Configuration and Characteristic=Appearance
            ///</summary>
            public double Appearance 
            { 
                get { return _Appearance; }
                set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged();}
            }

            private double _Interval_Min = -1;
            /// <summary>
            /// Interval_Min (U16 ms) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public double Interval_Min 
            { 
                get { return _Interval_Min; }
                set { if (value == _Interval_Min) return; _Interval_Min = value; OnPropertyChanged();}
            }
            private double _Interval_Max = 0;
            /// <summary>
            /// Interval_Max (U16 ms) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public double Interval_Max 
            { 
                get { return _Interval_Max; }
                set { if (value == _Interval_Max) return; _Interval_Max = value; OnPropertyChanged();}
            }
            private double _Latency = 0;
            /// <summary>
            /// Latency (U16 ms) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public double Latency 
            { 
                get { return _Latency; }
                set { if (value == _Latency) return; _Latency = value; OnPropertyChanged();}
            }
            private double _Timeout = 0;
            /// <summary>
            /// Timeout (U16 ms) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public double Timeout 
            { 
                get { return _Timeout; }
                set { if (value == _Timeout) return; _Timeout = value; OnPropertyChanged();}
            }
            public override Common_Configuration_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Common_Configuration_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Common_Configuration_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Device_Name = source.Device_Name;
                dest.Appearance = source.Appearance;
                dest.Interval_Min = source.Interval_Min;
                dest.Interval_Max = source.Interval_Max;
                dest.Latency = source.Latency;
                dest.Timeout = source.Timeout;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Common_Configuration_Data CopyToWithConvertAndCreate(Common_Configuration_Data source, Common_Configuration_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Device_Name = source.Device_Name;
                dest.Appearance = convert(source.Appearance, "");
                dest.Interval_Min = convert(source.Interval_Min, "ms");
                dest.Interval_Max = convert(source.Interval_Max, "ms");
                dest.Latency = convert(source.Latency, "ms");
                dest.Timeout = convert(source.Timeout, "ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "Interval_Min", "Interval_Max", "Latency", "Timeout"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(Interval_Min);
                exporter.CellSet(Interval_Max);
                exporter.CellSet(Latency);
                exporter.CellSet(Timeout);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {Interval_Min} {Interval_Max} {Latency} {Timeout}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Generic Service Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Generic_Service_Data :BTCommonMetaData<Generic_Service_Data> //, IExportDataSource
        {
            private double _StartRange = 0;
            /// <summary>
            /// StartRange (U16 ) from Service=Generic Service and Characteristic=Service Changes
            ///</summary>
            public double StartRange 
            { 
                get { return _StartRange; }
                set { if (value == _StartRange) return; _StartRange = value; OnPropertyChanged();}
            }
            private double _EndRange = 0;
            /// <summary>
            /// EndRange (U16 ) from Service=Generic Service and Characteristic=Service Changes
            ///</summary>
            public double EndRange 
            { 
                get { return _EndRange; }
                set { if (value == _EndRange) return; _EndRange = value; OnPropertyChanged();}
            }
            public override Generic_Service_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Generic_Service_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Generic_Service_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.StartRange = source.StartRange;
                dest.EndRange = source.EndRange;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Generic_Service_Data CopyToWithConvertAndCreate(Generic_Service_Data source, Generic_Service_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.StartRange = convert(source.StartRange, "");
                dest.EndRange = convert(source.EndRange, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["StartRange", "EndRange"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(StartRange);
                exporter.CellSet(EndRange);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {StartRange} {EndRange}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Device Info Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Device_Info_Data :BTCommonMetaData<Device_Info_Data> //, IExportDataSource
        {
            private double _VendorIDSource = 0;
            /// <summary>
            /// VendorIDSource (U8 ) from Service=Device Info and Characteristic=PnP ID
            ///</summary>
            public double VendorIDSource 
            { 
                get { return _VendorIDSource; }
                set { if (value == _VendorIDSource) return; _VendorIDSource = value; OnPropertyChanged();}
            }
            private double _VendorID = 0;
            /// <summary>
            /// VendorID (U16 ) from Service=Device Info and Characteristic=PnP ID
            ///</summary>
            public double VendorID 
            { 
                get { return _VendorID; }
                set { if (value == _VendorID) return; _VendorID = value; OnPropertyChanged();}
            }
            private double _ProductID = 0;
            /// <summary>
            /// ProductID (U16 ) from Service=Device Info and Characteristic=PnP ID
            ///</summary>
            public double ProductID 
            { 
                get { return _ProductID; }
                set { if (value == _ProductID) return; _ProductID = value; OnPropertyChanged();}
            }
            private double _ProductVersion = 0;
            /// <summary>
            /// ProductVersion (U16 ) from Service=Device Info and Characteristic=PnP ID
            ///</summary>
            public double ProductVersion 
            { 
                get { return _ProductVersion; }
                set { if (value == _ProductVersion) return; _ProductVersion = value; OnPropertyChanged();}
            }

            private string _SystemId = "";
            /// <summary>
            /// SystemId (STRING ) from Service=Device Info and Characteristic=System ID
            ///</summary>
            public string SystemId 
            { 
                get { return _SystemId; }
                set { if (value == _SystemId) return; _SystemId = value; OnPropertyChanged();}
            }

            private string _ModelNumber = "";
            /// <summary>
            /// ModelNumber (STRING ) from Service=Device Info and Characteristic=Model Number
            ///</summary>
            public string ModelNumber 
            { 
                get { return _ModelNumber; }
                set { if (value == _ModelNumber) return; _ModelNumber = value; OnPropertyChanged();}
            }

            private string _SerialNumber = "";
            /// <summary>
            /// SerialNumber (STRING ) from Service=Device Info and Characteristic=Serial Number
            ///</summary>
            public string SerialNumber 
            { 
                get { return _SerialNumber; }
                set { if (value == _SerialNumber) return; _SerialNumber = value; OnPropertyChanged();}
            }

            private string _FirmwareRevision = "";
            /// <summary>
            /// FirmwareRevision (STRING ) from Service=Device Info and Characteristic=Firmware Revision
            ///</summary>
            public string FirmwareRevision 
            { 
                get { return _FirmwareRevision; }
                set { if (value == _FirmwareRevision) return; _FirmwareRevision = value; OnPropertyChanged();}
            }

            private string _HardwareRevision = "";
            /// <summary>
            /// HardwareRevision (STRING ) from Service=Device Info and Characteristic=Hardware Revision
            ///</summary>
            public string HardwareRevision 
            { 
                get { return _HardwareRevision; }
                set { if (value == _HardwareRevision) return; _HardwareRevision = value; OnPropertyChanged();}
            }

            private string _SoftwareRevision = "";
            /// <summary>
            /// SoftwareRevision (STRING ) from Service=Device Info and Characteristic=Software Revision
            ///</summary>
            public string SoftwareRevision 
            { 
                get { return _SoftwareRevision; }
                set { if (value == _SoftwareRevision) return; _SoftwareRevision = value; OnPropertyChanged();}
            }
            public override Device_Info_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Device_Info_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Device_Info_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.VendorIDSource = source.VendorIDSource;
                dest.VendorID = source.VendorID;
                dest.ProductID = source.ProductID;
                dest.ProductVersion = source.ProductVersion;
                dest.SystemId = source.SystemId;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Device_Info_Data CopyToWithConvertAndCreate(Device_Info_Data source, Device_Info_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.VendorIDSource = convert(source.VendorIDSource, "");
                dest.VendorID = convert(source.VendorID, "");
                dest.ProductID = convert(source.ProductID, "");
                dest.ProductVersion = convert(source.ProductVersion, "");
                dest.SystemId = source.SystemId;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["VendorIDSource", "VendorID", "ProductID", "ProductVersion", "SystemId", "ModelNumber", "SerialNumber", "FirmwareRevision", "HardwareRevision", "SoftwareRevision"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(VendorIDSource);
                exporter.CellSet(VendorID);
                exporter.CellSet(ProductID);
                exporter.CellSet(ProductVersion);
                exporter.CellSet(SystemId);
                exporter.CellSet(ModelNumber);
                exporter.CellSet(SerialNumber);
                exporter.CellSet(FirmwareRevision);
                exporter.CellSet(HardwareRevision);
                exporter.CellSet(SoftwareRevision);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {VendorIDSource} {VendorID} {ProductID} {ProductVersion} {SystemId} {ModelNumber} {SerialNumber} {FirmwareRevision} {HardwareRevision} {SoftwareRevision}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Battery Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Battery_Data :BTCommonMetaData<Battery_Data> //, IExportDataSource
        {
            private double _BatteryLevel = 0;
            /// <summary>
            /// BatteryLevel (I8 %) from Service=Battery and Characteristic=BatteryLevel
            ///</summary>
            public double BatteryLevel 
            { 
                get { return _BatteryLevel; }
                set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged();}
            }
            public override Battery_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Battery_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Battery_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.BatteryLevel = source.BatteryLevel;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Battery_Data CopyToWithConvertAndCreate(Battery_Data source, Battery_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.BatteryLevel = convert(source.BatteryLevel, "%");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["BatteryLevel"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(BatteryLevel);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {BatteryLevel}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the TELink OTA Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class TELink_OTA_Data :BTCommonMetaData<TELink_OTA_Data> //, IExportDataSource
        {
            private byte[] _OTABytes = null;
            /// <summary>
            /// OTABytes (BYTES ) from Service=TELink OTA and Characteristic=OTABytes
            ///</summary>
            public byte[] OTABytes 
            { 
                get { return _OTABytes; }
                set { if (value == _OTABytes) return; _OTABytes = value; OnPropertyChanged();}
            }
            public override TELink_OTA_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as TELink_OTA_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(TELink_OTA_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.OTABytes = source.OTABytes;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static TELink_OTA_Data CopyToWithConvertAndCreate(TELink_OTA_Data source, TELink_OTA_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.OTABytes = source.OTABytes;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["OTABytes"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(OTABytes);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {OTABytes}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Unknown9 Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Unknown9_Data :BTCommonMetaData<Unknown9_Data> //, IExportDataSource
        {
            private byte[] _Unknown0 = null;
            /// <summary>
            /// Unknown0 (BYTES ) from Service=Unknown9 and Characteristic=Unknown0
            ///</summary>
            public byte[] Unknown0 
            { 
                get { return _Unknown0; }
                set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged();}
            }

            private byte[] _Unknown1 = null;
            /// <summary>
            /// Unknown1 (BYTES ) from Service=Unknown9 and Characteristic=Unknown1
            ///</summary>
            public byte[] Unknown1 
            { 
                get { return _Unknown1; }
                set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged();}
            }
            public override Unknown9_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Unknown9_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Unknown9_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Unknown0 = source.Unknown0;
                dest.Unknown1 = source.Unknown1;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Unknown9_Data CopyToWithConvertAndCreate(Unknown9_Data source, Unknown9_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Unknown0 = source.Unknown0;
                dest.Unknown1 = source.Unknown1;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Unknown0", "Unknown1"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Unknown0);
                exporter.CellSet(Unknown1);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Unknown0} {Unknown1}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Environment_index = 0,
            Common_Configuration_index = 1,
            Generic_Service_index = 2,
            Device_Info_index = 3,
            Battery_index = 4,
            TELink_OTA_index = 5,
            Unknown9_index = 6,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Environment_ClockTime_index = 0,     // GUID ebe0ccb7-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_StoredEntries_index = 1,     // GUID ebe0ccb9-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_SingleRecord_index = 2,     // GUID ebe0ccba-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_LastHourData_index = 3,     // GUID ebe0ccbb-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_FirstHistoryRecordIndex_index = 4,     // GUID ebe0ccbc-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_TemperatureUnits_index = 5,     // GUID ebe0ccbe-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_TempHumidity_index = 6,     // GUID ebe0ccc1-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_TemperatureCalibration_index = 7,     // GUID ebe0ccc2-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_Battery_index = 8,     // GUID ebe0ccc4-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_Disconnect_index = 9,     // GUID ebe0ccc8-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_MiHomeCommand_index = 10,     // GUID ebe0ccd4-7a0a-4b0c-8a1a-6ff2997da3a6
            Environment_TimeMode_index = 11,     // GUID ebe0cce4-7a0a-4b0c-8a1a-6ff2997da3a6
            Common_Configuration_Device_Name_index = 12,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 13,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 14,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Generic_Service_Service_Changes_index = 15,     // GUID 00002a05-0000-1000-8000-00805f9b34fb
            Device_Info_PnP_ID_index = 16,     // GUID 00002a50-0000-1000-8000-00805f9b34fb
            Device_Info_System_ID_index = 17,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
            Device_Info_Model_Number_index = 18,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Info_Serial_Number_index = 19,     // GUID 00002a25-0000-1000-8000-00805f9b34fb
            Device_Info_Firmware_Revision_index = 20,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Info_Hardware_Revision_index = 21,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Info_Software_Revision_index = 22,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Battery_BatteryLevel_index = 23,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
            TELink_OTA_OTABytes_index = 24,     // GUID 00010203-0405-0607-0809-0a0b0c0d2b12
            Unknown9_Unknown0_index = 25,     // GUID 00000101-0065-6c62-2e74-6f696d2e696d
            Unknown9_Unknown1_index = 26,     // GUID 00000102-0065-6c62-2e74-6f696d2e696d
        }

        // All of the services that this device supports
        /// <summary>
        /// Convenience GUID for Environment service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Environment = Guid.Parse("ebe0ccb0-7a0a-4b0c-8a1a-6ff2997da3a6"); // #0 is Environment
        /// <summary>
        /// Convenience GUID for Common Configuration service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Common_Configuration = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"); // #1 is Common Configuration
        /// <summary>
        /// Convenience GUID for Generic Service service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Generic_Service = Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"); // #2 is Generic Service
        /// <summary>
        /// Convenience GUID for Device Info service. Includes details on the manufacturer, model, firmware versions, and PNP ID
        /// </summary>
        public static readonly Guid ServiceGuid_Device_Info = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"); // #3 is Device Info
        /// <summary>
        /// Convenience GUID for Battery service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Battery = Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"); // #4 is Battery
        /// <summary>
        /// Convenience GUID for TELink OTA service. 
        /// </summary>
        public static readonly Guid ServiceGuid_TELink_OTA = Guid.Parse("00010203-0405-0607-0809-0a0b0c0d1912"); // #5 is TELink OTA
        /// <summary>
        /// Convenience GUID for Unknown9 service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Unknown9 = Guid.Parse("00000100-0065-6c62-2e74-6f696d2e696d"); // #6 is Unknown9        

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("ebe0ccb0-7a0a-4b0c-8a1a-6ff2997da3a6"), // #0 is Environment
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #1 is Common Configuration
            Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"), // #2 is Generic Service
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #3 is Device Info
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // #4 is Battery
            Guid.Parse("00010203-0405-0607-0809-0a0b0c0d1912"), // #5 is TELink OTA
            Guid.Parse("00000100-0065-6c62-2e74-6f696d2e696d"), // #6 is Unknown9
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, null, null, null, null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("ebe0ccb7-7a0a-4b0c-8a1a-6ff2997da3a6"), // #0 is Environment ClockTime
            Guid.Parse("ebe0ccb9-7a0a-4b0c-8a1a-6ff2997da3a6"), // #1 is Environment StoredEntries
            Guid.Parse("ebe0ccba-7a0a-4b0c-8a1a-6ff2997da3a6"), // #2 is Environment SingleRecord
            Guid.Parse("ebe0ccbb-7a0a-4b0c-8a1a-6ff2997da3a6"), // #3 is Environment LastHourData
            Guid.Parse("ebe0ccbc-7a0a-4b0c-8a1a-6ff2997da3a6"), // #4 is Environment FirstHistoryRecordIndex
            Guid.Parse("ebe0ccbe-7a0a-4b0c-8a1a-6ff2997da3a6"), // #5 is Environment TemperatureUnits
            Guid.Parse("ebe0ccc1-7a0a-4b0c-8a1a-6ff2997da3a6"), // #6 is Environment TempHumidity
            Guid.Parse("ebe0ccc2-7a0a-4b0c-8a1a-6ff2997da3a6"), // #7 is Environment TemperatureCalibration
            Guid.Parse("ebe0ccc4-7a0a-4b0c-8a1a-6ff2997da3a6"), // #8 is Environment Battery
            Guid.Parse("ebe0ccc8-7a0a-4b0c-8a1a-6ff2997da3a6"), // #9 is Environment Disconnect
            Guid.Parse("ebe0ccd4-7a0a-4b0c-8a1a-6ff2997da3a6"), // #10 is Environment MiHomeCommand
            Guid.Parse("ebe0cce4-7a0a-4b0c-8a1a-6ff2997da3a6"), // #11 is Environment TimeMode
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #12 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #13 is Common Configuration Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #14 is Common Configuration Connection Parameter
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #15 is Generic Service Service Changes
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #16 is Device Info PnP ID
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #17 is Device Info System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #18 is Device Info Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #19 is Device Info Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #20 is Device Info Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #21 is Device Info Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #22 is Device Info Software Revision
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #23 is Battery BatteryLevel
            Guid.Parse("00010203-0405-0607-0809-0a0b0c0d2b12"), // #24 is TELink OTA OTABytes
            Guid.Parse("00000101-0065-6c62-2e74-6f696d2e696d"), // #25 is Unknown9 Unknown0
            Guid.Parse("00000102-0065-6c62-2e74-6f696d2e696d"), // #26 is Unknown9 Unknown1
        };

        private List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };


        /// <summary>
        /// Delegate for all Notify events. this is specific to this device (the indexes are all for this device only)
        /// but otherwise is generic.
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(IotNumberFormats.ValueParserResult data);

        async Task<bool> Ensure_Characteristic_Async(ServiceIndex serviceIndex, string serviceName, CharacteristicIndex characteristicIndex, string characteristicName)
        {
            if (Characteristics[(int)characteristicIndex] == null)
            {
                if (ble == null) return false;
                if (Services[(int)serviceIndex] == null)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(Service_Guids[(int)serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {serviceName}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {serviceName}", serviceStatus);
                        return false;
                    }
                    Services[(int)serviceIndex] = serviceStatus.Services[0];
                }
                var service = Services[(int)serviceIndex];
                var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(Characteristic_Guids[(int)characteristicIndex]);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"unable to get characteristic for {characteristicName}", characteristicsStatus);
                    return false;
                }
                if (characteristicsStatus.Characteristics.Count == 0)
                {
                    Status.ReportStatus($"unable to get any characteristics for {characteristicName}", characteristicsStatus);
                    return false;
                }
                else if (characteristicsStatus.Characteristics.Count != 1)
                {
                    Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {characteristicName}", characteristicsStatus);
                    return false;
                }
                Characteristics[(int)characteristicIndex] = characteristicsStatus.Characteristics[0];
            }
            return true;
        }


        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(GattCharacteristic ch, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await ch.ReadValueAsync(cacheMode);
                if (readResult.Status == GattCommunicationStatus.Success)
                {
                    buffer = readResult.Value;
                }
                else
                {
                    // NOTE: reset the characteristics array?
                }
                Status.ReportStatus(method, readResult.Status);
            }
            catch (Exception)
            {
                Status.ReportStatus(method, GattCommunicationStatus.Unreachable);
                // NOTE: reset the characteristics array?
            }
            return buffer;
        }


        private async Task<bool> SetupNotifyAsync(string name, 
            ServiceIndex serviceIndex, string serviceName, CharacteristicIndex index, 
            Windows.Foundation.TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> callback,
            GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            await Ensure_Characteristic_Async(serviceIndex, serviceName, index, name);
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return false;
            }
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyCharacteristic_ValueChanged_set[(int)index])
                {
                    // Only set the event callback once
                    NotifyCharacteristic_ValueChanged_set[(int)index] = true;
                    ch.ValueChanged += callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"Notify{name}: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"Notify{name}: set notification", result);

            return true;
        }

        //
        //
        // Start of the service + characteristic
        //
        //


        //
        // All services / characteristics methods. 
        //


        #region Service_Environment
        // Service Environment 

        public Environment_Data CurrEnvironment_Data { get; set; } = new Environment_Data();

        // Per-characteristics methods for Environment ClockTime
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyClockTimeAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("ClockTime", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_ClockTime_index, NotifyClockTimeCallback, notifyType);
            return retval;
        }

        private void NotifyClockTimeCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_ClockTime_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U32|DEC|UnixTimestamp U16|DEC|TimezoneOffset");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.UnixTimestamp = vr.GetNextDouble();
            CurrEnvironment_Data.TimezoneOffset = vr.GetNextDouble();
            OnPropertyChanged(ClockTimePropertyChangedName); // "ClockTime"
        }
        // Per-characteristics methods for Environment StoredEntries
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyStoredEntriesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("StoredEntries", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_StoredEntries_index, NotifyStoredEntriesCallback, notifyType);
            return retval;
        }

        private void NotifyStoredEntriesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_StoredEntries_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|StoredEntries");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.StoredEntries = vr.GetNextByteArray();
            OnPropertyChanged(StoredEntriesPropertyChangedName); // "StoredEntries"
        }
        // Per-characteristics methods for Environment SingleRecord
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySingleRecordAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("SingleRecord", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_SingleRecord_index, NotifySingleRecordCallback, notifyType);
            return retval;
        }

        private void NotifySingleRecordCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_SingleRecord_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|SingleRecord");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.SingleRecord = vr.GetNextByteArray();
            OnPropertyChanged(SingleRecordPropertyChangedName); // "SingleRecord"
        }
        // Per-characteristics methods for Environment LastHourData
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyLastHourDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("LastHourData", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_LastHourData_index, NotifyLastHourDataCallback, notifyType);
            return retval;
        }

        private void NotifyLastHourDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_LastHourData_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|LastHourData");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.LastHourData = vr.GetNextByteArray();
            OnPropertyChanged(LastHourDataPropertyChangedName); // "LastHourData"
        }
        // Per-characteristics methods for Environment FirstHistoryRecordIndex
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFirstHistoryRecordIndexAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FirstHistoryRecordIndex", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_FirstHistoryRecordIndex_index, NotifyFirstHistoryRecordIndexCallback, notifyType);
            return retval;
        }

        private void NotifyFirstHistoryRecordIndexCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_FirstHistoryRecordIndex_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|FirstHistoryRecordIndex");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.FirstHistoryRecordIndex = vr.GetNextByteArray();
            OnPropertyChanged(FirstHistoryRecordIndexPropertyChangedName); // "FirstHistoryRecordIndex"
        }
        // Per-characteristics methods for Environment TemperatureUnits
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTemperatureUnitsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("TemperatureUnits", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_TemperatureUnits_index, NotifyTemperatureUnitsCallback, notifyType);
            return retval;
        }

        private void NotifyTemperatureUnitsCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_TemperatureUnits_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|TemperatureUnits");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.TemperatureUnits = vr.GetNextDouble();
            OnPropertyChanged(TemperatureUnitsPropertyChangedName); // "TemperatureUnits"
        }
        // Per-characteristics methods for Environment TempHumidity
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTempHumidityAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("TempHumidity", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_TempHumidity_index, NotifyTempHumidityCallback, notifyType);
            return retval;
        }

        private void NotifyTempHumidityCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_TempHumidity_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16^100_/|DEC|Temperature|C U16^512_/|DEC|Humidity|% U8|DEC|BatteryLevel");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.Temperature = vr.GetNextDouble();
            CurrEnvironment_Data.Humidity = vr.GetNextDouble();
            CurrEnvironment_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(TempHumidityPropertyChangedName); // "TempHumidity"
        }
        // Per-characteristics methods for Environment TemperatureCalibration
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTemperatureCalibrationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("TemperatureCalibration", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_TemperatureCalibration_index, NotifyTemperatureCalibrationCallback, notifyType);
            return retval;
        }

        private void NotifyTemperatureCalibrationCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_TemperatureCalibration_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrationData");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.CalibrationData = vr.GetNextByteArray();
            OnPropertyChanged(TemperatureCalibrationPropertyChangedName); // "TemperatureCalibration"
        }
        // Per-characteristics methods for Environment Battery
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBatteryAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Battery", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Battery_index, NotifyBatteryCallback, notifyType);
            return retval;
        }

        private void NotifyBatteryCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Battery_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|BatteryLevel100");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.param0 = vr.GetNextDouble();
            OnPropertyChanged(BatteryPropertyChangedName); // "Battery"
        }
        // Per-characteristics methods for Environment Disconnect
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDisconnectAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Disconnect", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Disconnect_index, NotifyDisconnectCallback, notifyType);
            return retval;
        }

        private void NotifyDisconnectCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Disconnect_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|DisconnectDAta");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.DisconnectDAta = vr.GetNextByteArray();
            OnPropertyChanged(DisconnectPropertyChangedName); // "Disconnect"
        }
        // Per-characteristics methods for Environment MiHomeCommand
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMiHomeCommandAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("MiHomeCommand", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_MiHomeCommand_index, NotifyMiHomeCommandCallback, notifyType);
            return retval;
        }

        private void NotifyMiHomeCommandCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_MiHomeCommand_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|MiHomeCommand");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.MiHomeCommand = vr.GetNextByteArray();
            OnPropertyChanged(MiHomeCommandPropertyChangedName); // "MiHomeCommand"
        }
        // Per-characteristics methods for Environment TimeMode
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTimeModeAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("TimeMode", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_TimeMode_index, NotifyTimeModeCallback, notifyType);
            return retval;
        }

        private void NotifyTimeModeCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_TimeMode_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|TimeMode");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.TimeMode = vr.GetNextDouble();
            OnPropertyChanged(TimeModePropertyChangedName); // "TimeMode"
        }
        /// <summary>
        /// Reads data from ClockTime and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadClockTime(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_ClockTime_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "ClockTime");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "ClockTime", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U32|DEC|UnixTimestamp U16|DEC|TimezoneOffset");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.UnixTimestamp = vr.GetNextDouble();
            CurrEnvironment_Data.TimezoneOffset = vr.GetNextDouble();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(ClockTimePropertyChangedName); // "ClockTime"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from StoredEntries and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadStoredEntries(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_StoredEntries_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "StoredEntries");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "StoredEntries", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|StoredEntries");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.StoredEntries = vr.GetNextByteArray();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(StoredEntriesPropertyChangedName); // "StoredEntries"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from SingleRecord and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadSingleRecord(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_SingleRecord_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "SingleRecord");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "SingleRecord", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|SingleRecord");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.SingleRecord = vr.GetNextByteArray();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(SingleRecordPropertyChangedName); // "SingleRecord"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from LastHourData and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadLastHourData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_LastHourData_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "LastHourData");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "LastHourData", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|LastHourData");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.LastHourData = vr.GetNextByteArray();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(LastHourDataPropertyChangedName); // "LastHourData"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from FirstHistoryRecordIndex and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadFirstHistoryRecordIndex(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_FirstHistoryRecordIndex_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "FirstHistoryRecordIndex");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FirstHistoryRecordIndex", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|FirstHistoryRecordIndex");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.FirstHistoryRecordIndex = vr.GetNextByteArray();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FirstHistoryRecordIndexPropertyChangedName); // "FirstHistoryRecordIndex"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from TemperatureUnits and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadTemperatureUnits(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_TemperatureUnits_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "TemperatureUnits");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "TemperatureUnits", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|TemperatureUnits");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.TemperatureUnits = vr.GetNextDouble();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(TemperatureUnitsPropertyChangedName); // "TemperatureUnits"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from TempHumidity and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadTempHumidity(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_TempHumidity_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "TempHumidity");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "TempHumidity", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16^100_/|DEC|Temperature|C U16^512_/|DEC|Humidity|% U8|DEC|BatteryLevel");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.Temperature = vr.GetNextDouble();
            CurrEnvironment_Data.Humidity = vr.GetNextDouble();
            CurrEnvironment_Data.BatteryLevel = vr.GetNextDouble();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(TempHumidityPropertyChangedName); // "TempHumidity"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from TemperatureCalibration and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadTemperatureCalibration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_TemperatureCalibration_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "TemperatureCalibration");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "TemperatureCalibration", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrationData");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.CalibrationData = vr.GetNextByteArray();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(TemperatureCalibrationPropertyChangedName); // "TemperatureCalibration"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from Battery and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadBattery(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Battery_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Battery");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Battery", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|BatteryLevel100");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.param0 = vr.GetNextDouble();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(BatteryPropertyChangedName); // "Battery"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from Disconnect and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadDisconnect(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Disconnect_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Disconnect");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Disconnect", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|DisconnectDAta");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.DisconnectDAta = vr.GetNextByteArray();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(DisconnectPropertyChangedName); // "Disconnect"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from MiHomeCommand and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadMiHomeCommand(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_MiHomeCommand_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "MiHomeCommand");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "MiHomeCommand", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|MiHomeCommand");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.MiHomeCommand = vr.GetNextByteArray();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(MiHomeCommandPropertyChangedName); // "MiHomeCommand"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Reads data from TimeMode and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadTimeMode(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_TimeMode_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "TimeMode");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "TimeMode", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|TimeMode");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.TimeMode = vr.GetNextDouble();
            CurrEnvironment_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(TimeModePropertyChangedName); // "TimeMode"
            return CurrEnvironment_Data;
        }
        /// <summary>
        /// Writes data to ClockTime 
        /// </summary>
        public async Task WriteClockTime(byte[] data)
        {
            var index = CharacteristicIndex.Environment_ClockTime_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "ClockTime");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteClockTime", result);
        }
        /// <summary>
        /// Writes data to SingleRecord 
        /// </summary>
        public async Task WriteSingleRecord(byte[] data)
        {
            var index = CharacteristicIndex.Environment_SingleRecord_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "SingleRecord");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteSingleRecord", result);
        }
        /// <summary>
        /// Writes data to TemperatureUnits 
        /// </summary>
        public async Task WriteTemperatureUnits(byte[] data)
        {
            var index = CharacteristicIndex.Environment_TemperatureUnits_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "TemperatureUnits");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteTemperatureUnits", result);
        }
        /// <summary>
        /// Writes data to TemperatureCalibration 
        /// </summary>
        public async Task WriteTemperatureCalibration(byte[] data)
        {
            var index = CharacteristicIndex.Environment_TemperatureCalibration_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "TemperatureCalibration");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteTemperatureCalibration", result);
        }
        /// <summary>
        /// Writes data to Disconnect 
        /// </summary>
        public async Task WriteDisconnect(byte[] data)
        {
            var index = CharacteristicIndex.Environment_Disconnect_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Disconnect");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteDisconnect", result);
        }
        /// <summary>
        /// Writes data to MiHomeCommand 
        /// </summary>
        public async Task WriteMiHomeCommand(byte[] data)
        {
            var index = CharacteristicIndex.Environment_MiHomeCommand_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "MiHomeCommand");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteMiHomeCommand", result);
        }
        /// <summary>
        /// Writes data to TimeMode 
        /// </summary>
        public async Task WriteTimeMode(byte[] data)
        {
            var index = CharacteristicIndex.Environment_TimeMode_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "TimeMode");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteTimeMode", result);
        }

        #endregion
//
        #region Service_Common_Configuration
        // Service Common Configuration 

        public Common_Configuration_Data CurrCommon_Configuration_Data { get; set; } = new Common_Configuration_Data();

        // Per-characteristics methods for Common_Configuration Device_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDevice_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Device_Name", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Device_Name_index, NotifyDevice_NameCallback, notifyType);
            return retval;
        }

        private void NotifyDevice_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Device_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Device_Name");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Device_Name = vr.GetNextString();
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
        }
        // Per-characteristics methods for Common_Configuration Appearance
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAppearanceAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Appearance", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Appearance_index, NotifyAppearanceCallback, notifyType);
            return retval;
        }

        private void NotifyAppearanceCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Appearance_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Appearance = vr.GetNextDouble();
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
        }
        // Per-characteristics methods for Common_Configuration Connection_Parameter
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyConnection_ParameterAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Connection_Parameter", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Connection_Parameter_index, NotifyConnection_ParameterCallback, notifyType);
            return retval;
        }

        private void NotifyConnection_ParameterCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Connection_Parameter_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16^1.25_*|DEC|Interval_Min|ms|-1 U16^1.25_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Interval_Min = vr.GetNextDouble();
            CurrCommon_Configuration_Data.Interval_Max = vr.GetNextDouble();
            CurrCommon_Configuration_Data.Latency = vr.GetNextDouble();
            CurrCommon_Configuration_Data.Timeout = vr.GetNextDouble();
            OnPropertyChanged(Connection_ParameterPropertyChangedName); // "Connection_Parameter"
        }
        /// <summary>
        /// Reads data from Device Name and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadDevice_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Device Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Device Name", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Device_Name");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Device_Name = vr.GetNextString();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Reads data from Appearance and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadAppearance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Appearance_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Appearance");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Appearance", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Appearance = vr.GetNextDouble();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Reads data from Connection Parameter and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadConnection_Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Connection_Parameter_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Connection Parameter");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Connection Parameter", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16^1.25_*|DEC|Interval_Min|ms|-1 U16^1.25_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Interval_Min = vr.GetNextDouble();
            CurrCommon_Configuration_Data.Interval_Max = vr.GetNextDouble();
            CurrCommon_Configuration_Data.Latency = vr.GetNextDouble();
            CurrCommon_Configuration_Data.Timeout = vr.GetNextDouble();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Connection_ParameterPropertyChangedName); // "Connection_Parameter"
            return CurrCommon_Configuration_Data;
        }

        #endregion
//
        #region Service_Generic_Service
        // Service Generic Service 

        public Generic_Service_Data CurrGeneric_Service_Data { get; set; } = new Generic_Service_Data();

        // Per-characteristics methods for Generic_Service Service_Changes
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyService_ChangesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Service_Changes", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Service_Changes_index, NotifyService_ChangesCallback, notifyType);
            return retval;
        }

        private void NotifyService_ChangesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Service_Changes_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|StartRange U16|DEC|EndRange");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.StartRange = vr.GetNextDouble();
            CurrGeneric_Service_Data.EndRange = vr.GetNextDouble();
            OnPropertyChanged(Service_ChangesPropertyChangedName); // "Service_Changes"
        }
        /// <summary>
        /// Reads data from Service Changes and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadService_Changes(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Service_Changes_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Service Changes");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Service Changes", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|StartRange U16|DEC|EndRange");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.StartRange = vr.GetNextDouble();
            CurrGeneric_Service_Data.EndRange = vr.GetNextDouble();
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Service_ChangesPropertyChangedName); // "Service_Changes"
            return CurrGeneric_Service_Data;
        }

        #endregion
//
        #region Service_Device_Info
        // Service Device Info 

        public Device_Info_Data CurrDevice_Info_Data { get; set; } = new Device_Info_Data();

        // Per-characteristics methods for Device_Info PnP_ID
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPnP_IDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("PnP_ID", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_PnP_ID_index, NotifyPnP_IDCallback, notifyType);
            return retval;
        }

        private void NotifyPnP_IDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_PnP_ID_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|VendorIDSource U16|DEC|VendorID U16|DEC|ProductID U16|DEC|ProductVersion");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.VendorIDSource = vr.GetNextDouble();
            CurrDevice_Info_Data.VendorID = vr.GetNextDouble();
            CurrDevice_Info_Data.ProductID = vr.GetNextDouble();
            CurrDevice_Info_Data.ProductVersion = vr.GetNextDouble();
            OnPropertyChanged(PnP_IDPropertyChangedName); // "PnP_ID"
        }
        // Per-characteristics methods for Device_Info System_ID
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySystem_IDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("System_ID", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_System_ID_index, NotifySystem_IDCallback, notifyType);
            return retval;
        }

        private void NotifySystem_IDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_System_ID_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SystemId");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.SystemId = vr.GetNextString();
            OnPropertyChanged(System_IDPropertyChangedName); // "System_ID"
        }
        // Per-characteristics methods for Device_Info Model_Number
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyModel_NumberAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Model_Number", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Model_Number_index, NotifyModel_NumberCallback, notifyType);
            return retval;
        }

        private void NotifyModel_NumberCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Model_Number_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.ModelNumber = vr.GetNextString();
            OnPropertyChanged(Model_NumberPropertyChangedName); // "Model_Number"
        }
        // Per-characteristics methods for Device_Info Serial_Number
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySerial_NumberAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Serial_Number", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Serial_Number_index, NotifySerial_NumberCallback, notifyType);
            return retval;
        }

        private void NotifySerial_NumberCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Serial_Number_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SerialNumber");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.SerialNumber = vr.GetNextString();
            OnPropertyChanged(Serial_NumberPropertyChangedName); // "Serial_Number"
        }
        // Per-characteristics methods for Device_Info Firmware_Revision
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFirmware_RevisionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Firmware_Revision", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Firmware_Revision_index, NotifyFirmware_RevisionCallback, notifyType);
            return retval;
        }

        private void NotifyFirmware_RevisionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Firmware_Revision_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|FirmwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.FirmwareRevision = vr.GetNextString();
            OnPropertyChanged(Firmware_RevisionPropertyChangedName); // "Firmware_Revision"
        }
        // Per-characteristics methods for Device_Info Hardware_Revision
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHardware_RevisionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Hardware_Revision", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Hardware_Revision_index, NotifyHardware_RevisionCallback, notifyType);
            return retval;
        }

        private void NotifyHardware_RevisionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Hardware_Revision_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|HardwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.HardwareRevision = vr.GetNextString();
            OnPropertyChanged(Hardware_RevisionPropertyChangedName); // "Hardware_Revision"
        }
        // Per-characteristics methods for Device_Info Software_Revision
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySoftware_RevisionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Software_Revision", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Software_Revision_index, NotifySoftware_RevisionCallback, notifyType);
            return retval;
        }

        private void NotifySoftware_RevisionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Software_Revision_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.SoftwareRevision = vr.GetNextString();
            OnPropertyChanged(Software_RevisionPropertyChangedName); // "Software_Revision"
        }
        /// <summary>
        /// Reads data from PnP ID and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadPnP_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_PnP_ID_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "PnP ID");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "PnP ID", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|VendorIDSource U16|DEC|VendorID U16|DEC|ProductID U16|DEC|ProductVersion");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.VendorIDSource = vr.GetNextDouble();
            CurrDevice_Info_Data.VendorID = vr.GetNextDouble();
            CurrDevice_Info_Data.ProductID = vr.GetNextDouble();
            CurrDevice_Info_Data.ProductVersion = vr.GetNextDouble();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(PnP_IDPropertyChangedName); // "PnP_ID"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from System ID and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadSystem_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_System_ID_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "System ID");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "System ID", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SystemId");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.SystemId = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(System_IDPropertyChangedName); // "System_ID"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Model Number and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadModel_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Model_Number_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Model Number");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Model Number", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.ModelNumber = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Model_NumberPropertyChangedName); // "Model_Number"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Serial Number and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadSerial_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Serial_Number_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Serial Number");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Serial Number", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SerialNumber");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.SerialNumber = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Serial_NumberPropertyChangedName); // "Serial_Number"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Firmware Revision and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadFirmware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Firmware_Revision_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Firmware Revision");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Firmware Revision", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|FirmwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.FirmwareRevision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Firmware_RevisionPropertyChangedName); // "Firmware_Revision"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Hardware Revision and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadHardware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Hardware_Revision_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Hardware Revision");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Hardware Revision", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|HardwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.HardwareRevision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Hardware_RevisionPropertyChangedName); // "Hardware_Revision"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Software Revision and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadSoftware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Software_Revision_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Software Revision");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Software Revision", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.SoftwareRevision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Software_RevisionPropertyChangedName); // "Software_Revision"
            return CurrDevice_Info_Data;
        }

        #endregion
//
        #region Service_Battery
        // Service Battery 

        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

        // Per-characteristics methods for Battery BatteryLevel
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBatteryLevelAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("BatteryLevel", ServiceIndex.Battery_index, "Battery", CharacteristicIndex.Battery_BatteryLevel_index, NotifyBatteryLevelCallback, notifyType);
            return retval;
        }

        private void NotifyBatteryLevelCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Battery_BatteryLevel_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.TimestampMostRecent = args.Timestamp;
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
        }
        /// <summary>
        /// Reads data from BatteryLevel and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Battery_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadBatteryLevel(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_BatteryLevel_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "BatteryLevel");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "BatteryLevel", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            CurrBattery_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
            return CurrBattery_Data;
        }

        #endregion
//
        #region Service_TELink_OTA
        // Service TELink OTA 

        public TELink_OTA_Data CurrTELink_OTA_Data { get; set; } = new TELink_OTA_Data();

        // Per-characteristics methods for TELink_OTA OTABytes
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyOTABytesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("OTABytes", ServiceIndex.TELink_OTA_index, "TELink OTA", CharacteristicIndex.TELink_OTA_OTABytes_index, NotifyOTABytesCallback, notifyType);
            return retval;
        }

        private void NotifyOTABytesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.TELink_OTA_OTABytes_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|OTABytes");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTELink_OTA_Data.TimestampMostRecent = args.Timestamp;
            CurrTELink_OTA_Data.OTABytes = vr.GetNextByteArray();
            OnPropertyChanged(OTABytesPropertyChangedName); // "OTABytes"
        }
        /// <summary>
        /// Reads data from OTABytes and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>TELink_OTA_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<TELink_OTA_Data> ReadOTABytes(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.TELink_OTA_OTABytes_index;
            await Ensure_Characteristic_Async(ServiceIndex.TELink_OTA_index, "TELink OTA", index, "OTABytes");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "OTABytes", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|OTABytes");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTELink_OTA_Data.OTABytes = vr.GetNextByteArray();
            CurrTELink_OTA_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(OTABytesPropertyChangedName); // "OTABytes"
            return CurrTELink_OTA_Data;
        }
        /// <summary>
        /// Writes data to OTABytes 
        /// </summary>
        public async Task WriteOTABytes(byte[] data)
        {
            var index = CharacteristicIndex.TELink_OTA_OTABytes_index;
            await Ensure_Characteristic_Async(ServiceIndex.TELink_OTA_index, "TELink OTA", index, "OTABytes");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteOTABytes", result);
        }

        #endregion
//
        #region Service_Unknown9
        // Service Unknown9 

        public Unknown9_Data CurrUnknown9_Data { get; set; } = new Unknown9_Data();

        // Per-characteristics methods for Unknown9 Unknown0
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyUnknown0Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Unknown0", ServiceIndex.Unknown9_index, "Unknown9", CharacteristicIndex.Unknown9_Unknown0_index, NotifyUnknown0Callback, notifyType);
            return retval;
        }

        private void NotifyUnknown0Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Unknown9_Unknown0_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrUnknown9_Data.TimestampMostRecent = args.Timestamp;
            CurrUnknown9_Data.Unknown0 = vr.GetNextByteArray();
            OnPropertyChanged(Unknown0PropertyChangedName); // "Unknown0"
        }
        // Per-characteristics methods for Unknown9 Unknown1
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyUnknown1Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Unknown1", ServiceIndex.Unknown9_index, "Unknown9", CharacteristicIndex.Unknown9_Unknown1_index, NotifyUnknown1Callback, notifyType);
            return retval;
        }

        private void NotifyUnknown1Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Unknown9_Unknown1_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrUnknown9_Data.TimestampMostRecent = args.Timestamp;
            CurrUnknown9_Data.Unknown1 = vr.GetNextByteArray();
            OnPropertyChanged(Unknown1PropertyChangedName); // "Unknown1"
        }
        /// <summary>
        /// Reads data from Unknown0 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Unknown9_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Unknown9_Data> ReadUnknown0(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Unknown9_Unknown0_index;
            await Ensure_Characteristic_Async(ServiceIndex.Unknown9_index, "Unknown9", index, "Unknown0");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Unknown0", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrUnknown9_Data.Unknown0 = vr.GetNextByteArray();
            CurrUnknown9_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Unknown0PropertyChangedName); // "Unknown0"
            return CurrUnknown9_Data;
        }
        /// <summary>
        /// Reads data from Unknown1 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Unknown9_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Unknown9_Data> ReadUnknown1(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Unknown9_Unknown1_index;
            await Ensure_Characteristic_Async(ServiceIndex.Unknown9_index, "Unknown9", index, "Unknown1");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Unknown1", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrUnknown9_Data.Unknown1 = vr.GetNextByteArray();
            CurrUnknown9_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Unknown1PropertyChangedName); // "Unknown1"
            return CurrUnknown9_Data;
        }
        /// <summary>
        /// Writes data to Unknown0 
        /// </summary>
        public async Task WriteUnknown0(byte[] data)
        {
            var index = CharacteristicIndex.Unknown9_Unknown0_index;
            await Ensure_Characteristic_Async(ServiceIndex.Unknown9_index, "Unknown9", index, "Unknown0");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteUnknown0", result);
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}