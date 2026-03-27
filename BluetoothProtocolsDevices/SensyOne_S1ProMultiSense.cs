//From template: Protocol_Body v2022-07-02 9:54
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using BluetoothDeviceController.Names;

#if NET8_0_OR_GREATER
#nullable disable
#endif

using Utilities;

namespace BluetoothProtocols
{
    /// <summary>
    /// The Sensy-One S1 Pro Multi Sense is an open-source presence and environmental sensor, built from the ground up with Home Assistant in mind.
    /// This class was automatically generated 2025-12-09::18:08
    /// </summary>

    public  class SensyOne_S1ProMultiSense : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://sensy-one.com/products/s1-pro-multi-sense


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000ae00-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Radar",
            "Common Configuration",
            "Unknown2",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("0000fff2-0000-1000-8000-00805f9b34fb"), // #0 is RadarControl
            Guid.Parse("0000fff1-0000-1000-8000-00805f9b34fb"), // #1 is RadarData
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("0000ae01-0000-1000-8000-00805f9b34fb"), // #0 is Unknown20
            Guid.Parse("0000ae02-0000-1000-8000-00805f9b34fb"), // #1 is Unknown21

        };
        String[] CharacteristicNames = new string[] {
            "RadarControl", // #0 is 0000fff2-0000-1000-8000-00805f9b34fb
            "RadarData", // #1 is 0000fff1-0000-1000-8000-00805f9b34fb
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Unknown20", // #0 is 0000ae01-0000-1000-8000-00805f9b34fb
            "Unknown21", // #1 is 0000ae02-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1,  },
            new HashSet<int>(){ 2,  },
            new HashSet<int>(){ 3, 4,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            1, // Characteristic 2
            2, // Characteristic 3
            2, // Characteristic 4
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            RadarControl_Radar_enum = 0,
            RadarData_Radar_enum = 1,
            Device_Name_Common_Configuration_enum = 2,
            Unknown20_Unknown2_enum = 3,
            Unknown21_Unknown2_enum = 4,

        };

        public async Task<GattCharacteristicsResult> EnsureCharacteristicOne(GattDeviceService service, CharacteristicsEnum characteristicIndex)
        {
            var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[(int)characteristicIndex]);
            Characteristics[(int)characteristicIndex] = null;
            if (characteristicsStatus.Status != GattCommunicationStatus.Success)
            {
                Status.ReportStatus($"unable to get characteristic for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
                return null;
            }
            if (characteristicsStatus.Characteristics.Count == 0)
            {
                Status.ReportStatus($"unable to get any characteristics for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
            }
            else if (characteristicsStatus.Characteristics.Count != 1)
            {
                Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
            }
            else
            {
                Characteristics[(int)characteristicIndex] = characteristicsStatus.Characteristics[0];
            }
            return characteristicsStatus;
        }

        bool readCharacteristics = false;
        public async Task<GattCommunicationStatus> EnsureCharacteristicAsync(CharacteristicsEnum characteristicIndex = CharacteristicsEnum.All_enum, bool forceReread = false)
        {
            if (Characteristics.Length == 0) return GattCommunicationStatus.Unreachable;
            if (ble == null) return GattCommunicationStatus.Unreachable; // might not be initialized yet

            if (characteristicIndex != CharacteristicsEnum.All_enum)
            {
                var serviceIndex = MapCharacteristicToService[(int)characteristicIndex];
                var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                if (serviceStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                    return serviceStatus.Status;
                }
                if (serviceStatus.Services.Count != 1)
                {
                    Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                    return GattCommunicationStatus.Unreachable;
                }
                var service = serviceStatus.Services[0];
                var characteristicsStatus = await EnsureCharacteristicOne(service, characteristicIndex);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    return characteristicsStatus.Status;
                }
                return GattCommunicationStatus.Success;
            }

            GattCharacteristicsResult lastResult = null;
            if (forceReread) 
            {
                readCharacteristics = false;
            }
            if (readCharacteristics == false)
            {
                for (int serviceIndex = 0; serviceIndex < MapServiceToCharacteristic.Count; serviceIndex++)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                        return GattCommunicationStatus.Unreachable;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                        continue; //return false;
                    }
                    var service = serviceStatus.Services[0];
                    var characteristicIndexSet = MapServiceToCharacteristic[serviceIndex];
                    foreach (var index in characteristicIndexSet)
                    {
                        var characteristicsStatus = await EnsureCharacteristicOne(service, (CharacteristicsEnum)index);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            return characteristicsStatus.Status;
                        }
                        lastResult = characteristicsStatus;
                    }
                }
                // Do not call ReportStatus on OK -- the actual read/write/etc. call will
                // call ReportStatus for them. It's important that for any one actual call
                // (public method) that there's only one ReportStatus.
                //Status.ReportStatus("OK: Connected to device", lastResult);
                readCharacteristics = true;
            }
            // Not quite right, but close.
            return readCharacteristics ? GattCommunicationStatus.Success : GattCommunicationStatus.Unreachable;
        }


        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="method" ></param>
        /// <param name="command" ></param>
        /// <returns></returns>
        private async Task<GattCommunicationStatus> WriteCommandAsync(CharacteristicsEnum characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[(int)characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
            }
            catch (Exception)
            {
                result = GattCommunicationStatus.Unreachable;
            }
            Status.ReportStatus(method, result);
            if (result != GattCommunicationStatus.Success)
            {
                // NOTE: should add a way to reset
            }
            return result;
        }
        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either success or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(CharacteristicsEnum characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[(int)characteristicIndex].ReadValueAsync(cacheMode);
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

        /// <summary>
        /// Delegate for all Notify events
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);



// method.list for RadarControl



        /// <summary>
        /// Writes data for RadarControl
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task<GattCommunicationStatus> WriteRadarControl(byte[] Unknown10)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.RadarControl_Radar_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return ensureResult;
            }

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Unknown10);

            var command = dw.DetachBuffer().ToArray();
            
            var retval = await WriteCommandAsync(CharacteristicsEnum.RadarControl_Radar_enum, "RadarControl", command, GattWriteOption.WriteWithoutResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.RadarControl_Radar_enum, "RadarControl", command, GattWriteOption.WriteWithoutResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.RadarControl_Radar_enum, "RadarControl", subcommand, GattWriteOption.WriteWithoutResponse);
            //}
            return retval;
        }


// method.list for RadarData
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Opcode = 0;
        private bool _RadarData_Opcode_set = false;
        public double RadarData_Opcode
        {
            get { return _RadarData_Opcode; }
            internal set { if (_RadarData_Opcode_set && value == _RadarData_Opcode) return; _RadarData_Opcode = value; _RadarData_Opcode_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_NElements = 0;
        private bool _RadarData_NElements_set = false;
        public double RadarData_NElements
        {
            get { return _RadarData_NElements; }
            internal set { if (_RadarData_NElements_set && value == _RadarData_NElements) return; _RadarData_NElements = value; _RadarData_NElements_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_X1cm = 0;
        private bool _RadarData_X1cm_set = false;
        public double RadarData_X1cm
        {
            get { return _RadarData_X1cm; }
            internal set { if (_RadarData_X1cm_set && value == _RadarData_X1cm) return; _RadarData_X1cm = value; _RadarData_X1cm_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Y1cm = 0;
        private bool _RadarData_Y1cm_set = false;
        public double RadarData_Y1cm
        {
            get { return _RadarData_Y1cm; }
            internal set { if (_RadarData_Y1cm_set && value == _RadarData_Y1cm) return; _RadarData_Y1cm = value; _RadarData_Y1cm_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Speed1 = 0;
        private bool _RadarData_Speed1_set = false;
        public double RadarData_Speed1
        {
            get { return _RadarData_Speed1; }
            internal set { if (_RadarData_Speed1_set && value == _RadarData_Speed1) return; _RadarData_Speed1 = value; _RadarData_Speed1_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Unused1 = 0;
        private bool _RadarData_Unused1_set = false;
        public double RadarData_Unused1
        {
            get { return _RadarData_Unused1; }
            internal set { if (_RadarData_Unused1_set && value == _RadarData_Unused1) return; _RadarData_Unused1 = value; _RadarData_Unused1_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_X2cm = 0;
        private bool _RadarData_X2cm_set = false;
        public double RadarData_X2cm
        {
            get { return _RadarData_X2cm; }
            internal set { if (_RadarData_X2cm_set && value == _RadarData_X2cm) return; _RadarData_X2cm = value; _RadarData_X2cm_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Y2cm = 0;
        private bool _RadarData_Y2cm_set = false;
        public double RadarData_Y2cm
        {
            get { return _RadarData_Y2cm; }
            internal set { if (_RadarData_Y2cm_set && value == _RadarData_Y2cm) return; _RadarData_Y2cm = value; _RadarData_Y2cm_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Speed2 = 0;
        private bool _RadarData_Speed2_set = false;
        public double RadarData_Speed2
        {
            get { return _RadarData_Speed2; }
            internal set { if (_RadarData_Speed2_set && value == _RadarData_Speed2) return; _RadarData_Speed2 = value; _RadarData_Speed2_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Unused2 = 0;
        private bool _RadarData_Unused2_set = false;
        public double RadarData_Unused2
        {
            get { return _RadarData_Unused2; }
            internal set { if (_RadarData_Unused2_set && value == _RadarData_Unused2) return; _RadarData_Unused2 = value; _RadarData_Unused2_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_X3cm = 0;
        private bool _RadarData_X3cm_set = false;
        public double RadarData_X3cm
        {
            get { return _RadarData_X3cm; }
            internal set { if (_RadarData_X3cm_set && value == _RadarData_X3cm) return; _RadarData_X3cm = value; _RadarData_X3cm_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Y3cm = 0;
        private bool _RadarData_Y3cm_set = false;
        public double RadarData_Y3cm
        {
            get { return _RadarData_Y3cm; }
            internal set { if (_RadarData_Y3cm_set && value == _RadarData_Y3cm) return; _RadarData_Y3cm = value; _RadarData_Y3cm_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Speed3 = 0;
        private bool _RadarData_Speed3_set = false;
        public double RadarData_Speed3
        {
            get { return _RadarData_Speed3; }
            internal set { if (_RadarData_Speed3_set && value == _RadarData_Speed3) return; _RadarData_Speed3 = value; _RadarData_Speed3_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_Unused3 = 0;
        private bool _RadarData_Unused3_set = false;
        public double RadarData_Unused3
        {
            get { return _RadarData_Unused3; }
            internal set { if (_RadarData_Unused3_set && value == _RadarData_Unused3) return; _RadarData_Unused3 = value; _RadarData_Unused3_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _RadarData_End55CC = 0;
        private bool _RadarData_End55CC_set = false;
        public double RadarData_End55CC
        {
            get { return _RadarData_End55CC; }
            internal set { if (_RadarData_End55CC_set && value == _RadarData_End55CC) return; _RadarData_End55CC = value; _RadarData_End55CC_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; RadarDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent RadarDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyRadarData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyRadarDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.RadarData_Radar_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.RadarData_Radar_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyRadarData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyRadarData_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyRadarDataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyRadarData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyRadarData: set notification", result);

            return true;
        }

        private void NotifyRadarDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U16|HEX|Opcode U16|DEC|NElements I16^10_/|FIXED|X1cm|cm I16^10_/|FIXED|Y1cm|cm I16|DEC|Speed1 I16|HEX|Unused1 I16^10_/|FIXED|X2cm|cm I16^10_/|FIXED|Y2cm|cm I16|DEC|Speed2 I16|HEX|Unused2 I16^10_/|FIXED|X3cm|cm I16^10_/|FIXED|Y3cm|cm I16|DEC|Speed3 I16|HEX|Unused3 U16|HEX|End55CC";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            RadarData_Opcode = parseResult.ValueList.GetValue("Opcode").AsDouble;
            RadarData_NElements = parseResult.ValueList.GetValue("NElements").AsDouble;
            RadarData_X1cm = parseResult.ValueList.GetValue("X1cm").AsDouble;
            RadarData_Y1cm = parseResult.ValueList.GetValue("Y1cm").AsDouble;
            RadarData_Speed1 = parseResult.ValueList.GetValue("Speed1").AsDouble;
            RadarData_Unused1 = parseResult.ValueList.GetValue("Unused1").AsDouble;
            RadarData_X2cm = parseResult.ValueList.GetValue("X2cm").AsDouble;
            RadarData_Y2cm = parseResult.ValueList.GetValue("Y2cm").AsDouble;
            RadarData_Speed2 = parseResult.ValueList.GetValue("Speed2").AsDouble;
            RadarData_Unused2 = parseResult.ValueList.GetValue("Unused2").AsDouble;
            RadarData_X3cm = parseResult.ValueList.GetValue("X3cm").AsDouble;
            RadarData_Y3cm = parseResult.ValueList.GetValue("Y3cm").AsDouble;
            RadarData_Speed3 = parseResult.ValueList.GetValue("Speed3").AsDouble;
            RadarData_Unused3 = parseResult.ValueList.GetValue("Unused3").AsDouble;
            RadarData_End55CC = parseResult.ValueList.GetValue("End55CC").AsDouble;

            RadarDataEvent?.Invoke(parseResult);

        }

        public void NotifyRadarDataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.RadarData_Radar_enum];
            if (ch == null) return;
            NotifyRadarData_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyRadarDataCallback;
        }



// method.list for Device_Name
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Device_Name = "";
        private bool _Device_Name_set = false;
        public string Device_Name
        {
            get { return _Device_Name; }
            internal set { if (_Device_Name_set && value == _Device_Name) return; _Device_Name = value; _Device_Name_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDevice_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Device_Name = parseResult.ValueList.GetValue("Device_Name").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Device_Name
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task<GattCommunicationStatus> WriteDevice_Name(String Device_Name)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return ensureResult;
            }

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(Device_Name);

            var command = dw.DetachBuffer().ToArray();
            
            var retval = await WriteCommandAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", command, GattWriteOption.WriteWithResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", command, GattWriteOption.WriteWithResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", subcommand, GattWriteOption.WriteWithResponse);
            //}
            return retval;
        }


// method.list for Unknown20



        /// <summary>
        /// Writes data for Unknown20
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task<GattCommunicationStatus> WriteUnknown20(byte[] Unknown20)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown20_Unknown2_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return ensureResult;
            }

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Unknown20);

            var command = dw.DetachBuffer().ToArray();
            
            var retval = await WriteCommandAsync(CharacteristicsEnum.Unknown20_Unknown2_enum, "Unknown20", command, GattWriteOption.WriteWithoutResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.Unknown20_Unknown2_enum, "Unknown20", command, GattWriteOption.WriteWithoutResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.Unknown20_Unknown2_enum, "Unknown20", subcommand, GattWriteOption.WriteWithoutResponse);
            //}
            return retval;
        }


// method.list for Unknown21
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Unknown21 = null;
        private bool _Unknown21_set = false;
        public string Unknown21
        {
            get { return _Unknown21; }
            internal set { if (_Unknown21_set && value == _Unknown21) return; _Unknown21 = value; _Unknown21_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Unknown21Event += _my function_
        /// </summary>
        public event BluetoothDataEvent Unknown21Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyUnknown21_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyUnknown21Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown21_Unknown2_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.Unknown21_Unknown2_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyUnknown21_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyUnknown21_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyUnknown21Callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyUnknown21: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyUnknown21: set notification", result);

            return true;
        }

        private void NotifyUnknown21Callback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown21";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Unknown21 = parseResult.ValueList.GetValue("Unknown21").AsString;

            Unknown21Event?.Invoke(parseResult);

        }

        public void NotifyUnknown21RemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Unknown21_Unknown2_enum];
            if (ch == null) return;
            NotifyUnknown21_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyUnknown21Callback;
        }




    }
}