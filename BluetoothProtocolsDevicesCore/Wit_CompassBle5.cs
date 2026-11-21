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
    /// This class was automatically generated 2026-11-20::17:03
    /// </summary>

    public  class Wit_CompassBle5 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://github.com/WITMOTION
        // Link: https://github.com/WITMOTION/WitBluetooth_BWT901BLE5_0
        // Link: https://wit-motion.gitbook.io/witmotion-sdk/ble-5.0-protocol/bluetooth-5.0-communication-protocol
        // Link: https://wit-motion.gitbook.io/witmotion-sdk/ble-5.0-protocol/sdk
        // Link: https://drive.google.com/drive/folders/1I6sBC-8Q3_vtY-GrFDZbWJZJFk7UnNfO

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "WT901BLE67";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device WT901BLE67

        Compass service Guid=0000ffe5-0000-1000-8000-00805f9a34fb
            Compass_Command (DataGroup record)
                CompassCommand characteristic has Command (Bytes-->string)  Guid=0000ffe9-0000-1000-8000-00805f9a34fb

            Compass_Data (DataGroup record)
                CompassData characteristic has Start (Byte-->double) DataPacketType (Byte-->double) AccelX (Int16-->double) AccelY (Int16-->double) AccelZ (Int16-->double) AngularVelocityX (Int16-->double) AngularVelocityY (Int16-->double) AngularVelocityZ (Int16-->double) AngleRoll (Int16-->double) AnglePitch (Int16-->double) AngleYaw (Int16-->double)  Guid=0000ffe4-0000-1000-8000-00805f9a34fb


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Connection Parameter characteristic has Interval_Min (UInt16-->double) Interval_Max (UInt16-->double) Latency (UInt16-->double) Timeout (UInt16-->double)  Guid=2a04
                Central Address Resolution characteristic has AddressResolutionSupported (Byte-->double)  Guid=2aa6
        */

        public const string CompassCommandPropertyChangedName = "CompassCommand";
        public const string CompassDataPropertyChangedName = "CompassData";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Connection_ParameterPropertyChangedName = "Connection_Parameter";
        public const string Central_Address_ResolutionPropertyChangedName = "Central_Address_Resolution";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the Compass Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Compass_Command :BTCommonMetaData<Compass_Command> //, IExportDataSource
        {
            private byte[] _Command = null;
            /// <summary>
            /// Command (BYTES ) from Service=Compass and Characteristic=CompassCommand
            ///</summary>
            public byte[] Command 
            { 
                get { return _Command; }
                set { if (value == _Command) return; _Command = value; OnPropertyChanged();}
            }
            public override Compass_Command Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Compass_Command;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Compass_Command source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Command = source.Command;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Compass_Command CopyToWithConvertAndCreate(Compass_Command source, Compass_Command dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Command = source.Command;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Command"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Command);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Command}");
            }
        }
        /// <summary>
        /// Data from all of the characteristics in the Compass Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Compass_Data :BTCommonMetaData<Compass_Data> //, IExportDataSource
        {
            private double _Start = 0x55;
            /// <summary>
            /// Start (U8 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double Start 
            { 
                get { return _Start; }
                set { if (value == _Start) return; _Start = value; OnPropertyChanged();}
            }
            private double _DataPacketType = 0;
            /// <summary>
            /// DataPacketType (U8 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double DataPacketType 
            { 
                get { return _DataPacketType; }
                set { if (value == _DataPacketType) return; _DataPacketType = value; OnPropertyChanged();}
            }
            private double _AccelX = 0;
            /// <summary>
            /// AccelX (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AccelX 
            { 
                get { return _AccelX; }
                set { if (value == _AccelX) return; _AccelX = value; OnPropertyChanged();}
            }
            private double _AccelY = 0;
            /// <summary>
            /// AccelY (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AccelY 
            { 
                get { return _AccelY; }
                set { if (value == _AccelY) return; _AccelY = value; OnPropertyChanged();}
            }
            private double _AccelZ = 0;
            /// <summary>
            /// AccelZ (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AccelZ 
            { 
                get { return _AccelZ; }
                set { if (value == _AccelZ) return; _AccelZ = value; OnPropertyChanged();}
            }
            private double _AngularVelocityX = 0;
            /// <summary>
            /// AngularVelocityX (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AngularVelocityX 
            { 
                get { return _AngularVelocityX; }
                set { if (value == _AngularVelocityX) return; _AngularVelocityX = value; OnPropertyChanged();}
            }
            private double _AngularVelocityY = 0;
            /// <summary>
            /// AngularVelocityY (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AngularVelocityY 
            { 
                get { return _AngularVelocityY; }
                set { if (value == _AngularVelocityY) return; _AngularVelocityY = value; OnPropertyChanged();}
            }
            private double _AngularVelocityZ = 0;
            /// <summary>
            /// AngularVelocityZ (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AngularVelocityZ 
            { 
                get { return _AngularVelocityZ; }
                set { if (value == _AngularVelocityZ) return; _AngularVelocityZ = value; OnPropertyChanged();}
            }
            private double _AngleRoll = 0;
            /// <summary>
            /// AngleRoll (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AngleRoll 
            { 
                get { return _AngleRoll; }
                set { if (value == _AngleRoll) return; _AngleRoll = value; OnPropertyChanged();}
            }
            private double _AnglePitch = 0;
            /// <summary>
            /// AnglePitch (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AnglePitch 
            { 
                get { return _AnglePitch; }
                set { if (value == _AnglePitch) return; _AnglePitch = value; OnPropertyChanged();}
            }
            private double _AngleYaw = 0;
            /// <summary>
            /// AngleYaw (I16 ) from Service=Compass and Characteristic=CompassData
            ///</summary>
            public double AngleYaw 
            { 
                get { return _AngleYaw; }
                set { if (value == _AngleYaw) return; _AngleYaw = value; OnPropertyChanged();}
            }
            public override Compass_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Compass_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Compass_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Start = source.Start;
                dest.DataPacketType = source.DataPacketType;
                dest.AccelX = source.AccelX;
                dest.AccelY = source.AccelY;
                dest.AccelZ = source.AccelZ;
                dest.AngularVelocityX = source.AngularVelocityX;
                dest.AngularVelocityY = source.AngularVelocityY;
                dest.AngularVelocityZ = source.AngularVelocityZ;
                dest.AngleRoll = source.AngleRoll;
                dest.AnglePitch = source.AnglePitch;
                dest.AngleYaw = source.AngleYaw;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Compass_Data CopyToWithConvertAndCreate(Compass_Data source, Compass_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Start = convert(source.Start, "");
                dest.DataPacketType = convert(source.DataPacketType, "");
                dest.AccelX = convert(source.AccelX, "");
                dest.AccelY = convert(source.AccelY, "");
                dest.AccelZ = convert(source.AccelZ, "");
                dest.AngularVelocityX = convert(source.AngularVelocityX, "");
                dest.AngularVelocityY = convert(source.AngularVelocityY, "");
                dest.AngularVelocityZ = convert(source.AngularVelocityZ, "");
                dest.AngleRoll = convert(source.AngleRoll, "");
                dest.AnglePitch = convert(source.AnglePitch, "");
                dest.AngleYaw = convert(source.AngleYaw, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["AccelX", "AccelY", "AccelZ", "AngularVelocityX", "AngularVelocityY", "AngularVelocityZ", "AngleRoll", "AnglePitch", "AngleYaw"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(AccelX);
                exporter.CellSet(AccelY);
                exporter.CellSet(AccelZ);
                exporter.CellSet(AngularVelocityX);
                exporter.CellSet(AngularVelocityY);
                exporter.CellSet(AngularVelocityZ);
                exporter.CellSet(AngleRoll);
                exporter.CellSet(AnglePitch);
                exporter.CellSet(AngleYaw);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {AccelX} {AccelY} {AccelZ} {AngularVelocityX} {AngularVelocityY} {AngularVelocityZ} {AngleRoll} {AnglePitch} {AngleYaw}");
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

            private double _AddressResolutionSupported = 0;
            /// <summary>
            /// AddressResolutionSupported (U8 ) from Service=Common Configuration and Characteristic=Central Address Resolution
            ///</summary>
            public double AddressResolutionSupported 
            { 
                get { return _AddressResolutionSupported; }
                set { if (value == _AddressResolutionSupported) return; _AddressResolutionSupported = value; OnPropertyChanged();}
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
                dest.AddressResolutionSupported = source.AddressResolutionSupported;
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
                dest.AddressResolutionSupported = convert(source.AddressResolutionSupported, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "Interval_Min", "Interval_Max", "Latency", "Timeout", "AddressResolutionSupported"];
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
                exporter.CellSet(AddressResolutionSupported);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {Interval_Min} {Interval_Max} {Latency} {Timeout} {AddressResolutionSupported}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Compass_index = 0,
            Common_Configuration_index = 1,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Compass_CompassCommand_index = 0,     // GUID 0000ffe9-0000-1000-8000-00805f9a34fb
            Compass_CompassData_index = 1,     // GUID 0000ffe4-0000-1000-8000-00805f9a34fb
            Common_Configuration_Device_Name_index = 2,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 3,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 4,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Common_Configuration_Central_Address_Resolution_index = 5,     // GUID 00002aa6-0000-1000-8000-00805f9b34fb
        }

        // All of the services that this device supports
        /// <summary>
        /// Convenience GUID for Compass service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Compass = Guid.Parse("0000ffe5-0000-1000-8000-00805f9a34fb"); // #0 is Compass
        /// <summary>
        /// Convenience GUID for Common Configuration service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Common_Configuration = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"); // #1 is Common Configuration        

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("0000ffe5-0000-1000-8000-00805f9a34fb"), // #0 is Compass
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #1 is Common Configuration
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("0000ffe9-0000-1000-8000-00805f9a34fb"), // #0 is Compass CompassCommand
            Guid.Parse("0000ffe4-0000-1000-8000-00805f9a34fb"), // #1 is Compass CompassData
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #2 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #3 is Common Configuration Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #4 is Common Configuration Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #5 is Common Configuration Central Address Resolution
        };

        private List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null,  };


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


        #region Service_Compass
        // Service Compass 

        public Compass_Command CurrCompass_Command { get; set; } = new Compass_Command();

        // Per-characteristics methods for Compass CompassCommand
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCompassCommandAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("CompassCommand", ServiceIndex.Compass_index, "Compass", CharacteristicIndex.Compass_CompassCommand_index, NotifyCompassCommandCallback, notifyType);
            return retval;
        }

        private void NotifyCompassCommandCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Compass_CompassCommand_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Command");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCompass_Command.TimestampMostRecent = args.Timestamp;
            CurrCompass_Command.Command = vr.GetNextByteArray();
            OnPropertyChanged(CompassCommandPropertyChangedName); // "CompassCommand"
        }
        /// <summary>
        /// Reads data from CompassCommand and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Compass_Command of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Compass_Command> ReadCompassCommand(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Compass_CompassCommand_index;
            await Ensure_Characteristic_Async(ServiceIndex.Compass_index, "Compass", index, "CompassCommand");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "CompassCommand", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Command");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCompass_Command.Command = vr.GetNextByteArray();
            CurrCompass_Command.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(CompassCommandPropertyChangedName); // "CompassCommand"
            return CurrCompass_Command;
        }
        /// <summary>
        /// Writes data to CompassCommand 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteCompassCommand(byte[] data, GattCommunicationStatus exceptionStatus = GattCommunicationStatus.Unreachable)
        {
            var index = CharacteristicIndex.Compass_CompassCommand_index;
            await Ensure_Characteristic_Async(ServiceIndex.Compass_index, "Compass", index, "CompassCommand");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var buffer = data.AsBuffer();
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await ch.WriteValueAsync(buffer);
                Status.ReportStatus("WriteCompassCommand", result);
            }
            catch (Exception ex)
            {
                result = exceptionStatus;
                Status.ReportStatus($"WriteCompassCommand: Exception {ex.Message}", exceptionStatus);
            }
            return result;
        }
        // Service Compass 

        public Compass_Data CurrCompass_Data { get; set; } = new Compass_Data();

        // Per-characteristics methods for Compass CompassData
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCompassDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("CompassData", ServiceIndex.Compass_index, "Compass", CharacteristicIndex.Compass_CompassData_index, NotifyCompassDataCallback, notifyType);
            return retval;
        }

        private void NotifyCompassDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Compass_CompassData_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX^^HIDDEN|Start||0x55 U8|HEX^^HIDDEN|DataPacketType I16^32768_/_16_*|FIXED|AccelX I16^32768_/_16_*|FIXED|AccelY I16^32768_/_16_*|FIXED|AccelZ I16^32768_/_2000_*|FIXED|AngularVelocityX I16^32768_/_2000_*|FIXED|AngularVelocityY I16^32768_/_2000_*|FIXED|AngularVelocityZ I16^32768_/_180_*|FIXED|AngleRoll I16^32768_/_180_*|FIXED|AnglePitch I16^32768_/_180_*|FIXED|AngleYaw");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCompass_Data.TimestampMostRecent = args.Timestamp;
            CurrCompass_Data.Start = vr.GetNextDouble();
            CurrCompass_Data.DataPacketType = vr.GetNextDouble();
            CurrCompass_Data.AccelX = vr.GetNextDouble();
            CurrCompass_Data.AccelY = vr.GetNextDouble();
            CurrCompass_Data.AccelZ = vr.GetNextDouble();
            CurrCompass_Data.AngularVelocityX = vr.GetNextDouble();
            CurrCompass_Data.AngularVelocityY = vr.GetNextDouble();
            CurrCompass_Data.AngularVelocityZ = vr.GetNextDouble();
            CurrCompass_Data.AngleRoll = vr.GetNextDouble();
            CurrCompass_Data.AnglePitch = vr.GetNextDouble();
            CurrCompass_Data.AngleYaw = vr.GetNextDouble();
            OnPropertyChanged(CompassDataPropertyChangedName); // "CompassData"
        }
        /// <summary>
        /// Reads data from CompassData and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Compass_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Compass_Data> ReadCompassData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Compass_CompassData_index;
            await Ensure_Characteristic_Async(ServiceIndex.Compass_index, "Compass", index, "CompassData");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "CompassData", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX^^HIDDEN|Start||0x55 U8|HEX^^HIDDEN|DataPacketType I16^32768_/_16_*|FIXED|AccelX I16^32768_/_16_*|FIXED|AccelY I16^32768_/_16_*|FIXED|AccelZ I16^32768_/_2000_*|FIXED|AngularVelocityX I16^32768_/_2000_*|FIXED|AngularVelocityY I16^32768_/_2000_*|FIXED|AngularVelocityZ I16^32768_/_180_*|FIXED|AngleRoll I16^32768_/_180_*|FIXED|AnglePitch I16^32768_/_180_*|FIXED|AngleYaw");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCompass_Data.Start = vr.GetNextDouble();
            CurrCompass_Data.DataPacketType = vr.GetNextDouble();
            CurrCompass_Data.AccelX = vr.GetNextDouble();
            CurrCompass_Data.AccelY = vr.GetNextDouble();
            CurrCompass_Data.AccelZ = vr.GetNextDouble();
            CurrCompass_Data.AngularVelocityX = vr.GetNextDouble();
            CurrCompass_Data.AngularVelocityY = vr.GetNextDouble();
            CurrCompass_Data.AngularVelocityZ = vr.GetNextDouble();
            CurrCompass_Data.AngleRoll = vr.GetNextDouble();
            CurrCompass_Data.AnglePitch = vr.GetNextDouble();
            CurrCompass_Data.AngleYaw = vr.GetNextDouble();
            CurrCompass_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(CompassDataPropertyChangedName); // "CompassData"
            return CurrCompass_Data;
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
        // Per-characteristics methods for Common_Configuration Central_Address_Resolution
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCentral_Address_ResolutionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Central_Address_Resolution", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Central_Address_Resolution_index, NotifyCentral_Address_ResolutionCallback, notifyType);
            return retval;
        }

        private void NotifyCentral_Address_ResolutionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Central_Address_Resolution_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|AddressResolutionSupported");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.AddressResolutionSupported = vr.GetNextDouble();
            OnPropertyChanged(Central_Address_ResolutionPropertyChangedName); // "Central_Address_Resolution"
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
        /// <summary>
        /// Reads data from Central Address Resolution and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadCentral_Address_Resolution(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Central_Address_Resolution_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Central Address Resolution");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Central Address Resolution", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|AddressResolutionSupported");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.AddressResolutionSupported = vr.GetNextDouble();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Central_Address_ResolutionPropertyChangedName); // "Central_Address_Resolution"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Writes data to Device Name 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteDevice_Name(byte[] data, GattCommunicationStatus exceptionStatus = GattCommunicationStatus.Unreachable)
        {
            var index = CharacteristicIndex.Common_Configuration_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Device Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var buffer = data.AsBuffer();
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await ch.WriteValueAsync(buffer);
                Status.ReportStatus("WriteDevice_Name", result);
            }
            catch (Exception ex)
            {
                result = exceptionStatus;
                Status.ReportStatus($"WriteDevice_Name: Exception {ex.Message}", exceptionStatus);
            }
            return result;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}