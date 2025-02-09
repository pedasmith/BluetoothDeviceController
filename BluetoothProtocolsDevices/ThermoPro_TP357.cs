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

using Utilities;

namespace BluetoothProtocols
{
    /// <summary>
    /// ThermoPro temperature and humidity meter.
    /// This class was automatically generated 2025-02-08::16:25
    /// </summary>

    public partial class ThermoPro_TP357 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://buythermopro.com/product/tp357/
    // Link: https://buythermopro.com/wp-content/uploads/2021/07/Thermopro-EN-FR-TP357-UM-20210309.pdf
    // Link: https://github.com/pasky/tp357
    // Link: https://github.com/zett90/tp357-browser-view/blob/2efdd3ca726f65c554d0674da70538ec79567061/app/src/utils/thermoPro.js#L11


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("00010203-0405-0607-0809-0a0b0c0d1910"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "SensorData",
            "Common Configuration",
            "Device Info",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00010203-0405-0607-0809-0a0b0c0d2b10"), // #0 is Data
            Guid.Parse("00010203-0405-0607-0809-0a0b0c0d2b11"), // #1 is Command
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #0 is PNP_ID

        };
        String[] CharacteristicNames = new string[] {
            "Data", // #0 is 00010203-0405-0607-0809-0a0b0c0d2b10
            "Command", // #1 is 00010203-0405-0607-0809-0a0b0c0d2b11
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "PNP_ID", // #0 is 00002a50-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1,  },
            new HashSet<int>(){ 2, 3, 4,  },
            new HashSet<int>(){ 5,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            1, // Characteristic 2
            1, // Characteristic 3
            1, // Characteristic 4
            2, // Characteristic 5
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Data_SensorData_enum = 0,
            Command_SensorData_enum = 1,
            Device_Name_Common_Configuration_enum = 2,
            Appearance_Common_Configuration_enum = 3,
            Connection_Parameter_Common_Configuration_enum = 4,
            PNP_ID_Device_Info_enum = 5,

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



        private double _Data_Opcode = 0;
        private bool _Data_Opcode_set = false;
        public double Data_Opcode
        {
            get { return _Data_Opcode; }
            internal set { if (_Data_Opcode_set && value == _Data_Opcode) return; _Data_Opcode = value; _Data_Opcode_set = true; OnPropertyChanged(); }
        }
        private double _Data_Unknown1 = 0;
        private bool _Data_Unknown1_set = false;
        public double Data_Unknown1
        {
            get { return _Data_Unknown1; }
            internal set { if (_Data_Unknown1_set && value == _Data_Unknown1) return; _Data_Unknown1 = value; _Data_Unknown1_set = true; OnPropertyChanged(); }
        }
        private double _Data_Flag = 0;
        private bool _Data_Flag_set = false;
        public double Data_Flag
        {
            get { return _Data_Flag; }
            internal set { if (_Data_Flag_set && value == _Data_Flag) return; _Data_Flag = value; _Data_Flag_set = true; OnPropertyChanged(); }
        }
        private double _Data_Temperature = 0;
        private bool _Data_Temperature_set = false;
        public double Data_Temperature
        {
            get { return _Data_Temperature; }
            internal set { if (_Data_Temperature_set && value == _Data_Temperature) return; _Data_Temperature = value; _Data_Temperature_set = true; OnPropertyChanged(); }
        }
        private double _Data_Humidity = 0;
        private bool _Data_Humidity_set = false;
        public double Data_Humidity
        {
            get { return _Data_Humidity; }
            internal set { if (_Data_Humidity_set && value == _Data_Humidity) return; _Data_Humidity = value; _Data_Humidity_set = true; OnPropertyChanged(); }
        }
        private string _Data_CrcExtra = null;
        private bool _Data_CrcExtra_set = false;
        public string Data_CrcExtra
        {
            get { return _Data_CrcExtra; }
            internal set { if (_Data_CrcExtra_set && value == _Data_CrcExtra) return; _Data_CrcExtra = value; _Data_CrcExtra_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Data_SensorData_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Data_SensorData_enum, "Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX^^HIDDEN|Opcode U8|HEX^^HIDDEN|Unknown1 U8|HEX^^HIDDEN|Flag U16^10_/|FIXED|Temperature|c U8|DEC|Humidity|Percent BYTES|HEX^^HIDDEN|CrcExtra";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Data_Opcode = parseResult.ValueList.GetValue("Opcode").AsDouble;
            Data_Unknown1 = parseResult.ValueList.GetValue("Unknown1").AsDouble;
            Data_Flag = parseResult.ValueList.GetValue("Flag").AsDouble;
            Data_Temperature = parseResult.ValueList.GetValue("Temperature").AsDouble;
            Data_Humidity = parseResult.ValueList.GetValue("Humidity").AsDouble;
            Data_CrcExtra = parseResult.ValueList.GetValue("CrcExtra").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Data_SensorData_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.Data_SensorData_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyData_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyData: set notification", result);

            return true;
        }

        private void NotifyDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX^^HIDDEN|Opcode U8|HEX^^HIDDEN|Unknown1 U8|HEX^^HIDDEN|Flag U16^10_/|FIXED|Temperature|c U8|DEC|Humidity|Percent BYTES|HEX^^HIDDEN|CrcExtra";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Data_Opcode = parseResult.ValueList.GetValue("Opcode").AsDouble;
            Data_Unknown1 = parseResult.ValueList.GetValue("Unknown1").AsDouble;
            Data_Flag = parseResult.ValueList.GetValue("Flag").AsDouble;
            Data_Temperature = parseResult.ValueList.GetValue("Temperature").AsDouble;
            Data_Humidity = parseResult.ValueList.GetValue("Humidity").AsDouble;
            Data_CrcExtra = parseResult.ValueList.GetValue("CrcExtra").AsString;

            DataEvent?.Invoke(parseResult);

        }

        public void NotifyDataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Data_SensorData_enum];
            if (ch == null) return;
            NotifyData_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDataCallback;
        }



        private string _Command = null;
        private bool _Command_set = false;
        public string Command
        {
            get { return _Command; }
            internal set { if (_Command_set && value == _Command) return; _Command = value; _Command_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadCommand(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Command_SensorData_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Command_SensorData_enum, "Command", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Command";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Command = parseResult.ValueList.GetValue("Command").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Command
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task<GattCommunicationStatus> WriteCommand(byte[] Command)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Command_SensorData_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return ensureResult;
            }

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Command);

            var command = dw.DetachBuffer().ToArray();
            
            var retval = await WriteCommandAsync(CharacteristicsEnum.Command_SensorData_enum, "Command", command, GattWriteOption.WriteWithoutResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.Command_SensorData_enum, "Command", command, GattWriteOption.WriteWithoutResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.Command_SensorData_enum, "Command", subcommand, GattWriteOption.WriteWithoutResponse);
            //}
            return retval;
        }


        private string _Device_Name = "";
        private bool _Device_Name_set = false;
        public string Device_Name
        {
            get { return _Device_Name; }
            internal set { if (_Device_Name_set && value == _Device_Name) return; _Device_Name = value; _Device_Name_set = true; OnPropertyChanged(); }
        }

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

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Device_NameEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Device_NameEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyDevice_Name_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDevice_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.Device_Name_Common_Configuration_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyDevice_Name_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyDevice_Name_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDevice_NameCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyDevice_Name: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyDevice_Name: set notification", result);

            return true;
        }

        private void NotifyDevice_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Device_Name = parseResult.ValueList.GetValue("Device_Name").AsString;

            Device_NameEvent?.Invoke(parseResult);

        }

        public void NotifyDevice_NameRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Device_Name_Common_Configuration_enum];
            if (ch == null) return;
            NotifyDevice_Name_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDevice_NameCallback;
        }



        private double _Appearance = 0;
        private bool _Appearance_set = false;
        public double Appearance
        {
            get { return _Appearance; }
            internal set { if (_Appearance_set && value == _Appearance) return; _Appearance = value; _Appearance_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAppearance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum, "Appearance", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|Speciality^Appearance|Appearance";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Appearance = parseResult.ValueList.GetValue("Appearance").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Connection_Parameter_Interval_Min = 0;
        private bool _Connection_Parameter_Interval_Min_set = false;
        public double Connection_Parameter_Interval_Min
        {
            get { return _Connection_Parameter_Interval_Min; }
            internal set { if (_Connection_Parameter_Interval_Min_set && value == _Connection_Parameter_Interval_Min) return; _Connection_Parameter_Interval_Min = value; _Connection_Parameter_Interval_Min_set = true; OnPropertyChanged(); }
        }
        private double _Connection_Parameter_Interval_Max = 0;
        private bool _Connection_Parameter_Interval_Max_set = false;
        public double Connection_Parameter_Interval_Max
        {
            get { return _Connection_Parameter_Interval_Max; }
            internal set { if (_Connection_Parameter_Interval_Max_set && value == _Connection_Parameter_Interval_Max) return; _Connection_Parameter_Interval_Max = value; _Connection_Parameter_Interval_Max_set = true; OnPropertyChanged(); }
        }
        private double _Connection_Parameter_Latency = 0;
        private bool _Connection_Parameter_Latency_set = false;
        public double Connection_Parameter_Latency
        {
            get { return _Connection_Parameter_Latency; }
            internal set { if (_Connection_Parameter_Latency_set && value == _Connection_Parameter_Latency) return; _Connection_Parameter_Latency = value; _Connection_Parameter_Latency_set = true; OnPropertyChanged(); }
        }
        private double _Connection_Parameter_Timeout = 0;
        private bool _Connection_Parameter_Timeout_set = false;
        public double Connection_Parameter_Timeout
        {
            get { return _Connection_Parameter_Timeout; }
            internal set { if (_Connection_Parameter_Timeout_set && value == _Connection_Parameter_Timeout) return; _Connection_Parameter_Timeout = value; _Connection_Parameter_Timeout_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConnection_Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum, "Connection_Parameter", cacheMode);
            if (result == null) return null;

            var datameaning = "U16^1.25_*|DEC|Interval_Min|ms U16^1.15_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Connection_Parameter_Interval_Min = parseResult.ValueList.GetValue("Interval_Min").AsDouble;
            Connection_Parameter_Interval_Max = parseResult.ValueList.GetValue("Interval_Max").AsDouble;
            Connection_Parameter_Latency = parseResult.ValueList.GetValue("Latency").AsDouble;
            Connection_Parameter_Timeout = parseResult.ValueList.GetValue("Timeout").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _PNP_ID = "";
        private bool _PNP_ID_set = false;
        public string PNP_ID
        {
            get { return _PNP_ID; }
            internal set { if (_PNP_ID_set && value == _PNP_ID) return; _PNP_ID = value; _PNP_ID_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPNP_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.PNP_ID_Device_Info_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.PNP_ID_Device_Info_enum, "PNP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Pnp_ID";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            PNP_ID = parseResult.ValueList.GetValue("Pnp_ID").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }





    }
}