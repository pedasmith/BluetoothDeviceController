//From template: Protocol_Body
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
    /// The TI 1350 and 2650 are the latest in the TI range of Sensor. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth.
    /// This class was automatically generated 7/9/2022 8:17 AM
    /// </summary>

    public partial class TI_SensorTag_1350 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation


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
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("f000aa00-0451-4000-b000-000000000000"),
            Guid.Parse("f000aa20-0451-4000-b000-000000000000"),
            Guid.Parse("f000aa40-0451-4000-b000-000000000000"),
            Guid.Parse("f000aa80-0451-4000-b000-000000000000"),
            Guid.Parse("f000aa70-0451-4000-b000-000000000000"),
            Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("f000aa64-0451-4000-b000-000000000000"),
            Guid.Parse("f000ac00-0451-4000-b000-000000000000"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Device Info",
            "Battery",
            "IR Service",
            "Humidity",
            "Barometer",
            "Accelerometer",
            "Optical Service",
            "Key Press",
            "IO Service",
            "Register service",

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
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #0 is System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #2 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #3 is Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #4 is Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #5 is Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #6 is Manufacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #7 is Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #8 is PnP ID
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #0 is BatteryLevel
            Guid.Parse("f000aa01-0451-4000-b000-000000000000"), // #0 is IR Data
            Guid.Parse("f000aa02-0451-4000-b000-000000000000"), // #1 is IR Service Config
            Guid.Parse("f000aa03-0451-4000-b000-000000000000"), // #2 is IR Service Period
            Guid.Parse("f000aa21-0451-4000-b000-000000000000"), // #0 is Humidity Data
            Guid.Parse("f000aa22-0451-4000-b000-000000000000"), // #1 is Humidity Config
            Guid.Parse("f000aa23-0451-4000-b000-000000000000"), // #2 is Humidity Period
            Guid.Parse("f000aa41-0451-4000-b000-000000000000"), // #0 is Barometer Data
            Guid.Parse("f000aa42-0451-4000-b000-000000000000"), // #1 is Barometer Config
            Guid.Parse("f000aa44-0451-4000-b000-000000000000"), // #2 is Barometer Period
            Guid.Parse("f000aa81-0451-4000-b000-000000000000"), // #0 is Accelerometer Data
            Guid.Parse("f000aa82-0451-4000-b000-000000000000"), // #1 is Accelerometer Config
            Guid.Parse("f000aa83-0451-4000-b000-000000000000"), // #2 is Accelerometer Period
            Guid.Parse("f000aa71-0451-4000-b000-000000000000"), // #0 is Optical Service Data
            Guid.Parse("f000aa72-0451-4000-b000-000000000000"), // #1 is Optical Service Config
            Guid.Parse("f000aa73-0451-4000-b000-000000000000"), // #2 is Optical Service Period
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #0 is Key Press State
            Guid.Parse("f000aa65-0451-4000-b000-000000000000"), // #0 is IO Service Data
            Guid.Parse("f000aa66-0451-4000-b000-000000000000"), // #1 is IO Service Config
            Guid.Parse("f000ac01-0451-4000-b000-000000000000"), // #0 is Register Data
            Guid.Parse("f000ac02-0451-4000-b000-000000000000"), // #1 is Register Address
            Guid.Parse("f000ac03-0451-4000-b000-000000000000"), // #2 is Register Device ID

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "System ID", // #0 is 00002a23-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #2 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #3 is 00002a26-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #4 is 00002a27-0000-1000-8000-00805f9b34fb
            "Software Revision", // #5 is 00002a28-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #6 is 00002a29-0000-1000-8000-00805f9b34fb
            "Regulatory List", // #7 is 00002a2a-0000-1000-8000-00805f9b34fb
            "PnP ID", // #8 is 00002a50-0000-1000-8000-00805f9b34fb
            "BatteryLevel", // #0 is 00002a19-0000-1000-8000-00805f9b34fb
            "IR Data", // #0 is f000aa01-0451-4000-b000-000000000000
            "IR Service Config", // #1 is f000aa02-0451-4000-b000-000000000000
            "IR Service Period", // #2 is f000aa03-0451-4000-b000-000000000000
            "Humidity Data", // #0 is f000aa21-0451-4000-b000-000000000000
            "Humidity Config", // #1 is f000aa22-0451-4000-b000-000000000000
            "Humidity Period", // #2 is f000aa23-0451-4000-b000-000000000000
            "Barometer Data", // #0 is f000aa41-0451-4000-b000-000000000000
            "Barometer Config", // #1 is f000aa42-0451-4000-b000-000000000000
            "Barometer Period", // #2 is f000aa44-0451-4000-b000-000000000000
            "Accelerometer Data", // #0 is f000aa81-0451-4000-b000-000000000000
            "Accelerometer Config", // #1 is f000aa82-0451-4000-b000-000000000000
            "Accelerometer Period", // #2 is f000aa83-0451-4000-b000-000000000000
            "Optical Service Data", // #0 is f000aa71-0451-4000-b000-000000000000
            "Optical Service Config", // #1 is f000aa72-0451-4000-b000-000000000000
            "Optical Service Period", // #2 is f000aa73-0451-4000-b000-000000000000
            "Key Press State", // #0 is 0000ffe1-0000-1000-8000-00805f9b34fb
            "IO Service Data", // #0 is f000aa65-0451-4000-b000-000000000000
            "IO Service Config", // #1 is f000aa66-0451-4000-b000-000000000000
            "Register Data", // #0 is f000ac01-0451-4000-b000-000000000000
            "Register Address", // #1 is f000ac02-0451-4000-b000-000000000000
            "Register Device ID", // #2 is f000ac03-0451-4000-b000-000000000000

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
            new HashSet<int>(){ 0, 1, 2,  },
            new HashSet<int>(){ 3, 4, 5, 6, 7, 8, 9, 10, 11,  },
            new HashSet<int>(){ 12,  },
            new HashSet<int>(){ 13, 14, 15,  },
            new HashSet<int>(){ 16, 17, 18,  },
            new HashSet<int>(){ 19, 20, 21,  },
            new HashSet<int>(){ 22, 23, 24,  },
            new HashSet<int>(){ 25, 26, 27,  },
            new HashSet<int>(){ 28,  },
            new HashSet<int>(){ 29, 30,  },
            new HashSet<int>(){ 31, 32, 33,  },

        };


        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;
            if (ble == null) return false; // might not be initialized yet

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
                    foreach (var characteristicIndex in characteristicIndexSet)
                    {
                        var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[characteristicIndex]);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            Status.ReportStatus($"unable to get characteristic for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            return false;
                        }
                        if (characteristicsStatus.Characteristics.Count == 0)
                        {
                            Status.ReportStatus($"unable to get any characteristics for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else if (characteristicsStatus.Characteristics.Count != 1)
                        {
                            Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else
                        {
                            Characteristics[characteristicIndex] = characteristicsStatus.Characteristics[0];
                            lastResult = characteristicsStatus;
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
        private async Task WriteCommandAsync(int characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
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
        private async Task<IBuffer> ReadAsync(int characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[characteristicIndex].ReadValueAsync(cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(0, "Device_Name", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(1, "Appearance", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(2, "Connection_Parameter", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(3, "System_ID", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(4, "Model_Number", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(5, "Serial_Number", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(6, "Firmware_Revision", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(7, "Hardware_Revision", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(8, "Software_Revision", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(9, "Manufacturer_Name", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(10, "Regulatory_List", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(11, "PnP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            PnP_ID = parseResult.ValueList.GetValue("param0").AsString;

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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(12, "BatteryLevel", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[12];
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
            var ch = Characteristics[12];
            if (ch == null) return;
            NotifyBatteryLevel_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBatteryLevelCallback;
        }


        private double _IR_Data_ObjTemp = 0;
        private bool _IR_Data_ObjTemp_set = false;
        public double IR_Data_ObjTemp
        {
            get { return _IR_Data_ObjTemp; }
            internal set { if (_IR_Data_ObjTemp_set && value == _IR_Data_ObjTemp) return; _IR_Data_ObjTemp = value; _IR_Data_ObjTemp_set = true; OnPropertyChanged(); }
        }

        private double _IR_Data_AmbTemp = 0;
        private bool _IR_Data_AmbTemp_set = false;
        public double IR_Data_AmbTemp
        {
            get { return _IR_Data_AmbTemp; }
            internal set { if (_IR_Data_AmbTemp_set && value == _IR_Data_AmbTemp) return; _IR_Data_AmbTemp = value; _IR_Data_AmbTemp_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(13, "IR_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16^4_/_0.03125_*|FIXED|ObjTemp|C I16^4_/_0.03125_*|FIXED|AmbTemp|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            IR_Data_ObjTemp = parseResult.ValueList.GetValue("ObjTemp").AsDouble;

            IR_Data_AmbTemp = parseResult.ValueList.GetValue("AmbTemp").AsDouble;

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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[13];
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
            var datameaning = "I16^4_/_0.03125_*|FIXED|ObjTemp|C I16^4_/_0.03125_*|FIXED|AmbTemp|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            IR_Data_ObjTemp = parseResult.ValueList.GetValue("ObjTemp").AsDouble;

            IR_Data_AmbTemp = parseResult.ValueList.GetValue("AmbTemp").AsDouble;

            IR_DataEvent?.Invoke(parseResult);

        }

        public void NotifyIR_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[13];
            if (ch == null) return;
            NotifyIR_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyIR_DataCallback;
        }


        private double _IR_Service_Config = 0;
        private bool _IR_Service_Config_set = false;
        public double IR_Service_Config
        {
            get { return _IR_Service_Config; }
            internal set { if (_IR_Service_Config_set && value == _IR_Service_Config) return; _IR_Service_Config = value; _IR_Service_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR_Service_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(14, "IR_Service_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            IR_Service_Config = parseResult.ValueList.GetValue("Enable").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for IR_Service_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteIR_Service_Config(byte Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Enable);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(14, "IR_Service_Config", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(15, "IR_Service_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            IR_Service_Period = parseResult.ValueList.GetValue("Period").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for IR_Service_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteIR_Service_Period(byte Period)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Period);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(15, "IR_Service_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(16, "Humidity_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U16^DU_3_ZE_175.72_*_65536_/_46.85_-_SW_2_JZ_PO_NP|FIXED|Temp U16^DU_3_ZE_125.0_*_65536_/_6.0_-_SW_2_JZ_PO_NP|FIXED|Humidity";
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[16];
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
            var datameaning = "U16^DU_3_ZE_175.72_*_65536_/_46.85_-_SW_2_JZ_PO_NP|FIXED|Temp U16^DU_3_ZE_125.0_*_65536_/_6.0_-_SW_2_JZ_PO_NP|FIXED|Humidity";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            Humidity_Data_Temp = parseResult.ValueList.GetValue("Temp").AsDouble;

            Humidity_Data_Humidity = parseResult.ValueList.GetValue("Humidity").AsDouble;

            Humidity_DataEvent?.Invoke(parseResult);

        }

        public void NotifyHumidity_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[16];
            if (ch == null) return;
            NotifyHumidity_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyHumidity_DataCallback;
        }


        private double _Humidity_Config = 0;
        private bool _Humidity_Config_set = false;
        public double Humidity_Config
        {
            get { return _Humidity_Config; }
            internal set { if (_Humidity_Config_set && value == _Humidity_Config) return; _Humidity_Config = value; _Humidity_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(17, "Humidity_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Humidity_Config = parseResult.ValueList.GetValue("Enable").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Humidity_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteHumidity_Config(byte Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Enable);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(17, "Humidity_Config", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(18, "Humidity_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Humidity_Period = parseResult.ValueList.GetValue("Period").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Humidity_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteHumidity_Period(byte Period)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Period);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(18, "Humidity_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Barometer_Data_Temp = 0;
        private bool _Barometer_Data_Temp_set = false;
        public double Barometer_Data_Temp
        {
            get { return _Barometer_Data_Temp; }
            internal set { if (_Barometer_Data_Temp_set && value == _Barometer_Data_Temp) return; _Barometer_Data_Temp = value; _Barometer_Data_Temp_set = true; OnPropertyChanged(); }
        }

        private double _Barometer_Data_Pressure = 0;
        private bool _Barometer_Data_Pressure_set = false;
        public double Barometer_Data_Pressure
        {
            get { return _Barometer_Data_Pressure; }
            internal set { if (_Barometer_Data_Pressure_set && value == _Barometer_Data_Pressure) return; _Barometer_Data_Pressure = value; _Barometer_Data_Pressure_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(19, "Barometer_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I24^100_/|FIXED|Temp|C I24^100_/|FIXED|Pressure|hPa";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Barometer_Data_Temp = parseResult.ValueList.GetValue("Temp").AsDouble;

            Barometer_Data_Pressure = parseResult.ValueList.GetValue("Pressure").AsDouble;

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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[19];
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
            var datameaning = "I24^100_/|FIXED|Temp|C I24^100_/|FIXED|Pressure|hPa";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            Barometer_Data_Temp = parseResult.ValueList.GetValue("Temp").AsDouble;

            Barometer_Data_Pressure = parseResult.ValueList.GetValue("Pressure").AsDouble;

            Barometer_DataEvent?.Invoke(parseResult);

        }

        public void NotifyBarometer_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[19];
            if (ch == null) return;
            NotifyBarometer_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBarometer_DataCallback;
        }


        private double _Barometer_Config = 0;
        private bool _Barometer_Config_set = false;
        public double Barometer_Config
        {
            get { return _Barometer_Config; }
            internal set { if (_Barometer_Config_set && value == _Barometer_Config) return; _Barometer_Config = value; _Barometer_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(20, "Barometer_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Barometer_Config = parseResult.ValueList.GetValue("Enable").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Barometer_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteBarometer_Config(byte Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Enable);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(20, "Barometer_Config", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(21, "Barometer_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Barometer_Period = parseResult.ValueList.GetValue("Period").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Barometer_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteBarometer_Period(byte Period)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Period);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(21, "Barometer_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Accelerometer_Data_GyroX = 0;
        private bool _Accelerometer_Data_GyroX_set = false;
        public double Accelerometer_Data_GyroX
        {
            get { return _Accelerometer_Data_GyroX; }
            internal set { if (_Accelerometer_Data_GyroX_set && value == _Accelerometer_Data_GyroX) return; _Accelerometer_Data_GyroX = value; _Accelerometer_Data_GyroX_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_GyroY = 0;
        private bool _Accelerometer_Data_GyroY_set = false;
        public double Accelerometer_Data_GyroY
        {
            get { return _Accelerometer_Data_GyroY; }
            internal set { if (_Accelerometer_Data_GyroY_set && value == _Accelerometer_Data_GyroY) return; _Accelerometer_Data_GyroY = value; _Accelerometer_Data_GyroY_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_GyroZ = 0;
        private bool _Accelerometer_Data_GyroZ_set = false;
        public double Accelerometer_Data_GyroZ
        {
            get { return _Accelerometer_Data_GyroZ; }
            internal set { if (_Accelerometer_Data_GyroZ_set && value == _Accelerometer_Data_GyroZ) return; _Accelerometer_Data_GyroZ = value; _Accelerometer_Data_GyroZ_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_AccX = 0;
        private bool _Accelerometer_Data_AccX_set = false;
        public double Accelerometer_Data_AccX
        {
            get { return _Accelerometer_Data_AccX; }
            internal set { if (_Accelerometer_Data_AccX_set && value == _Accelerometer_Data_AccX) return; _Accelerometer_Data_AccX = value; _Accelerometer_Data_AccX_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_AccY = 0;
        private bool _Accelerometer_Data_AccY_set = false;
        public double Accelerometer_Data_AccY
        {
            get { return _Accelerometer_Data_AccY; }
            internal set { if (_Accelerometer_Data_AccY_set && value == _Accelerometer_Data_AccY) return; _Accelerometer_Data_AccY = value; _Accelerometer_Data_AccY_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_AccZ = 0;
        private bool _Accelerometer_Data_AccZ_set = false;
        public double Accelerometer_Data_AccZ
        {
            get { return _Accelerometer_Data_AccZ; }
            internal set { if (_Accelerometer_Data_AccZ_set && value == _Accelerometer_Data_AccZ) return; _Accelerometer_Data_AccZ = value; _Accelerometer_Data_AccZ_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_MagnetometerX = 0;
        private bool _Accelerometer_Data_MagnetometerX_set = false;
        public double Accelerometer_Data_MagnetometerX
        {
            get { return _Accelerometer_Data_MagnetometerX; }
            internal set { if (_Accelerometer_Data_MagnetometerX_set && value == _Accelerometer_Data_MagnetometerX) return; _Accelerometer_Data_MagnetometerX = value; _Accelerometer_Data_MagnetometerX_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_MagnetometerY = 0;
        private bool _Accelerometer_Data_MagnetometerY_set = false;
        public double Accelerometer_Data_MagnetometerY
        {
            get { return _Accelerometer_Data_MagnetometerY; }
            internal set { if (_Accelerometer_Data_MagnetometerY_set && value == _Accelerometer_Data_MagnetometerY) return; _Accelerometer_Data_MagnetometerY = value; _Accelerometer_Data_MagnetometerY_set = true; OnPropertyChanged(); }
        }

        private double _Accelerometer_Data_MagnetometerZ = 0;
        private bool _Accelerometer_Data_MagnetometerZ_set = false;
        public double Accelerometer_Data_MagnetometerZ
        {
            get { return _Accelerometer_Data_MagnetometerZ; }
            internal set { if (_Accelerometer_Data_MagnetometerZ_set && value == _Accelerometer_Data_MagnetometerZ) return; _Accelerometer_Data_MagnetometerZ = value; _Accelerometer_Data_MagnetometerZ_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(22, "Accelerometer_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16^500_*_65536_/|FIXED^N3|GyroX|dps I16^500_*_65536_/|FIXED^N3|GyroY|dps I16^500_*_65536_/|FIXED^N3|GyroZ|dps I16^8_*_32768_/|FIXED^N3|AccX|g I16^8_*_32768_/|FIXED^N3|AccY|g I16^8_*_32768_/|FIXED^N3|AccZ|g I16|DEC|MagnetometerX|microTesla I16|DEC|MagnetometerY|microTesla I16|DEC|MagnetometerZ|microTesla";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Accelerometer_Data_GyroX = parseResult.ValueList.GetValue("GyroX").AsDouble;

            Accelerometer_Data_GyroY = parseResult.ValueList.GetValue("GyroY").AsDouble;

            Accelerometer_Data_GyroZ = parseResult.ValueList.GetValue("GyroZ").AsDouble;

            Accelerometer_Data_AccX = parseResult.ValueList.GetValue("AccX").AsDouble;

            Accelerometer_Data_AccY = parseResult.ValueList.GetValue("AccY").AsDouble;

            Accelerometer_Data_AccZ = parseResult.ValueList.GetValue("AccZ").AsDouble;

            Accelerometer_Data_MagnetometerX = parseResult.ValueList.GetValue("MagnetometerX").AsDouble;

            Accelerometer_Data_MagnetometerY = parseResult.ValueList.GetValue("MagnetometerY").AsDouble;

            Accelerometer_Data_MagnetometerZ = parseResult.ValueList.GetValue("MagnetometerZ").AsDouble;

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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[22];
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
            var datameaning = "I16^500_*_65536_/|FIXED^N3|GyroX|dps I16^500_*_65536_/|FIXED^N3|GyroY|dps I16^500_*_65536_/|FIXED^N3|GyroZ|dps I16^8_*_32768_/|FIXED^N3|AccX|g I16^8_*_32768_/|FIXED^N3|AccY|g I16^8_*_32768_/|FIXED^N3|AccZ|g I16|DEC|MagnetometerX|microTesla I16|DEC|MagnetometerY|microTesla I16|DEC|MagnetometerZ|microTesla";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            Accelerometer_Data_GyroX = parseResult.ValueList.GetValue("GyroX").AsDouble;

            Accelerometer_Data_GyroY = parseResult.ValueList.GetValue("GyroY").AsDouble;

            Accelerometer_Data_GyroZ = parseResult.ValueList.GetValue("GyroZ").AsDouble;

            Accelerometer_Data_AccX = parseResult.ValueList.GetValue("AccX").AsDouble;

            Accelerometer_Data_AccY = parseResult.ValueList.GetValue("AccY").AsDouble;

            Accelerometer_Data_AccZ = parseResult.ValueList.GetValue("AccZ").AsDouble;

            Accelerometer_Data_MagnetometerX = parseResult.ValueList.GetValue("MagnetometerX").AsDouble;

            Accelerometer_Data_MagnetometerY = parseResult.ValueList.GetValue("MagnetometerY").AsDouble;

            Accelerometer_Data_MagnetometerZ = parseResult.ValueList.GetValue("MagnetometerZ").AsDouble;

            Accelerometer_DataEvent?.Invoke(parseResult);

        }

        public void NotifyAccelerometer_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[22];
            if (ch == null) return;
            NotifyAccelerometer_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyAccelerometer_DataCallback;
        }


        private double _Accelerometer_Config = 0;
        private bool _Accelerometer_Config_set = false;
        public double Accelerometer_Config
        {
            get { return _Accelerometer_Config; }
            internal set { if (_Accelerometer_Config_set && value == _Accelerometer_Config) return; _Accelerometer_Config = value; _Accelerometer_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(23, "Accelerometer_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Accelerometer_Config = parseResult.ValueList.GetValue("Enable").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Accelerometer_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccelerometer_Config(UInt16 Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(Enable);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(23, "Accelerometer_Config", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(24, "Accelerometer_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Accelerometer_Period = parseResult.ValueList.GetValue("Period").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Accelerometer_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccelerometer_Period(byte Period)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Period);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(24, "Accelerometer_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Optical_Service_Data = 0;
        private bool _Optical_Service_Data_set = false;
        public double Optical_Service_Data
        {
            get { return _Optical_Service_Data; }
            internal set { if (_Optical_Service_Data_set && value == _Optical_Service_Data) return; _Optical_Service_Data = value; _Optical_Service_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadOptical_Service_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(25, "Optical_Service_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U16^DU_12_RS_15_AN_2_XY_0.01_*_SW_4095_AN_*|FIXED^N1|Lux";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Optical_Service_Data = parseResult.ValueList.GetValue("Lux").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Optical_Service_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Optical_Service_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>
        
        private bool NotifyOptical_Service_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyOptical_Service_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[25];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyOptical_Service_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyOptical_Service_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyOptical_Service_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyOptical_Service_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyOptical_Service_Data: set notification", result);

            return true;
        }

        private void NotifyOptical_Service_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U16^DU_12_RS_15_AN_2_XY_0.01_*_SW_4095_AN_*|FIXED^N1|Lux";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            Optical_Service_Data = parseResult.ValueList.GetValue("Lux").AsDouble;

            Optical_Service_DataEvent?.Invoke(parseResult);

        }

        public void NotifyOptical_Service_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[25];
            if (ch == null) return;
            NotifyOptical_Service_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyOptical_Service_DataCallback;
        }


        private double _Optical_Service_Config = 0;
        private bool _Optical_Service_Config_set = false;
        public double Optical_Service_Config
        {
            get { return _Optical_Service_Config; }
            internal set { if (_Optical_Service_Config_set && value == _Optical_Service_Config) return; _Optical_Service_Config = value; _Optical_Service_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadOptical_Service_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(26, "Optical_Service_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Optical_Service_Config = parseResult.ValueList.GetValue("Enable").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Optical_Service_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteOptical_Service_Config(byte Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Enable);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(26, "Optical_Service_Config", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Optical_Service_Period = 0;
        private bool _Optical_Service_Period_set = false;
        public double Optical_Service_Period
        {
            get { return _Optical_Service_Period; }
            internal set { if (_Optical_Service_Period_set && value == _Optical_Service_Period) return; _Optical_Service_Period = value; _Optical_Service_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadOptical_Service_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(27, "Optical_Service_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Optical_Service_Period = parseResult.ValueList.GetValue("Period").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Optical_Service_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteOptical_Service_Period(byte Period)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Period);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(27, "Optical_Service_Period", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[28];
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
            var datameaning = "U8|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            Key_Press_State = parseResult.ValueList.GetValue("param0").AsDouble;

            Key_Press_StateEvent?.Invoke(parseResult);

        }

        public void NotifyKey_Press_StateRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[28];
            if (ch == null) return;
            NotifyKey_Press_State_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKey_Press_StateCallback;
        }


        private string _IO_Service_Data = null;
        private bool _IO_Service_Data_set = false;
        public string IO_Service_Data
        {
            get { return _IO_Service_Data; }
            internal set { if (_IO_Service_Data_set && value == _IO_Service_Data) return; _IO_Service_Data = value; _IO_Service_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIO_Service_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(29, "IO_Service_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            IO_Service_Data = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for IO_Service_Data
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteIO_Service_Data(byte[] param0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(param0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(29, "IO_Service_Data", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _IO_Service_Config = 0;
        private bool _IO_Service_Config_set = false;
        public double IO_Service_Config
        {
            get { return _IO_Service_Config; }
            internal set { if (_IO_Service_Config_set && value == _IO_Service_Config) return; _IO_Service_Config = value; _IO_Service_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIO_Service_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(30, "IO_Service_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            IO_Service_Config = parseResult.ValueList.GetValue("param0").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for IO_Service_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteIO_Service_Config(byte param0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(param0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(30, "IO_Service_Config", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _Register_Data = null;
        private bool _Register_Data_set = false;
        public string Register_Data
        {
            get { return _Register_Data; }
            internal set { if (_Register_Data_set && value == _Register_Data) return; _Register_Data = value; _Register_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegister_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(31, "Register_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Register_Data = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Register_Data
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteRegister_Data(byte[] param0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(param0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(31, "Register_Data", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _Register_Address = null;
        private bool _Register_Address_set = false;
        public string Register_Address
        {
            get { return _Register_Address; }
            internal set { if (_Register_Address_set && value == _Register_Address) return; _Register_Address = value; _Register_Address_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegister_Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(32, "Register_Address", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Register_Address = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Register_Address
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteRegister_Address(byte[] param0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(param0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(32, "Register_Address", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _Register_Device_ID = null;
        private bool _Register_Device_ID_set = false;
        public string Register_Device_ID
        {
            get { return _Register_Device_ID; }
            internal set { if (_Register_Device_ID_set && value == _Register_Device_ID) return; _Register_Device_ID = value; _Register_Device_ID_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegister_Device_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(33, "Register_Device_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Register_Device_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
//From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Register_Device_ID
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteRegister_Device_ID(byte[] param0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(param0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(33, "Register_Device_ID", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

    }
}
