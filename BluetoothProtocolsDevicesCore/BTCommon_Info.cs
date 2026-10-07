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
    /// This class was automatically generated 2026-10-07::08:43
    /// </summary>

    public  class BTCommon_Info : INotifyPropertyChanged
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

        public string Name { get; } = "BTCommon_Info";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device BTCommon_Info

        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Peripheral Privacy Flag characteristic has Flag (Byte-->double)  Guid=2a02
                Reconnect Address characteristic has ReconnectAddress (Bytes-->string)  Guid=2a03
                Connection Parameter characteristic has Interval_Min (UInt16-->double) Interval_Max (UInt16-->double) Latency (UInt16-->double) Timeout (UInt16-->double)  Guid=2a04
                Central Address Resolution characteristic has AddressResolutionSupported (Byte-->double)  Guid=2aa6
                Resolvable Private Address Only characteristic has ResolvablePrivateAddressFlag (Byte-->double)  Guid=2ac9


        Generic Service service Guid=1801
            Generic Service_Data (DataGroup record)
                Server_Supported_Features characteristic has FeatureServerBitmap (Byte-->double)  Guid=2803
                Service Changes characteristic has StartRange (UInt16-->double) EndRange (UInt16-->double)  Guid=2a05
                Client_Supported_Features characteristic has FeatureClientBitmap (Byte-->double)  Guid=2b29
                Database Hash characteristic has Hash0 (UInt32-->double) Hash1 (UInt32-->double) Hash2 (UInt32-->double) Hash3 (UInt32-->double)  Guid=2b2a


        Immediate Alert service Guid=1802
            Immediate Alert_Data (DataGroup record)
                Alert Immediate characteristic has LevelImmediate (Byte-->double)  Guid=2a06


        Link Loss Alert service Guid=1803
            Link Loss Alert_Data (DataGroup record)
                Alert LinkLoss characteristic has LevelLinkLoss (Byte-->double)  Guid=2a06


        Transmit Power service Guid=1804
            Transmit Power_Data (DataGroup record)
                Transmit Power characteristic has Power (SByte-->double)  Guid=2a07


        Device Info service Guid=180a
            Device Info_Data (DataGroup record)
                System ID characteristic has SystemId (String-->string)  Guid=2a23
                Model Number characteristic has ModelNumber (String-->string)  Guid=2a24
                Serial Number characteristic has SerialNumber (String-->string)  Guid=2a25
                Firmware Revision characteristic has FirmwareRevision (String-->string)  Guid=2a26
                Hardware Revision characteristic has HardwareRevision (String-->string)  Guid=2a27
                Software Revision characteristic has SoftwareRevision (String-->string)  Guid=2a28
                Manufacturer Name characteristic has ManufacturerName (String-->string)  Guid=2a29
                Regulatory List characteristic has BodyType (Byte-->double) BodyStructure (Byte-->double) Data (String-->string)  Guid=2a2a
                PnP ID characteristic has VendorIDSource (Byte-->double) VendorID (UInt16-->double) ProductID (UInt16-->double) ProductVersion (UInt16-->double)  Guid=2a50


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                BatteryLevel characteristic has BatteryLevel (SByte-->double)  Guid=2a19
        */

        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Peripheral_Privacy_FlagPropertyChangedName = "Peripheral_Privacy_Flag";
        public const string Reconnect_AddressPropertyChangedName = "Reconnect_Address";
        public const string Connection_ParameterPropertyChangedName = "Connection_Parameter";
        public const string Central_Address_ResolutionPropertyChangedName = "Central_Address_Resolution";
        public const string Resolvable_Private_Address_OnlyPropertyChangedName = "Resolvable_Private_Address_Only";
        public const string Server_Supported_FeaturesPropertyChangedName = "Server_Supported_Features";
        public const string Service_ChangesPropertyChangedName = "Service_Changes";
        public const string Client_Supported_FeaturesPropertyChangedName = "Client_Supported_Features";
        public const string Database_HashPropertyChangedName = "Database_Hash";
        public const string Alert_ImmediatePropertyChangedName = "Alert_Immediate";
        public const string Alert_LinkLossPropertyChangedName = "Alert_LinkLoss";
        public const string Transmit_PowerPropertyChangedName = "Transmit_Power";
        public const string System_IDPropertyChangedName = "System_ID";
        public const string Model_NumberPropertyChangedName = "Model_Number";
        public const string Serial_NumberPropertyChangedName = "Serial_Number";
        public const string Firmware_RevisionPropertyChangedName = "Firmware_Revision";
        public const string Hardware_RevisionPropertyChangedName = "Hardware_Revision";
        public const string Software_RevisionPropertyChangedName = "Software_Revision";
        public const string Manufacturer_NamePropertyChangedName = "Manufacturer_Name";
        public const string Regulatory_ListPropertyChangedName = "Regulatory_List";
        public const string PnP_IDPropertyChangedName = "PnP_ID";
        public const string BatteryLevelPropertyChangedName = "BatteryLevel";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
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

            private double _Appearance = 65535;
            /// <summary>
            /// Appearance (U16 ) from Service=Common Configuration and Characteristic=Appearance
            ///</summary>
            public double Appearance 
            { 
                get { return _Appearance; }
                set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged();}
            }

            private double _Flag = 0;
            /// <summary>
            /// Flag (U8 ) from Service=Common Configuration and Characteristic=Peripheral Privacy Flag
            ///</summary>
            public double Flag 
            { 
                get { return _Flag; }
                set { if (value == _Flag) return; _Flag = value; OnPropertyChanged();}
            }

            private byte[] _ReconnectAddress = null;
            /// <summary>
            /// ReconnectAddress (BYTES ) from Service=Common Configuration and Characteristic=Reconnect Address
            ///</summary>
            public byte[] ReconnectAddress 
            { 
                get { return _ReconnectAddress; }
                set { if (value == _ReconnectAddress) return; _ReconnectAddress = value; OnPropertyChanged();}
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
            private double _Interval_Max = -1;
            /// <summary>
            /// Interval_Max (U16 ms) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public double Interval_Max 
            { 
                get { return _Interval_Max; }
                set { if (value == _Interval_Max) return; _Interval_Max = value; OnPropertyChanged();}
            }
            private double _Latency = -1;
            /// <summary>
            /// Latency (U16 ms) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public double Latency 
            { 
                get { return _Latency; }
                set { if (value == _Latency) return; _Latency = value; OnPropertyChanged();}
            }
            private double _Timeout = -1;
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

            private double _ResolvablePrivateAddressFlag = 0;
            /// <summary>
            /// ResolvablePrivateAddressFlag (U8 ) from Service=Common Configuration and Characteristic=Resolvable Private Address Only
            ///</summary>
            public double ResolvablePrivateAddressFlag 
            { 
                get { return _ResolvablePrivateAddressFlag; }
                set { if (value == _ResolvablePrivateAddressFlag) return; _ResolvablePrivateAddressFlag = value; OnPropertyChanged();}
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
                dest.Flag = source.Flag;
                dest.ReconnectAddress = source.ReconnectAddress;
                dest.Interval_Min = source.Interval_Min;
                dest.Interval_Max = source.Interval_Max;
                dest.Latency = source.Latency;
                dest.Timeout = source.Timeout;
                dest.AddressResolutionSupported = source.AddressResolutionSupported;
                dest.ResolvablePrivateAddressFlag = source.ResolvablePrivateAddressFlag;
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
                dest.Flag = convert(source.Flag, "");
                dest.ReconnectAddress = source.ReconnectAddress;
                dest.Interval_Min = convert(source.Interval_Min, "ms");
                dest.Interval_Max = convert(source.Interval_Max, "ms");
                dest.Latency = convert(source.Latency, "ms");
                dest.Timeout = convert(source.Timeout, "ms");
                dest.AddressResolutionSupported = convert(source.AddressResolutionSupported, "");
                dest.ResolvablePrivateAddressFlag = convert(source.ResolvablePrivateAddressFlag, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "Flag", "ReconnectAddress", "Interval_Min", "Interval_Max", "Latency", "Timeout", "AddressResolutionSupported", "ResolvablePrivateAddressFlag"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(Flag);
                exporter.CellSet(ReconnectAddress);
                exporter.CellSet(Interval_Min);
                exporter.CellSet(Interval_Max);
                exporter.CellSet(Latency);
                exporter.CellSet(Timeout);
                exporter.CellSet(AddressResolutionSupported);
                exporter.CellSet(ResolvablePrivateAddressFlag);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {Flag} {ReconnectAddress} {Interval_Min} {Interval_Max} {Latency} {Timeout} {AddressResolutionSupported} {ResolvablePrivateAddressFlag}");
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
            private double _FeatureServerBitmap = 0;
            /// <summary>
            /// FeatureServerBitmap (U8 ) from Service=Generic Service and Characteristic=Server_Supported_Features
            ///</summary>
            public double FeatureServerBitmap 
            { 
                get { return _FeatureServerBitmap; }
                set { if (value == _FeatureServerBitmap) return; _FeatureServerBitmap = value; OnPropertyChanged();}
            }

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

            private double _FeatureClientBitmap = 0;
            /// <summary>
            /// FeatureClientBitmap (U8 ) from Service=Generic Service and Characteristic=Client_Supported_Features
            ///</summary>
            public double FeatureClientBitmap 
            { 
                get { return _FeatureClientBitmap; }
                set { if (value == _FeatureClientBitmap) return; _FeatureClientBitmap = value; OnPropertyChanged();}
            }

            private double _Hash0 = 0;
            /// <summary>
            /// Hash0 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash0 
            { 
                get { return _Hash0; }
                set { if (value == _Hash0) return; _Hash0 = value; OnPropertyChanged();}
            }
            private double _Hash1 = 0;
            /// <summary>
            /// Hash1 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash1 
            { 
                get { return _Hash1; }
                set { if (value == _Hash1) return; _Hash1 = value; OnPropertyChanged();}
            }
            private double _Hash2 = 0;
            /// <summary>
            /// Hash2 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash2 
            { 
                get { return _Hash2; }
                set { if (value == _Hash2) return; _Hash2 = value; OnPropertyChanged();}
            }
            private double _Hash3 = 0;
            /// <summary>
            /// Hash3 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash3 
            { 
                get { return _Hash3; }
                set { if (value == _Hash3) return; _Hash3 = value; OnPropertyChanged();}
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
                dest.FeatureServerBitmap = source.FeatureServerBitmap;
                dest.StartRange = source.StartRange;
                dest.EndRange = source.EndRange;
                dest.FeatureClientBitmap = source.FeatureClientBitmap;
                dest.Hash0 = source.Hash0;
                dest.Hash1 = source.Hash1;
                dest.Hash2 = source.Hash2;
                dest.Hash3 = source.Hash3;
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
                dest.FeatureServerBitmap = convert(source.FeatureServerBitmap, "");
                dest.StartRange = convert(source.StartRange, "");
                dest.EndRange = convert(source.EndRange, "");
                dest.FeatureClientBitmap = convert(source.FeatureClientBitmap, "");
                dest.Hash0 = convert(source.Hash0, "");
                dest.Hash1 = convert(source.Hash1, "");
                dest.Hash2 = convert(source.Hash2, "");
                dest.Hash3 = convert(source.Hash3, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["FeatureServerBitmap", "StartRange", "EndRange", "FeatureClientBitmap", "Hash0", "Hash1", "Hash2", "Hash3"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(FeatureServerBitmap);
                exporter.CellSet(StartRange);
                exporter.CellSet(EndRange);
                exporter.CellSet(FeatureClientBitmap);
                exporter.CellSet(Hash0);
                exporter.CellSet(Hash1);
                exporter.CellSet(Hash2);
                exporter.CellSet(Hash3);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {FeatureServerBitmap} {StartRange} {EndRange} {FeatureClientBitmap} {Hash0} {Hash1} {Hash2} {Hash3}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Immediate Alert Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Immediate_Alert_Data :BTCommonMetaData<Immediate_Alert_Data> //, IExportDataSource
        {
            private double _LevelImmediate = 0;
            /// <summary>
            /// LevelImmediate (U8 ) from Service=Immediate Alert and Characteristic=Alert Immediate
            ///</summary>
            public double LevelImmediate 
            { 
                get { return _LevelImmediate; }
                set { if (value == _LevelImmediate) return; _LevelImmediate = value; OnPropertyChanged();}
            }
            public override Immediate_Alert_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Immediate_Alert_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Immediate_Alert_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.LevelImmediate = source.LevelImmediate;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Immediate_Alert_Data CopyToWithConvertAndCreate(Immediate_Alert_Data source, Immediate_Alert_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.LevelImmediate = convert(source.LevelImmediate, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["LevelImmediate"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(LevelImmediate);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {LevelImmediate}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Link Loss Alert Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Link_Loss_Alert_Data :BTCommonMetaData<Link_Loss_Alert_Data> //, IExportDataSource
        {
            private double _LevelLinkLoss = 0;
            /// <summary>
            /// LevelLinkLoss (U8 ) from Service=Link Loss Alert and Characteristic=Alert LinkLoss
            ///</summary>
            public double LevelLinkLoss 
            { 
                get { return _LevelLinkLoss; }
                set { if (value == _LevelLinkLoss) return; _LevelLinkLoss = value; OnPropertyChanged();}
            }
            public override Link_Loss_Alert_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Link_Loss_Alert_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Link_Loss_Alert_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.LevelLinkLoss = source.LevelLinkLoss;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Link_Loss_Alert_Data CopyToWithConvertAndCreate(Link_Loss_Alert_Data source, Link_Loss_Alert_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.LevelLinkLoss = convert(source.LevelLinkLoss, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["LevelLinkLoss"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(LevelLinkLoss);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {LevelLinkLoss}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Transmit Power Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Transmit_Power_Data :BTCommonMetaData<Transmit_Power_Data> //, IExportDataSource
        {
            private double _Power = 0;
            /// <summary>
            /// Power (I8 db) from Service=Transmit Power and Characteristic=Transmit Power
            ///</summary>
            public double Power 
            { 
                get { return _Power; }
                set { if (value == _Power) return; _Power = value; OnPropertyChanged();}
            }
            public override Transmit_Power_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Transmit_Power_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Transmit_Power_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Power = source.Power;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Transmit_Power_Data CopyToWithConvertAndCreate(Transmit_Power_Data source, Transmit_Power_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Power = convert(source.Power, "db");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Power"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Power);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Power}");
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

            private string _ManufacturerName = "";
            /// <summary>
            /// ManufacturerName (STRING ) from Service=Device Info and Characteristic=Manufacturer Name
            ///</summary>
            public string ManufacturerName 
            { 
                get { return _ManufacturerName; }
                set { if (value == _ManufacturerName) return; _ManufacturerName = value; OnPropertyChanged();}
            }

            private double _BodyType = 0;
            /// <summary>
            /// BodyType (U8 ) from Service=Device Info and Characteristic=Regulatory List
            ///</summary>
            public double BodyType 
            { 
                get { return _BodyType; }
                set { if (value == _BodyType) return; _BodyType = value; OnPropertyChanged();}
            }
            private double _BodyStructure = 0;
            /// <summary>
            /// BodyStructure (U8 ) from Service=Device Info and Characteristic=Regulatory List
            ///</summary>
            public double BodyStructure 
            { 
                get { return _BodyStructure; }
                set { if (value == _BodyStructure) return; _BodyStructure = value; OnPropertyChanged();}
            }
            private string _Data = "";
            /// <summary>
            /// Data (STRING ) from Service=Device Info and Characteristic=Regulatory List
            ///</summary>
            public string Data 
            { 
                get { return _Data; }
                set { if (value == _Data) return; _Data = value; OnPropertyChanged();}
            }

            private double _VendorIDSource = 255;
            /// <summary>
            /// VendorIDSource (U8 ) from Service=Device Info and Characteristic=PnP ID
            ///</summary>
            public double VendorIDSource 
            { 
                get { return _VendorIDSource; }
                set { if (value == _VendorIDSource) return; _VendorIDSource = value; OnPropertyChanged();}
            }
            private double _VendorID = 65535;
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
                dest.SystemId = source.SystemId;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
                dest.BodyType = source.BodyType;
                dest.BodyStructure = source.BodyStructure;
                dest.Data = source.Data;
                dest.VendorIDSource = source.VendorIDSource;
                dest.VendorID = source.VendorID;
                dest.ProductID = source.ProductID;
                dest.ProductVersion = source.ProductVersion;
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
                dest.SystemId = source.SystemId;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
                dest.BodyType = convert(source.BodyType, "");
                dest.BodyStructure = convert(source.BodyStructure, "");
                dest.Data = source.Data;
                dest.VendorIDSource = convert(source.VendorIDSource, "");
                dest.VendorID = convert(source.VendorID, "");
                dest.ProductID = convert(source.ProductID, "");
                dest.ProductVersion = convert(source.ProductVersion, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["SystemId", "ModelNumber", "SerialNumber", "FirmwareRevision", "HardwareRevision", "SoftwareRevision", "ManufacturerName", "BodyType", "BodyStructure", "Data", "VendorIDSource", "VendorID", "ProductID", "ProductVersion"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(SystemId);
                exporter.CellSet(ModelNumber);
                exporter.CellSet(SerialNumber);
                exporter.CellSet(FirmwareRevision);
                exporter.CellSet(HardwareRevision);
                exporter.CellSet(SoftwareRevision);
                exporter.CellSet(ManufacturerName);
                exporter.CellSet(BodyType);
                exporter.CellSet(BodyStructure);
                exporter.CellSet(Data);
                exporter.CellSet(VendorIDSource);
                exporter.CellSet(VendorID);
                exporter.CellSet(ProductID);
                exporter.CellSet(ProductVersion);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {SystemId} {ModelNumber} {SerialNumber} {FirmwareRevision} {HardwareRevision} {SoftwareRevision} {ManufacturerName} {BodyType} {BodyStructure} {Data} {VendorIDSource} {VendorID} {ProductID} {ProductVersion}");
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
            private double _BatteryLevel = -1;
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


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Common_Configuration_index = 0,
            Generic_Service_index = 1,
            Immediate_Alert_index = 2,
            Link_Loss_Alert_index = 3,
            Transmit_Power_index = 4,
            Device_Info_index = 5,
            Battery_index = 6,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Common_Configuration_Device_Name_index = 0,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 1,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Peripheral_Privacy_Flag_index = 2,     // GUID 00002a02-0000-1000-8000-00805f9b34fb
            Common_Configuration_Reconnect_Address_index = 3,     // GUID 00002a03-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 4,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Common_Configuration_Central_Address_Resolution_index = 5,     // GUID 00002aa6-0000-1000-8000-00805f9b34fb
            Common_Configuration_Resolvable_Private_Address_Only_index = 6,     // GUID 00002ac9-0000-1000-8000-00805f9b34fb
            Generic_Service_Server_Supported_Features_index = 7,     // GUID 00002803-0000-1000-8000-00805f9b34fb
            Generic_Service_Service_Changes_index = 8,     // GUID 00002a05-0000-1000-8000-00805f9b34fb
            Generic_Service_Client_Supported_Features_index = 9,     // GUID 00002b29-0000-1000-8000-00805f9b34fb
            Generic_Service_Database_Hash_index = 10,     // GUID 00002b2a-0000-1000-8000-00805f9b34fb
            Immediate_Alert_Alert_Immediate_index = 11,     // GUID 00002a06-0000-1000-8000-00805f9b34fb
            Link_Loss_Alert_Alert_LinkLoss_index = 12,     // GUID 00002a06-0000-1000-8000-00805f9b34fb
            Transmit_Power_Transmit_Power_index = 13,     // GUID 00002a07-0000-1000-8000-00805f9b34fb
            Device_Info_System_ID_index = 14,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
            Device_Info_Model_Number_index = 15,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Info_Serial_Number_index = 16,     // GUID 00002a25-0000-1000-8000-00805f9b34fb
            Device_Info_Firmware_Revision_index = 17,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Info_Hardware_Revision_index = 18,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Info_Software_Revision_index = 19,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Info_Manufacturer_Name_index = 20,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Info_Regulatory_List_index = 21,     // GUID 00002a2a-0000-1000-8000-00805f9b34fb
            Device_Info_PnP_ID_index = 22,     // GUID 00002a50-0000-1000-8000-00805f9b34fb
            Battery_BatteryLevel_index = 23,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
        }

        // All of the services that this device supports
        /// <summary>
        /// Convenience GUID for Common Configuration service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Common_Configuration = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"); // #0 is Common Configuration
        /// <summary>
        /// Convenience GUID for Generic Service service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Generic_Service = Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"); // #1 is Generic Service
        /// <summary>
        /// Convenience GUID for Immediate Alert service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Immediate_Alert = Guid.Parse("00001802-0000-1000-8000-00805f9b34fb"); // #2 is Immediate Alert
        /// <summary>
        /// Convenience GUID for Link Loss Alert service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Link_Loss_Alert = Guid.Parse("00001803-0000-1000-8000-00805f9b34fb"); // #3 is Link Loss Alert
        /// <summary>
        /// Convenience GUID for Transmit Power service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Transmit_Power = Guid.Parse("00001804-0000-1000-8000-00805f9b34fb"); // #4 is Transmit Power
        /// <summary>
        /// Convenience GUID for Device Info service. Includes details on the manufacturer, model, firmware versions, and PNP ID
        /// </summary>
        public static readonly Guid ServiceGuid_Device_Info = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"); // #5 is Device Info
        /// <summary>
        /// Convenience GUID for Battery service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Battery = Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"); // #6 is Battery        

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #0 is Common Configuration
            Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"), // #1 is Generic Service
            Guid.Parse("00001802-0000-1000-8000-00805f9b34fb"), // #2 is Immediate Alert
            Guid.Parse("00001803-0000-1000-8000-00805f9b34fb"), // #3 is Link Loss Alert
            Guid.Parse("00001804-0000-1000-8000-00805f9b34fb"), // #4 is Transmit Power
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #5 is Device Info
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // #6 is Battery
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
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Common Configuration Appearance
            Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb"), // #2 is Common Configuration Peripheral Privacy Flag
            Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb"), // #3 is Common Configuration Reconnect Address
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #4 is Common Configuration Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #5 is Common Configuration Central Address Resolution
            Guid.Parse("00002ac9-0000-1000-8000-00805f9b34fb"), // #6 is Common Configuration Resolvable Private Address Only
            Guid.Parse("00002803-0000-1000-8000-00805f9b34fb"), // #7 is Generic Service Server_Supported_Features
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #8 is Generic Service Service Changes
            Guid.Parse("00002b29-0000-1000-8000-00805f9b34fb"), // #9 is Generic Service Client_Supported_Features
            Guid.Parse("00002b2a-0000-1000-8000-00805f9b34fb"), // #10 is Generic Service Database Hash
            Guid.Parse("00002a06-0000-1000-8000-00805f9b34fb"), // #11 is Immediate Alert Alert Immediate
            Guid.Parse("00002a06-0000-1000-8000-00805f9b34fb"), // #12 is Link Loss Alert Alert LinkLoss
            Guid.Parse("00002a07-0000-1000-8000-00805f9b34fb"), // #13 is Transmit Power Transmit Power
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #14 is Device Info System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #15 is Device Info Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #16 is Device Info Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #17 is Device Info Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #18 is Device Info Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #19 is Device Info Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #20 is Device Info Manufacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #21 is Device Info Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #22 is Device Info PnP ID
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #23 is Battery BatteryLevel
        };

        private List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };


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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance||65535");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Appearance = vr.GetNextDouble();
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
        }
        // Per-characteristics methods for Common_Configuration Peripheral_Privacy_Flag
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPeripheral_Privacy_FlagAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Peripheral_Privacy_Flag", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Peripheral_Privacy_Flag_index, NotifyPeripheral_Privacy_FlagCallback, notifyType);
            return retval;
        }

        private void NotifyPeripheral_Privacy_FlagCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Peripheral_Privacy_Flag_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Flag");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Flag = vr.GetNextDouble();
            OnPropertyChanged(Peripheral_Privacy_FlagPropertyChangedName); // "Peripheral_Privacy_Flag"
        }
        // Per-characteristics methods for Common_Configuration Reconnect_Address
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyReconnect_AddressAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Reconnect_Address", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Reconnect_Address_index, NotifyReconnect_AddressCallback, notifyType);
            return retval;
        }

        private void NotifyReconnect_AddressCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Reconnect_Address_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReconnectAddress");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.ReconnectAddress = vr.GetNextByteArray();
            OnPropertyChanged(Reconnect_AddressPropertyChangedName); // "Reconnect_Address"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16^1.25_*|DEC|Interval_Min|ms|-1 U16^1.25_*|DEC|Interval_Max|ms|-1 U16|DEC|Latency|ms|-1 U16^10_*|DEC|Timeout|ms|-1");
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
        // Per-characteristics methods for Common_Configuration Resolvable_Private_Address_Only
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyResolvable_Private_Address_OnlyAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Resolvable_Private_Address_Only", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Resolvable_Private_Address_Only_index, NotifyResolvable_Private_Address_OnlyCallback, notifyType);
            return retval;
        }

        private void NotifyResolvable_Private_Address_OnlyCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Resolvable_Private_Address_Only_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|ResolvablePrivateAddressFlag");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.ResolvablePrivateAddressFlag = vr.GetNextDouble();
            OnPropertyChanged(Resolvable_Private_Address_OnlyPropertyChangedName); // "Resolvable_Private_Address_Only"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance||65535");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Appearance = vr.GetNextDouble();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Reads data from Peripheral Privacy Flag and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadPeripheral_Privacy_Flag(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Peripheral_Privacy_Flag_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Peripheral Privacy Flag");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Peripheral Privacy Flag", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Flag");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Flag = vr.GetNextDouble();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Peripheral_Privacy_FlagPropertyChangedName); // "Peripheral_Privacy_Flag"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Reads data from Reconnect Address and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadReconnect_Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Reconnect_Address_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Reconnect Address");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Reconnect Address", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReconnectAddress");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.ReconnectAddress = vr.GetNextByteArray();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Reconnect_AddressPropertyChangedName); // "Reconnect_Address"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16^1.25_*|DEC|Interval_Min|ms|-1 U16^1.25_*|DEC|Interval_Max|ms|-1 U16|DEC|Latency|ms|-1 U16^10_*|DEC|Timeout|ms|-1");
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
        /// Reads data from Resolvable Private Address Only and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadResolvable_Private_Address_Only(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Resolvable_Private_Address_Only_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Resolvable Private Address Only");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Resolvable Private Address Only", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|ResolvablePrivateAddressFlag");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.ResolvablePrivateAddressFlag = vr.GetNextDouble();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Resolvable_Private_Address_OnlyPropertyChangedName); // "Resolvable_Private_Address_Only"
            return CurrCommon_Configuration_Data;
        }

        #endregion
//
        #region Service_Generic_Service
        // Service Generic Service 

        public Generic_Service_Data CurrGeneric_Service_Data { get; set; } = new Generic_Service_Data();

        // Per-characteristics methods for Generic_Service Server_Supported_Features
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyServer_Supported_FeaturesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Server_Supported_Features", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Server_Supported_Features_index, NotifyServer_Supported_FeaturesCallback, notifyType);
            return retval;
        }

        private void NotifyServer_Supported_FeaturesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Server_Supported_Features_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureServerBitmap");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.FeatureServerBitmap = vr.GetNextDouble();
            OnPropertyChanged(Server_Supported_FeaturesPropertyChangedName); // "Server_Supported_Features"
        }
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
        // Per-characteristics methods for Generic_Service Client_Supported_Features
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyClient_Supported_FeaturesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Client_Supported_Features", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Client_Supported_Features_index, NotifyClient_Supported_FeaturesCallback, notifyType);
            return retval;
        }

        private void NotifyClient_Supported_FeaturesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Client_Supported_Features_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureClientBitmap");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.FeatureClientBitmap = vr.GetNextDouble();
            OnPropertyChanged(Client_Supported_FeaturesPropertyChangedName); // "Client_Supported_Features"
        }
        // Per-characteristics methods for Generic_Service Database_Hash
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDatabase_HashAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Database_Hash", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Database_Hash_index, NotifyDatabase_HashCallback, notifyType);
            return retval;
        }

        private void NotifyDatabase_HashCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Database_Hash_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U32|HEX|Hash0 U32|HEX|Hash1 U32|HEX|Hash2 U32|HEX|Hash3");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.Hash0 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash1 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash2 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash3 = vr.GetNextDouble();
            OnPropertyChanged(Database_HashPropertyChangedName); // "Database_Hash"
        }
        /// <summary>
        /// Reads data from Server_Supported_Features and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadServer_Supported_Features(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Server_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Server_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Server_Supported_Features", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureServerBitmap");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.FeatureServerBitmap = vr.GetNextDouble();
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Server_Supported_FeaturesPropertyChangedName); // "Server_Supported_Features"
            return CurrGeneric_Service_Data;
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
        /// <summary>
        /// Reads data from Client_Supported_Features and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadClient_Supported_Features(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Client_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Client_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Client_Supported_Features", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureClientBitmap");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.FeatureClientBitmap = vr.GetNextDouble();
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Client_Supported_FeaturesPropertyChangedName); // "Client_Supported_Features"
            return CurrGeneric_Service_Data;
        }
        /// <summary>
        /// Reads data from Database Hash and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadDatabase_Hash(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Database_Hash_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Database Hash");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Database Hash", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U32|HEX|Hash0 U32|HEX|Hash1 U32|HEX|Hash2 U32|HEX|Hash3");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.Hash0 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash1 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash2 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash3 = vr.GetNextDouble();
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Database_HashPropertyChangedName); // "Database_Hash"
            return CurrGeneric_Service_Data;
        }
        /// <summary>
        /// Writes data to Server_Supported_Features 
        /// </summary>
        public async Task WriteServer_Supported_Features(byte[] data)
        {
            var index = CharacteristicIndex.Generic_Service_Server_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Server_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteServer_Supported_Features", result);
        }
        /// <summary>
        /// Writes data to Client_Supported_Features 
        /// </summary>
        public async Task WriteClient_Supported_Features(byte[] data)
        {
            var index = CharacteristicIndex.Generic_Service_Client_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Client_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteClient_Supported_Features", result);
        }

        #endregion
//
        #region Service_Immediate_Alert
        // Service Immediate Alert 

        public Immediate_Alert_Data CurrImmediate_Alert_Data { get; set; } = new Immediate_Alert_Data();

        // Per-characteristics methods for Immediate_Alert Alert_Immediate
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAlert_ImmediateAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Alert_Immediate", ServiceIndex.Immediate_Alert_index, "Immediate Alert", CharacteristicIndex.Immediate_Alert_Alert_Immediate_index, NotifyAlert_ImmediateCallback, notifyType);
            return retval;
        }

        private void NotifyAlert_ImmediateCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Immediate_Alert_Alert_Immediate_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|LevelImmediate");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrImmediate_Alert_Data.TimestampMostRecent = args.Timestamp;
            CurrImmediate_Alert_Data.LevelImmediate = vr.GetNextDouble();
            OnPropertyChanged(Alert_ImmediatePropertyChangedName); // "Alert_Immediate"
        }
        /// <summary>
        /// Reads data from Alert Immediate and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Immediate_Alert_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Immediate_Alert_Data> ReadAlert_Immediate(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Immediate_Alert_Alert_Immediate_index;
            await Ensure_Characteristic_Async(ServiceIndex.Immediate_Alert_index, "Immediate Alert", index, "Alert Immediate");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Alert Immediate", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|LevelImmediate");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrImmediate_Alert_Data.LevelImmediate = vr.GetNextDouble();
            CurrImmediate_Alert_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Alert_ImmediatePropertyChangedName); // "Alert_Immediate"
            return CurrImmediate_Alert_Data;
        }
        /// <summary>
        /// Writes data to Alert Immediate 
        /// </summary>
        public async Task WriteAlert_Immediate(byte[] data)
        {
            var index = CharacteristicIndex.Immediate_Alert_Alert_Immediate_index;
            await Ensure_Characteristic_Async(ServiceIndex.Immediate_Alert_index, "Immediate Alert", index, "Alert Immediate");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteAlert_Immediate", result);
        }

        #endregion
//
        #region Service_Link_Loss_Alert
        // Service Link Loss Alert 

        public Link_Loss_Alert_Data CurrLink_Loss_Alert_Data { get; set; } = new Link_Loss_Alert_Data();

        // Per-characteristics methods for Link_Loss_Alert Alert_LinkLoss
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAlert_LinkLossAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Alert_LinkLoss", ServiceIndex.Link_Loss_Alert_index, "Link Loss Alert", CharacteristicIndex.Link_Loss_Alert_Alert_LinkLoss_index, NotifyAlert_LinkLossCallback, notifyType);
            return retval;
        }

        private void NotifyAlert_LinkLossCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Link_Loss_Alert_Alert_LinkLoss_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|LevelLinkLoss");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrLink_Loss_Alert_Data.TimestampMostRecent = args.Timestamp;
            CurrLink_Loss_Alert_Data.LevelLinkLoss = vr.GetNextDouble();
            OnPropertyChanged(Alert_LinkLossPropertyChangedName); // "Alert_LinkLoss"
        }
        /// <summary>
        /// Reads data from Alert LinkLoss and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Link_Loss_Alert_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Link_Loss_Alert_Data> ReadAlert_LinkLoss(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Link_Loss_Alert_Alert_LinkLoss_index;
            await Ensure_Characteristic_Async(ServiceIndex.Link_Loss_Alert_index, "Link Loss Alert", index, "Alert LinkLoss");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Alert LinkLoss", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|LevelLinkLoss");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrLink_Loss_Alert_Data.LevelLinkLoss = vr.GetNextDouble();
            CurrLink_Loss_Alert_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Alert_LinkLossPropertyChangedName); // "Alert_LinkLoss"
            return CurrLink_Loss_Alert_Data;
        }
        /// <summary>
        /// Writes data to Alert LinkLoss 
        /// </summary>
        public async Task WriteAlert_LinkLoss(byte[] data)
        {
            var index = CharacteristicIndex.Link_Loss_Alert_Alert_LinkLoss_index;
            await Ensure_Characteristic_Async(ServiceIndex.Link_Loss_Alert_index, "Link Loss Alert", index, "Alert LinkLoss");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteAlert_LinkLoss", result);
        }

        #endregion
//
        #region Service_Transmit_Power
        // Service Transmit Power 

        public Transmit_Power_Data CurrTransmit_Power_Data { get; set; } = new Transmit_Power_Data();

        // Per-characteristics methods for Transmit_Power Transmit_Power
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTransmit_PowerAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Transmit_Power", ServiceIndex.Transmit_Power_index, "Transmit Power", CharacteristicIndex.Transmit_Power_Transmit_Power_index, NotifyTransmit_PowerCallback, notifyType);
            return retval;
        }

        private void NotifyTransmit_PowerCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Transmit_Power_Transmit_Power_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|Power|db");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTransmit_Power_Data.TimestampMostRecent = args.Timestamp;
            CurrTransmit_Power_Data.Power = vr.GetNextDouble();
            OnPropertyChanged(Transmit_PowerPropertyChangedName); // "Transmit_Power"
        }
        /// <summary>
        /// Reads data from Transmit Power and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Transmit_Power_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Transmit_Power_Data> ReadTransmit_Power(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Transmit_Power_Transmit_Power_index;
            await Ensure_Characteristic_Async(ServiceIndex.Transmit_Power_index, "Transmit Power", index, "Transmit Power");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Transmit Power", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|Power|db");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTransmit_Power_Data.Power = vr.GetNextDouble();
            CurrTransmit_Power_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Transmit_PowerPropertyChangedName); // "Transmit_Power"
            return CurrTransmit_Power_Data;
        }

        #endregion
//
        #region Service_Device_Info
        // Service Device Info 

        public Device_Info_Data CurrDevice_Info_Data { get; set; } = new Device_Info_Data();

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
        // Per-characteristics methods for Device_Info Manufacturer_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyManufacturer_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Manufacturer_Name", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Manufacturer_Name_index, NotifyManufacturer_NameCallback, notifyType);
            return retval;
        }

        private void NotifyManufacturer_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Manufacturer_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|ManufacturerName");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.ManufacturerName = vr.GetNextString();
            OnPropertyChanged(Manufacturer_NamePropertyChangedName); // "Manufacturer_Name"
        }
        // Per-characteristics methods for Device_Info Regulatory_List
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyRegulatory_ListAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Regulatory_List", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Regulatory_List_index, NotifyRegulatory_ListCallback, notifyType);
            return retval;
        }

        private void NotifyRegulatory_ListCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Regulatory_List_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|BodyType U8|HEX|BodyStructure STRING|ASCII|Data");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.BodyType = vr.GetNextDouble();
            CurrDevice_Info_Data.BodyStructure = vr.GetNextDouble();
            CurrDevice_Info_Data.Data = vr.GetNextString();
            OnPropertyChanged(Regulatory_ListPropertyChangedName); // "Regulatory_List"
        }
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|VendorIDSource||255 U16|DEC|VendorID||65535 U16|DEC|ProductID U16|DEC|ProductVersion");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.VendorIDSource = vr.GetNextDouble();
            CurrDevice_Info_Data.VendorID = vr.GetNextDouble();
            CurrDevice_Info_Data.ProductID = vr.GetNextDouble();
            CurrDevice_Info_Data.ProductVersion = vr.GetNextDouble();
            OnPropertyChanged(PnP_IDPropertyChangedName); // "PnP_ID"
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
        /// <summary>
        /// Reads data from Manufacturer Name and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadManufacturer_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Manufacturer_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Manufacturer Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Manufacturer Name", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|ManufacturerName");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.ManufacturerName = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Manufacturer_NamePropertyChangedName); // "Manufacturer_Name"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Regulatory List and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadRegulatory_List(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Regulatory_List_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Regulatory List");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Regulatory List", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|BodyType U8|HEX|BodyStructure STRING|ASCII|Data");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.BodyType = vr.GetNextDouble();
            CurrDevice_Info_Data.BodyStructure = vr.GetNextDouble();
            CurrDevice_Info_Data.Data = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Regulatory_ListPropertyChangedName); // "Regulatory_List"
            return CurrDevice_Info_Data;
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|VendorIDSource||255 U16|DEC|VendorID||65535 U16|DEC|ProductID U16|DEC|ProductVersion");
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%|-1");
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%|-1");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            CurrBattery_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
            return CurrBattery_Data;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}