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
    /// This class was automatically generated 2026-06-16::16:44
    /// </summary>

    public  class TAOPE_CyclingSpeedCadence : INotifyPropertyChanged
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

        public string Name { get; } = "BK6";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device BK6

        Cycling Speed and Cadence service Guid=1816
            Cycling Speed and Cadence_Data (DataGroup record)
                CSC Measurement characteristic has Flags (Byte-->double) RevolutionWheel (UInt32-->double) TimeWheel (UInt16-->double) RevolutionCrank (UInt16-->double) TimeCrank (UInt16-->double)  Guid=2a5b
                CSC Feature characteristic has FeatureFlags (UInt16-->double)  Guid=2a5c
                Sensor Location characteristic has SensorLocation (Byte-->double)  Guid=2a5d
                SC Control Point characteristic has Unknown3 (Bytes-->string)  Guid=2a55


        Service_FD00 service Guid=fd00
            Service_FD00_Data (DataGroup record)
                FD09 characteristic has Unknown0 (Bytes-->string)  Guid=fd09
                FD0A characteristic has Unknown1 (Bytes-->string)  Guid=fd0a
                FD19 characteristic has Unknown2 (Bytes-->string)  Guid=fd19
                FD1A characteristic has Unknown3 (Bytes-->string)  Guid=fd1a


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                Battery Level characteristic has Unknown0 (Bytes-->string)  Guid=2a19


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
        public const string FD09PropertyChangedName = "FD09";
        public const string FD0APropertyChangedName = "FD0A";
        public const string FD19PropertyChangedName = "FD19";
        public const string FD1APropertyChangedName = "FD1A";
        public const string Battery_LevelPropertyChangedName = "Battery_Level";
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
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Cycling_Speed_and_Cadence_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private double _Flags = 0;
            /// <summary>
            /// From Cycling Speed and Cadence and CSC Measurement
            ///</summary>
            public double Flags 
            { 
                get { return _Flags; }
                set { if (value == _Flags) return; _Flags = value; OnPropertyChanged();}
            } 
            private double _RevolutionWheel = 0;
            /// <summary>
            /// From Cycling Speed and Cadence and CSC Measurement
            ///</summary>
            public double RevolutionWheel 
            { 
                get { return _RevolutionWheel; }
                set { if (value == _RevolutionWheel) return; _RevolutionWheel = value; OnPropertyChanged();}
            } 
            private double _TimeWheel = 0;
            /// <summary>
            /// From Cycling Speed and Cadence and CSC Measurement
            ///</summary>
            public double TimeWheel 
            { 
                get { return _TimeWheel; }
                set { if (value == _TimeWheel) return; _TimeWheel = value; OnPropertyChanged();}
            } 
            private double _RevolutionCrank = 0;
            /// <summary>
            /// From Cycling Speed and Cadence and CSC Measurement
            ///</summary>
            public double RevolutionCrank 
            { 
                get { return _RevolutionCrank; }
                set { if (value == _RevolutionCrank) return; _RevolutionCrank = value; OnPropertyChanged();}
            } 
            private double _TimeCrank = 0;
            /// <summary>
            /// From Cycling Speed and Cadence and CSC Measurement
            ///</summary>
            public double TimeCrank 
            { 
                get { return _TimeCrank; }
                set { if (value == _TimeCrank) return; _TimeCrank = value; OnPropertyChanged();}
            }
            private double _FeatureFlags = 0;
            /// <summary>
            /// From Cycling Speed and Cadence and CSC Feature
            ///</summary>
            public double FeatureFlags 
            { 
                get { return _FeatureFlags; }
                set { if (value == _FeatureFlags) return; _FeatureFlags = value; OnPropertyChanged();}
            }
            private double _SensorLocation = 0;
            /// <summary>
            /// From Cycling Speed and Cadence and Sensor Location
            ///</summary>
            public double SensorLocation 
            { 
                get { return _SensorLocation; }
                set { if (value == _SensorLocation) return; _SensorLocation = value; OnPropertyChanged();}
            }
            private byte[] _Unknown3 = null;
            /// <summary>
            /// From Cycling Speed and Cadence and SC Control Point
            ///</summary>
            public byte[] Unknown3 
            { 
                get { return _Unknown3; }
                set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged();}
            }
            public Cycling_Speed_and_Cadence_Data Clone()
            {
                return this.MemberwiseClone() as Cycling_Speed_and_Cadence_Data;
            }

            public void CopyFrom(Cycling_Speed_and_Cadence_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Flags = value.Flags;
                this.RevolutionWheel = value.RevolutionWheel;
                this.TimeWheel = value.TimeWheel;
                this.RevolutionCrank = value.RevolutionCrank;
                this.TimeCrank = value.TimeCrank;
                this.FeatureFlags = value.FeatureFlags;
                this.SensorLocation = value.SensorLocation;
                this.Unknown3 = value.Unknown3;
            }

            public override void ExportHeaders(IExportData exporter)
            {
                exporter.HeadersSet(["Date", "Time", "Flags", "RevolutionWheel", "TimeWheel", "RevolutionCrank", "TimeCrank", "FeatureFlags", "SensorLocation", "Unknown3"]);
            }

            public override void ExportRow(IExportData exporter)
            {
                exporter.RowStart();
                exporter.CellSet(TimestampMostRecentDT.ToString("yyyy-MM-dd"));
                exporter.CellSet(TimestampMostRecentDT.ToString("HH:mm:ss"));
                exporter.CellSet(Flags);
                exporter.CellSet(RevolutionWheel);
                exporter.CellSet(TimeWheel);
                exporter.CellSet(RevolutionCrank);
                exporter.CellSet(TimeCrank);
                exporter.CellSet(FeatureFlags);
                exporter.CellSet(SensorLocation);
                exporter.CellSet(Unknown3);                
                exporter.RowEnd();
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Flags} {RevolutionWheel} {TimeWheel} {RevolutionCrank} {TimeCrank} {FeatureFlags} {SensorLocation} {Unknown3}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Service_FD00 Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Service_FD00_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private byte[] _Unknown0 = null;
            /// <summary>
            /// From Service_FD00 and FD09
            ///</summary>
            public byte[] Unknown0 
            { 
                get { return _Unknown0; }
                set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged();}
            }
            private byte[] _Unknown1 = null;
            /// <summary>
            /// From Service_FD00 and FD0A
            ///</summary>
            public byte[] Unknown1 
            { 
                get { return _Unknown1; }
                set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged();}
            }
            private byte[] _Unknown2 = null;
            /// <summary>
            /// From Service_FD00 and FD19
            ///</summary>
            public byte[] Unknown2 
            { 
                get { return _Unknown2; }
                set { if (value == _Unknown2) return; _Unknown2 = value; OnPropertyChanged();}
            }
            private byte[] _Unknown3 = null;
            /// <summary>
            /// From Service_FD00 and FD1A
            ///</summary>
            public byte[] Unknown3 
            { 
                get { return _Unknown3; }
                set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged();}
            }
            public Service_FD00_Data Clone()
            {
                return this.MemberwiseClone() as Service_FD00_Data;
            }

            public void CopyFrom(Service_FD00_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Unknown0 = value.Unknown0;
                this.Unknown1 = value.Unknown1;
                this.Unknown2 = value.Unknown2;
                this.Unknown3 = value.Unknown3;
            }

            public override void ExportHeaders(IExportData exporter)
            {
                exporter.HeadersSet(["Date", "Time", "Unknown0", "Unknown1", "Unknown2", "Unknown3"]);
            }

            public override void ExportRow(IExportData exporter)
            {
                exporter.RowStart();
                exporter.CellSet(TimestampMostRecentDT.ToString("yyyy-MM-dd"));
                exporter.CellSet(TimestampMostRecentDT.ToString("HH:mm:ss"));
                exporter.CellSet(Unknown0);
                exporter.CellSet(Unknown1);
                exporter.CellSet(Unknown2);
                exporter.CellSet(Unknown3);                
                exporter.RowEnd();
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
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Battery_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private byte[] _Unknown0 = null;
            /// <summary>
            /// From Battery and Battery Level
            ///</summary>
            public byte[] Unknown0 
            { 
                get { return _Unknown0; }
                set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged();}
            }
            public Battery_Data Clone()
            {
                return this.MemberwiseClone() as Battery_Data;
            }

            public void CopyFrom(Battery_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Unknown0 = value.Unknown0;
            }

            public override void ExportHeaders(IExportData exporter)
            {
                exporter.HeadersSet(["Date", "Time", "Unknown0"]);
            }

            public override void ExportRow(IExportData exporter)
            {
                exporter.RowStart();
                exporter.CellSet(TimestampMostRecentDT.ToString("yyyy-MM-dd"));
                exporter.CellSet(TimestampMostRecentDT.ToString("HH:mm:ss"));
                exporter.CellSet(Unknown0);                
                exporter.RowEnd();
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Unknown0}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Device Information Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Device_Information_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private string _Manufacturer = "";
            /// <summary>
            /// From Device Information and Manufacturer Name String
            ///</summary>
            public string Manufacturer 
            { 
                get { return _Manufacturer; }
                set { if (value == _Manufacturer) return; _Manufacturer = value; OnPropertyChanged();}
            }
            private string _ModelNumber = "";
            /// <summary>
            /// From Device Information and Model Number String
            ///</summary>
            public string ModelNumber 
            { 
                get { return _ModelNumber; }
                set { if (value == _ModelNumber) return; _ModelNumber = value; OnPropertyChanged();}
            }
            private string _HardwareRevision = "";
            /// <summary>
            /// From Device Information and Hardware Revision String
            ///</summary>
            public string HardwareRevision 
            { 
                get { return _HardwareRevision; }
                set { if (value == _HardwareRevision) return; _HardwareRevision = value; OnPropertyChanged();}
            }
            private string _FirmwareRevision = "";
            /// <summary>
            /// From Device Information and Firmware Revision String
            ///</summary>
            public string FirmwareRevision 
            { 
                get { return _FirmwareRevision; }
                set { if (value == _FirmwareRevision) return; _FirmwareRevision = value; OnPropertyChanged();}
            }
            private string _SoftwareRevision = "";
            /// <summary>
            /// From Device Information and Software Revision String
            ///</summary>
            public string SoftwareRevision 
            { 
                get { return _SoftwareRevision; }
                set { if (value == _SoftwareRevision) return; _SoftwareRevision = value; OnPropertyChanged();}
            }
            private byte[] _SystemID = null;
            /// <summary>
            /// From Device Information and System ID
            ///</summary>
            public byte[] SystemID 
            { 
                get { return _SystemID; }
                set { if (value == _SystemID) return; _SystemID = value; OnPropertyChanged();}
            }
            public Device_Information_Data Clone()
            {
                return this.MemberwiseClone() as Device_Information_Data;
            }

            public void CopyFrom(Device_Information_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Manufacturer = value.Manufacturer;
                this.ModelNumber = value.ModelNumber;
                this.HardwareRevision = value.HardwareRevision;
                this.FirmwareRevision = value.FirmwareRevision;
                this.SoftwareRevision = value.SoftwareRevision;
                this.SystemID = value.SystemID;
            }

            public override void ExportHeaders(IExportData exporter)
            {
                exporter.HeadersSet(["Date", "Time", "Manufacturer", "ModelNumber", "HardwareRevision", "FirmwareRevision", "SoftwareRevision", "SystemID"]);
            }

            public override void ExportRow(IExportData exporter)
            {
                exporter.RowStart();
                exporter.CellSet(TimestampMostRecentDT.ToString("yyyy-MM-dd"));
                exporter.CellSet(TimestampMostRecentDT.ToString("HH:mm:ss"));
                exporter.CellSet(Manufacturer);
                exporter.CellSet(ModelNumber);
                exporter.CellSet(HardwareRevision);
                exporter.CellSet(FirmwareRevision);
                exporter.CellSet(SoftwareRevision);
                exporter.CellSet(SystemID);                
                exporter.RowEnd();
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
            Service_FD00_index = 1,
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
            Service_FD00_FD09_index = 4,     // GUID 0000fd09-0000-1000-8000-00805f9b34fb
            Service_FD00_FD0A_index = 5,     // GUID 0000fd0a-0000-1000-8000-00805f9b34fb
            Service_FD00_FD19_index = 6,     // GUID 0000fd19-0000-1000-8000-00805f9b34fb
            Service_FD00_FD1A_index = 7,     // GUID 0000fd1a-0000-1000-8000-00805f9b34fb
            Battery_Battery_Level_index = 8,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
            Device_Information_Manufacturer_Name_String_index = 9,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Information_Model_Number_String_index = 10,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Information_Hardware_Revision_String_index = 11,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Information_Firmware_Revision_String_index = 12,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Information_Software_Revision_String_index = 13,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Information_System_ID_index = 14,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("00001816-0000-1000-8000-00805f9b34fb"), // #0 is Cycling Speed and Cadence
            Guid.Parse("0000fd00-0000-1000-8000-00805f9b34fb"), // #1 is Service_FD00
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
            Guid.Parse("0000fd09-0000-1000-8000-00805f9b34fb"), // #4 is Service_FD00 FD09
            Guid.Parse("0000fd0a-0000-1000-8000-00805f9b34fb"), // #5 is Service_FD00 FD0A
            Guid.Parse("0000fd19-0000-1000-8000-00805f9b34fb"), // #6 is Service_FD00 FD19
            Guid.Parse("0000fd1a-0000-1000-8000-00805f9b34fb"), // #7 is Service_FD00 FD1A
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #8 is Battery Battery Level
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #9 is Device Information Manufacturer Name String
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #10 is Device Information Model Number String
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #11 is Device Information Hardware Revision String
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #12 is Device Information Firmware Revision String
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #13 is Device Information Software Revision String
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #14 is Device Information System ID
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };


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

        public Cycling_Speed_and_Cadence_Data CurrCycling_Speed_and_Cadence_Data { get; set; } = new Cycling_Speed_and_Cadence_Data();

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
            CurrCycling_Speed_and_Cadence_Data.TimestampMostRecent = args.Timestamp;
            CurrCycling_Speed_and_Cadence_Data.Flags = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.RevolutionWheel = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.TimeWheel = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.RevolutionCrank = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.TimeCrank = vr.GetNextDouble();
            OnPropertyChanged(CSC_MeasurementPropertyChangedName); // "CSC_Measurement"
        }
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
            CurrCycling_Speed_and_Cadence_Data.TimestampMostRecent = args.Timestamp;
            CurrCycling_Speed_and_Cadence_Data.FeatureFlags = vr.GetNextDouble();
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
            CurrCycling_Speed_and_Cadence_Data.TimestampMostRecent = args.Timestamp;
            CurrCycling_Speed_and_Cadence_Data.SensorLocation = vr.GetNextDouble();
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
            CurrCycling_Speed_and_Cadence_Data.TimestampMostRecent = args.Timestamp;
            CurrCycling_Speed_and_Cadence_Data.Unknown3 = vr.GetNextByteArray();
            OnPropertyChanged(SC_Control_PointPropertyChangedName); // "SC_Control_Point"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Cycling_Speed_and_Cadence_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Cycling_Speed_and_Cadence_Data> ReadCSC_Measurement(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
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
            CurrCycling_Speed_and_Cadence_Data.Flags = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.RevolutionWheel = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.TimeWheel = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.RevolutionCrank = vr.GetNextDouble();
            CurrCycling_Speed_and_Cadence_Data.TimeCrank = vr.GetNextDouble();
            OnPropertyChanged(CSC_MeasurementPropertyChangedName); // "CSC_Measurement"
            return CurrCycling_Speed_and_Cadence_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Cycling_Speed_and_Cadence_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Cycling_Speed_and_Cadence_Data> ReadCSC_Feature(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
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
            CurrCycling_Speed_and_Cadence_Data.FeatureFlags = vr.GetNextDouble();
            OnPropertyChanged(CSC_FeaturePropertyChangedName); // "CSC_Feature"
            return CurrCycling_Speed_and_Cadence_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Cycling_Speed_and_Cadence_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Cycling_Speed_and_Cadence_Data> ReadSensor_Location(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
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
            CurrCycling_Speed_and_Cadence_Data.SensorLocation = vr.GetNextDouble();
            OnPropertyChanged(Sensor_LocationPropertyChangedName); // "Sensor_Location"
            return CurrCycling_Speed_and_Cadence_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Cycling_Speed_and_Cadence_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Cycling_Speed_and_Cadence_Data> ReadSC_Control_Point(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
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
            CurrCycling_Speed_and_Cadence_Data.Unknown3 = vr.GetNextByteArray();
            OnPropertyChanged(SC_Control_PointPropertyChangedName); // "SC_Control_Point"
            return CurrCycling_Speed_and_Cadence_Data;
        }

        #endregion
//
        #region Service_Service_FD00
        // Service Service_FD00 

        public Service_FD00_Data CurrService_FD00_Data { get; set; } = new Service_FD00_Data();

        // Per-characteristics methods for Service_FD00 FD09
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD09Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD09", ServiceIndex.Service_FD00_index, "Service_FD00", CharacteristicIndex.Service_FD00_FD09_index, NotifyFD09Callback, notifyType);
            return retval;
        }

        private void NotifyFD09Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_FD09_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_Data.Unknown0 = vr.GetNextByteArray();
            OnPropertyChanged(FD09PropertyChangedName); // "FD09"
        }
        // Per-characteristics methods for Service_FD00 FD0A
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD0AAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD0A", ServiceIndex.Service_FD00_index, "Service_FD00", CharacteristicIndex.Service_FD00_FD0A_index, NotifyFD0ACallback, notifyType);
            return retval;
        }

        private void NotifyFD0ACallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_FD0A_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_Data.Unknown1 = vr.GetNextByteArray();
            OnPropertyChanged(FD0APropertyChangedName); // "FD0A"
        }
        // Per-characteristics methods for Service_FD00 FD19
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD19Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD19", ServiceIndex.Service_FD00_index, "Service_FD00", CharacteristicIndex.Service_FD00_FD19_index, NotifyFD19Callback, notifyType);
            return retval;
        }

        private void NotifyFD19Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_FD19_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown2");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_Data.Unknown2 = vr.GetNextByteArray();
            OnPropertyChanged(FD19PropertyChangedName); // "FD19"
        }
        // Per-characteristics methods for Service_FD00 FD1A
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFD1AAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FD1A", ServiceIndex.Service_FD00_index, "Service_FD00", CharacteristicIndex.Service_FD00_FD1A_index, NotifyFD1ACallback, notifyType);
            return retval;
        }

        private void NotifyFD1ACallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_FD00_FD1A_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown3");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_FD00_Data.TimestampMostRecent = args.Timestamp;
            CurrService_FD00_Data.Unknown3 = vr.GetNextByteArray();
            OnPropertyChanged(FD1APropertyChangedName); // "FD1A"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_Data> ReadFD09(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_FD09_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_index, "Service_FD00", index, "FD09");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD09", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_Data.Unknown0 = vr.GetNextByteArray();
            OnPropertyChanged(FD09PropertyChangedName); // "FD09"
            return CurrService_FD00_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_Data> ReadFD0A(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_FD0A_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_index, "Service_FD00", index, "FD0A");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD0A", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_Data.Unknown1 = vr.GetNextByteArray();
            OnPropertyChanged(FD0APropertyChangedName); // "FD0A"
            return CurrService_FD00_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_Data> ReadFD19(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_FD19_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_index, "Service_FD00", index, "FD19");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD19", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown2");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_Data.Unknown2 = vr.GetNextByteArray();
            OnPropertyChanged(FD19PropertyChangedName); // "FD19"
            return CurrService_FD00_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_FD00_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_FD00_Data> ReadFD1A(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_FD00_FD1A_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_FD00_index, "Service_FD00", index, "FD1A");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FD1A", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown3");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_FD00_Data.Unknown3 = vr.GetNextByteArray();
            OnPropertyChanged(FD1APropertyChangedName); // "FD1A"
            return CurrService_FD00_Data;
        }

        #endregion
//
        #region Service_Battery
        // Service Battery 

        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

        // Per-characteristics methods for Battery Battery_Level
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBattery_LevelAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Battery_Level", ServiceIndex.Battery_index, "Battery", CharacteristicIndex.Battery_Battery_Level_index, NotifyBattery_LevelCallback, notifyType);
            return retval;
        }

        private void NotifyBattery_LevelCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Battery_Battery_Level_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.TimestampMostRecent = args.Timestamp;
            CurrBattery_Data.Unknown0 = vr.GetNextByteArray();
            OnPropertyChanged(Battery_LevelPropertyChangedName); // "Battery_Level"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Battery_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadBattery_Level(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_Battery_Level_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "Battery Level");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Battery Level", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.Unknown0 = vr.GetNextByteArray();
            OnPropertyChanged(Battery_LevelPropertyChangedName); // "Battery_Level"
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
        /// Reads data
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
            OnPropertyChanged(Manufacturer_Name_StringPropertyChangedName); // "Manufacturer_Name_String"
            return CurrDevice_Information_Data;
        }
        /// Reads data
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
            OnPropertyChanged(Model_Number_StringPropertyChangedName); // "Model_Number_String"
            return CurrDevice_Information_Data;
        }
        /// Reads data
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
            OnPropertyChanged(Hardware_Revision_StringPropertyChangedName); // "Hardware_Revision_String"
            return CurrDevice_Information_Data;
        }
        /// Reads data
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
            OnPropertyChanged(Firmware_Revision_StringPropertyChangedName); // "Firmware_Revision_String"
            return CurrDevice_Information_Data;
        }
        /// Reads data
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
            OnPropertyChanged(Software_Revision_StringPropertyChangedName); // "Software_Revision_String"
            return CurrDevice_Information_Data;
        }
        /// Reads data
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
            OnPropertyChanged(System_IDPropertyChangedName); // "System_ID"
            return CurrDevice_Information_Data;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}