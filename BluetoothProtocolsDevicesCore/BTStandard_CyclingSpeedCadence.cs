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

    public  class BTStandard_CyclingSpeedCadence : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://www.bluetooth.com/specifications/specs/html/?src=CSCP_v1.0.1/out/en/index-en.html
        // Link: https://www.bluetooth.com/specifications/specs/cycling-speed-and-cadence-service/

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "BK6";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device BK6

        Cycling Speed and Cadence service Guid=1816
            SpeedCadence_Data (DataGroup record)
                CSC Measurement characteristic has Flags (Byte-->double) RevolutionWheel (UInt32-->double) TimeWheel (UInt16-->double) RevolutionCrank (UInt16-->double) TimeCrank (UInt16-->double)  Guid=2a5b

            Feature_Data (DataGroup record)
                CSC Feature characteristic has FeatureFlags (UInt16-->double)  Guid=2a5c
                Sensor Location characteristic has SensorLocation (Byte-->double)  Guid=2a5d
                SC Control Point characteristic has Unknown3 (Bytes-->string)  Guid=2a55


        Service_FD00_OTA service Guid=fd00
            Service_FD00_OTA_Data (DataGroup record)
                FD09_OTA_Notify characteristic has Unknown0 (Bytes-->string)  Guid=fd09
                FD0A_OTA_Write characteristic has Unknown1 (Bytes-->string)  Guid=fd0a
                FD19_Notify characteristic has Unknown2 (Bytes-->string)  Guid=fd19
                FD1A_Write characteristic has Unknown3 (Bytes-->string)  Guid=fd1a


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

        public const string CSC_MeasurementPropertyChangedName = "CSC_Measurement";
        public const string CSC_FeaturePropertyChangedName = "CSC_Feature";
        public const string Sensor_LocationPropertyChangedName = "Sensor_Location";
        public const string SC_Control_PointPropertyChangedName = "SC_Control_Point";
        public const string FD09_OTA_NotifyPropertyChangedName = "FD09_OTA_Notify";
        public const string FD0A_OTA_WritePropertyChangedName = "FD0A_OTA_Write";
        public const string FD19_NotifyPropertyChangedName = "FD19_Notify";
        public const string FD1A_WritePropertyChangedName = "FD1A_Write";
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
        /// Data from all of the characteristics in the Cycling Speed and Cadence Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class SpeedCadence_Data :BTCommonMetaData<SpeedCadence_Data> //, IExportDataSource
        {
            private double _Flags = 0;
            /// <summary>
            /// Flags (U8 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
            ///</summary>
            public double Flags 
            { 
                get { return _Flags; }
                set { if (value == _Flags) return; _Flags = value; OnPropertyChanged();}
            }
            private double _RevolutionWheel = 0;
            /// <summary>
            /// RevolutionWheel (U32 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
            ///</summary>
            public double RevolutionWheel 
            { 
                get { return _RevolutionWheel; }
                set { if (value == _RevolutionWheel) return; _RevolutionWheel = value; OnPropertyChanged();}
            }
            private double _TimeWheel = 0;
            /// <summary>
            /// TimeWheel (U16 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
            ///</summary>
            public double TimeWheel 
            { 
                get { return _TimeWheel; }
                set { if (value == _TimeWheel) return; _TimeWheel = value; OnPropertyChanged();}
            }
            private double _RevolutionCrank = 0;
            /// <summary>
            /// RevolutionCrank (U16 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
            ///</summary>
            public double RevolutionCrank 
            { 
                get { return _RevolutionCrank; }
                set { if (value == _RevolutionCrank) return; _RevolutionCrank = value; OnPropertyChanged();}
            }
            private double _TimeCrank = 0;
            /// <summary>
            /// TimeCrank (U16 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
            ///</summary>
            public double TimeCrank 
            { 
                get { return _TimeCrank; }
                set { if (value == _TimeCrank) return; _TimeCrank = value; OnPropertyChanged();}
            }
            public override SpeedCadence_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as SpeedCadence_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(SpeedCadence_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Flags = source.Flags;
                dest.RevolutionWheel = source.RevolutionWheel;
                dest.TimeWheel = source.TimeWheel;
                dest.RevolutionCrank = source.RevolutionCrank;
                dest.TimeCrank = source.TimeCrank;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static SpeedCadence_Data CopyToWithConvertAndCreate(SpeedCadence_Data source, SpeedCadence_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Flags = convert(source.Flags, "");
                dest.RevolutionWheel = convert(source.RevolutionWheel, "");
                dest.TimeWheel = convert(source.TimeWheel, "");
                dest.RevolutionCrank = convert(source.RevolutionCrank, "");
                dest.TimeCrank = convert(source.TimeCrank, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Flags", "RevolutionWheel", "TimeWheel", "RevolutionCrank", "TimeCrank"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Flags);
                exporter.CellSet(RevolutionWheel);
                exporter.CellSet(TimeWheel);
                exporter.CellSet(RevolutionCrank);
                exporter.CellSet(TimeCrank);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Flags} {RevolutionWheel} {TimeWheel} {RevolutionCrank} {TimeCrank}");
            }
        }
        /// <summary>
        /// Data from all of the characteristics in the Cycling Speed and Cadence Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Feature_Data :BTCommonMetaData<Feature_Data> //, IExportDataSource
        {
            private double _FeatureFlags = 0;
            /// <summary>
            /// FeatureFlags (U16 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Feature
            ///</summary>
            public double FeatureFlags 
            { 
                get { return _FeatureFlags; }
                set { if (value == _FeatureFlags) return; _FeatureFlags = value; OnPropertyChanged();}
            }

            private double _SensorLocation = 0;
            /// <summary>
            /// SensorLocation (U8 ) from Service=Cycling Speed and Cadence and Characteristic=Sensor Location
            ///</summary>
            public double SensorLocation 
            { 
                get { return _SensorLocation; }
                set { if (value == _SensorLocation) return; _SensorLocation = value; OnPropertyChanged();}
            }

            private byte[] _Unknown3 = null;
            /// <summary>
            /// Unknown3 (BYTES ) from Service=Cycling Speed and Cadence and Characteristic=SC Control Point
            ///</summary>
            public byte[] Unknown3 
            { 
                get { return _Unknown3; }
                set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged();}
            }
            public override Feature_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Feature_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Feature_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.FeatureFlags = source.FeatureFlags;
                dest.SensorLocation = source.SensorLocation;
                dest.Unknown3 = source.Unknown3;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Feature_Data CopyToWithConvertAndCreate(Feature_Data source, Feature_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.FeatureFlags = convert(source.FeatureFlags, "");
                dest.SensorLocation = convert(source.SensorLocation, "");
                dest.Unknown3 = source.Unknown3;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["FeatureFlags", "SensorLocation", "Unknown3"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(FeatureFlags);
                exporter.CellSet(SensorLocation);
                exporter.CellSet(Unknown3);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {FeatureFlags} {SensorLocation} {Unknown3}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Service_FD00_OTA Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Service_FD00_OTA_Data :BTCommonMetaData<Service_FD00_OTA_Data> //, IExportDataSource
        {
            private byte[] _Unknown0 = null;
            /// <summary>
            /// Unknown0 (BYTES ) from Service=Service_FD00_OTA and Characteristic=FD09_OTA_Notify
            ///</summary>
            public byte[] Unknown0 
            { 
                get { return _Unknown0; }
                set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged();}
            }

            private byte[] _Unknown1 = null;
            /// <summary>
            /// Unknown1 (BYTES ) from Service=Service_FD00_OTA and Characteristic=FD0A_OTA_Write
            ///</summary>
            public byte[] Unknown1 
            { 
                get { return _Unknown1; }
                set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged();}
            }

            private byte[] _Unknown2 = null;
            /// <summary>
            /// Unknown2 (BYTES ) from Service=Service_FD00_OTA and Characteristic=FD19_Notify
            ///</summary>
            public byte[] Unknown2 
            { 
                get { return _Unknown2; }
                set { if (value == _Unknown2) return; _Unknown2 = value; OnPropertyChanged();}
            }

            private byte[] _Unknown3 = null;
            /// <summary>
            /// Unknown3 (BYTES ) from Service=Service_FD00_OTA and Characteristic=FD1A_Write
            ///</summary>
            public byte[] Unknown3 
            { 
                get { return _Unknown3; }
                set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged();}
            }
            public override Service_FD00_OTA_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Service_FD00_OTA_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Service_FD00_OTA_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Unknown0 = source.Unknown0;
                dest.Unknown1 = source.Unknown1;
                dest.Unknown2 = source.Unknown2;
                dest.Unknown3 = source.Unknown3;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Service_FD00_OTA_Data CopyToWithConvertAndCreate(Service_FD00_OTA_Data source, Service_FD00_OTA_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Unknown0 = source.Unknown0;
                dest.Unknown1 = source.Unknown1;
                dest.Unknown2 = source.Unknown2;
                dest.Unknown3 = source.Unknown3;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Unknown0", "Unknown1", "Unknown2", "Unknown3"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Unknown0);
                exporter.CellSet(Unknown1);
                exporter.CellSet(Unknown2);
                exporter.CellSet(Unknown3);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Unknown0} {Unknown1} {Unknown2} {Unknown3}");
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
            Cycling_Speed_and_Cadence_index = 0,
            Service_FD00_OTA_index = 1,
            Battery_index = 2,
            Device_Information_index = 3,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Cycling_Speed_and_Cadence_CSC_Measurement_index = 0,     // GUID 00002a5b-0000-1000-8000-00805f9b34fb
            Cycling_Speed_and_Cadence_CSC_Feature_index = 1,     // GUID 00002a5c-0000-1000-8000-00805f9b34fb
            Cycling_Speed_and_Cadence_Sensor_Location_index = 2,     // GUID 00002a5d-0000-1000-8000-00805f9b34fb
            Cycling_Speed_and_Cadence_SC_Control_Point_index = 3,     // GUID 00002a55-0000-1000-8000-00805f9b34fb
            Service_FD00_OTA_FD09_OTA_Notify_index = 4,     // GUID 0000fd09-0000-1000-8000-00805f9b34fb
            Service_FD00_OTA_FD0A_OTA_Write_index = 5,     // GUID 0000fd0a-0000-1000-8000-00805f9b34fb
            Service_FD00_OTA_FD19_Notify_index = 6,     // GUID 0000fd19-0000-1000-8000-00805f9b34fb
            Service_FD00_OTA_FD1A_Write_index = 7,     // GUID 0000fd1a-0000-1000-8000-00805f9b34fb
            Battery_Transmit_Power_index = 8,     // GUID 00002a07-0000-1000-8000-00805f9b34fb
            Battery_BatteryLevel_index = 9,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
            Device_Information_Manufacturer_Name_String_index = 10,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Information_Model_Number_String_index = 11,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Information_Hardware_Revision_String_index = 12,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Information_Firmware_Revision_String_index = 13,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Information_Software_Revision_String_index = 14,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Information_System_ID_index = 15,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("00001816-0000-1000-8000-00805f9b34fb"), // #0 is Cycling Speed and Cadence
            Guid.Parse("0000fd00-0000-1000-8000-00805f9b34fb"), // #1 is Service_FD00_OTA
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
            Guid.Parse("00002a5b-0000-1000-8000-00805f9b34fb"), // #0 is Cycling Speed and Cadence CSC Measurement
            Guid.Parse("00002a5c-0000-1000-8000-00805f9b34fb"), // #1 is Cycling Speed and Cadence CSC Feature
            Guid.Parse("00002a5d-0000-1000-8000-00805f9b34fb"), // #2 is Cycling Speed and Cadence Sensor Location
            Guid.Parse("00002a55-0000-1000-8000-00805f9b34fb"), // #3 is Cycling Speed and Cadence SC Control Point
            Guid.Parse("0000fd09-0000-1000-8000-00805f9b34fb"), // #4 is Service_FD00_OTA FD09_OTA_Notify
            Guid.Parse("0000fd0a-0000-1000-8000-00805f9b34fb"), // #5 is Service_FD00_OTA FD0A_OTA_Write
            Guid.Parse("0000fd19-0000-1000-8000-00805f9b34fb"), // #6 is Service_FD00_OTA FD19_Notify
            Guid.Parse("0000fd1a-0000-1000-8000-00805f9b34fb"), // #7 is Service_FD00_OTA FD1A_Write
            Guid.Parse("00002a07-0000-1000-8000-00805f9b34fb"), // #8 is Battery Transmit Power
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #9 is Battery BatteryLevel
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #10 is Device Information Manufacturer Name String
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #11 is Device Information Model Number String
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #12 is Device Information Hardware Revision String
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #13 is Device Information Firmware Revision String
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #14 is Device Information Software Revision String
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #15 is Device Information System ID
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };


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


        #region Service_Cycling_Speed_and_Cadence
        // Service Cycling Speed and Cadence 

        public SpeedCadence_Data CurrSpeedCadence_Data { get; set; } = new SpeedCadence_Data();

        // Per-characteristics methods for Cycling_Speed_and_Cadence CSC_Measurement
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCSC_MeasurementAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("CSC_Measurement", ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", CharacteristicIndex.Cycling_Speed_and_Cadence_CSC_Measurement_index, NotifyCSC_MeasurementCallback, notifyType);
            return retval;
        }

        private void NotifyCSC_MeasurementCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Cycling_Speed_and_Cadence_CSC_Measurement_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Flags OSKIP^2^$Flags_GN_1_AN_NT U32|HEX|RevolutionWheel U16^1024_/|FIXED|TimeWheel| OSKIP^2^$Flags_GN_2_AN_NT U16|DEC|RevolutionCrank U16^1024_/|FIXED|TimeCrank|");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrSpeedCadence_Data.TimestampMostRecent = args.Timestamp;
            CurrSpeedCadence_Data.Flags = vr.GetNextDouble();
            CurrSpeedCadence_Data.RevolutionWheel = vr.GetNextDouble();
            CurrSpeedCadence_Data.TimeWheel = vr.GetNextDouble();
            CurrSpeedCadence_Data.RevolutionCrank = vr.GetNextDouble();
            CurrSpeedCadence_Data.TimeCrank = vr.GetNextDouble();
            OnPropertyChanged(CSC_MeasurementPropertyChangedName); // "CSC_Measurement"
        }
        /// <summary>
        /// Reads data from CSC Measurement and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>SpeedCadence_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<SpeedCadence_Data> ReadCSC_Measurement(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Cycling_Speed_and_Cadence_CSC_Measurement_index;
            await Ensure_Characteristic_Async(ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", index, "CSC Measurement");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "CSC Measurement", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Flags OSKIP^2^$Flags_GN_1_AN_NT U32|HEX|RevolutionWheel U16^1024_/|FIXED|TimeWheel| OSKIP^2^$Flags_GN_2_AN_NT U16|DEC|RevolutionCrank U16^1024_/|FIXED|TimeCrank|");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrSpeedCadence_Data.Flags = vr.GetNextDouble();
            CurrSpeedCadence_Data.RevolutionWheel = vr.GetNextDouble();
            CurrSpeedCadence_Data.TimeWheel = vr.GetNextDouble();
            CurrSpeedCadence_Data.RevolutionCrank = vr.GetNextDouble();
            CurrSpeedCadence_Data.TimeCrank = vr.GetNextDouble();
            CurrSpeedCadence_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(CSC_MeasurementPropertyChangedName); // "CSC_Measurement"
            return CurrSpeedCadence_Data;
        }
        // Service Cycling Speed and Cadence 

        public Feature_Data CurrFeature_Data { get; set; } = new Feature_Data();

        // Per-characteristics methods for Cycling_Speed_and_Cadence CSC_Feature
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCSC_FeatureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("CSC_Feature", ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", CharacteristicIndex.Cycling_Speed_and_Cadence_CSC_Feature_index, NotifyCSC_FeatureCallback, notifyType);
            return retval;
        }

        private void NotifyCSC_FeatureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Cycling_Speed_and_Cadence_CSC_Feature_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|HEX|FeatureFlags");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrFeature_Data.TimestampMostRecent = args.Timestamp;
            CurrFeature_Data.FeatureFlags = vr.GetNextDouble();
            OnPropertyChanged(CSC_FeaturePropertyChangedName); // "CSC_Feature"
        }
        // Per-characteristics methods for Cycling_Speed_and_Cadence Sensor_Location
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySensor_LocationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Sensor_Location", ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", CharacteristicIndex.Cycling_Speed_and_Cadence_Sensor_Location_index, NotifySensor_LocationCallback, notifyType);
            return retval;
        }

        private void NotifySensor_LocationCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Cycling_Speed_and_Cadence_Sensor_Location_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|SensorLocation");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrFeature_Data.TimestampMostRecent = args.Timestamp;
            CurrFeature_Data.SensorLocation = vr.GetNextDouble();
            OnPropertyChanged(Sensor_LocationPropertyChangedName); // "Sensor_Location"
        }
        // Per-characteristics methods for Cycling_Speed_and_Cadence SC_Control_Point
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySC_Control_PointAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("SC_Control_Point", ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", CharacteristicIndex.Cycling_Speed_and_Cadence_SC_Control_Point_index, NotifySC_Control_PointCallback, notifyType);
            return retval;
        }

        private void NotifySC_Control_PointCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Cycling_Speed_and_Cadence_SC_Control_Point_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown3");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrFeature_Data.TimestampMostRecent = args.Timestamp;
            CurrFeature_Data.Unknown3 = vr.GetNextByteArray();
            OnPropertyChanged(SC_Control_PointPropertyChangedName); // "SC_Control_Point"
        }
        /// <summary>
        /// Reads data from CSC Feature and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Feature_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Feature_Data> ReadCSC_Feature(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Cycling_Speed_and_Cadence_CSC_Feature_index;
            await Ensure_Characteristic_Async(ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", index, "CSC Feature");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "CSC Feature", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|HEX|FeatureFlags");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrFeature_Data.FeatureFlags = vr.GetNextDouble();
            CurrFeature_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(CSC_FeaturePropertyChangedName); // "CSC_Feature"
            return CurrFeature_Data;
        }
        /// <summary>
        /// Reads data from Sensor Location and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Feature_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Feature_Data> ReadSensor_Location(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Cycling_Speed_and_Cadence_Sensor_Location_index;
            await Ensure_Characteristic_Async(ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", index, "Sensor Location");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Sensor Location", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|SensorLocation");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrFeature_Data.SensorLocation = vr.GetNextDouble();
            CurrFeature_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Sensor_LocationPropertyChangedName); // "Sensor_Location"
            return CurrFeature_Data;
        }
        /// <summary>
        /// Reads data from SC Control Point and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Feature_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Feature_Data> ReadSC_Control_Point(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Cycling_Speed_and_Cadence_SC_Control_Point_index;
            await Ensure_Characteristic_Async(ServiceIndex.Cycling_Speed_and_Cadence_index, "Cycling Speed and Cadence", index, "SC Control Point");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "SC Control Point", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown3");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrFeature_Data.Unknown3 = vr.GetNextByteArray();
            CurrFeature_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(SC_Control_PointPropertyChangedName); // "SC_Control_Point"
            return CurrFeature_Data;
        }

        #endregion
//
        #region Service_Service_FD00_OTA
        // Service Service_FD00_OTA 

        public Service_FD00_OTA_Data CurrService_FD00_OTA_Data { get; set; } = new Service_FD00_OTA_Data();

        // Per-characteristics methods for Service_FD00_OTA FD09_OTA_Notify
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD09_OTA_NotifyAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD09_OTA_Notify", ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", CharacteristicIndex.Service_FD00_OTA_FD09_OTA_Notify_index, NotifyFD09_OTA_NotifyCallback, notifyType);
            return retval;
        }

        private void NotifyFD09_OTA_NotifyCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_OTA_FD09_OTA_Notify_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_OTA_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_OTA_Data.Unknown0 = vr.GetNextByteArray();
            OnPropertyChanged(FD09_OTA_NotifyPropertyChangedName); // "FD09_OTA_Notify"
        }
        // Per-characteristics methods for Service_FD00_OTA FD0A_OTA_Write
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD0A_OTA_WriteAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD0A_OTA_Write", ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", CharacteristicIndex.Service_FD00_OTA_FD0A_OTA_Write_index, NotifyFD0A_OTA_WriteCallback, notifyType);
            return retval;
        }

        private void NotifyFD0A_OTA_WriteCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_OTA_FD0A_OTA_Write_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_OTA_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_OTA_Data.Unknown1 = vr.GetNextByteArray();
            OnPropertyChanged(FD0A_OTA_WritePropertyChangedName); // "FD0A_OTA_Write"
        }
        // Per-characteristics methods for Service_FD00_OTA FD19_Notify
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD19_NotifyAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD19_Notify", ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", CharacteristicIndex.Service_FD00_OTA_FD19_Notify_index, NotifyFD19_NotifyCallback, notifyType);
            return retval;
        }

        private void NotifyFD19_NotifyCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_OTA_FD19_Notify_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown2");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_OTA_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_OTA_Data.Unknown2 = vr.GetNextByteArray();
            OnPropertyChanged(FD19_NotifyPropertyChangedName); // "FD19_Notify"
        }
        // Per-characteristics methods for Service_FD00_OTA FD1A_Write
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD1A_WriteAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD1A_Write", ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", CharacteristicIndex.Service_FD00_OTA_FD1A_Write_index, NotifyFD1A_WriteCallback, notifyType);
            return retval;
        }

        private void NotifyFD1A_WriteCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_OTA_FD1A_Write_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown3");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_OTA_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_OTA_Data.Unknown3 = vr.GetNextByteArray();
            OnPropertyChanged(FD1A_WritePropertyChangedName); // "FD1A_Write"
        }
        /// <summary>
        /// Reads data from FD09_OTA_Notify and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_OTA_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_OTA_Data> ReadFD09_OTA_Notify(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_OTA_FD09_OTA_Notify_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", index, "FD09_OTA_Notify");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD09_OTA_Notify", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_OTA_Data.Unknown0 = vr.GetNextByteArray();
            CurrService_FD00_OTA_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FD09_OTA_NotifyPropertyChangedName); // "FD09_OTA_Notify"
            return CurrService_FD00_OTA_Data;
        }
        /// <summary>
        /// Reads data from FD0A_OTA_Write and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_OTA_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_OTA_Data> ReadFD0A_OTA_Write(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_OTA_FD0A_OTA_Write_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", index, "FD0A_OTA_Write");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD0A_OTA_Write", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_OTA_Data.Unknown1 = vr.GetNextByteArray();
            CurrService_FD00_OTA_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FD0A_OTA_WritePropertyChangedName); // "FD0A_OTA_Write"
            return CurrService_FD00_OTA_Data;
        }
        /// <summary>
        /// Reads data from FD19_Notify and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_OTA_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_OTA_Data> ReadFD19_Notify(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_OTA_FD19_Notify_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", index, "FD19_Notify");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD19_Notify", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown2");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_OTA_Data.Unknown2 = vr.GetNextByteArray();
            CurrService_FD00_OTA_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FD19_NotifyPropertyChangedName); // "FD19_Notify"
            return CurrService_FD00_OTA_Data;
        }
        /// <summary>
        /// Reads data from FD1A_Write and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_OTA_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_OTA_Data> ReadFD1A_Write(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_OTA_FD1A_Write_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_OTA_index, "Service_FD00_OTA", index, "FD1A_Write");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD1A_Write", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown3");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_OTA_Data.Unknown3 = vr.GetNextByteArray();
            CurrService_FD00_OTA_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FD1A_WritePropertyChangedName); // "FD1A_Write"
            return CurrService_FD00_OTA_Data;
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