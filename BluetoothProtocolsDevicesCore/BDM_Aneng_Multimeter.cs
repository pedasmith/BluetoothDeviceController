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
    /// This class was automatically generated 2026-10-11::19:59
    /// </summary>

    public  class BDM_Aneng_Multimeter : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://github.com/libreble/multimeter/blob/main/docs/protocols/bdm.md
        // Link: https://github.com/libreble/multimeter/blob/main/packages/protocol/src/drivers/bdm.ts

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "Bluetooth DMM";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device Bluetooth DMM

        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01


        Multimeter service Guid=fff0
            Multimeter_Data (DataGroup record)
                MultimeterData characteristic has Data (Bytes-->string)  Guid=fff4


        Device Info service Guid=180a
            Device Info_Data (DataGroup record)
                Manufacturer Name characteristic has ManufacturerName (String-->string)  Guid=2a29
                Model Number characteristic has ModelNumber (String-->string)  Guid=2a24
                Serial Number characteristic has SerialNumber (String-->string)  Guid=2a25
                Hardware Revision characteristic has HardwareRevision (String-->string)  Guid=2a27
                Firmware Revision characteristic has FirmwareRevision (String-->string)  Guid=2a26
                Software Revision characteristic has SoftwareRevision (String-->string)  Guid=2a28
                System ID characteristic has SystemId (String-->string)  Guid=2a23
                Regulatory List characteristic has BodyType (Byte-->double) BodyStructure (Byte-->double) Data (String-->string)  Guid=2a2a
                PnP ID characteristic has VendorIDSource (Byte-->double) VendorID (UInt16-->double) ProductID (UInt16-->double) ProductVersion (UInt16-->double)  Guid=2a50
        */

        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string MultimeterDataPropertyChangedName = "MultimeterData";
        public const string Manufacturer_NamePropertyChangedName = "Manufacturer_Name";
        public const string Model_NumberPropertyChangedName = "Model_Number";
        public const string Serial_NumberPropertyChangedName = "Serial_Number";
        public const string Hardware_RevisionPropertyChangedName = "Hardware_Revision";
        public const string Firmware_RevisionPropertyChangedName = "Firmware_Revision";
        public const string Software_RevisionPropertyChangedName = "Software_Revision";
        public const string System_IDPropertyChangedName = "System_ID";
        public const string Regulatory_ListPropertyChangedName = "Regulatory_List";
        public const string PnP_IDPropertyChangedName = "PnP_ID";



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

            private double _Appearance = 0;
            /// <summary>
            /// Appearance (U16 ) from Service=Common Configuration and Characteristic=Appearance
            ///</summary>
            public double Appearance 
            { 
                get { return _Appearance; }
                set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged();}
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
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Multimeter Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Multimeter_Data :BTCommonMetaData<Multimeter_Data> //, IExportDataSource
        {
            private byte[] _Data = null;
            /// <summary>
            /// Data (BYTES ) from Service=Multimeter and Characteristic=MultimeterData
            ///</summary>
            public byte[] Data 
            { 
                get { return _Data; }
                set { if (value == _Data) return; _Data = value; OnPropertyChanged();}
            }
            public override Multimeter_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Multimeter_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Multimeter_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Data = source.Data;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Multimeter_Data CopyToWithConvertAndCreate(Multimeter_Data source, Multimeter_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Data = source.Data;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Data"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Data);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Data}");
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
            private string _ManufacturerName = "";
            /// <summary>
            /// ManufacturerName (STRING ) from Service=Device Info and Characteristic=Manufacturer Name
            ///</summary>
            public string ManufacturerName 
            { 
                get { return _ManufacturerName; }
                set { if (value == _ManufacturerName) return; _ManufacturerName = value; OnPropertyChanged();}
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

            private string _HardwareRevision = "";
            /// <summary>
            /// HardwareRevision (STRING ) from Service=Device Info and Characteristic=Hardware Revision
            ///</summary>
            public string HardwareRevision 
            { 
                get { return _HardwareRevision; }
                set { if (value == _HardwareRevision) return; _HardwareRevision = value; OnPropertyChanged();}
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

            private string _SoftwareRevision = "";
            /// <summary>
            /// SoftwareRevision (STRING ) from Service=Device Info and Characteristic=Software Revision
            ///</summary>
            public string SoftwareRevision 
            { 
                get { return _SoftwareRevision; }
                set { if (value == _SoftwareRevision) return; _SoftwareRevision = value; OnPropertyChanged();}
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
                dest.ManufacturerName = source.ManufacturerName;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.HardwareRevision = source.HardwareRevision;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.SystemId = source.SystemId;
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
                dest.ManufacturerName = source.ManufacturerName;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.HardwareRevision = source.HardwareRevision;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.SystemId = source.SystemId;
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
                return ["ManufacturerName", "ModelNumber", "SerialNumber", "HardwareRevision", "FirmwareRevision", "SoftwareRevision", "SystemId", "BodyType", "BodyStructure", "Data", "VendorIDSource", "VendorID", "ProductID", "ProductVersion"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(ManufacturerName);
                exporter.CellSet(ModelNumber);
                exporter.CellSet(SerialNumber);
                exporter.CellSet(HardwareRevision);
                exporter.CellSet(FirmwareRevision);
                exporter.CellSet(SoftwareRevision);
                exporter.CellSet(SystemId);
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
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {ManufacturerName} {ModelNumber} {SerialNumber} {HardwareRevision} {FirmwareRevision} {SoftwareRevision} {SystemId} {BodyType} {BodyStructure} {Data} {VendorIDSource} {VendorID} {ProductID} {ProductVersion}");
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
            Multimeter_index = 1,
            Device_Info_index = 2,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Common_Configuration_Device_Name_index = 0,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 1,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Multimeter_MultimeterData_index = 2,     // GUID 0000fff4-0000-1000-8000-00805f9b34fb
            Device_Info_Manufacturer_Name_index = 3,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Info_Model_Number_index = 4,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Info_Serial_Number_index = 5,     // GUID 00002a25-0000-1000-8000-00805f9b34fb
            Device_Info_Hardware_Revision_index = 6,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Info_Firmware_Revision_index = 7,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Info_Software_Revision_index = 8,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Info_System_ID_index = 9,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
            Device_Info_Regulatory_List_index = 10,     // GUID 00002a2a-0000-1000-8000-00805f9b34fb
            Device_Info_PnP_ID_index = 11,     // GUID 00002a50-0000-1000-8000-00805f9b34fb
        }

        // All of the services that this device supports
        /// <summary>
        /// Convenience GUID for Common Configuration service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Common_Configuration = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"); // #0 is Common Configuration
        /// <summary>
        /// Convenience GUID for Multimeter service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Multimeter = Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"); // #1 is Multimeter
        /// <summary>
        /// Convenience GUID for Device Info service. Includes details on the manufacturer, model, firmware versions, and PNP ID
        /// </summary>
        public static readonly Guid ServiceGuid_Device_Info = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"); // #2 is Device Info        

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #0 is Common Configuration
            Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"), // #1 is Multimeter
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #2 is Device Info
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Common Configuration Appearance
            Guid.Parse("0000fff4-0000-1000-8000-00805f9b34fb"), // #2 is Multimeter MultimeterData
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #3 is Device Info Manufacturer Name
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #4 is Device Info Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #5 is Device Info Serial Number
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #6 is Device Info Hardware Revision
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #7 is Device Info Firmware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #8 is Device Info Software Revision
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #9 is Device Info System ID
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #10 is Device Info Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #11 is Device Info PnP ID
        };

        private List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null,  };


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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Appearance = vr.GetNextDouble();
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
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

        #endregion
//
        #region Service_Multimeter
        // Service Multimeter 

        public Multimeter_Data CurrMultimeter_Data { get; set; } = new Multimeter_Data();

        // Per-characteristics methods for Multimeter MultimeterData
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMultimeterDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("MultimeterData", ServiceIndex.Multimeter_index, "Multimeter", CharacteristicIndex.Multimeter_MultimeterData_index, NotifyMultimeterDataCallback, notifyType);
            return retval;
        }

        private void NotifyMultimeterDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Multimeter_MultimeterData_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Data");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrMultimeter_Data.TimestampMostRecent = args.Timestamp;
            CurrMultimeter_Data.Data = vr.GetNextByteArray();
            OnPropertyChanged(MultimeterDataPropertyChangedName); // "MultimeterData"
        }
        /// <summary>
        /// Reads data from MultimeterData and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Multimeter_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Multimeter_Data> ReadMultimeterData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Multimeter_MultimeterData_index;
            await Ensure_Characteristic_Async(ServiceIndex.Multimeter_index, "Multimeter", index, "MultimeterData");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "MultimeterData", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Data");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrMultimeter_Data.Data = vr.GetNextByteArray();
            CurrMultimeter_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(MultimeterDataPropertyChangedName); // "MultimeterData"
            return CurrMultimeter_Data;
        }
        /// <summary>
        /// Writes data to MultimeterData 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteMultimeterData(byte[] data)
        {
            var index = CharacteristicIndex.Multimeter_MultimeterData_index;
            await Ensure_Characteristic_Async(ServiceIndex.Multimeter_index, "Multimeter", index, "MultimeterData");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteMultimeterData", result);
            return result;
        }

        #endregion
//
        #region Service_Device_Info
        // Service Device Info 

        public Device_Info_Data CurrDevice_Info_Data { get; set; } = new Device_Info_Data();

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

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}