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
    /// This class was automatically generated 2022-07-24::10:01
    /// </summary>

    public  class Kano_Wand : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://kano.me/store/us/products/coding-wand


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
           Guid.Parse("64a70010-f691-4b93-a6f4-0968f5b648f8"),
           Guid.Parse("64a70012-f691-4b93-a6f4-0968f5b648f8"),
           Guid.Parse("64a70011-f691-4b93-a6f4-0968f5b648f8"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "WandSoftwareInfo",
            "HardwareControl",
            "WandData",

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
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #3 is Central Address Resolution
            Guid.Parse("64a7000b-f691-4b93-a6f4-0968f5b648f8"), // #0 is Maker Name
            Guid.Parse("64a70013-f691-4b93-a6f4-0968f5b648f8"), // #1 is Version
            Guid.Parse("64a70001-f691-4b93-a6f4-0968f5b648f8"), // #2 is HardwareDescription
            Guid.Parse("64a70007-f691-4b93-a6f4-0968f5b648f8"), // #0 is Battery
            Guid.Parse("64a70008-f691-4b93-a6f4-0968f5b648f8"), // #1 is Vibration
            Guid.Parse("64a70009-f691-4b93-a6f4-0968f5b648f8"), // #2 is LED Control
            Guid.Parse("64a7000d-f691-4b93-a6f4-0968f5b648f8"), // #3 is Button
            Guid.Parse("64a7000f-f691-4b93-a6f4-0968f5b648f8"), // #4 is Keepalive
            Guid.Parse("64a70002-f691-4b93-a6f4-0968f5b648f8"), // #0 is WandData
            Guid.Parse("64a70004-f691-4b93-a6f4-0968f5b648f8"), // #1 is RstQuaternions
            Guid.Parse("64a7000a-f691-4b93-a6f4-0968f5b648f8"), // #2 is Raw9Axis
            Guid.Parse("64a7000c-f691-4b93-a6f4-0968f5b648f8"), // #3 is Linear Acc
            Guid.Parse("64a70014-f691-4b93-a6f4-0968f5b648f8"), // #4 is Temperature
            Guid.Parse("64a70021-f691-4b93-a6f4-0968f5b648f8"), // #5 is MagnetometerCalibration

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "Central Address Resolution", // #3 is 00002aa6-0000-1000-8000-00805f9b34fb
            "Maker Name", // #0 is 64a7000b-f691-4b93-a6f4-0968f5b648f8
            "Version", // #1 is 64a70013-f691-4b93-a6f4-0968f5b648f8
            "HardwareDescription", // #2 is 64a70001-f691-4b93-a6f4-0968f5b648f8
            "Battery", // #0 is 64a70007-f691-4b93-a6f4-0968f5b648f8
            "Vibration", // #1 is 64a70008-f691-4b93-a6f4-0968f5b648f8
            "LED Control", // #2 is 64a70009-f691-4b93-a6f4-0968f5b648f8
            "Button", // #3 is 64a7000d-f691-4b93-a6f4-0968f5b648f8
            "Keepalive", // #4 is 64a7000f-f691-4b93-a6f4-0968f5b648f8
            "WandData", // #0 is 64a70002-f691-4b93-a6f4-0968f5b648f8
            "RstQuaternions", // #1 is 64a70004-f691-4b93-a6f4-0968f5b648f8
            "Raw9Axis", // #2 is 64a7000a-f691-4b93-a6f4-0968f5b648f8
            "Linear Acc", // #3 is 64a7000c-f691-4b93-a6f4-0968f5b648f8
            "Temperature", // #4 is 64a70014-f691-4b93-a6f4-0968f5b648f8
            "MagnetometerCalibration", // #5 is 64a70021-f691-4b93-a6f4-0968f5b648f8

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3,  },
            new HashSet<int>(){ 4, 5, 6,  },
            new HashSet<int>(){ 7, 8, 9, 10, 11,  },
            new HashSet<int>(){ 12, 13, 14, 15, 16, 17,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            0, // Characteristic 2
            0, // Characteristic 3
            1, // Characteristic 4
            1, // Characteristic 5
            1, // Characteristic 6
            2, // Characteristic 7
            2, // Characteristic 8
            2, // Characteristic 9
            2, // Characteristic 10
            2, // Characteristic 11
            3, // Characteristic 12
            3, // Characteristic 13
            3, // Characteristic 14
            3, // Characteristic 15
            3, // Characteristic 16
            3, // Characteristic 17
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Device_Name_Common_Configuration_enum = 0,
            Appearance_Common_Configuration_enum = 1,
            Connection_Parameter_Common_Configuration_enum = 2,
            Central_Address_Resolution_Common_Configuration_enum = 3,
            Maker_Name_WandSoftwareInfo_enum = 4,
            Version_WandSoftwareInfo_enum = 5,
            HardwareDescription_WandSoftwareInfo_enum = 6,
            Battery_HardwareControl_enum = 7,
            Vibration_HardwareControl_enum = 8,
            LED_Control_HardwareControl_enum = 9,
            Button_HardwareControl_enum = 10,
            Keepalive_HardwareControl_enum = 11,
            WandData_WandData_enum = 12,
            RstQuaternions_WandData_enum = 13,
            Raw9Axis_WandData_enum = 14,
            Linear_Acc_WandData_enum = 15,
            Temperature_WandData_enum = 16,
            MagnetometerCalibration_WandData_enum = 17,

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




        private string _Connection_Parameter = null;
        private bool _Connection_Parameter_set = false;
        public string Connection_Parameter
        {
            get { return _Connection_Parameter; }
            internal set { if (_Connection_Parameter_set && value == _Connection_Parameter) return; _Connection_Parameter = value; _Connection_Parameter_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConnection_Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum, "Connection_Parameter", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|ConnectionParameter";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Connection_Parameter = parseResult.ValueList.GetValue("ConnectionParameter").AsString;

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




        private string _Maker_Name = "";
        private bool _Maker_Name_set = false;
        public string Maker_Name
        {
            get { return _Maker_Name; }
            internal set { if (_Maker_Name_set && value == _Maker_Name) return; _Maker_Name = value; _Maker_Name_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMaker_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Maker_Name_WandSoftwareInfo_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Maker_Name_WandSoftwareInfo_enum, "Maker_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Maker";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Maker_Name = parseResult.ValueList.GetValue("Maker").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Version = "";
        private bool _Version_set = false;
        public string Version
        {
            get { return _Version; }
            internal set { if (_Version_set && value == _Version) return; _Version = value; _Version_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadVersion(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Version_WandSoftwareInfo_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Version_WandSoftwareInfo_enum, "Version", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Version";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Version = parseResult.ValueList.GetValue("Version").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _HardwareDescription = "";
        private bool _HardwareDescription_set = false;
        public string HardwareDescription
        {
            get { return _HardwareDescription; }
            internal set { if (_HardwareDescription_set && value == _HardwareDescription) return; _HardwareDescription = value; _HardwareDescription_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHardwareDescription(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.HardwareDescription_WandSoftwareInfo_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.HardwareDescription_WandSoftwareInfo_enum, "HardwareDescription", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|HardwareDescription";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            HardwareDescription = parseResult.ValueList.GetValue("HardwareDescription").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Battery = 0;
        private bool _Battery_set = false;
        public double Battery
        {
            get { return _Battery; }
            internal set { if (_Battery_set && value == _Battery) return; _Battery = value; _Battery_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBattery(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Battery_HardwareControl_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Battery_HardwareControl_enum, "Battery", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Battery";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Battery = parseResult.ValueList.GetValue("Battery").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; BatteryEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent BatteryEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyBattery_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyBatteryAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Battery_HardwareControl_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Battery_HardwareControl_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyBattery_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyBattery_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyBatteryCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyBattery: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyBattery: set notification", result);

            return true;
        }

        private void NotifyBatteryCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|DEC|Battery";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Battery = parseResult.ValueList.GetValue("Battery").AsDouble;

            BatteryEvent?.Invoke(parseResult);

        }

        public void NotifyBatteryRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[7];
            if (ch == null) return;
            NotifyBattery_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBatteryCallback;
        }



        private double _Vibration = 0;
        private bool _Vibration_set = false;
        public double Vibration
        {
            get { return _Vibration; }
            internal set { if (_Vibration_set && value == _Vibration) return; _Vibration = value; _Vibration_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadVibration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Vibration_HardwareControl_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Vibration_HardwareControl_enum, "Vibration", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Vibration";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Vibration = parseResult.ValueList.GetValue("Vibration").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Vibration
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteVibration(byte Vibration)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Vibration_HardwareControl_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  Vibration);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Vibration_HardwareControl_enum, "Vibration", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Vibration_HardwareControl_enum, "Vibration", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _LED_Control_OnOff = 0;
        private bool _LED_Control_OnOff_set = false;
        public double LED_Control_OnOff
        {
            get { return _LED_Control_OnOff; }
            internal set { if (_LED_Control_OnOff_set && value == _LED_Control_OnOff) return; _LED_Control_OnOff = value; _LED_Control_OnOff_set = true; OnPropertyChanged(); }
        }
        private double _LED_Control_R5G6B5 = 0;
        private bool _LED_Control_R5G6B5_set = false;
        public double LED_Control_R5G6B5
        {
            get { return _LED_Control_R5G6B5; }
            internal set { if (_LED_Control_R5G6B5_set && value == _LED_Control_R5G6B5) return; _LED_Control_R5G6B5 = value; _LED_Control_R5G6B5_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLED_Control(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.LED_Control_HardwareControl_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.LED_Control_HardwareControl_enum, "LED_Control", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|OnOff U16|HEX|R5G6B5";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            LED_Control_OnOff = parseResult.ValueList.GetValue("OnOff").AsDouble;
            LED_Control_R5G6B5 = parseResult.ValueList.GetValue("R5G6B5").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for LED_Control
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLED_Control(byte OnOff, UInt16 R5G6B5)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.LED_Control_HardwareControl_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  OnOff);
            dw.WriteUInt16(  R5G6B5);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.LED_Control_HardwareControl_enum, "LED_Control", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.LED_Control_HardwareControl_enum, "LED_Control", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Button = 0;
        private bool _Button_set = false;
        public double Button
        {
            get { return _Button; }
            internal set { if (_Button_set && value == _Button) return; _Button = value; _Button_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadButton(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Button_HardwareControl_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Button_HardwareControl_enum, "Button", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Button0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Button = parseResult.ValueList.GetValue("Button0").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ButtonEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ButtonEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyButton_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyButtonAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Button_HardwareControl_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Button_HardwareControl_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyButton_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyButton_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyButtonCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyButton: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyButton: set notification", result);

            return true;
        }

        private void NotifyButtonCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX|Button0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Button = parseResult.ValueList.GetValue("Button0").AsDouble;

            ButtonEvent?.Invoke(parseResult);

        }

        public void NotifyButtonRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[10];
            if (ch == null) return;
            NotifyButton_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyButtonCallback;
        }






        /// <summary>
        /// Writes data for Keepalive
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteKeepalive(byte[] Keepalive)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Keepalive_HardwareControl_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Keepalive);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Keepalive_HardwareControl_enum, "Keepalive", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Keepalive_HardwareControl_enum, "Keepalive", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _WandData_Angle = 0;
        private bool _WandData_Angle_set = false;
        public double WandData_Angle
        {
            get { return _WandData_Angle; }
            internal set { if (_WandData_Angle_set && value == _WandData_Angle) return; _WandData_Angle = value; _WandData_Angle_set = true; OnPropertyChanged(); }
        }
        private double _WandData_LeftRight = 0;
        private bool _WandData_LeftRight_set = false;
        public double WandData_LeftRight
        {
            get { return _WandData_LeftRight; }
            internal set { if (_WandData_LeftRight_set && value == _WandData_LeftRight) return; _WandData_LeftRight = value; _WandData_LeftRight_set = true; OnPropertyChanged(); }
        }
        private double _WandData_Twist = 0;
        private bool _WandData_Twist_set = false;
        public double WandData_Twist
        {
            get { return _WandData_Twist; }
            internal set { if (_WandData_Twist_set && value == _WandData_Twist) return; _WandData_Twist = value; _WandData_Twist_set = true; OnPropertyChanged(); }
        }
        private double _WandData_Pitch = 0;
        private bool _WandData_Pitch_set = false;
        public double WandData_Pitch
        {
            get { return _WandData_Pitch; }
            internal set { if (_WandData_Pitch_set && value == _WandData_Pitch) return; _WandData_Pitch = value; _WandData_Pitch_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; WandDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent WandDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyWandData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyWandDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.WandData_WandData_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.WandData_WandData_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyWandData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyWandData_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyWandDataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyWandData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyWandData: set notification", result);

            return true;
        }

        private void NotifyWandDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I16|DEC|Angle I16|DEC|LeftRight I16|DEC|Twist I16|DEC|Pitch";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            WandData_Angle = parseResult.ValueList.GetValue("Angle").AsDouble;
            WandData_LeftRight = parseResult.ValueList.GetValue("LeftRight").AsDouble;
            WandData_Twist = parseResult.ValueList.GetValue("Twist").AsDouble;
            WandData_Pitch = parseResult.ValueList.GetValue("Pitch").AsDouble;

            WandDataEvent?.Invoke(parseResult);

        }

        public void NotifyWandDataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[12];
            if (ch == null) return;
            NotifyWandData_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyWandDataCallback;
        }






        /// <summary>
        /// Writes data for RstQuaternions
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteRstQuaternions(byte[] RawQuaternions)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.RstQuaternions_WandData_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  RawQuaternions);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.RstQuaternions_WandData_enum, "RstQuaternions", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.RstQuaternions_WandData_enum, "RstQuaternions", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _Raw9Axis = null;
        private bool _Raw9Axis_set = false;
        public string Raw9Axis
        {
            get { return _Raw9Axis; }
            internal set { if (_Raw9Axis_set && value == _Raw9Axis) return; _Raw9Axis = value; _Raw9Axis_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Raw9AxisEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Raw9AxisEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyRaw9Axis_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyRaw9AxisAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Raw9Axis_WandData_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Raw9Axis_WandData_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyRaw9Axis_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyRaw9Axis_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyRaw9AxisCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyRaw9Axis: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyRaw9Axis: set notification", result);

            return true;
        }

        private void NotifyRaw9AxisCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|RawAxisData";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Raw9Axis = parseResult.ValueList.GetValue("RawAxisData").AsString;

            Raw9AxisEvent?.Invoke(parseResult);

        }

        public void NotifyRaw9AxisRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[14];
            if (ch == null) return;
            NotifyRaw9Axis_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyRaw9AxisCallback;
        }



        private string _Linear_Acc = null;
        private bool _Linear_Acc_set = false;
        public string Linear_Acc
        {
            get { return _Linear_Acc; }
            internal set { if (_Linear_Acc_set && value == _Linear_Acc) return; _Linear_Acc = value; _Linear_Acc_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Linear_AccEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Linear_AccEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyLinear_Acc_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyLinear_AccAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Linear_Acc_WandData_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Linear_Acc_WandData_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyLinear_Acc_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyLinear_Acc_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyLinear_AccCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyLinear_Acc: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyLinear_Acc: set notification", result);

            return true;
        }

        private void NotifyLinear_AccCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|RawLinearAcceleration";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Linear_Acc = parseResult.ValueList.GetValue("RawLinearAcceleration").AsString;

            Linear_AccEvent?.Invoke(parseResult);

        }

        public void NotifyLinear_AccRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[15];
            if (ch == null) return;
            NotifyLinear_Acc_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyLinear_AccCallback;
        }



        private string _Temperature = null;
        private bool _Temperature_set = false;
        public string Temperature
        {
            get { return _Temperature; }
            internal set { if (_Temperature_set && value == _Temperature) return; _Temperature = value; _Temperature_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; TemperatureEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent TemperatureEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyTemperature_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyTemperatureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_WandData_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Temperature_WandData_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTemperature_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTemperature_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyTemperatureCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyTemperature: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyTemperature: set notification", result);

            return true;
        }

        private void NotifyTemperatureCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Temperature";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Temperature = parseResult.ValueList.GetValue("Temperature").AsString;

            TemperatureEvent?.Invoke(parseResult);

        }

        public void NotifyTemperatureRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[16];
            if (ch == null) return;
            NotifyTemperature_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyTemperatureCallback;
        }



        private string _MagnetometerCalibration = null;
        private bool _MagnetometerCalibration_set = false;
        public string MagnetometerCalibration
        {
            get { return _MagnetometerCalibration; }
            internal set { if (_MagnetometerCalibration_set && value == _MagnetometerCalibration) return; _MagnetometerCalibration = value; _MagnetometerCalibration_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; MagnetometerCalibrationEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent MagnetometerCalibrationEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMagnetometerCalibration_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMagnetometerCalibrationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MagnetometerCalibration_WandData_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.MagnetometerCalibration_WandData_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMagnetometerCalibration_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMagnetometerCalibration_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyMagnetometerCalibrationCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMagnetometerCalibration: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMagnetometerCalibration: set notification", result);

            return true;
        }

        private void NotifyMagnetometerCalibrationCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Calibration";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            MagnetometerCalibration = parseResult.ValueList.GetValue("Calibration").AsString;

            MagnetometerCalibrationEvent?.Invoke(parseResult);

        }

        public void NotifyMagnetometerCalibrationRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[17];
            if (ch == null) return;
            NotifyMagnetometerCalibration_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyMagnetometerCalibrationCallback;
        }

        /// <summary>
        /// Writes data for MagnetometerCalibration
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMagnetometerCalibration(byte[] Calibration)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MagnetometerCalibration_WandData_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Calibration);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.MagnetometerCalibration_WandData_enum, "MagnetometerCalibration", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.MagnetometerCalibration_WandData_enum, "MagnetometerCalibration", subcommand, GattWriteOption.WriteWithResponse);
            }
        }



    }
}