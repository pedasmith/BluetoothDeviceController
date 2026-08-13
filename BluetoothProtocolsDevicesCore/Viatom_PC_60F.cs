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
    /// This class was automatically generated 2026-08-13::10:27
    /// </summary>

    public  class Viatom_PC_60F : INotifyPropertyChanged
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

        public string Name { get; } = "PC-60F_SN109400";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device PC-60F_SN109400

        Transmit service Guid=6e400001-b5a3-f393-e0a9-e50e24dcca9e
            Transmit_Data (DataGroup record)
                Transmit characteristic has Command (Bytes-->string)  Guid=6e400002-b5a3-f393-e0a9-e50e24dcca9e
                Receive characteristic has Receive (Bytes-->string)  Guid=6e400003-b5a3-f393-e0a9-e50e24dcca9e


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Connection Parameter characteristic has ConnectionParameter (Bytes-->string)  Guid=2a04
                Central Address Resolution characteristic has AddressResolutionSupported (Byte-->double)  Guid=2aa6
        */

        public const string TransmitPropertyChangedName = "Transmit";
        public const string ReceivePropertyChangedName = "Receive";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Connection_ParameterPropertyChangedName = "Connection_Parameter";
        public const string Central_Address_ResolutionPropertyChangedName = "Central_Address_Resolution";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the Transmit Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Transmit_Data :BTCommonMetaData<Transmit_Data> //, IExportDataSource
        {
            private byte[] _Command = null;
            /// <summary>
            /// Command (BYTES ) from Service=Transmit and Characteristic=Transmit
            ///</summary>
            public byte[] Command 
            { 
                get { return _Command; }
                set { if (value == _Command) return; _Command = value; OnPropertyChanged();}
            }

            private byte[] _Receive = null;
            /// <summary>
            /// Receive (BYTES ) from Service=Transmit and Characteristic=Receive
            ///</summary>
            public byte[] Receive 
            { 
                get { return _Receive; }
                set { if (value == _Receive) return; _Receive = value; OnPropertyChanged();}
            }
            public override Transmit_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Transmit_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Transmit_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Command = source.Command;
                dest.Receive = source.Receive;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Transmit_Data CopyToWithConvertAndCreate(Transmit_Data source, Transmit_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Command = source.Command;
                dest.Receive = source.Receive;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Command", "Receive"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Command);
                exporter.CellSet(Receive);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Command} {Receive}");
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

            private byte[] _ConnectionParameter = null;
            /// <summary>
            /// ConnectionParameter (BYTES ) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public byte[] ConnectionParameter 
            { 
                get { return _ConnectionParameter; }
                set { if (value == _ConnectionParameter) return; _ConnectionParameter = value; OnPropertyChanged();}
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
                dest.ConnectionParameter = source.ConnectionParameter;
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
                dest.ConnectionParameter = source.ConnectionParameter;
                dest.AddressResolutionSupported = convert(source.AddressResolutionSupported, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "ConnectionParameter", "AddressResolutionSupported"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(ConnectionParameter);
                exporter.CellSet(AddressResolutionSupported);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {ConnectionParameter} {AddressResolutionSupported}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Transmit_index = 0,
            Common_Configuration_index = 1,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Transmit_Transmit_index = 0,     // GUID 6e400002-b5a3-f393-e0a9-e50e24dcca9e
            Transmit_Receive_index = 1,     // GUID 6e400003-b5a3-f393-e0a9-e50e24dcca9e
            Common_Configuration_Device_Name_index = 2,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 3,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 4,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Common_Configuration_Central_Address_Resolution_index = 5,     // GUID 00002aa6-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"), // #0 is Transmit
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
            Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e"), // #0 is Transmit Transmit
            Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"), // #1 is Transmit Receive
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #2 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #3 is Common Configuration Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #4 is Common Configuration Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #5 is Common Configuration Central Address Resolution
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null,  };


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


        #region Service_Transmit
        // Service Transmit 

        public Transmit_Data CurrTransmit_Data { get; set; } = new Transmit_Data();

        // Per-characteristics methods for Transmit Transmit
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTransmitAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Transmit", ServiceIndex.Transmit_index, "Transmit", CharacteristicIndex.Transmit_Transmit_index, NotifyTransmitCallback, notifyType);
            return retval;
        }

        private void NotifyTransmitCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Transmit_Transmit_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Command");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTransmit_Data.TimestampMostRecent = args.Timestamp;
            CurrTransmit_Data.Command = vr.GetNextByteArray();
            OnPropertyChanged(TransmitPropertyChangedName); // "Transmit"
        }
        // Per-characteristics methods for Transmit Receive
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyReceiveAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Receive", ServiceIndex.Transmit_index, "Transmit", CharacteristicIndex.Transmit_Receive_index, NotifyReceiveCallback, notifyType);
            return retval;
        }

        private void NotifyReceiveCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Transmit_Receive_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Receive");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTransmit_Data.TimestampMostRecent = args.Timestamp;
            CurrTransmit_Data.Receive = vr.GetNextByteArray();
            OnPropertyChanged(ReceivePropertyChangedName); // "Receive"
        }
        /// <summary>
        /// Reads data from Transmit and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Transmit_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Transmit_Data> ReadTransmit(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Transmit_Transmit_index;
            await Ensure_Characteristic_Async(ServiceIndex.Transmit_index, "Transmit", index, "Transmit");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Transmit", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Command");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTransmit_Data.Command = vr.GetNextByteArray();
            CurrTransmit_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(TransmitPropertyChangedName); // "Transmit"
            return CurrTransmit_Data;
        }
        /// <summary>
        /// Reads data from Receive and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Transmit_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Transmit_Data> ReadReceive(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Transmit_Receive_index;
            await Ensure_Characteristic_Async(ServiceIndex.Transmit_index, "Transmit", index, "Receive");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Receive", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Receive");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTransmit_Data.Receive = vr.GetNextByteArray();
            CurrTransmit_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(ReceivePropertyChangedName); // "Receive"
            return CurrTransmit_Data;
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ConnectionParameter");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.ConnectionParameter = vr.GetNextByteArray();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ConnectionParameter");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.ConnectionParameter = vr.GetNextByteArray();
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

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}