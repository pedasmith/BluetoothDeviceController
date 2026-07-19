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
    /// .
    /// This class was automatically generated 2026-07-19::09:02
    /// </summary>

    public  class BTStandard_HeartRate : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // No links for this device

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "Powrlabs_0759864";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device Powrlabs_0759864

        Heart Rate service Guid=180d
            Heart Rate_Data (DataGroup record)
                Heart Rate Measurement characteristic has Flags (Byte-->double) HeartRateLowRange (Byte-->double) HeartRateHighRange (UInt16-->double) EnergyExpended (UInt16-->double) RRInterval (UInt16-->double[])  Guid=2a37
                Body Sensor Location characteristic has SensorLocation (Byte-->double)  Guid=2a38


        GAP service Guid=1800
            GAP_Data (DataGroup record)
                Device Name characteristic has DeviceName (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Peripheral Preferred Connection Parameters characteristic has ConnectionParameters (Bytes-->string)  Guid=2a04


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                Transmit Power characteristic has TransmitPower (SByte-->double)  Guid=2a07
                BatteryLevel characteristic has BatteryLevel (SByte-->double)  Guid=2a19


        Device Information service Guid=180a
            Device Information_Data (DataGroup record)
                Manufacturer Name String characteristic has Manufacturer (String-->string)  Guid=2a29
                Model Number String characteristic has ModelNumber (String-->string)  Guid=2a24
                Hardware Revision String characteristic has HardwareRevision (String-->string)  Guid=2a27
                Firmware Revision String characteristic has FirmwareRevision (String-->string)  Guid=2a26
                Software Revision String characteristic has SoftwareRevision (String-->string)  Guid=2a28
                System ID characteristic has SystemID (Bytes-->string)  Guid=2a23
        */

        public const string Heart_Rate_MeasurementPropertyChangedName = "Heart_Rate_Measurement";
        public const string Body_Sensor_LocationPropertyChangedName = "Body_Sensor_Location";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Peripheral_Preferred_Connection_ParametersPropertyChangedName = "Peripheral_Preferred_Connection_Parameters";
        public const string Transmit_PowerPropertyChangedName = "Transmit_Power";
        public const string BatteryLevelPropertyChangedName = "BatteryLevel";
        public const string Manufacturer_Name_StringPropertyChangedName = "Manufacturer_Name_String";
        public const string Model_Number_StringPropertyChangedName = "Model_Number_String";
        public const string Hardware_Revision_StringPropertyChangedName = "Hardware_Revision_String";
        public const string Firmware_Revision_StringPropertyChangedName = "Firmware_Revision_String";
        public const string Software_Revision_StringPropertyChangedName = "Software_Revision_String";
        public const string System_IDPropertyChangedName = "System_ID";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the Heart Rate Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Heart_Rate_Data :BTCommonMetaData<Heart_Rate_Data> //, IExportDataSource
        {
            private double _Flags = 0;
            /// <summary>
            /// Flags (U8 ) from Service=Heart Rate and Characteristic=Heart Rate Measurement
            ///</summary>
            public double Flags 
            { 
                get { return _Flags; }
                set { if (value == _Flags) return; _Flags = value; OnPropertyChanged();}
            }
            private double _HeartRateLowRange = 0;
            /// <summary>
            /// HeartRateLowRange (U8 bpm) from Service=Heart Rate and Characteristic=Heart Rate Measurement
            ///</summary>
            public double HeartRateLowRange 
            { 
                get { return _HeartRateLowRange; }
                set { if (value == _HeartRateLowRange) return; _HeartRateLowRange = value; OnPropertyChanged();}
            }
            private double _HeartRateHighRange = 0;
            /// <summary>
            /// HeartRateHighRange (U16 bpm) from Service=Heart Rate and Characteristic=Heart Rate Measurement
            ///</summary>
            public double HeartRateHighRange 
            { 
                get { return _HeartRateHighRange; }
                set { if (value == _HeartRateHighRange) return; _HeartRateHighRange = value; OnPropertyChanged();}
            }
            private double _EnergyExpended = 0;
            /// <summary>
            /// EnergyExpended (U16 Joules) from Service=Heart Rate and Characteristic=Heart Rate Measurement
            ///</summary>
            public double EnergyExpended 
            { 
                get { return _EnergyExpended; }
                set { if (value == _EnergyExpended) return; _EnergyExpended = value; OnPropertyChanged();}
            }
            private List<double> _RRInterval = new();
            /// <summary>
            /// RRInterval (U16S ) from Service=Heart Rate and Characteristic=Heart Rate Measurement
            ///</summary>
            public List<double> RRInterval 
            { 
                get { return _RRInterval; }
                set { if (value == _RRInterval) return; _RRInterval = value; OnPropertyChanged();}
            }

            private double _SensorLocation = 0;
            /// <summary>
            /// SensorLocation (U8 ) from Service=Heart Rate and Characteristic=Body Sensor Location
            ///</summary>
            public double SensorLocation 
            { 
                get { return _SensorLocation; }
                set { if (value == _SensorLocation) return; _SensorLocation = value; OnPropertyChanged();}
            }
            public override Heart_Rate_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Heart_Rate_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Heart_Rate_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Flags = source.Flags;
                dest.HeartRateLowRange = source.HeartRateLowRange;
                dest.HeartRateHighRange = source.HeartRateHighRange;
                dest.EnergyExpended = source.EnergyExpended;                dest.RRInterval = new List<double>(source.RRInterval);
                dest.SensorLocation = source.SensorLocation;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Heart_Rate_Data CopyToWithConvertAndCreate(Heart_Rate_Data source, Heart_Rate_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Flags = convert(source.Flags, "");
                dest.HeartRateLowRange = convert(source.HeartRateLowRange, "bpm");
                dest.HeartRateHighRange = convert(source.HeartRateHighRange, "bpm");
                dest.EnergyExpended = convert(source.EnergyExpended, "Joules");
                if (source.RRInterval != null)
                {
                    dest.RRInterval = new List<double>();
                    foreach (var value in source.RRInterval)
                    {
                        double newvalue = convert(value, "");
                        dest.RRInterval.Add(newvalue);
                    }
                }
                dest.SensorLocation = convert(source.SensorLocation, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Flags", "HeartRateLowRange", "HeartRateHighRange", "EnergyExpended", "RRInterval", "SensorLocation"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Flags);
                exporter.CellSet(HeartRateLowRange);
                exporter.CellSet(HeartRateHighRange);
                exporter.CellSet(EnergyExpended);
                exporter.CellSet(RRInterval);
                exporter.CellSet(SensorLocation);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Flags} {HeartRateLowRange} {HeartRateHighRange} {EnergyExpended} {RRInterval} {SensorLocation}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the GAP Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class GAP_Data :BTCommonMetaData<GAP_Data> //, IExportDataSource
        {
            private string _DeviceName = "";
            /// <summary>
            /// DeviceName (STRING ) from Service=GAP and Characteristic=Device Name
            ///</summary>
            public string DeviceName 
            { 
                get { return _DeviceName; }
                set { if (value == _DeviceName) return; _DeviceName = value; OnPropertyChanged();}
            }

            private double _Appearance = 0;
            /// <summary>
            /// Appearance (U16 ) from Service=GAP and Characteristic=Appearance
            ///</summary>
            public double Appearance 
            { 
                get { return _Appearance; }
                set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged();}
            }

            private byte[] _ConnectionParameters = null;
            /// <summary>
            /// ConnectionParameters (BYTES ) from Service=GAP and Characteristic=Peripheral Preferred Connection Parameters
            ///</summary>
            public byte[] ConnectionParameters 
            { 
                get { return _ConnectionParameters; }
                set { if (value == _ConnectionParameters) return; _ConnectionParameters = value; OnPropertyChanged();}
            }
            public override GAP_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as GAP_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(GAP_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.DeviceName = source.DeviceName;
                dest.Appearance = source.Appearance;
                dest.ConnectionParameters = source.ConnectionParameters;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static GAP_Data CopyToWithConvertAndCreate(GAP_Data source, GAP_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.DeviceName = source.DeviceName;
                dest.Appearance = convert(source.Appearance, "");
                dest.ConnectionParameters = source.ConnectionParameters;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["DeviceName", "Appearance", "ConnectionParameters"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(DeviceName);
                exporter.CellSet(Appearance);
                exporter.CellSet(ConnectionParameters);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {DeviceName} {Appearance} {ConnectionParameters}");
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
            private double _TransmitPower = 0;
            /// <summary>
            /// TransmitPower (I8 db) from Service=Battery and Characteristic=Transmit Power
            ///</summary>
            public double TransmitPower 
            { 
                get { return _TransmitPower; }
                set { if (value == _TransmitPower) return; _TransmitPower = value; OnPropertyChanged();}
            }

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
                dest.TransmitPower = source.TransmitPower;
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
                dest.TransmitPower = convert(source.TransmitPower, "db");
                dest.BatteryLevel = convert(source.BatteryLevel, "%");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["TransmitPower", "BatteryLevel"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(TransmitPower);
                exporter.CellSet(BatteryLevel);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {TransmitPower} {BatteryLevel}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Device Information Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Device_Information_Data :BTCommonMetaData<Device_Information_Data> //, IExportDataSource
        {
            private string _Manufacturer = "";
            /// <summary>
            /// Manufacturer (STRING ) from Service=Device Information and Characteristic=Manufacturer Name String
            ///</summary>
            public string Manufacturer 
            { 
                get { return _Manufacturer; }
                set { if (value == _Manufacturer) return; _Manufacturer = value; OnPropertyChanged();}
            }

            private string _ModelNumber = "";
            /// <summary>
            /// ModelNumber (STRING ) from Service=Device Information and Characteristic=Model Number String
            ///</summary>
            public string ModelNumber 
            { 
                get { return _ModelNumber; }
                set { if (value == _ModelNumber) return; _ModelNumber = value; OnPropertyChanged();}
            }

            private string _HardwareRevision = "";
            /// <summary>
            /// HardwareRevision (STRING ) from Service=Device Information and Characteristic=Hardware Revision String
            ///</summary>
            public string HardwareRevision 
            { 
                get { return _HardwareRevision; }
                set { if (value == _HardwareRevision) return; _HardwareRevision = value; OnPropertyChanged();}
            }

            private string _FirmwareRevision = "";
            /// <summary>
            /// FirmwareRevision (STRING ) from Service=Device Information and Characteristic=Firmware Revision String
            ///</summary>
            public string FirmwareRevision 
            { 
                get { return _FirmwareRevision; }
                set { if (value == _FirmwareRevision) return; _FirmwareRevision = value; OnPropertyChanged();}
            }

            private string _SoftwareRevision = "";
            /// <summary>
            /// SoftwareRevision (STRING ) from Service=Device Information and Characteristic=Software Revision String
            ///</summary>
            public string SoftwareRevision 
            { 
                get { return _SoftwareRevision; }
                set { if (value == _SoftwareRevision) return; _SoftwareRevision = value; OnPropertyChanged();}
            }

            private byte[] _SystemID = null;
            /// <summary>
            /// SystemID (BYTES ) from Service=Device Information and Characteristic=System ID
            ///</summary>
            public byte[] SystemID 
            { 
                get { return _SystemID; }
                set { if (value == _SystemID) return; _SystemID = value; OnPropertyChanged();}
            }
            public override Device_Information_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Device_Information_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Device_Information_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Manufacturer = source.Manufacturer;
                dest.ModelNumber = source.ModelNumber;
                dest.HardwareRevision = source.HardwareRevision;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.SystemID = source.SystemID;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Device_Information_Data CopyToWithConvertAndCreate(Device_Information_Data source, Device_Information_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Manufacturer = source.Manufacturer;
                dest.ModelNumber = source.ModelNumber;
                dest.HardwareRevision = source.HardwareRevision;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.SystemID = source.SystemID;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Manufacturer", "ModelNumber", "HardwareRevision", "FirmwareRevision", "SoftwareRevision", "SystemID"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Manufacturer);
                exporter.CellSet(ModelNumber);
                exporter.CellSet(HardwareRevision);
                exporter.CellSet(FirmwareRevision);
                exporter.CellSet(SoftwareRevision);
                exporter.CellSet(SystemID);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Manufacturer} {ModelNumber} {HardwareRevision} {FirmwareRevision} {SoftwareRevision} {SystemID}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Heart_Rate_index = 0,
            GAP_index = 1,
            Battery_index = 2,
            Device_Information_index = 3,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Heart_Rate_Heart_Rate_Measurement_index = 0,     // GUID 00002a37-0000-1000-8000-00805f9b34fb
            Heart_Rate_Body_Sensor_Location_index = 1,     // GUID 00002a38-0000-1000-8000-00805f9b34fb
            GAP_Device_Name_index = 2,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            GAP_Appearance_index = 3,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            GAP_Peripheral_Preferred_Connection_Parameters_index = 4,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Battery_Transmit_Power_index = 5,     // GUID 00002a07-0000-1000-8000-00805f9b34fb
            Battery_BatteryLevel_index = 6,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
            Device_Information_Manufacturer_Name_String_index = 7,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Information_Model_Number_String_index = 8,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Information_Hardware_Revision_String_index = 9,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Information_Firmware_Revision_String_index = 10,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Information_Software_Revision_String_index = 11,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Information_System_ID_index = 12,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("0000180d-0000-1000-8000-00805f9b34fb"), // #0 is Heart Rate
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #1 is GAP
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // #2 is Battery
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #3 is Device Information
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("00002a37-0000-1000-8000-00805f9b34fb"), // #0 is Heart Rate Heart Rate Measurement
            Guid.Parse("00002a38-0000-1000-8000-00805f9b34fb"), // #1 is Heart Rate Body Sensor Location
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #2 is GAP Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #3 is GAP Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #4 is GAP Peripheral Preferred Connection Parameters
            Guid.Parse("00002a07-0000-1000-8000-00805f9b34fb"), // #5 is Battery Transmit Power
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #6 is Battery BatteryLevel
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #7 is Device Information Manufacturer Name String
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #8 is Device Information Model Number String
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #9 is Device Information Hardware Revision String
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #10 is Device Information Firmware Revision String
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #11 is Device Information Software Revision String
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #12 is Device Information System ID
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null,  };


        /// <summary>
        /// Delegate for all Notify events. this is specific to this device (the indexes are all for this device only)
        /// but otherwise is generic.
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(IotNumberFormats.ValueParserResult data);

        private async Task<bool> Ensure_Characteristic_Async(ServiceIndex serviceIndex, string serviceName, CharacteristicIndex characteristicIndex, string characteristicName)
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


        #region Service_Heart_Rate
        // Service Heart Rate 

        public Heart_Rate_Data CurrHeart_Rate_Data { get; set; } = new Heart_Rate_Data();

        // Per-characteristics methods for Heart_Rate Heart_Rate_Measurement
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHeart_Rate_MeasurementAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Heart_Rate_Measurement", ServiceIndex.Heart_Rate_index, "Heart Rate", CharacteristicIndex.Heart_Rate_Heart_Rate_Measurement_index, NotifyHeart_Rate_MeasurementCallback, notifyType);
            return retval;
        }

        private void NotifyHeart_Rate_MeasurementCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Heart_Rate_Heart_Rate_Measurement_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Flags OSKIP^1^$Flags_GN_1_AN U8|DEC|HeartRateLowRange|bpm OSKIP^1^$Flags_GN_1_AN_NT U16|DEC|HeartRateHighRange|bpm OSKIP^1^$Flags_GN_8_AN_NT U16|DEC|EnergyExpended|Joules OSKIP^1^$Flags_GN_16_AN_NT U16S|DEC|RRInterval");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHeart_Rate_Data.TimestampMostRecent = args.Timestamp;
            CurrHeart_Rate_Data.Flags = vr.GetNextDouble();
            CurrHeart_Rate_Data.HeartRateLowRange = vr.GetNextDouble();
            CurrHeart_Rate_Data.HeartRateHighRange = vr.GetNextDouble();
            CurrHeart_Rate_Data.EnergyExpended = vr.GetNextDouble();
            CurrHeart_Rate_Data.RRInterval = vr.GetNextDoubleArray();
            OnPropertyChanged(Heart_Rate_MeasurementPropertyChangedName); // "Heart_Rate_Measurement"
        }
        // Per-characteristics methods for Heart_Rate Body_Sensor_Location
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBody_Sensor_LocationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Body_Sensor_Location", ServiceIndex.Heart_Rate_index, "Heart Rate", CharacteristicIndex.Heart_Rate_Body_Sensor_Location_index, NotifyBody_Sensor_LocationCallback, notifyType);
            return retval;
        }

        private void NotifyBody_Sensor_LocationCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Heart_Rate_Body_Sensor_Location_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|SensorLocation");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHeart_Rate_Data.TimestampMostRecent = args.Timestamp;
            CurrHeart_Rate_Data.SensorLocation = vr.GetNextDouble();
            OnPropertyChanged(Body_Sensor_LocationPropertyChangedName); // "Body_Sensor_Location"
        }
        /// <summary>
        /// Reads data from Heart Rate Measurement and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Heart_Rate_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Heart_Rate_Data> ReadHeart_Rate_Measurement(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Heart_Rate_Heart_Rate_Measurement_index;
            await Ensure_Characteristic_Async(ServiceIndex.Heart_Rate_index, "Heart Rate", index, "Heart Rate Measurement");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Heart Rate Measurement", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Flags OSKIP^1^$Flags_GN_1_AN U8|DEC|HeartRateLowRange|bpm OSKIP^1^$Flags_GN_1_AN_NT U16|DEC|HeartRateHighRange|bpm OSKIP^1^$Flags_GN_8_AN_NT U16|DEC|EnergyExpended|Joules OSKIP^1^$Flags_GN_16_AN_NT U16S|DEC|RRInterval");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHeart_Rate_Data.Flags = vr.GetNextDouble();
            CurrHeart_Rate_Data.HeartRateLowRange = vr.GetNextDouble();
            CurrHeart_Rate_Data.HeartRateHighRange = vr.GetNextDouble();
            CurrHeart_Rate_Data.EnergyExpended = vr.GetNextDouble();
            CurrHeart_Rate_Data.RRInterval = vr.GetNextDoubleArray();
            CurrHeart_Rate_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Heart_Rate_MeasurementPropertyChangedName); // "Heart_Rate_Measurement"
            return CurrHeart_Rate_Data;
        }
        /// <summary>
        /// Reads data from Body Sensor Location and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Heart_Rate_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Heart_Rate_Data> ReadBody_Sensor_Location(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Heart_Rate_Body_Sensor_Location_index;
            await Ensure_Characteristic_Async(ServiceIndex.Heart_Rate_index, "Heart Rate", index, "Body Sensor Location");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Body Sensor Location", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|SensorLocation");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHeart_Rate_Data.SensorLocation = vr.GetNextDouble();
            CurrHeart_Rate_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Body_Sensor_LocationPropertyChangedName); // "Body_Sensor_Location"
            return CurrHeart_Rate_Data;
        }

        #endregion
//
        #region Service_GAP
        // Service GAP 

        public GAP_Data CurrGAP_Data { get; set; } = new GAP_Data();

        // Per-characteristics methods for GAP Device_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDevice_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Device_Name", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Device_Name_index, NotifyDevice_NameCallback, notifyType);
            return retval;
        }

        private void NotifyDevice_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Device_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|DeviceName");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.DeviceName = vr.GetNextString();
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
        }
        // Per-characteristics methods for GAP Appearance
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAppearanceAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Appearance", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Appearance_index, NotifyAppearanceCallback, notifyType);
            return retval;
        }

        private void NotifyAppearanceCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Appearance_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|HEX|Appearance");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.Appearance = vr.GetNextDouble();
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
        }
        // Per-characteristics methods for GAP Peripheral_Preferred_Connection_Parameters
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPeripheral_Preferred_Connection_ParametersAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Peripheral_Preferred_Connection_Parameters", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Peripheral_Preferred_Connection_Parameters_index, NotifyPeripheral_Preferred_Connection_ParametersCallback, notifyType);
            return retval;
        }

        private void NotifyPeripheral_Preferred_Connection_ParametersCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Peripheral_Preferred_Connection_Parameters_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ConnectionParameters");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.ConnectionParameters = vr.GetNextByteArray();
            OnPropertyChanged(Peripheral_Preferred_Connection_ParametersPropertyChangedName); // "Peripheral_Preferred_Connection_Parameters"
        }
        /// <summary>
        /// Reads data from Device Name and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadDevice_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Device Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Device Name", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|DeviceName");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGAP_Data.DeviceName = vr.GetNextString();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
            return CurrGAP_Data;
        }
        /// <summary>
        /// Reads data from Appearance and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadAppearance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Appearance_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Appearance");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Appearance", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|HEX|Appearance");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGAP_Data.Appearance = vr.GetNextDouble();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
            return CurrGAP_Data;
        }
        /// <summary>
        /// Reads data from Peripheral Preferred Connection Parameters and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadPeripheral_Preferred_Connection_Parameters(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Peripheral_Preferred_Connection_Parameters_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Peripheral Preferred Connection Parameters");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Peripheral Preferred Connection Parameters", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ConnectionParameters");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGAP_Data.ConnectionParameters = vr.GetNextByteArray();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Peripheral_Preferred_Connection_ParametersPropertyChangedName); // "Peripheral_Preferred_Connection_Parameters"
            return CurrGAP_Data;
        }

        #endregion
//
        #region Service_Battery
        // Service Battery 

        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

        // Per-characteristics methods for Battery Transmit_Power
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTransmit_PowerAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Transmit_Power", ServiceIndex.Battery_index, "Battery", CharacteristicIndex.Battery_Transmit_Power_index, NotifyTransmit_PowerCallback, notifyType);
            return retval;
        }

        private void NotifyTransmit_PowerCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Battery_Transmit_Power_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|TransmitPower|db");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.TimestampMostRecent = args.Timestamp;
            CurrBattery_Data.TransmitPower = vr.GetNextDouble();
            OnPropertyChanged(Transmit_PowerPropertyChangedName); // "Transmit_Power"
        }
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
        /// Reads data from Transmit Power and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Battery_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadTransmit_Power(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_Transmit_Power_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "Transmit Power");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Transmit Power", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|TransmitPower|db");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.TransmitPower = vr.GetNextDouble();
            CurrBattery_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Transmit_PowerPropertyChangedName); // "Transmit_Power"
            return CurrBattery_Data;
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
        #region Service_Device_Information
        // Service Device Information 

        public Device_Information_Data CurrDevice_Information_Data { get; set; } = new Device_Information_Data();

        // Per-characteristics methods for Device_Information Manufacturer_Name_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyManufacturer_Name_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Manufacturer_Name_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Manufacturer_Name_String_index, NotifyManufacturer_Name_StringCallback, notifyType);
            return retval;
        }

        private void NotifyManufacturer_Name_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Manufacturer_Name_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Manufacturer");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.Manufacturer = vr.GetNextString();
            OnPropertyChanged(Manufacturer_Name_StringPropertyChangedName); // "Manufacturer_Name_String"
        }
        // Per-characteristics methods for Device_Information Model_Number_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyModel_Number_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Model_Number_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Model_Number_String_index, NotifyModel_Number_StringCallback, notifyType);
            return retval;
        }

        private void NotifyModel_Number_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Model_Number_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.ModelNumber = vr.GetNextString();
            OnPropertyChanged(Model_Number_StringPropertyChangedName); // "Model_Number_String"
        }
        // Per-characteristics methods for Device_Information Hardware_Revision_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHardware_Revision_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Hardware_Revision_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Hardware_Revision_String_index, NotifyHardware_Revision_StringCallback, notifyType);
            return retval;
        }

        private void NotifyHardware_Revision_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Hardware_Revision_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|HardwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.HardwareRevision = vr.GetNextString();
            OnPropertyChanged(Hardware_Revision_StringPropertyChangedName); // "Hardware_Revision_String"
        }
        // Per-characteristics methods for Device_Information Firmware_Revision_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFirmware_Revision_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Firmware_Revision_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Firmware_Revision_String_index, NotifyFirmware_Revision_StringCallback, notifyType);
            return retval;
        }

        private void NotifyFirmware_Revision_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Firmware_Revision_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|FirmwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.FirmwareRevision = vr.GetNextString();
            OnPropertyChanged(Firmware_Revision_StringPropertyChangedName); // "Firmware_Revision_String"
        }
        // Per-characteristics methods for Device_Information Software_Revision_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySoftware_Revision_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Software_Revision_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Software_Revision_String_index, NotifySoftware_Revision_StringCallback, notifyType);
            return retval;
        }

        private void NotifySoftware_Revision_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Software_Revision_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.SoftwareRevision = vr.GetNextString();
            OnPropertyChanged(Software_Revision_StringPropertyChangedName); // "Software_Revision_String"
        }
        // Per-characteristics methods for Device_Information System_ID
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySystem_IDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("System_ID", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_System_ID_index, NotifySystem_IDCallback, notifyType);
            return retval;
        }

        private void NotifySystem_IDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_System_ID_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|SystemID");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.SystemID = vr.GetNextByteArray();
            OnPropertyChanged(System_IDPropertyChangedName); // "System_ID"
        }
        /// <summary>
        /// Reads data from Manufacturer Name String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadManufacturer_Name_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Manufacturer_Name_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Manufacturer Name String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Manufacturer Name String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Manufacturer");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.Manufacturer = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Manufacturer_Name_StringPropertyChangedName); // "Manufacturer_Name_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from Model Number String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadModel_Number_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Model_Number_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Model Number String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Model Number String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.ModelNumber = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Model_Number_StringPropertyChangedName); // "Model_Number_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from Hardware Revision String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadHardware_Revision_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Hardware_Revision_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Hardware Revision String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Hardware Revision String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|HardwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.HardwareRevision = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Hardware_Revision_StringPropertyChangedName); // "Hardware_Revision_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from Firmware Revision String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadFirmware_Revision_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Firmware_Revision_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Firmware Revision String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Firmware Revision String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|FirmwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.FirmwareRevision = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Firmware_Revision_StringPropertyChangedName); // "Firmware_Revision_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from Software Revision String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadSoftware_Revision_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Software_Revision_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Software Revision String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Software Revision String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.SoftwareRevision = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Software_Revision_StringPropertyChangedName); // "Software_Revision_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from System ID and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadSystem_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_System_ID_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "System ID");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "System ID", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|SystemID");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.SystemID = vr.GetNextByteArray();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(System_IDPropertyChangedName); // "System_ID"
            return CurrDevice_Information_Data;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}