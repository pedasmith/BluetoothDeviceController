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
    /// The original smaller (upright) TI SensorTag. As of 2017, it's obsolete. The CC2541 SensorTag is the first Bluetooth Smart development kit focused on wireless sensor applications and it is the only development kit targeted for smart phone app developers. The SensorTag can be used as reference design and development platform for a variety of smart phone accessories..
    /// This class was automatically generated 2022-07-24::10:01
    /// </summary>

    public partial class TI_SensorTag_2541 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // No links for this device

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
           Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("f000aa00-0451-4000-b000-000000000000"),
           Guid.Parse("f000aa10-0451-4000-b000-000000000000"),
           Guid.Parse("f000aa20-0451-4000-b000-000000000000"),
           Guid.Parse("f000aa30-0451-4000-b000-000000000000"),
           Guid.Parse("f000aa40-0451-4000-b000-000000000000"),
           Guid.Parse("f000aa50-0451-4000-b000-000000000000"),
           Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Device Info",
            "IR Service",
            "Accelerometer",
            "Humidity",
            "Magnetometer",
            "Barometer",
            "Gyroscope",
            "Key Press",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
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
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb"), // #2 is Privacy
            Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb"), // #3 is Reconnect Address
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #4 is Connection Parameter
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #0 is System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #2 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #3 is Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #4 is Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #5 is Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #6 is Manufacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #7 is Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #8 is PnP ID
            Guid.Parse("f000aa01-0451-4000-b000-000000000000"), // #0 is IR Data
            Guid.Parse("f000aa02-0451-4000-b000-000000000000"), // #1 is IR Service Configure
            Guid.Parse("f000aa03-0451-4000-b000-000000000000"), // #2 is IR Service Period
            Guid.Parse("f000aa11-0451-4000-b000-000000000000"), // #0 is Accelerometer Data
            Guid.Parse("f000aa12-0451-4000-b000-000000000000"), // #1 is Accelerometer Configure
            Guid.Parse("f000aa13-0451-4000-b000-000000000000"), // #2 is Accelerometer Period
            Guid.Parse("f000aa21-0451-4000-b000-000000000000"), // #0 is Humidity Data
            Guid.Parse("f000aa22-0451-4000-b000-000000000000"), // #1 is Humidity Configure
            Guid.Parse("f000aa23-0451-4000-b000-000000000000"), // #2 is Humidity Period
            Guid.Parse("f000aa31-0451-4000-b000-000000000000"), // #0 is Magnetometer Data
            Guid.Parse("f000aa32-0451-4000-b000-000000000000"), // #1 is Magnetometer Configure
            Guid.Parse("f000aa33-0451-4000-b000-000000000000"), // #2 is Magnetometer Period
            Guid.Parse("f000aa41-0451-4000-b000-000000000000"), // #0 is Barometer Data
            Guid.Parse("f000aa42-0451-4000-b000-000000000000"), // #1 is Barometer Configure
            Guid.Parse("f000aa43-0451-4000-b000-000000000000"), // #2 is Barometer Calibration
            Guid.Parse("f000aa44-0451-4000-b000-000000000000"), // #3 is Barometer Period
            Guid.Parse("f000aa51-0451-4000-b000-000000000000"), // #0 is Gyroscope Data
            Guid.Parse("f000aa52-0451-4000-b000-000000000000"), // #1 is Gyroscope Configure
            Guid.Parse("f000aa53-0451-4000-b000-000000000000"), // #2 is Gyroscope Period
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #0 is Key Press State

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Privacy", // #2 is 00002a02-0000-1000-8000-00805f9b34fb
            "Reconnect Address", // #3 is 00002a03-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #4 is 00002a04-0000-1000-8000-00805f9b34fb
            "System ID", // #0 is 00002a23-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #2 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #3 is 00002a26-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #4 is 00002a27-0000-1000-8000-00805f9b34fb
            "Software Revision", // #5 is 00002a28-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #6 is 00002a29-0000-1000-8000-00805f9b34fb
            "Regulatory List", // #7 is 00002a2a-0000-1000-8000-00805f9b34fb
            "PnP ID", // #8 is 00002a50-0000-1000-8000-00805f9b34fb
            "IR Data", // #0 is f000aa01-0451-4000-b000-000000000000
            "IR Service Configure", // #1 is f000aa02-0451-4000-b000-000000000000
            "IR Service Period", // #2 is f000aa03-0451-4000-b000-000000000000
            "Accelerometer Data", // #0 is f000aa11-0451-4000-b000-000000000000
            "Accelerometer Configure", // #1 is f000aa12-0451-4000-b000-000000000000
            "Accelerometer Period", // #2 is f000aa13-0451-4000-b000-000000000000
            "Humidity Data", // #0 is f000aa21-0451-4000-b000-000000000000
            "Humidity Configure", // #1 is f000aa22-0451-4000-b000-000000000000
            "Humidity Period", // #2 is f000aa23-0451-4000-b000-000000000000
            "Magnetometer Data", // #0 is f000aa31-0451-4000-b000-000000000000
            "Magnetometer Configure", // #1 is f000aa32-0451-4000-b000-000000000000
            "Magnetometer Period", // #2 is f000aa33-0451-4000-b000-000000000000
            "Barometer Data", // #0 is f000aa41-0451-4000-b000-000000000000
            "Barometer Configure", // #1 is f000aa42-0451-4000-b000-000000000000
            "Barometer Calibration", // #2 is f000aa43-0451-4000-b000-000000000000
            "Barometer Period", // #3 is f000aa44-0451-4000-b000-000000000000
            "Gyroscope Data", // #0 is f000aa51-0451-4000-b000-000000000000
            "Gyroscope Configure", // #1 is f000aa52-0451-4000-b000-000000000000
            "Gyroscope Period", // #2 is f000aa53-0451-4000-b000-000000000000
            "Key Press State", // #0 is 0000ffe1-0000-1000-8000-00805f9b34fb

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
            new HashSet<int>(){ 0, 1, 2, 3, 4,  },
            new HashSet<int>(){ 5, 6, 7, 8, 9, 10, 11, 12, 13,  },
            new HashSet<int>(){ 14, 15, 16,  },
            new HashSet<int>(){ 17, 18, 19,  },
            new HashSet<int>(){ 20, 21, 22,  },
            new HashSet<int>(){ 23, 24, 25,  },
            new HashSet<int>(){ 26, 27, 28, 29,  },
            new HashSet<int>(){ 30, 31, 32,  },
            new HashSet<int>(){ 33,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            0, // Characteristic 2
            0, // Characteristic 3
            0, // Characteristic 4
            1, // Characteristic 5
            1, // Characteristic 6
            1, // Characteristic 7
            1, // Characteristic 8
            1, // Characteristic 9
            1, // Characteristic 10
            1, // Characteristic 11
            1, // Characteristic 12
            1, // Characteristic 13
            2, // Characteristic 14
            2, // Characteristic 15
            2, // Characteristic 16
            3, // Characteristic 17
            3, // Characteristic 18
            3, // Characteristic 19
            4, // Characteristic 20
            4, // Characteristic 21
            4, // Characteristic 22
            5, // Characteristic 23
            5, // Characteristic 24
            5, // Characteristic 25
            6, // Characteristic 26
            6, // Characteristic 27
            6, // Characteristic 28
            6, // Characteristic 29
            7, // Characteristic 30
            7, // Characteristic 31
            7, // Characteristic 32
            8, // Characteristic 33
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Device_Name_Common_Configuration_enum = 0,
            Appearance_Common_Configuration_enum = 1,
            Privacy_Common_Configuration_enum = 2,
            Reconnect_Address_Common_Configuration_enum = 3,
            Connection_Parameter_Common_Configuration_enum = 4,
            System_ID_Device_Info_enum = 5,
            Model_Number_Device_Info_enum = 6,
            Serial_Number_Device_Info_enum = 7,
            Firmware_Revision_Device_Info_enum = 8,
            Hardware_Revision_Device_Info_enum = 9,
            Software_Revision_Device_Info_enum = 10,
            Manufacturer_Name_Device_Info_enum = 11,
            Regulatory_List_Device_Info_enum = 12,
            PnP_ID_Device_Info_enum = 13,
            IR_Data_IR_Service_enum = 14,
            IR_Service_Configure_IR_Service_enum = 15,
            IR_Service_Period_IR_Service_enum = 16,
            Accelerometer_Data_Accelerometer_enum = 17,
            Accelerometer_Configure_Accelerometer_enum = 18,
            Accelerometer_Period_Accelerometer_enum = 19,
            Humidity_Data_Humidity_enum = 20,
            Humidity_Configure_Humidity_enum = 21,
            Humidity_Period_Humidity_enum = 22,
            Magnetometer_Data_Magnetometer_enum = 23,
            Magnetometer_Configure_Magnetometer_enum = 24,
            Magnetometer_Period_Magnetometer_enum = 25,
            Barometer_Data_Barometer_enum = 26,
            Barometer_Configure_Barometer_enum = 27,
            Barometer_Calibration_Barometer_enum = 28,
            Barometer_Period_Barometer_enum = 29,
            Gyroscope_Data_Gyroscope_enum = 30,
            Gyroscope_Configure_Gyroscope_enum = 31,
            Gyroscope_Period_Gyroscope_enum = 32,
            Key_Press_State_Key_Press_enum = 33,

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




        private string _Privacy = null;
        private bool _Privacy_set = false;
        public string Privacy
        {
            get { return _Privacy; }
            internal set { if (_Privacy_set && value == _Privacy) return; _Privacy = value; _Privacy_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPrivacy(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Privacy_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Privacy_Common_Configuration_enum, "Privacy", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Privacy";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Privacy = parseResult.ValueList.GetValue("Privacy").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Reconnect_Address = null;
        private bool _Reconnect_Address_set = false;
        public string Reconnect_Address
        {
            get { return _Reconnect_Address; }
            internal set { if (_Reconnect_Address_set && value == _Reconnect_Address) return; _Reconnect_Address = value; _Reconnect_Address_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadReconnect_Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum, "Reconnect_Address", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|ReconnectAddress";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Reconnect_Address = parseResult.ValueList.GetValue("ReconnectAddress").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Reconnect_Address
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteReconnect_Address(byte[] ReconnectAddress)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  ReconnectAddress);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum, "Reconnect_Address", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum, "Reconnect_Address", subcommand, GattWriteOption.WriteWithResponse);
            }
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




        private string _System_ID = "";
        private bool _System_ID_set = false;
        public string System_ID
        {
            get { return _System_ID; }
            internal set { if (_System_ID_set && value == _System_ID) return; _System_ID = value; _System_ID_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSystem_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.System_ID_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.System_ID_Device_Info_enum, "System_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            System_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Model_Number = "";
        private bool _Model_Number_set = false;
        public string Model_Number
        {
            get { return _Model_Number; }
            internal set { if (_Model_Number_set && value == _Model_Number) return; _Model_Number = value; _Model_Number_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadModel_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Model_Number_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Model_Number_Device_Info_enum, "Model_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Model_Number = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Serial_Number = "";
        private bool _Serial_Number_set = false;
        public string Serial_Number
        {
            get { return _Serial_Number; }
            internal set { if (_Serial_Number_set && value == _Serial_Number) return; _Serial_Number = value; _Serial_Number_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSerial_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Serial_Number_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Serial_Number_Device_Info_enum, "Serial_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Serial_Number = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Firmware_Revision = "";
        private bool _Firmware_Revision_set = false;
        public string Firmware_Revision
        {
            get { return _Firmware_Revision; }
            internal set { if (_Firmware_Revision_set && value == _Firmware_Revision) return; _Firmware_Revision = value; _Firmware_Revision_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadFirmware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Firmware_Revision_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Firmware_Revision_Device_Info_enum, "Firmware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Firmware_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Hardware_Revision = "";
        private bool _Hardware_Revision_set = false;
        public string Hardware_Revision
        {
            get { return _Hardware_Revision; }
            internal set { if (_Hardware_Revision_set && value == _Hardware_Revision) return; _Hardware_Revision = value; _Hardware_Revision_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHardware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Hardware_Revision_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Hardware_Revision_Device_Info_enum, "Hardware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Hardware_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Software_Revision = "";
        private bool _Software_Revision_set = false;
        public string Software_Revision
        {
            get { return _Software_Revision; }
            internal set { if (_Software_Revision_set && value == _Software_Revision) return; _Software_Revision = value; _Software_Revision_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSoftware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Software_Revision_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Software_Revision_Device_Info_enum, "Software_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Software_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Manufacturer_Name = "";
        private bool _Manufacturer_Name_set = false;
        public string Manufacturer_Name
        {
            get { return _Manufacturer_Name; }
            internal set { if (_Manufacturer_Name_set && value == _Manufacturer_Name) return; _Manufacturer_Name = value; _Manufacturer_Name_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadManufacturer_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Manufacturer_Name_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Manufacturer_Name_Device_Info_enum, "Manufacturer_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Manufacturer_Name = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Regulatory_List_BodyType = 0;
        private bool _Regulatory_List_BodyType_set = false;
        public double Regulatory_List_BodyType
        {
            get { return _Regulatory_List_BodyType; }
            internal set { if (_Regulatory_List_BodyType_set && value == _Regulatory_List_BodyType) return; _Regulatory_List_BodyType = value; _Regulatory_List_BodyType_set = true; OnPropertyChanged(); }
        }
        private double _Regulatory_List_BodyStructure = 0;
        private bool _Regulatory_List_BodyStructure_set = false;
        public double Regulatory_List_BodyStructure
        {
            get { return _Regulatory_List_BodyStructure; }
            internal set { if (_Regulatory_List_BodyStructure_set && value == _Regulatory_List_BodyStructure) return; _Regulatory_List_BodyStructure = value; _Regulatory_List_BodyStructure_set = true; OnPropertyChanged(); }
        }
        private string _Regulatory_List_Data = "";
        private bool _Regulatory_List_Data_set = false;
        public string Regulatory_List_Data
        {
            get { return _Regulatory_List_Data; }
            internal set { if (_Regulatory_List_Data_set && value == _Regulatory_List_Data) return; _Regulatory_List_Data = value; _Regulatory_List_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegulatory_List(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Regulatory_List_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Regulatory_List_Device_Info_enum, "Regulatory_List", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|BodyType U8|HEX|BodyStructure STRING|ASCII|Data";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Regulatory_List_BodyType = parseResult.ValueList.GetValue("BodyType").AsDouble;
            Regulatory_List_BodyStructure = parseResult.ValueList.GetValue("BodyStructure").AsDouble;
            Regulatory_List_Data = parseResult.ValueList.GetValue("Data").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _PnP_ID = "";
        private bool _PnP_ID_set = false;
        public string PnP_ID
        {
            get { return _PnP_ID; }
            internal set { if (_PnP_ID_set && value == _PnP_ID) return; _PnP_ID = value; _PnP_ID_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPnP_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum, "PnP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            PnP_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _IR_Data_ObjTemp = 0;
        private bool _IR_Data_ObjTemp_set = false;
        public double IR_Data_ObjTemp
        {
            get { return _IR_Data_ObjTemp; }
            internal set { if (_IR_Data_ObjTemp_set && value == _IR_Data_ObjTemp) return; _IR_Data_ObjTemp = value; _IR_Data_ObjTemp_set = true; OnPropertyChanged(); }
        }
        private double _IR_Data_AmbientTemp = 0;
        private bool _IR_Data_AmbientTemp_set = false;
        public double IR_Data_AmbientTemp
        {
            get { return _IR_Data_AmbientTemp; }
            internal set { if (_IR_Data_AmbientTemp_set && value == _IR_Data_AmbientTemp) return; _IR_Data_AmbientTemp = value; _IR_Data_AmbientTemp_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.IR_Data_IR_Service_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.IR_Data_IR_Service_enum, "IR_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16|HEX|ObjTemp|C I16|HEX|AmbientTemp|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            IR_Data_ObjTemp = parseResult.ValueList.GetValue("ObjTemp").AsDouble;
            IR_Data_AmbientTemp = parseResult.ValueList.GetValue("AmbientTemp").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; IR_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent IR_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyIR_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyIR_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.IR_Data_IR_Service_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.IR_Data_IR_Service_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyIR_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyIR_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyIR_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyIR_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyIR_Data: set notification", result);

            return true;
        }

        private void NotifyIR_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I16|HEX|ObjTemp|C I16|HEX|AmbientTemp|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            IR_Data_ObjTemp = parseResult.ValueList.GetValue("ObjTemp").AsDouble;
            IR_Data_AmbientTemp = parseResult.ValueList.GetValue("AmbientTemp").AsDouble;

            IR_DataEvent?.Invoke(parseResult);

        }

        public void NotifyIR_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[14];
            if (ch == null) return;
            NotifyIR_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyIR_DataCallback;
        }



        private double _IR_Service_Configure = 0;
        private bool _IR_Service_Configure_set = false;
        public double IR_Service_Configure
        {
            get { return _IR_Service_Configure; }
            internal set { if (_IR_Service_Configure_set && value == _IR_Service_Configure) return; _IR_Service_Configure = value; _IR_Service_Configure_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR_Service_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.IR_Service_Configure_IR_Service_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.IR_Service_Configure_IR_Service_enum, "IR_Service_Configure", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|IRConfigure";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            IR_Service_Configure = parseResult.ValueList.GetValue("IRConfigure").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for IR_Service_Configure
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteIR_Service_Configure(byte IRConfigure)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.IR_Service_Configure_IR_Service_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  IRConfigure);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.IR_Service_Configure_IR_Service_enum, "IR_Service_Configure", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.IR_Service_Configure_IR_Service_enum, "IR_Service_Configure", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _IR_Service_Period = 0;
        private bool _IR_Service_Period_set = false;
        public double IR_Service_Period
        {
            get { return _IR_Service_Period; }
            internal set { if (_IR_Service_Period_set && value == _IR_Service_Period) return; _IR_Service_Period = value; _IR_Service_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR_Service_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.IR_Service_Period_IR_Service_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.IR_Service_Period_IR_Service_enum, "IR_Service_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|IRPeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            IR_Service_Period = parseResult.ValueList.GetValue("IRPeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for IR_Service_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteIR_Service_Period(byte IRPeriod)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.IR_Service_Period_IR_Service_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  IRPeriod);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.IR_Service_Period_IR_Service_enum, "IR_Service_Period", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.IR_Service_Period_IR_Service_enum, "IR_Service_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Accelerometer_Data_AccelX = 0;
        private bool _Accelerometer_Data_AccelX_set = false;
        public double Accelerometer_Data_AccelX
        {
            get { return _Accelerometer_Data_AccelX; }
            internal set { if (_Accelerometer_Data_AccelX_set && value == _Accelerometer_Data_AccelX) return; _Accelerometer_Data_AccelX = value; _Accelerometer_Data_AccelX_set = true; OnPropertyChanged(); }
        }
        private double _Accelerometer_Data_AccelY = 0;
        private bool _Accelerometer_Data_AccelY_set = false;
        public double Accelerometer_Data_AccelY
        {
            get { return _Accelerometer_Data_AccelY; }
            internal set { if (_Accelerometer_Data_AccelY_set && value == _Accelerometer_Data_AccelY) return; _Accelerometer_Data_AccelY = value; _Accelerometer_Data_AccelY_set = true; OnPropertyChanged(); }
        }
        private double _Accelerometer_Data_AccelZ = 0;
        private bool _Accelerometer_Data_AccelZ_set = false;
        public double Accelerometer_Data_AccelZ
        {
            get { return _Accelerometer_Data_AccelZ; }
            internal set { if (_Accelerometer_Data_AccelZ_set && value == _Accelerometer_Data_AccelZ) return; _Accelerometer_Data_AccelZ = value; _Accelerometer_Data_AccelZ_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum, "Accelerometer_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I8^64_/|FIXED|AccelX|g I8^64_/|FIXED|AccelY|g I8^64_/_IV|FIXED|AccelZ|g";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Accelerometer_Data_AccelX = parseResult.ValueList.GetValue("AccelX").AsDouble;
            Accelerometer_Data_AccelY = parseResult.ValueList.GetValue("AccelY").AsDouble;
            Accelerometer_Data_AccelZ = parseResult.ValueList.GetValue("AccelZ").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Accelerometer_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Accelerometer_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyAccelerometer_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyAccelerometer_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyAccelerometer_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyAccelerometer_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyAccelerometer_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyAccelerometer_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyAccelerometer_Data: set notification", result);

            return true;
        }

        private void NotifyAccelerometer_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I8^64_/|FIXED|AccelX|g I8^64_/|FIXED|AccelY|g I8^64_/_IV|FIXED|AccelZ|g";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Accelerometer_Data_AccelX = parseResult.ValueList.GetValue("AccelX").AsDouble;
            Accelerometer_Data_AccelY = parseResult.ValueList.GetValue("AccelY").AsDouble;
            Accelerometer_Data_AccelZ = parseResult.ValueList.GetValue("AccelZ").AsDouble;

            Accelerometer_DataEvent?.Invoke(parseResult);

        }

        public void NotifyAccelerometer_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[17];
            if (ch == null) return;
            NotifyAccelerometer_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyAccelerometer_DataCallback;
        }



        private double _Accelerometer_Configure = 0;
        private bool _Accelerometer_Configure_set = false;
        public double Accelerometer_Configure
        {
            get { return _Accelerometer_Configure; }
            internal set { if (_Accelerometer_Configure_set && value == _Accelerometer_Configure) return; _Accelerometer_Configure = value; _Accelerometer_Configure_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Configure_Accelerometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Accelerometer_Configure_Accelerometer_enum, "Accelerometer_Configure", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|AccelerometerConfigure";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Accelerometer_Configure = parseResult.ValueList.GetValue("AccelerometerConfigure").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Accelerometer_Configure
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccelerometer_Configure(byte AccelerometerConfigure)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Configure_Accelerometer_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  AccelerometerConfigure);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Configure_Accelerometer_enum, "Accelerometer_Configure", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Configure_Accelerometer_enum, "Accelerometer_Configure", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Accelerometer_Period = 0;
        private bool _Accelerometer_Period_set = false;
        public double Accelerometer_Period
        {
            get { return _Accelerometer_Period; }
            internal set { if (_Accelerometer_Period_set && value == _Accelerometer_Period) return; _Accelerometer_Period = value; _Accelerometer_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Period_Accelerometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Accelerometer_Period_Accelerometer_enum, "Accelerometer_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|AccelerometerPeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Accelerometer_Period = parseResult.ValueList.GetValue("AccelerometerPeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Accelerometer_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccelerometer_Period(byte AccelerometerPeriod)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Period_Accelerometer_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  AccelerometerPeriod);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Period_Accelerometer_enum, "Accelerometer_Period", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Period_Accelerometer_enum, "Accelerometer_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Humidity_Data_Temp = 0;
        private bool _Humidity_Data_Temp_set = false;
        public double Humidity_Data_Temp
        {
            get { return _Humidity_Data_Temp; }
            internal set { if (_Humidity_Data_Temp_set && value == _Humidity_Data_Temp) return; _Humidity_Data_Temp = value; _Humidity_Data_Temp_set = true; OnPropertyChanged(); }
        }
        private double _Humidity_Data_Humidity = 0;
        private bool _Humidity_Data_Humidity_set = false;
        public double Humidity_Data_Humidity
        {
            get { return _Humidity_Data_Humidity; }
            internal set { if (_Humidity_Data_Humidity_set && value == _Humidity_Data_Humidity) return; _Humidity_Data_Humidity = value; _Humidity_Data_Humidity_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Humidity_Data_Humidity_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Humidity_Data_Humidity_enum, "Humidity_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U16^175.72_*_65536_/_46.86_-|FIXED|Temp U16^125.0_*_65536_/_6.0_-|FIXED|Humidity";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Humidity_Data_Temp = parseResult.ValueList.GetValue("Temp").AsDouble;
            Humidity_Data_Humidity = parseResult.ValueList.GetValue("Humidity").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Humidity_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Humidity_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyHumidity_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyHumidity_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Humidity_Data_Humidity_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Humidity_Data_Humidity_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyHumidity_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyHumidity_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyHumidity_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyHumidity_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyHumidity_Data: set notification", result);

            return true;
        }

        private void NotifyHumidity_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U16^175.72_*_65536_/_46.86_-|FIXED|Temp U16^125.0_*_65536_/_6.0_-|FIXED|Humidity";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Humidity_Data_Temp = parseResult.ValueList.GetValue("Temp").AsDouble;
            Humidity_Data_Humidity = parseResult.ValueList.GetValue("Humidity").AsDouble;

            Humidity_DataEvent?.Invoke(parseResult);

        }

        public void NotifyHumidity_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[20];
            if (ch == null) return;
            NotifyHumidity_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyHumidity_DataCallback;
        }



        private double _Humidity_Configure = 0;
        private bool _Humidity_Configure_set = false;
        public double Humidity_Configure
        {
            get { return _Humidity_Configure; }
            internal set { if (_Humidity_Configure_set && value == _Humidity_Configure) return; _Humidity_Configure = value; _Humidity_Configure_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Humidity_Configure_Humidity_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Humidity_Configure_Humidity_enum, "Humidity_Configure", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|HumidityConfigure";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Humidity_Configure = parseResult.ValueList.GetValue("HumidityConfigure").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Humidity_Configure
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteHumidity_Configure(byte HumidityConfigure)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Humidity_Configure_Humidity_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  HumidityConfigure);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Humidity_Configure_Humidity_enum, "Humidity_Configure", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Humidity_Configure_Humidity_enum, "Humidity_Configure", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Humidity_Period = 0;
        private bool _Humidity_Period_set = false;
        public double Humidity_Period
        {
            get { return _Humidity_Period; }
            internal set { if (_Humidity_Period_set && value == _Humidity_Period) return; _Humidity_Period = value; _Humidity_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Humidity_Period_Humidity_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Humidity_Period_Humidity_enum, "Humidity_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|HumidityPeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Humidity_Period = parseResult.ValueList.GetValue("HumidityPeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Humidity_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteHumidity_Period(byte HumidityPeriod)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Humidity_Period_Humidity_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  HumidityPeriod);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Humidity_Period_Humidity_enum, "Humidity_Period", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Humidity_Period_Humidity_enum, "Humidity_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Magnetometer_Data_X = 0;
        private bool _Magnetometer_Data_X_set = false;
        public double Magnetometer_Data_X
        {
            get { return _Magnetometer_Data_X; }
            internal set { if (_Magnetometer_Data_X_set && value == _Magnetometer_Data_X) return; _Magnetometer_Data_X = value; _Magnetometer_Data_X_set = true; OnPropertyChanged(); }
        }
        private double _Magnetometer_Data_Y = 0;
        private bool _Magnetometer_Data_Y_set = false;
        public double Magnetometer_Data_Y
        {
            get { return _Magnetometer_Data_Y; }
            internal set { if (_Magnetometer_Data_Y_set && value == _Magnetometer_Data_Y) return; _Magnetometer_Data_Y = value; _Magnetometer_Data_Y_set = true; OnPropertyChanged(); }
        }
        private double _Magnetometer_Data_Z = 0;
        private bool _Magnetometer_Data_Z_set = false;
        public double Magnetometer_Data_Z
        {
            get { return _Magnetometer_Data_Z; }
            internal set { if (_Magnetometer_Data_Z_set && value == _Magnetometer_Data_Z) return; _Magnetometer_Data_Z = value; _Magnetometer_Data_Z_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMagnetometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Magnetometer_Data_Magnetometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Magnetometer_Data_Magnetometer_enum, "Magnetometer_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16^2000_*_65536_/_IV|FIXED|X I16^2000_*_65536_/_IV|FIXED|Y I16^2000_*_65536_/|FIXED|Z";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Magnetometer_Data_X = parseResult.ValueList.GetValue("X").AsDouble;
            Magnetometer_Data_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Magnetometer_Data_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Magnetometer_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Magnetometer_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMagnetometer_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMagnetometer_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Magnetometer_Data_Magnetometer_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Magnetometer_Data_Magnetometer_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMagnetometer_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMagnetometer_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyMagnetometer_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMagnetometer_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMagnetometer_Data: set notification", result);

            return true;
        }

        private void NotifyMagnetometer_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I16^2000_*_65536_/_IV|FIXED|X I16^2000_*_65536_/_IV|FIXED|Y I16^2000_*_65536_/|FIXED|Z";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Magnetometer_Data_X = parseResult.ValueList.GetValue("X").AsDouble;
            Magnetometer_Data_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Magnetometer_Data_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            Magnetometer_DataEvent?.Invoke(parseResult);

        }

        public void NotifyMagnetometer_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[23];
            if (ch == null) return;
            NotifyMagnetometer_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyMagnetometer_DataCallback;
        }



        private double _Magnetometer_Configure = 0;
        private bool _Magnetometer_Configure_set = false;
        public double Magnetometer_Configure
        {
            get { return _Magnetometer_Configure; }
            internal set { if (_Magnetometer_Configure_set && value == _Magnetometer_Configure) return; _Magnetometer_Configure = value; _Magnetometer_Configure_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMagnetometer_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Magnetometer_Configure_Magnetometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Magnetometer_Configure_Magnetometer_enum, "Magnetometer_Configure", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|MagnetometerConfigure";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Magnetometer_Configure = parseResult.ValueList.GetValue("MagnetometerConfigure").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Magnetometer_Configure
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMagnetometer_Configure(byte MagnetometerConfigure)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Magnetometer_Configure_Magnetometer_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  MagnetometerConfigure);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Magnetometer_Configure_Magnetometer_enum, "Magnetometer_Configure", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Magnetometer_Configure_Magnetometer_enum, "Magnetometer_Configure", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Magnetometer_Period = 0;
        private bool _Magnetometer_Period_set = false;
        public double Magnetometer_Period
        {
            get { return _Magnetometer_Period; }
            internal set { if (_Magnetometer_Period_set && value == _Magnetometer_Period) return; _Magnetometer_Period = value; _Magnetometer_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMagnetometer_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Magnetometer_Period_Magnetometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Magnetometer_Period_Magnetometer_enum, "Magnetometer_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|MagnetometerPeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Magnetometer_Period = parseResult.ValueList.GetValue("MagnetometerPeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Magnetometer_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMagnetometer_Period(byte MagnetometerPeriod)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Magnetometer_Period_Magnetometer_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  MagnetometerPeriod);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Magnetometer_Period_Magnetometer_enum, "Magnetometer_Period", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Magnetometer_Period_Magnetometer_enum, "Magnetometer_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Barometer_Data_TempRaw = 0;
        private bool _Barometer_Data_TempRaw_set = false;
        public double Barometer_Data_TempRaw
        {
            get { return _Barometer_Data_TempRaw; }
            internal set { if (_Barometer_Data_TempRaw_set && value == _Barometer_Data_TempRaw) return; _Barometer_Data_TempRaw = value; _Barometer_Data_TempRaw_set = true; OnPropertyChanged(); }
        }
        private double _Barometer_Data_PressureRaw = 0;
        private bool _Barometer_Data_PressureRaw_set = false;
        public double Barometer_Data_PressureRaw
        {
            get { return _Barometer_Data_PressureRaw; }
            internal set { if (_Barometer_Data_PressureRaw_set && value == _Barometer_Data_PressureRaw) return; _Barometer_Data_PressureRaw = value; _Barometer_Data_PressureRaw_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Barometer_Data_Barometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Barometer_Data_Barometer_enum, "Barometer_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|HEX|TempRaw U16|HEX|PressureRaw";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Barometer_Data_TempRaw = parseResult.ValueList.GetValue("TempRaw").AsDouble;
            Barometer_Data_PressureRaw = parseResult.ValueList.GetValue("PressureRaw").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Barometer_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Barometer_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyBarometer_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyBarometer_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Barometer_Data_Barometer_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Barometer_Data_Barometer_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyBarometer_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyBarometer_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyBarometer_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyBarometer_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyBarometer_Data: set notification", result);

            return true;
        }

        private void NotifyBarometer_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U16|HEX|TempRaw U16|HEX|PressureRaw";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Barometer_Data_TempRaw = parseResult.ValueList.GetValue("TempRaw").AsDouble;
            Barometer_Data_PressureRaw = parseResult.ValueList.GetValue("PressureRaw").AsDouble;

            Barometer_DataEvent?.Invoke(parseResult);

        }

        public void NotifyBarometer_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[26];
            if (ch == null) return;
            NotifyBarometer_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBarometer_DataCallback;
        }



        private double _Barometer_Configure = 0;
        private bool _Barometer_Configure_set = false;
        public double Barometer_Configure
        {
            get { return _Barometer_Configure; }
            internal set { if (_Barometer_Configure_set && value == _Barometer_Configure) return; _Barometer_Configure = value; _Barometer_Configure_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Barometer_Configure_Barometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Barometer_Configure_Barometer_enum, "Barometer_Configure", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|BarometerConfigure";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Barometer_Configure = parseResult.ValueList.GetValue("BarometerConfigure").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Barometer_Configure
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteBarometer_Configure(byte BarometerConfigure)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Barometer_Configure_Barometer_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  BarometerConfigure);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Barometer_Configure_Barometer_enum, "Barometer_Configure", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Barometer_Configure_Barometer_enum, "Barometer_Configure", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _Barometer_Calibration = null;
        private bool _Barometer_Calibration_set = false;
        public string Barometer_Calibration
        {
            get { return _Barometer_Calibration; }
            internal set { if (_Barometer_Calibration_set && value == _Barometer_Calibration) return; _Barometer_Calibration = value; _Barometer_Calibration_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer_Calibration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Barometer_Calibration_Barometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Barometer_Calibration_Barometer_enum, "Barometer_Calibration", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|BarometerCalibration";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Barometer_Calibration = parseResult.ValueList.GetValue("BarometerCalibration").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Barometer_Period = 0;
        private bool _Barometer_Period_set = false;
        public double Barometer_Period
        {
            get { return _Barometer_Period; }
            internal set { if (_Barometer_Period_set && value == _Barometer_Period) return; _Barometer_Period = value; _Barometer_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Barometer_Period_Barometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Barometer_Period_Barometer_enum, "Barometer_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|BarometerPeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Barometer_Period = parseResult.ValueList.GetValue("BarometerPeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Gyroscope_Data_X = 0;
        private bool _Gyroscope_Data_X_set = false;
        public double Gyroscope_Data_X
        {
            get { return _Gyroscope_Data_X; }
            internal set { if (_Gyroscope_Data_X_set && value == _Gyroscope_Data_X) return; _Gyroscope_Data_X = value; _Gyroscope_Data_X_set = true; OnPropertyChanged(); }
        }
        private double _Gyroscope_Data_Y = 0;
        private bool _Gyroscope_Data_Y_set = false;
        public double Gyroscope_Data_Y
        {
            get { return _Gyroscope_Data_Y; }
            internal set { if (_Gyroscope_Data_Y_set && value == _Gyroscope_Data_Y) return; _Gyroscope_Data_Y = value; _Gyroscope_Data_Y_set = true; OnPropertyChanged(); }
        }
        private double _Gyroscope_Data_Z = 0;
        private bool _Gyroscope_Data_Z_set = false;
        public double Gyroscope_Data_Z
        {
            get { return _Gyroscope_Data_Z; }
            internal set { if (_Gyroscope_Data_Z_set && value == _Gyroscope_Data_Z) return; _Gyroscope_Data_Z = value; _Gyroscope_Data_Z_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadGyroscope_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Gyroscope_Data_Gyroscope_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Gyroscope_Data_Gyroscope_enum, "Gyroscope_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16^500_*_65536_/_IV|FIXED|X I16^500_*_65536_/|FIXED|Y I16^500_*_65536_/|FIXED|Z";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Gyroscope_Data_X = parseResult.ValueList.GetValue("X").AsDouble;
            Gyroscope_Data_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Gyroscope_Data_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Gyroscope_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Gyroscope_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyGyroscope_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyGyroscope_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Gyroscope_Data_Gyroscope_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Gyroscope_Data_Gyroscope_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyGyroscope_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyGyroscope_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyGyroscope_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyGyroscope_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyGyroscope_Data: set notification", result);

            return true;
        }

        private void NotifyGyroscope_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I16^500_*_65536_/_IV|FIXED|X I16^500_*_65536_/|FIXED|Y I16^500_*_65536_/|FIXED|Z";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Gyroscope_Data_X = parseResult.ValueList.GetValue("X").AsDouble;
            Gyroscope_Data_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Gyroscope_Data_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            Gyroscope_DataEvent?.Invoke(parseResult);

        }

        public void NotifyGyroscope_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[30];
            if (ch == null) return;
            NotifyGyroscope_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyGyroscope_DataCallback;
        }



        private double _Gyroscope_Configure = 0;
        private bool _Gyroscope_Configure_set = false;
        public double Gyroscope_Configure
        {
            get { return _Gyroscope_Configure; }
            internal set { if (_Gyroscope_Configure_set && value == _Gyroscope_Configure) return; _Gyroscope_Configure = value; _Gyroscope_Configure_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadGyroscope_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Gyroscope_Configure_Gyroscope_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Gyroscope_Configure_Gyroscope_enum, "Gyroscope_Configure", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|GyroscopeConfigure";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Gyroscope_Configure = parseResult.ValueList.GetValue("GyroscopeConfigure").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Gyroscope_Configure
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteGyroscope_Configure(byte GyroscopeConfigure)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Gyroscope_Configure_Gyroscope_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  GyroscopeConfigure);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Gyroscope_Configure_Gyroscope_enum, "Gyroscope_Configure", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Gyroscope_Configure_Gyroscope_enum, "Gyroscope_Configure", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Gyroscope_Period = 0;
        private bool _Gyroscope_Period_set = false;
        public double Gyroscope_Period
        {
            get { return _Gyroscope_Period; }
            internal set { if (_Gyroscope_Period_set && value == _Gyroscope_Period) return; _Gyroscope_Period = value; _Gyroscope_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadGyroscope_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Gyroscope_Period_Gyroscope_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Gyroscope_Period_Gyroscope_enum, "Gyroscope_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|GyroscopePeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Gyroscope_Period = parseResult.ValueList.GetValue("GyroscopePeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Gyroscope_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteGyroscope_Period(byte GyroscopePeriod)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Gyroscope_Period_Gyroscope_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  GyroscopePeriod);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Gyroscope_Period_Gyroscope_enum, "Gyroscope_Period", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Gyroscope_Period_Gyroscope_enum, "Gyroscope_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Key_Press_State = 0;
        private bool _Key_Press_State_set = false;
        public double Key_Press_State
        {
            get { return _Key_Press_State; }
            internal set { if (_Key_Press_State_set && value == _Key_Press_State) return; _Key_Press_State = value; _Key_Press_State_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Key_Press_StateEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Key_Press_StateEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyKey_Press_State_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyKey_Press_StateAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Key_Press_State_Key_Press_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Key_Press_State_Key_Press_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyKey_Press_State_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyKey_Press_State_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyKey_Press_StateCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyKey_Press_State: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyKey_Press_State: set notification", result);

            return true;
        }

        private void NotifyKey_Press_StateCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX|KeyPressState";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Key_Press_State = parseResult.ValueList.GetValue("KeyPressState").AsDouble;

            Key_Press_StateEvent?.Invoke(parseResult);

        }

        public void NotifyKey_Press_StateRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[33];
            if (ch == null) return;
            NotifyKey_Press_State_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKey_Press_StateCallback;
        }




    }
}