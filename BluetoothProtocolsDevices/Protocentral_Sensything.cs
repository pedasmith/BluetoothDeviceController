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
    /// Sensything is an open source, high-resolution (24-bit), Wi-Fi and Bluetooth-enabled sensor interface platform that supports multiple sensor readings. In most cases, it offers a single-board, single-platform solution for acquiring and logging multiple sensor readings that can be seen/sent through an Android app, an IoT or analytics platform, over an ordinary USB connection, or logged right to a microSD..
    /// This class was automatically generated 2022-12-09::06:15
    /// </summary>

    public  class Protocentral_Sensything : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://www.crowdsupply.com/protocentral/sensything
    // Link: https://github.com/Protocentral/protocentral_sensything


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("cd5c1105-4448-7db8-ae4c-d1da8cba36d0"),
           Guid.Parse("cd5c1100-4448-7db8-ae4c-d1da8cba36d0"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Battery",
            "Primary",
            "QWIIC",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #2 is Central Address Resolution
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #0 is BatteryLevel
            Guid.Parse("cd5c1106-4448-7db8-ae4c-d1da8cba36d0"), // #0 is Analog
            Guid.Parse("cd5c1101-4448-7db8-ae4c-d1da8cba36d0"), // #0 is QWIIC

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Central Address Resolution", // #2 is 00002aa6-0000-1000-8000-00805f9b34fb
            "BatteryLevel", // #0 is 00002a19-0000-1000-8000-00805f9b34fb
            "Analog", // #0 is cd5c1106-4448-7db8-ae4c-d1da8cba36d0
            "QWIIC", // #0 is cd5c1101-4448-7db8-ae4c-d1da8cba36d0

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
            new HashSet<int>(){ 0, 1, 2,  },
            new HashSet<int>(){ 3,  },
            new HashSet<int>(){ 4,  },
            new HashSet<int>(){ 5,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            0, // Characteristic 2
            1, // Characteristic 3
            2, // Characteristic 4
            3, // Characteristic 5
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Device_Name_Common_Configuration_enum = 0,
            Appearance_Common_Configuration_enum = 1,
            Central_Address_Resolution_Common_Configuration_enum = 2,
            BatteryLevel_Battery_enum = 3,
            Analog_Primary_enum = 4,
            QWIIC_QWIIC_enum = 5,

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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum, "Appearance", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|Speciality^Appearance|Appearance";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Appearance = parseResult.ValueList.GetValue("Appearance").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Central_Address_Resolution = 0;
        private bool _Central_Address_Resolution_set = false;
        public double Central_Address_Resolution
        {
            get { return _Central_Address_Resolution; }
            internal set { if (_Central_Address_Resolution_set && value == _Central_Address_Resolution) return; _Central_Address_Resolution = value; _Central_Address_Resolution_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadCentral_Address_Resolution(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Central_Address_Resolution_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Central_Address_Resolution_Common_Configuration_enum, "Central_Address_Resolution", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|AddressResolutionSupported";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Central_Address_Resolution = parseResult.ValueList.GetValue("AddressResolutionSupported").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _BatteryLevel = 0;
        private bool _BatteryLevel_set = false;
        public double BatteryLevel
        {
            get { return _BatteryLevel; }
            internal set { if (_BatteryLevel_set && value == _BatteryLevel) return; _BatteryLevel = value; _BatteryLevel_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBatteryLevel(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BatteryLevel_Battery_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.BatteryLevel_Battery_enum, "BatteryLevel", cacheMode);
            if (result == null) return null;

            var datameaning = "I8|DEC|BatteryLevel|%";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            BatteryLevel = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; BatteryLevelEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent BatteryLevelEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyBatteryLevel_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyBatteryLevelAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BatteryLevel_Battery_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.BatteryLevel_Battery_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyBatteryLevel_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyBatteryLevel_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyBatteryLevelCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyBatteryLevel: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyBatteryLevel: set notification", result);

            return true;
        }

        private void NotifyBatteryLevelCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I8|DEC|BatteryLevel|%";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            BatteryLevel = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

            BatteryLevelEvent?.Invoke(parseResult);

        }

        public void NotifyBatteryLevelRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.BatteryLevel_Battery_enum];
            if (ch == null) return;
            NotifyBatteryLevel_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBatteryLevelCallback;
        }

        /// <summary>
        /// Writes data for BatteryLevel
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteBatteryLevel(sbyte BatteryLevel)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BatteryLevel_Battery_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte( (byte) BatteryLevel);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.BatteryLevel_Battery_enum, "BatteryLevel", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.BatteryLevel_Battery_enum, "BatteryLevel", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Analog_A1 = 0;
        private bool _Analog_A1_set = false;
        public double Analog_A1
        {
            get { return _Analog_A1; }
            internal set { if (_Analog_A1_set && value == _Analog_A1) return; _Analog_A1 = value; _Analog_A1_set = true; OnPropertyChanged(); }
        }
        private double _Analog_A2 = 0;
        private bool _Analog_A2_set = false;
        public double Analog_A2
        {
            get { return _Analog_A2; }
            internal set { if (_Analog_A2_set && value == _Analog_A2) return; _Analog_A2 = value; _Analog_A2_set = true; OnPropertyChanged(); }
        }
        private double _Analog_A3 = 0;
        private bool _Analog_A3_set = false;
        public double Analog_A3
        {
            get { return _Analog_A3; }
            internal set { if (_Analog_A3_set && value == _Analog_A3) return; _Analog_A3 = value; _Analog_A3_set = true; OnPropertyChanged(); }
        }
        private double _Analog_A4 = 0;
        private bool _Analog_A4_set = false;
        public double Analog_A4
        {
            get { return _Analog_A4; }
            internal set { if (_Analog_A4_set && value == _Analog_A4) return; _Analog_A4 = value; _Analog_A4_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; AnalogEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent AnalogEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyAnalog_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyAnalogAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Analog_Primary_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Analog_Primary_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyAnalog_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyAnalog_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyAnalogCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyAnalog: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyAnalog: set notification", result);

            return true;
        }

        private void NotifyAnalogCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I32^100000_/|FIXED|A1|volts I32^100000_/|FIXED|A2|volts I32^100000_/|FIXED|A3|volts I32^100000_/|FIXED|A4|volts";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Analog_A1 = parseResult.ValueList.GetValue("A1").AsDouble;
            Analog_A2 = parseResult.ValueList.GetValue("A2").AsDouble;
            Analog_A3 = parseResult.ValueList.GetValue("A3").AsDouble;
            Analog_A4 = parseResult.ValueList.GetValue("A4").AsDouble;

            AnalogEvent?.Invoke(parseResult);

        }

        public void NotifyAnalogRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Analog_Primary_enum];
            if (ch == null) return;
            NotifyAnalog_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyAnalogCallback;
        }



        private double _QWIIC_Sensor = 0;
        private bool _QWIIC_Sensor_set = false;
        public double QWIIC_Sensor
        {
            get { return _QWIIC_Sensor; }
            internal set { if (_QWIIC_Sensor_set && value == _QWIIC_Sensor) return; _QWIIC_Sensor = value; _QWIIC_Sensor_set = true; OnPropertyChanged(); }
        }
        private double _QWIIC_Channel1 = 0;
        private bool _QWIIC_Channel1_set = false;
        public double QWIIC_Channel1
        {
            get { return _QWIIC_Channel1; }
            internal set { if (_QWIIC_Channel1_set && value == _QWIIC_Channel1) return; _QWIIC_Channel1 = value; _QWIIC_Channel1_set = true; OnPropertyChanged(); }
        }
        private double _QWIIC_Channel2 = 0;
        private bool _QWIIC_Channel2_set = false;
        public double QWIIC_Channel2
        {
            get { return _QWIIC_Channel2; }
            internal set { if (_QWIIC_Channel2_set && value == _QWIIC_Channel2) return; _QWIIC_Channel2 = value; _QWIIC_Channel2_set = true; OnPropertyChanged(); }
        }
        private double _QWIIC_Channel3 = 0;
        private bool _QWIIC_Channel3_set = false;
        public double QWIIC_Channel3
        {
            get { return _QWIIC_Channel3; }
            internal set { if (_QWIIC_Channel3_set && value == _QWIIC_Channel3) return; _QWIIC_Channel3 = value; _QWIIC_Channel3_set = true; OnPropertyChanged(); }
        }
        private double _QWIIC_Channel4 = 0;
        private bool _QWIIC_Channel4_set = false;
        public double QWIIC_Channel4
        {
            get { return _QWIIC_Channel4; }
            internal set { if (_QWIIC_Channel4_set && value == _QWIIC_Channel4) return; _QWIIC_Channel4 = value; _QWIIC_Channel4_set = true; OnPropertyChanged(); }
        }
        private double _QWIIC_Channel5 = 0;
        private bool _QWIIC_Channel5_set = false;
        public double QWIIC_Channel5
        {
            get { return _QWIIC_Channel5; }
            internal set { if (_QWIIC_Channel5_set && value == _QWIIC_Channel5) return; _QWIIC_Channel5 = value; _QWIIC_Channel5_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadQWIIC(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.QWIIC_QWIIC_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.QWIIC_QWIIC_enum, "QWIIC", cacheMode);
            if (result == null) return null;

            var datameaning = "OEB U8|HEX|Sensor U16|HEX|Channel1 U16|HEX|Channel2 U16|HEX|Channel3 U16|HEX|Channel4 U16|HEX|Channel5";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            QWIIC_Sensor = parseResult.ValueList.GetValue("Sensor").AsDouble;
            QWIIC_Channel1 = parseResult.ValueList.GetValue("Channel1").AsDouble;
            QWIIC_Channel2 = parseResult.ValueList.GetValue("Channel2").AsDouble;
            QWIIC_Channel3 = parseResult.ValueList.GetValue("Channel3").AsDouble;
            QWIIC_Channel4 = parseResult.ValueList.GetValue("Channel4").AsDouble;
            QWIIC_Channel5 = parseResult.ValueList.GetValue("Channel5").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; QWIICEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent QWIICEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyQWIIC_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyQWIICAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.QWIIC_QWIIC_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.QWIIC_QWIIC_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyQWIIC_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyQWIIC_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyQWIICCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyQWIIC: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyQWIIC: set notification", result);

            return true;
        }

        private void NotifyQWIICCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "OEB U8|HEX|Sensor U16|HEX|Channel1 U16|HEX|Channel2 U16|HEX|Channel3 U16|HEX|Channel4 U16|HEX|Channel5";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            QWIIC_Sensor = parseResult.ValueList.GetValue("Sensor").AsDouble;
            QWIIC_Channel1 = parseResult.ValueList.GetValue("Channel1").AsDouble;
            QWIIC_Channel2 = parseResult.ValueList.GetValue("Channel2").AsDouble;
            QWIIC_Channel3 = parseResult.ValueList.GetValue("Channel3").AsDouble;
            QWIIC_Channel4 = parseResult.ValueList.GetValue("Channel4").AsDouble;
            QWIIC_Channel5 = parseResult.ValueList.GetValue("Channel5").AsDouble;

            QWIICEvent?.Invoke(parseResult);

        }

        public void NotifyQWIICRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.QWIIC_QWIIC_enum];
            if (ch == null) return;
            NotifyQWIIC_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyQWIICCallback;
        }

        /// <summary>
        /// Writes data for QWIIC
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteQWIIC(byte Sensor, UInt16 Channel1, UInt16 Channel2, UInt16 Channel3, UInt16 Channel4, UInt16 Channel5)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.QWIIC_QWIIC_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  Sensor);
            dw.WriteUInt16(  Channel1);
            dw.WriteUInt16(  Channel2);
            dw.WriteUInt16(  Channel3);
            dw.WriteUInt16(  Channel4);
            dw.WriteUInt16(  Channel5);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.QWIIC_QWIIC_enum, "QWIIC", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.QWIIC_QWIIC_enum, "QWIIC", subcommand, GattWriteOption.WriteWithResponse);
            }
        }



    }
}