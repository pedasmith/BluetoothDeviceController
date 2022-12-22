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


namespace BluetoothProtocols
{
    /// <summary>
    /// .
    /// This class was automatically generated 2022-12-11::04:50
    /// </summary>

    public  class WilliamWeilerEngineering_Skoobot : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://www.william-weiler-engineering.com/
    // Link: https://hackaday.io/project/75832-skoobot
    // Link: https://www.facebook.com/skoobot/
    // Link: https://twitter.com/BillBsee


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("00001523-1212-efde-1523-785feabcd123"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Robot",
            "Common Configuration",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00001525-1212-efde-1523-785feabcd123"), // #0 is Command
            Guid.Parse("00001524-1212-efde-1523-785feabcd123"), // #1 is Distance
            Guid.Parse("00001526-1212-efde-1523-785feabcd123"), // #2 is AmbientLight
            Guid.Parse("00001527-1212-efde-1523-785feabcd123"), // #3 is Microphone
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name

        };
        String[] CharacteristicNames = new string[] {
            "Command", // #0 is 00001525-1212-efde-1523-785feabcd123
            "Distance", // #1 is 00001524-1212-efde-1523-785feabcd123
            "AmbientLight", // #2 is 00001526-1212-efde-1523-785feabcd123
            "Microphone", // #3 is 00001527-1212-efde-1523-785feabcd123
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3,  },
            new HashSet<int>(){ 4,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            0, // Characteristic 2
            0, // Characteristic 3
            1, // Characteristic 4
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Command_Robot_enum = 0,
            Distance_Robot_enum = 1,
            AmbientLight_Robot_enum = 2,
            Microphone_Robot_enum = 3,
            Device_Name_Common_Configuration_enum = 4,

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
        public async Task<bool> EnsureCharacteristicAsync(CharacteristicsEnum characteristicIndex = CharacteristicsEnum.All_enum, bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;
            if (ble == null) return false; // might not be initialized yet

            if (characteristicIndex != CharacteristicsEnum.All_enum)
            {
                var serviceIndex = MapCharacteristicToService[(int)characteristicIndex];
                var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                if (serviceStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                    return false;
                }
                if (serviceStatus.Services.Count != 1)
                {
                    Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                    return false;
                }
                var service = serviceStatus.Services[0];
                var characteristicsStatus = await EnsureCharacteristicOne(service, characteristicIndex);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    return false;
                }
                return true;
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
                        return false;
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
                            return false;
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
            return readCharacteristics;
        }


        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="method" ></param>
        /// <param name="command" ></param>
        /// <returns></returns>
        private async Task WriteCommandAsync(CharacteristicsEnum characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
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
        }
        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
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



        private double _Command = 0;
        private bool _Command_set = false;
        public double Command
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Command_Robot_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Command_Robot_enum, "Command", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Command";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Command = parseResult.ValueList.GetValue("Command").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Command
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteCommand(byte Command)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Command_Robot_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  Command);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Command_Robot_enum, "Command", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Command_Robot_enum, "Command", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Distance = 0;
        private bool _Distance_set = false;
        public double Distance
        {
            get { return _Distance; }
            internal set { if (_Distance_set && value == _Distance) return; _Distance = value; _Distance_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDistance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Distance_Robot_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Distance_Robot_enum, "Distance", cacheMode);
            if (result == null) return null;

            var datameaning = "U8^20_/_2.54_*|HEX|Distance|cm";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Distance = parseResult.ValueList.GetValue("Distance").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; DistanceEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent DistanceEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyDistance_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDistanceAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Distance_Robot_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Distance_Robot_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyDistance_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyDistance_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDistanceCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyDistance: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyDistance: set notification", result);

            return true;
        }

        private void NotifyDistanceCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8^20_/_2.54_*|HEX|Distance|cm";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Distance = parseResult.ValueList.GetValue("Distance").AsDouble;

            DistanceEvent?.Invoke(parseResult);

        }

        public void NotifyDistanceRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Distance_Robot_enum];
            if (ch == null) return;
            NotifyDistance_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDistanceCallback;
        }



        private double _AmbientLight_AmbientLight = 0;
        private bool _AmbientLight_AmbientLight_set = false;
        public double AmbientLight_AmbientLight
        {
            get { return _AmbientLight_AmbientLight; }
            internal set { if (_AmbientLight_AmbientLight_set && value == _AmbientLight_AmbientLight) return; _AmbientLight_AmbientLight = value; _AmbientLight_AmbientLight_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAmbientLight(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.AmbientLight_Robot_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.AmbientLight_Robot_enum, "AmbientLight", cacheMode);
            if (result == null) return null;

            var datameaning = "OEB U16|HEX|AmbientLight|Lux";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            AmbientLight_AmbientLight = parseResult.ValueList.GetValue("AmbientLight").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; AmbientLightEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent AmbientLightEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyAmbientLight_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyAmbientLightAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.AmbientLight_Robot_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.AmbientLight_Robot_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyAmbientLight_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyAmbientLight_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyAmbientLightCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyAmbientLight: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyAmbientLight: set notification", result);

            return true;
        }

        private void NotifyAmbientLightCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "OEB U16|HEX|AmbientLight|Lux";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            AmbientLight_AmbientLight = parseResult.ValueList.GetValue("AmbientLight").AsDouble;

            AmbientLightEvent?.Invoke(parseResult);

        }

        public void NotifyAmbientLightRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.AmbientLight_Robot_enum];
            if (ch == null) return;
            NotifyAmbientLight_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyAmbientLightCallback;
        }



        private string _Microphone = null;
        private bool _Microphone_set = false;
        public string Microphone
        {
            get { return _Microphone; }
            internal set { if (_Microphone_set && value == _Microphone) return; _Microphone = value; _Microphone_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMicrophone(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Microphone_Robot_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Microphone_Robot_enum, "Microphone", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Audio";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Microphone = parseResult.ValueList.GetValue("Audio").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; MicrophoneEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent MicrophoneEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMicrophone_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMicrophoneAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Microphone_Robot_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Microphone_Robot_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMicrophone_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMicrophone_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyMicrophoneCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMicrophone: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMicrophone: set notification", result);

            return true;
        }

        private void NotifyMicrophoneCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Audio";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Microphone = parseResult.ValueList.GetValue("Audio").AsString;

            MicrophoneEvent?.Invoke(parseResult);

        }

        public void NotifyMicrophoneRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Microphone_Robot_enum];
            if (ch == null) return;
            NotifyMicrophone_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyMicrophoneCallback;
        }

        /// <summary>
        /// Writes data for Microphone
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMicrophone(byte[] Audio)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Microphone_Robot_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Audio);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Microphone_Robot_enum, "Microphone", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Microphone_Robot_enum, "Microphone", subcommand, GattWriteOption.WriteWithResponse);
            }
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum)) return null;
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
        public async Task WriteDevice_Name(String Device_Name)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(  Device_Name);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", subcommand, GattWriteOption.WriteWithResponse);
            }
        }



    }
}