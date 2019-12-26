using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothProtocols
{
    /// <summary>
    /// The TI 1352 is the latest 2019 version in the TI range of Sensor. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth.
    /// This class was automatically generated 12/26/2019 11:55 AM
    /// </summary>

    public partial class TI_SensorTag_1352 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // [[LINKS]]TODO: create LINKS

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
            Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("f000ffc0-0451-4000-b000-000000000000"),
            Guid.Parse("f000aa00-0451-4000-b000-000000000000"),
            Guid.Parse("f000aa20-0451-4000-b000-000000000000"),
            Guid.Parse("f000aa70-0451-4000-b000-000000000000"),
            Guid.Parse("f0001110-0451-4000-b000-000000000000"),
            Guid.Parse("f0001120-0451-4000-b000-000000000000"),
            Guid.Parse("f000180f-0451-4000-b000-000000000000"),
            Guid.Parse("f000ffa0-0451-4000-b000-000000000000"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Generic Service",
            "Device Info",
            "Imaging",
            "Temperature",
            "Humidity",
            "Optical Service",
            "LED",
            "Button",
            "Battery",
            "Accelerometer",

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
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #3 is Central Address Resolution
            Guid.Parse("00002ac9-0000-1000-8000-00805f9b34fb"), // #4 is Resolvable Private Address Only
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #0 is System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #2 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #3 is Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #4 is Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #5 is Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #6 is Manuafacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #7 is Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #8 is PnP ID
            Guid.Parse("f000ffc1-0451-4000-b000-000000000000"), // #0 is Img Identify
            Guid.Parse("f000ffc2-0451-4000-b000-000000000000"), // #1 is Img Block
            Guid.Parse("f000ffc5-0451-4000-b000-000000000000"), // #2 is OAD Extended Control
            Guid.Parse("f000aa01-0451-4000-b000-000000000000"), // #0 is Temperature Data
            Guid.Parse("f000aa02-0451-4000-b000-000000000000"), // #1 is Temperature Conf.
            Guid.Parse("f000aa03-0451-4000-b000-000000000000"), // #2 is Temperature Period
            Guid.Parse("f000aa21-0451-4000-b000-000000000000"), // #0 is Humidity Data
            Guid.Parse("f000aa22-0451-4000-b000-000000000000"), // #1 is Humidity Conf.
            Guid.Parse("f000aa23-0451-4000-b000-000000000000"), // #2 is Humidity Period
            Guid.Parse("f000aa71-0451-4000-b000-000000000000"), // #0 is Light Data
            Guid.Parse("f000aa72-0451-4000-b000-000000000000"), // #1 is Light Conf.
            Guid.Parse("f000aa73-0451-4000-b000-000000000000"), // #2 is Light Period
            Guid.Parse("f0001111-0451-4000-b000-000000000000"), // #0 is Red
            Guid.Parse("f0001112-0451-4000-b000-000000000000"), // #1 is Green
            Guid.Parse("f0001113-0451-4000-b000-000000000000"), // #2 is Blue
            Guid.Parse("f0001121-0451-4000-b000-000000000000"), // #0 is Button 0
            Guid.Parse("f0001122-0451-4000-b000-000000000000"), // #1 is Button 1
            Guid.Parse("f0002a19-0451-4000-b000-000000000000"), // #0 is Battery Data
            Guid.Parse("f000ffa1-0451-4000-b000-000000000000"), // #0 is Accel Enable
            Guid.Parse("f000ffa2-0451-4000-b000-000000000000"), // #1 is Accel Range
            Guid.Parse("f000ffa3-0451-4000-b000-000000000000"), // #2 is X
            Guid.Parse("f000ffa4-0451-4000-b000-000000000000"), // #3 is Y
            Guid.Parse("f000ffa5-0451-4000-b000-000000000000"), // #4 is Z

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "Central Address Resolution", // #3 is 00002aa6-0000-1000-8000-00805f9b34fb
            "Resolvable Private Address Only", // #4 is 00002ac9-0000-1000-8000-00805f9b34fb
            "System ID", // #0 is 00002a23-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #2 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #3 is 00002a26-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #4 is 00002a27-0000-1000-8000-00805f9b34fb
            "Software Revision", // #5 is 00002a28-0000-1000-8000-00805f9b34fb
            "Manuafacturer Name", // #6 is 00002a29-0000-1000-8000-00805f9b34fb
            "Regulatory List", // #7 is 00002a2a-0000-1000-8000-00805f9b34fb
            "PnP ID", // #8 is 00002a50-0000-1000-8000-00805f9b34fb
            "Img Identify", // #0 is f000ffc1-0451-4000-b000-000000000000
            "Img Block", // #1 is f000ffc2-0451-4000-b000-000000000000
            "OAD Extended Control", // #2 is f000ffc5-0451-4000-b000-000000000000
            "Temperature Data", // #0 is f000aa01-0451-4000-b000-000000000000
            "Temperature Conf.", // #1 is f000aa02-0451-4000-b000-000000000000
            "Temperature Period", // #2 is f000aa03-0451-4000-b000-000000000000
            "Humidity Data", // #0 is f000aa21-0451-4000-b000-000000000000
            "Humidity Conf.", // #1 is f000aa22-0451-4000-b000-000000000000
            "Humidity Period", // #2 is f000aa23-0451-4000-b000-000000000000
            "Light Data", // #0 is f000aa71-0451-4000-b000-000000000000
            "Light Conf.", // #1 is f000aa72-0451-4000-b000-000000000000
            "Light Period", // #2 is f000aa73-0451-4000-b000-000000000000
            "Red", // #0 is f0001111-0451-4000-b000-000000000000
            "Green", // #1 is f0001112-0451-4000-b000-000000000000
            "Blue", // #2 is f0001113-0451-4000-b000-000000000000
            "Button 0", // #0 is f0001121-0451-4000-b000-000000000000
            "Button 1", // #1 is f0001122-0451-4000-b000-000000000000
            "Battery Data", // #0 is f0002a19-0451-4000-b000-000000000000
            "Accel Enable", // #0 is f000ffa1-0451-4000-b000-000000000000
            "Accel Range", // #1 is f000ffa2-0451-4000-b000-000000000000
            "X", // #2 is f000ffa3-0451-4000-b000-000000000000
            "Y", // #3 is f000ffa4-0451-4000-b000-000000000000
            "Z", // #4 is f000ffa5-0451-4000-b000-000000000000

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
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3, 4,  },
            new HashSet<int>(){  },
            new HashSet<int>(){ 5, 6, 7, 8, 9, 10, 11, 12, 13,  },
            new HashSet<int>(){ 14, 15, 16,  },
            new HashSet<int>(){ 17, 18, 19,  },
            new HashSet<int>(){ 20, 21, 22,  },
            new HashSet<int>(){ 23, 24, 25,  },
            new HashSet<int>(){ 26, 27, 28,  },
            new HashSet<int>(){ 29, 30,  },
            new HashSet<int>(){ 31,  },
            new HashSet<int>(){ 32, 33, 34, 35, 36,  },

        };


        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;

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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(3, "Central_Address_Resolution", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|AddressResolutionSupported";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Central_Address_Resolution = parseResult.ValueList.GetValue("AddressResolutionSupported").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        private double _Resolvable_Private_Address_Only = 0;
        private bool _Resolvable_Private_Address_Only_set = false;
        public double Resolvable_Private_Address_Only
        {
            get { return _Resolvable_Private_Address_Only; }
            internal set { if (_Resolvable_Private_Address_Only_set && value == _Resolvable_Private_Address_Only) return; _Resolvable_Private_Address_Only = value; _Resolvable_Private_Address_Only_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadResolvable_Private_Address_Only(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(4, "Resolvable_Private_Address_Only", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|ResolvablePrivateAddressFlag";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Resolvable_Private_Address_Only = parseResult.ValueList.GetValue("ResolvablePrivateAddressFlag").AsDouble;

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
            IBuffer result = await ReadAsync(5, "System_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|SystemId";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            System_ID = parseResult.ValueList.GetValue("SystemId").AsString;

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
            IBuffer result = await ReadAsync(6, "Model_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|ModelNumber";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Model_Number = parseResult.ValueList.GetValue("ModelNumber").AsString;

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
            IBuffer result = await ReadAsync(7, "Serial_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|SerialNumber";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Serial_Number = parseResult.ValueList.GetValue("SerialNumber").AsString;

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
            IBuffer result = await ReadAsync(8, "Firmware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|FirmwareRevision";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Firmware_Revision = parseResult.ValueList.GetValue("FirmwareRevision").AsString;

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
            IBuffer result = await ReadAsync(9, "Hardware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|HardwareRevision";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Hardware_Revision = parseResult.ValueList.GetValue("HardwareRevision").AsString;

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
            IBuffer result = await ReadAsync(10, "Software_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|SoftwareRevision";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Software_Revision = parseResult.ValueList.GetValue("SoftwareRevision").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        private string _Manuafacturer_Name = "";
        private bool _Manuafacturer_Name_set = false;
        public string Manuafacturer_Name
        {
            get { return _Manuafacturer_Name; }
            internal set { if (_Manuafacturer_Name_set && value == _Manuafacturer_Name) return; _Manuafacturer_Name = value; _Manuafacturer_Name_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadManuafacturer_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(11, "Manuafacturer_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|ManufacturerName";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Manuafacturer_Name = parseResult.ValueList.GetValue("ManufacturerName").AsString;

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
            IBuffer result = await ReadAsync(12, "Regulatory_List", cacheMode);
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
            IBuffer result = await ReadAsync(13, "PnP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|PnPID";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            PnP_ID = parseResult.ValueList.GetValue("PnPID").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        private string _Img_Identify = null;
        private bool _Img_Identify_set = false;
        public string Img_Identify
        {
            get { return _Img_Identify; }
            internal set { if (_Img_Identify_set && value == _Img_Identify) return; _Img_Identify = value; _Img_Identify_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Img_IdentifyEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Img_IdentifyEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyImg_Identify_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyImg_IdentifyAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[14];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyImg_Identify_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyImg_Identify_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|Img_Identify";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Img_Identify = parseResult.ValueList.GetValue("Img_Identify").AsString;

                        Img_IdentifyEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyImg_Identify: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyImg_Identify: set notification", result);

            return true;
        }

        /// <summary>
        /// Writes data for Img_Identify
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteImg_Identify(byte[] Img_Identify)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Img_Identify);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(14, "Img_Identify", command, GattWriteOption.WriteWithoutResponse);
        }

        private string _Img_Block = null;
        private bool _Img_Block_set = false;
        public string Img_Block
        {
            get { return _Img_Block; }
            internal set { if (_Img_Block_set && value == _Img_Block) return; _Img_Block = value; _Img_Block_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Img_BlockEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Img_BlockEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyImg_Block_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyImg_BlockAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[15];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyImg_Block_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyImg_Block_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|Img_Block";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Img_Block = parseResult.ValueList.GetValue("Img_Block").AsString;

                        Img_BlockEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyImg_Block: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyImg_Block: set notification", result);

            return true;
        }

        /// <summary>
        /// Writes data for Img_Block
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteImg_Block(byte[] Img_Block)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Img_Block);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(15, "Img_Block", command, GattWriteOption.WriteWithoutResponse);
        }

        private string _OAD_Extended_Control = null;
        private bool _OAD_Extended_Control_set = false;
        public string OAD_Extended_Control
        {
            get { return _OAD_Extended_Control; }
            internal set { if (_OAD_Extended_Control_set && value == _OAD_Extended_Control) return; _OAD_Extended_Control = value; _OAD_Extended_Control_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; OAD_Extended_ControlEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent OAD_Extended_ControlEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyOAD_Extended_Control_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyOAD_Extended_ControlAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[16];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyOAD_Extended_Control_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyOAD_Extended_Control_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|OAD_Extended_Control";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        OAD_Extended_Control = parseResult.ValueList.GetValue("OAD_Extended_Control").AsString;

                        OAD_Extended_ControlEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyOAD_Extended_Control: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyOAD_Extended_Control: set notification", result);

            return true;
        }

        /// <summary>
        /// Writes data for OAD_Extended_Control
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteOAD_Extended_Control(byte[] OAD_Extended_Control)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(OAD_Extended_Control);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(16, "OAD_Extended_Control", command, GattWriteOption.WriteWithoutResponse);
        }

        private double _Temperature_Data = 0.0;
        private bool _Temperature_Data_set = false;
        public double Temperature_Data
        {
            get { return _Temperature_Data; }
            internal set { if (_Temperature_Data_set && value == _Temperature_Data) return; _Temperature_Data = value; _Temperature_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperature_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(17, "Temperature_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "F32|FIXED|Temperature|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Temperature_Data = parseResult.ValueList.GetValue("Temperature").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Temperature_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Temperature_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyTemperature_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyTemperature_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[17];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTemperature_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTemperature_Data_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "F32|FIXED|Temperature|C";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Temperature_Data = parseResult.ValueList.GetValue("Temperature").AsDouble;

                        Temperature_DataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyTemperature_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyTemperature_Data: set notification", result);

            return true;
        }

        private double _Temperature_Conf = 0;
        private bool _Temperature_Conf_set = false;
        public double Temperature_Conf
        {
            get { return _Temperature_Conf; }
            internal set { if (_Temperature_Conf_set && value == _Temperature_Conf) return; _Temperature_Conf = value; _Temperature_Conf_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperature_Conf(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(18, "Temperature_Conf", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Temperature_Conf = parseResult.ValueList.GetValue("Enable").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Temperature_Conf
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTemperature_Conf(byte Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Enable);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(18, "Temperature_Conf", command, GattWriteOption.WriteWithResponse);
        }

        private double _Temperature_Period = 0;
        private bool _Temperature_Period_set = false;
        public double Temperature_Period
        {
            get { return _Temperature_Period; }
            internal set { if (_Temperature_Period_set && value == _Temperature_Period) return; _Temperature_Period = value; _Temperature_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperature_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(19, "Temperature_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Temperature_Period = parseResult.ValueList.GetValue("Period").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Temperature_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTemperature_Period(byte Period)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Period);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(19, "Temperature_Period", command, GattWriteOption.WriteWithResponse);
        }

        private double _Humidity_Data = 0.0;
        private bool _Humidity_Data_set = false;
        public double Humidity_Data
        {
            get { return _Humidity_Data; }
            internal set { if (_Humidity_Data_set && value == _Humidity_Data) return; _Humidity_Data = value; _Humidity_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(20, "Humidity_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "F32|FIXED|Humidty|Percent";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Humidity_Data = parseResult.ValueList.GetValue("Humidty").AsDouble;

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
            var ch = Characteristics[20];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyHumidity_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyHumidity_Data_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "F32|FIXED|Humidty|Percent";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Humidity_Data = parseResult.ValueList.GetValue("Humidty").AsDouble;

                        Humidity_DataEvent?.Invoke(parseResult);
                    };
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

        private double _Humidity_Conf = 0;
        private bool _Humidity_Conf_set = false;
        public double Humidity_Conf
        {
            get { return _Humidity_Conf; }
            internal set { if (_Humidity_Conf_set && value == _Humidity_Conf) return; _Humidity_Conf = value; _Humidity_Conf_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity_Conf(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(21, "Humidity_Conf", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Humidity_Conf = parseResult.ValueList.GetValue("Enable").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Humidity_Conf
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteHumidity_Conf(byte Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Enable);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(21, "Humidity_Conf", command, GattWriteOption.WriteWithResponse);
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
            IBuffer result = await ReadAsync(22, "Humidity_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Humidity_Period = parseResult.ValueList.GetValue("Period").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

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
            await WriteCommandAsync(22, "Humidity_Period", command, GattWriteOption.WriteWithResponse);
        }

        private double _Light_Data = 0.0;
        private bool _Light_Data_set = false;
        public double Light_Data
        {
            get { return _Light_Data; }
            internal set { if (_Light_Data_set && value == _Light_Data) return; _Light_Data = value; _Light_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLight_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(23, "Light_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "F32|FIXED^N0|Lux";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Light_Data = parseResult.ValueList.GetValue("Lux").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Light_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Light_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyLight_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyLight_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[23];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyLight_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyLight_Data_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "F32|FIXED^N0|Lux";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Light_Data = parseResult.ValueList.GetValue("Lux").AsDouble;

                        Light_DataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyLight_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyLight_Data: set notification", result);

            return true;
        }

        private string _Light_Conf = null;
        private bool _Light_Conf_set = false;
        public string Light_Conf
        {
            get { return _Light_Conf; }
            internal set { if (_Light_Conf_set && value == _Light_Conf) return; _Light_Conf = value; _Light_Conf_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLight_Conf(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(24, "Light_Conf", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Light_Conf = parseResult.ValueList.GetValue("Enable").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Light_Conf
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLight_Conf(byte[] Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Enable);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(24, "Light_Conf", command, GattWriteOption.WriteWithResponse);
        }

        private string _Light_Period = null;
        private bool _Light_Period_set = false;
        public string Light_Period
        {
            get { return _Light_Period; }
            internal set { if (_Light_Period_set && value == _Light_Period) return; _Light_Period = value; _Light_Period_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLight_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(25, "Light_Period", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Light_Period";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Light_Period = parseResult.ValueList.GetValue("Light_Period").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Light_Period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLight_Period(byte[] Light_Period)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Light_Period);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(25, "Light_Period", command, GattWriteOption.WriteWithResponse);
        }

        private double _Red = 0;
        private bool _Red_set = false;
        public double Red
        {
            get { return _Red; }
            internal set { if (_Red_set && value == _Red) return; _Red = value; _Red_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRed(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(26, "Red", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Red";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Red = parseResult.ValueList.GetValue("Red").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Red
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteRed(byte Red)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Red);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(26, "Red", command, GattWriteOption.WriteWithoutResponse);
        }

        private double _Green = 0;
        private bool _Green_set = false;
        public double Green
        {
            get { return _Green; }
            internal set { if (_Green_set && value == _Green) return; _Green = value; _Green_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadGreen(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(27, "Green", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Green";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Green = parseResult.ValueList.GetValue("Green").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Green
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteGreen(byte Green)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Green);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(27, "Green", command, GattWriteOption.WriteWithoutResponse);
        }

        private double _Blue = 0;
        private bool _Blue_set = false;
        public double Blue
        {
            get { return _Blue; }
            internal set { if (_Blue_set && value == _Blue) return; _Blue = value; _Blue_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBlue(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(28, "Blue", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Blue";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Blue = parseResult.ValueList.GetValue("Blue").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Blue
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteBlue(byte Blue)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Blue);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(28, "Blue", command, GattWriteOption.WriteWithoutResponse);
        }

        private double _Button_0 = 0;
        private bool _Button_0_set = false;
        public double Button_0
        {
            get { return _Button_0; }
            internal set { if (_Button_0_set && value == _Button_0) return; _Button_0 = value; _Button_0_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadButton_0(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(29, "Button_0", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Button0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Button_0 = parseResult.ValueList.GetValue("Button0").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Button_0Event += _my function_
        /// </summary>
        public event BluetoothDataEvent Button_0Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyButton_0_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyButton_0Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[29];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyButton_0_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyButton_0_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "U8|HEX|Button0";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Button_0 = parseResult.ValueList.GetValue("Button0").AsDouble;

                        Button_0Event?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyButton_0: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyButton_0: set notification", result);

            return true;
        }

        private double _Button_1 = 0;
        private bool _Button_1_set = false;
        public double Button_1
        {
            get { return _Button_1; }
            internal set { if (_Button_1_set && value == _Button_1) return; _Button_1 = value; _Button_1_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadButton_1(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(30, "Button_1", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Button1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Button_1 = parseResult.ValueList.GetValue("Button1").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Button_1Event += _my function_
        /// </summary>
        public event BluetoothDataEvent Button_1Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyButton_1_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyButton_1Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[30];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyButton_1_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyButton_1_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "U8|HEX|Button1";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Button_1 = parseResult.ValueList.GetValue("Button1").AsDouble;

                        Button_1Event?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyButton_1: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyButton_1: set notification", result);

            return true;
        }

        private double _Battery_Data = 0;
        private bool _Battery_Data_set = false;
        public double Battery_Data
        {
            get { return _Battery_Data; }
            internal set { if (_Battery_Data_set && value == _Battery_Data) return; _Battery_Data = value; _Battery_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBattery_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(31, "Battery_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I8|DEC|BatteryLevel|%";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Battery_Data = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Battery_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Battery_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyBattery_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyBattery_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[31];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyBattery_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyBattery_Data_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I8|DEC|BatteryLevel|%";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Battery_Data = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

                        Battery_DataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyBattery_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyBattery_Data: set notification", result);

            return true;
        }

        private string _Accel_Enable = null;
        private bool _Accel_Enable_set = false;
        public string Accel_Enable
        {
            get { return _Accel_Enable; }
            internal set { if (_Accel_Enable_set && value == _Accel_Enable) return; _Accel_Enable = value; _Accel_Enable_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccel_Enable(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(32, "Accel_Enable", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Accel_Enable = parseResult.ValueList.GetValue("Enable").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for Accel_Enable
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccel_Enable(byte[] Enable)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Enable);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(32, "Accel_Enable", command, GattWriteOption.WriteWithResponse);
        }

        private double _Accel_Range = 0;
        private bool _Accel_Range_set = false;
        public double Accel_Range
        {
            get { return _Accel_Range; }
            internal set { if (_Accel_Range_set && value == _Accel_Range) return; _Accel_Range = value; _Accel_Range_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccel_Range(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(33, "Accel_Range", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|HEX|Accel_Range";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Accel_Range = parseResult.ValueList.GetValue("Accel_Range").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        private double _X = 0;
        private bool _X_set = false;
        public double X
        {
            get { return _X; }
            internal set { if (_X_set && value == _X) return; _X = value; _X_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; XEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent XEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyX_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyXAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[34];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyX_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyX_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I16^1000_/|FIXED|AccelX";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        X = parseResult.ValueList.GetValue("AccelX").AsDouble;

                        XEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyX: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyX: set notification", result);

            return true;
        }

        private double _Y = 0;
        private bool _Y_set = false;
        public double Y
        {
            get { return _Y; }
            internal set { if (_Y_set && value == _Y) return; _Y = value; _Y_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; YEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent YEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyY_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyYAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[35];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyY_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyY_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I16^1000_/|FIXED|AccelY";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Y = parseResult.ValueList.GetValue("AccelY").AsDouble;

                        YEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyY: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyY: set notification", result);

            return true;
        }

        private double _Z = 0;
        private bool _Z_set = false;
        public double Z
        {
            get { return _Z; }
            internal set { if (_Z_set && value == _Z) return; _Z = value; _Z_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ZEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ZEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyZ_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyZAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[36];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyZ_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyZ_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I16^1000_/|FIXED|AccelZ";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Z = parseResult.ValueList.GetValue("AccelZ").AsDouble;

                        ZEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyZ: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyZ: set notification", result);

            return true;
        }

    }
}
