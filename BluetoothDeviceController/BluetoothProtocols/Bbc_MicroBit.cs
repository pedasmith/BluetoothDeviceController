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
    /// .
    /// This class was automatically generated 1/4/2020 11:14 AM
    /// </summary>

    public class Bbc_MicroBit : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://lancaster-university.github.io/microbit-docs/resources/bluetooth/bluetooth_profile.html


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
            Guid.Parse("e95d93af-251d-470a-a062-fa1922dfa9a8"),
            Guid.Parse("e95d0753-251d-470a-a062-fa1922dfa9a8"),
            Guid.Parse("e95d9882-251d-470a-a062-fa1922dfa9a8"),
            Guid.Parse("e95d127b-251d-470a-a062-fa1922dfa9a8"),
            Guid.Parse("e95dd91d-251d-470a-a062-fa1922dfa9a8"),
            Guid.Parse("e95df2d8-251d-470a-a062-fa1922dfa9a8"),
            Guid.Parse("e95d6100-251d-470a-a062-fa1922dfa9a8"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Device Info",
            "EventReadWrite",
            "Accelerometer",
            "Button",
            "IOPin",
            "LED",
            "Magnetometer",
            "Temperature",

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
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #0 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #1 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #2 is Firmware Revision
            Guid.Parse("e95db84c-251d-470a-a062-fa1922dfa9a8"), // #0 is EventReadA
            Guid.Parse("e95d9775-251d-470a-a062-fa1922dfa9a8"), // #1 is EventReadB
            Guid.Parse("e95d23c4-251d-470a-a062-fa1922dfa9a8"), // #2 is EventWriteA
            Guid.Parse("e95d5404-251d-470a-a062-fa1922dfa9a8"), // #3 is EventWriteB
            Guid.Parse("e95dca4b-251d-470a-a062-fa1922dfa9a8"), // #0 is AccelerometerData
            Guid.Parse("e95dfb24-251d-470a-a062-fa1922dfa9a8"), // #1 is AccelerometerPeriod
            Guid.Parse("e95dda90-251d-470a-a062-fa1922dfa9a8"), // #0 is ButtonDataA
            Guid.Parse("e95dda91-251d-470a-a062-fa1922dfa9a8"), // #1 is ButtonDataB
            Guid.Parse("e95d5899-251d-470a-a062-fa1922dfa9a8"), // #0 is PinAnalog
            Guid.Parse("e95db9fe-251d-470a-a062-fa1922dfa9a8"), // #1 is PinInput
            Guid.Parse("e95dd822-251d-470a-a062-fa1922dfa9a8"), // #2 is PinPwm
            Guid.Parse("e95d8d00-251d-470a-a062-fa1922dfa9a8"), // #3 is PinData
            Guid.Parse("e95d7b77-251d-470a-a062-fa1922dfa9a8"), // #0 is LedPattern
            Guid.Parse("e95d93ee-251d-470a-a062-fa1922dfa9a8"), // #1 is LedText
            Guid.Parse("e95d0d2d-251d-470a-a062-fa1922dfa9a8"), // #2 is LedScrollTime
            Guid.Parse("e95dfb11-251d-470a-a062-fa1922dfa9a8"), // #0 is MagnetometerData
            Guid.Parse("e95d9715-251d-470a-a062-fa1922dfa9a8"), // #1 is MagnetometerBearing
            Guid.Parse("e95d386c-251d-470a-a062-fa1922dfa9a8"), // #2 is MagnetometerPeriod
            Guid.Parse("e95db358-251d-470a-a062-fa1922dfa9a8"), // #3 is MagnetometerCalibration
            Guid.Parse("e95d9250-251d-470a-a062-fa1922dfa9a8"), // #0 is TemperatureData
            Guid.Parse("e95d1b25-251d-470a-a062-fa1922dfa9a8"), // #1 is TemperaturePeriod

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "Model Number", // #0 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #1 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #2 is 00002a26-0000-1000-8000-00805f9b34fb
            "EventReadA", // #0 is e95db84c-251d-470a-a062-fa1922dfa9a8
            "EventReadB", // #1 is e95d9775-251d-470a-a062-fa1922dfa9a8
            "EventWriteA", // #2 is e95d23c4-251d-470a-a062-fa1922dfa9a8
            "EventWriteB", // #3 is e95d5404-251d-470a-a062-fa1922dfa9a8
            "AccelerometerData", // #0 is e95dca4b-251d-470a-a062-fa1922dfa9a8
            "AccelerometerPeriod", // #1 is e95dfb24-251d-470a-a062-fa1922dfa9a8
            "ButtonDataA", // #0 is e95dda90-251d-470a-a062-fa1922dfa9a8
            "ButtonDataB", // #1 is e95dda91-251d-470a-a062-fa1922dfa9a8
            "PinAnalog", // #0 is e95d5899-251d-470a-a062-fa1922dfa9a8
            "PinInput", // #1 is e95db9fe-251d-470a-a062-fa1922dfa9a8
            "PinPwm", // #2 is e95dd822-251d-470a-a062-fa1922dfa9a8
            "PinData", // #3 is e95d8d00-251d-470a-a062-fa1922dfa9a8
            "LedPattern", // #0 is e95d7b77-251d-470a-a062-fa1922dfa9a8
            "LedText", // #1 is e95d93ee-251d-470a-a062-fa1922dfa9a8
            "LedScrollTime", // #2 is e95d0d2d-251d-470a-a062-fa1922dfa9a8
            "MagnetometerData", // #0 is e95dfb11-251d-470a-a062-fa1922dfa9a8
            "MagnetometerBearing", // #1 is e95d9715-251d-470a-a062-fa1922dfa9a8
            "MagnetometerPeriod", // #2 is e95d386c-251d-470a-a062-fa1922dfa9a8
            "MagnetometerCalibration", // #3 is e95db358-251d-470a-a062-fa1922dfa9a8
            "TemperatureData", // #0 is e95d9250-251d-470a-a062-fa1922dfa9a8
            "TemperaturePeriod", // #1 is e95d1b25-251d-470a-a062-fa1922dfa9a8

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

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2,  },
            new HashSet<int>(){ 3, 4, 5,  },
            new HashSet<int>(){ 6, 7, 8, 9,  },
            new HashSet<int>(){ 10, 11,  },
            new HashSet<int>(){ 12, 13,  },
            new HashSet<int>(){ 14, 15, 16, 17,  },
            new HashSet<int>(){ 18, 19, 20,  },
            new HashSet<int>(){ 21, 22, 23, 24,  },
            new HashSet<int>(){ 25, 26,  },

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

        /// <summary>
        /// Writes data for Device_Name
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteDevice_Name(String Device_Name)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(Device_Name);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(0, "Device_Name", command, GattWriteOption.WriteWithResponse);
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
            IBuffer result = await ReadAsync(3, "Model_Number", cacheMode);
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
            IBuffer result = await ReadAsync(4, "Serial_Number", cacheMode);
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
            IBuffer result = await ReadAsync(5, "Firmware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Firmware_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        private double _EventReadA_EventType1 = 0;
        private bool _EventReadA_EventType1_set = false;
        public double EventReadA_EventType1
        {
            get { return _EventReadA_EventType1; }
            internal set { if (_EventReadA_EventType1_set && value == _EventReadA_EventType1) return; _EventReadA_EventType1 = value; _EventReadA_EventType1_set = true; OnPropertyChanged(); }
        }

        private double _EventReadA_EventValue1 = 0;
        private bool _EventReadA_EventValue1_set = false;
        public double EventReadA_EventValue1
        {
            get { return _EventReadA_EventValue1; }
            internal set { if (_EventReadA_EventValue1_set && value == _EventReadA_EventValue1) return; _EventReadA_EventValue1 = value; _EventReadA_EventValue1_set = true; OnPropertyChanged(); }
        }

        private double _EventReadA_EventType2 = 0;
        private bool _EventReadA_EventType2_set = false;
        public double EventReadA_EventType2
        {
            get { return _EventReadA_EventType2; }
            internal set { if (_EventReadA_EventType2_set && value == _EventReadA_EventType2) return; _EventReadA_EventType2 = value; _EventReadA_EventType2_set = true; OnPropertyChanged(); }
        }

        private double _EventReadA_EventValue2 = 0;
        private bool _EventReadA_EventValue2_set = false;
        public double EventReadA_EventValue2
        {
            get { return _EventReadA_EventValue2; }
            internal set { if (_EventReadA_EventValue2_set && value == _EventReadA_EventValue2) return; _EventReadA_EventValue2 = value; _EventReadA_EventValue2_set = true; OnPropertyChanged(); }
        }

        private double _EventReadA_EventType3 = 0;
        private bool _EventReadA_EventType3_set = false;
        public double EventReadA_EventType3
        {
            get { return _EventReadA_EventType3; }
            internal set { if (_EventReadA_EventType3_set && value == _EventReadA_EventType3) return; _EventReadA_EventType3 = value; _EventReadA_EventType3_set = true; OnPropertyChanged(); }
        }

        private double _EventReadA_EventValue3 = 0;
        private bool _EventReadA_EventValue3_set = false;
        public double EventReadA_EventValue3
        {
            get { return _EventReadA_EventValue3; }
            internal set { if (_EventReadA_EventValue3_set && value == _EventReadA_EventValue3) return; _EventReadA_EventValue3 = value; _EventReadA_EventValue3_set = true; OnPropertyChanged(); }
        }

        private double _EventReadA_EventType4 = 0;
        private bool _EventReadA_EventType4_set = false;
        public double EventReadA_EventType4
        {
            get { return _EventReadA_EventType4; }
            internal set { if (_EventReadA_EventType4_set && value == _EventReadA_EventType4) return; _EventReadA_EventType4 = value; _EventReadA_EventType4_set = true; OnPropertyChanged(); }
        }

        private double _EventReadA_EventValue4 = 0;
        private bool _EventReadA_EventValue4_set = false;
        public double EventReadA_EventValue4
        {
            get { return _EventReadA_EventValue4; }
            internal set { if (_EventReadA_EventValue4_set && value == _EventReadA_EventValue4) return; _EventReadA_EventValue4 = value; _EventReadA_EventValue4_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadEventReadA(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(6, "EventReadA", cacheMode);
            if (result == null) return null;

            var datameaning = "OOPT U16|HEX|EventType1 U16|HEX|EventValue1 U16|HEX|EventType2 U16|HEX|EventValue2 U16|HEX|EventType3 U16|HEX|EventValue3 U16|HEX|EventType4 U16|HEX|EventValue4";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            EventReadA_EventType1 = parseResult.ValueList.GetValue("EventType1").AsDouble;

            EventReadA_EventValue1 = parseResult.ValueList.GetValue("EventValue1").AsDouble;

            EventReadA_EventType2 = parseResult.ValueList.GetValue("EventType2").AsDouble;

            EventReadA_EventValue2 = parseResult.ValueList.GetValue("EventValue2").AsDouble;

            EventReadA_EventType3 = parseResult.ValueList.GetValue("EventType3").AsDouble;

            EventReadA_EventValue3 = parseResult.ValueList.GetValue("EventValue3").AsDouble;

            EventReadA_EventType4 = parseResult.ValueList.GetValue("EventType4").AsDouble;

            EventReadA_EventValue4 = parseResult.ValueList.GetValue("EventValue4").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; EventReadAEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent EventReadAEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyEventReadA_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyEventReadAAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[6];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyEventReadA_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyEventReadA_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "OOPT U16|HEX|EventType1 U16|HEX|EventValue1 U16|HEX|EventType2 U16|HEX|EventValue2 U16|HEX|EventType3 U16|HEX|EventValue3 U16|HEX|EventType4 U16|HEX|EventValue4";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        EventReadA_EventType1 = parseResult.ValueList.GetValue("EventType1").AsDouble;

                        EventReadA_EventValue1 = parseResult.ValueList.GetValue("EventValue1").AsDouble;

                        EventReadA_EventType2 = parseResult.ValueList.GetValue("EventType2").AsDouble;

                        EventReadA_EventValue2 = parseResult.ValueList.GetValue("EventValue2").AsDouble;

                        EventReadA_EventType3 = parseResult.ValueList.GetValue("EventType3").AsDouble;

                        EventReadA_EventValue3 = parseResult.ValueList.GetValue("EventValue3").AsDouble;

                        EventReadA_EventType4 = parseResult.ValueList.GetValue("EventType4").AsDouble;

                        EventReadA_EventValue4 = parseResult.ValueList.GetValue("EventValue4").AsDouble;

                        EventReadAEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyEventReadA: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyEventReadA: set notification", result);

            return true;
        }

        private double _EventReadB_EventType1 = 0;
        private bool _EventReadB_EventType1_set = false;
        public double EventReadB_EventType1
        {
            get { return _EventReadB_EventType1; }
            internal set { if (_EventReadB_EventType1_set && value == _EventReadB_EventType1) return; _EventReadB_EventType1 = value; _EventReadB_EventType1_set = true; OnPropertyChanged(); }
        }

        private double _EventReadB_EventValue1 = 0;
        private bool _EventReadB_EventValue1_set = false;
        public double EventReadB_EventValue1
        {
            get { return _EventReadB_EventValue1; }
            internal set { if (_EventReadB_EventValue1_set && value == _EventReadB_EventValue1) return; _EventReadB_EventValue1 = value; _EventReadB_EventValue1_set = true; OnPropertyChanged(); }
        }

        private double _EventReadB_EventType2 = 0;
        private bool _EventReadB_EventType2_set = false;
        public double EventReadB_EventType2
        {
            get { return _EventReadB_EventType2; }
            internal set { if (_EventReadB_EventType2_set && value == _EventReadB_EventType2) return; _EventReadB_EventType2 = value; _EventReadB_EventType2_set = true; OnPropertyChanged(); }
        }

        private double _EventReadB_EventValue2 = 0;
        private bool _EventReadB_EventValue2_set = false;
        public double EventReadB_EventValue2
        {
            get { return _EventReadB_EventValue2; }
            internal set { if (_EventReadB_EventValue2_set && value == _EventReadB_EventValue2) return; _EventReadB_EventValue2 = value; _EventReadB_EventValue2_set = true; OnPropertyChanged(); }
        }

        private double _EventReadB_EventType3 = 0;
        private bool _EventReadB_EventType3_set = false;
        public double EventReadB_EventType3
        {
            get { return _EventReadB_EventType3; }
            internal set { if (_EventReadB_EventType3_set && value == _EventReadB_EventType3) return; _EventReadB_EventType3 = value; _EventReadB_EventType3_set = true; OnPropertyChanged(); }
        }

        private double _EventReadB_EventValue3 = 0;
        private bool _EventReadB_EventValue3_set = false;
        public double EventReadB_EventValue3
        {
            get { return _EventReadB_EventValue3; }
            internal set { if (_EventReadB_EventValue3_set && value == _EventReadB_EventValue3) return; _EventReadB_EventValue3 = value; _EventReadB_EventValue3_set = true; OnPropertyChanged(); }
        }

        private double _EventReadB_EventType4 = 0;
        private bool _EventReadB_EventType4_set = false;
        public double EventReadB_EventType4
        {
            get { return _EventReadB_EventType4; }
            internal set { if (_EventReadB_EventType4_set && value == _EventReadB_EventType4) return; _EventReadB_EventType4 = value; _EventReadB_EventType4_set = true; OnPropertyChanged(); }
        }

        private double _EventReadB_EventValue4 = 0;
        private bool _EventReadB_EventValue4_set = false;
        public double EventReadB_EventValue4
        {
            get { return _EventReadB_EventValue4; }
            internal set { if (_EventReadB_EventValue4_set && value == _EventReadB_EventValue4) return; _EventReadB_EventValue4 = value; _EventReadB_EventValue4_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadEventReadB(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(7, "EventReadB", cacheMode);
            if (result == null) return null;

            var datameaning = "OOPT U16|HEX|EventType1 U16|HEX|EventValue1 U16|HEX|EventType2 U16|HEX|EventValue2 U16|HEX|EventType3 U16|HEX|EventValue3 U16|HEX|EventType4 U16|HEX|EventValue4";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            EventReadB_EventType1 = parseResult.ValueList.GetValue("EventType1").AsDouble;

            EventReadB_EventValue1 = parseResult.ValueList.GetValue("EventValue1").AsDouble;

            EventReadB_EventType2 = parseResult.ValueList.GetValue("EventType2").AsDouble;

            EventReadB_EventValue2 = parseResult.ValueList.GetValue("EventValue2").AsDouble;

            EventReadB_EventType3 = parseResult.ValueList.GetValue("EventType3").AsDouble;

            EventReadB_EventValue3 = parseResult.ValueList.GetValue("EventValue3").AsDouble;

            EventReadB_EventType4 = parseResult.ValueList.GetValue("EventType4").AsDouble;

            EventReadB_EventValue4 = parseResult.ValueList.GetValue("EventValue4").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; EventReadBEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent EventReadBEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyEventReadB_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyEventReadBAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[7];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyEventReadB_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyEventReadB_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "OOPT U16|HEX|EventType1 U16|HEX|EventValue1 U16|HEX|EventType2 U16|HEX|EventValue2 U16|HEX|EventType3 U16|HEX|EventValue3 U16|HEX|EventType4 U16|HEX|EventValue4";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        EventReadB_EventType1 = parseResult.ValueList.GetValue("EventType1").AsDouble;

                        EventReadB_EventValue1 = parseResult.ValueList.GetValue("EventValue1").AsDouble;

                        EventReadB_EventType2 = parseResult.ValueList.GetValue("EventType2").AsDouble;

                        EventReadB_EventValue2 = parseResult.ValueList.GetValue("EventValue2").AsDouble;

                        EventReadB_EventType3 = parseResult.ValueList.GetValue("EventType3").AsDouble;

                        EventReadB_EventValue3 = parseResult.ValueList.GetValue("EventValue3").AsDouble;

                        EventReadB_EventType4 = parseResult.ValueList.GetValue("EventType4").AsDouble;

                        EventReadB_EventValue4 = parseResult.ValueList.GetValue("EventValue4").AsDouble;

                        EventReadBEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyEventReadB: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyEventReadB: set notification", result);

            return true;
        }

        /// <summary>
        /// Writes data for EventWriteA
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteEventWriteA(UInt16 EventType1, UInt16 EventValue1, UInt16 EventType2, UInt16 EventValue2, UInt16 EventType3, UInt16 EventValue3, UInt16 EventType4, UInt16 EventValue4)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(EventType1);
            dw.WriteUInt16(EventValue1);
            dw.WriteUInt16(EventType2);
            dw.WriteUInt16(EventValue2);
            dw.WriteUInt16(EventType3);
            dw.WriteUInt16(EventValue3);
            dw.WriteUInt16(EventType4);
            dw.WriteUInt16(EventValue4);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(8, "EventWriteA", command, GattWriteOption.WriteWithResponse);
        }

        /// <summary>
        /// Writes data for EventWriteB
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteEventWriteB(UInt16 EventType1, UInt16 EventValue1, UInt16 EventType2, UInt16 EventValue2, UInt16 EventType3, UInt16 EventValue3, UInt16 EventType4, UInt16 EventValue4)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(EventType1);
            dw.WriteUInt16(EventValue1);
            dw.WriteUInt16(EventType2);
            dw.WriteUInt16(EventValue2);
            dw.WriteUInt16(EventType3);
            dw.WriteUInt16(EventValue3);
            dw.WriteUInt16(EventType4);
            dw.WriteUInt16(EventValue4);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(9, "EventWriteB", command, GattWriteOption.WriteWithoutResponse);
        }

        private double _AccelerometerData_X = 0;
        private bool _AccelerometerData_X_set = false;
        public double AccelerometerData_X
        {
            get { return _AccelerometerData_X; }
            internal set { if (_AccelerometerData_X_set && value == _AccelerometerData_X) return; _AccelerometerData_X = value; _AccelerometerData_X_set = true; OnPropertyChanged(); }
        }

        private double _AccelerometerData_Y = 0;
        private bool _AccelerometerData_Y_set = false;
        public double AccelerometerData_Y
        {
            get { return _AccelerometerData_Y; }
            internal set { if (_AccelerometerData_Y_set && value == _AccelerometerData_Y) return; _AccelerometerData_Y = value; _AccelerometerData_Y_set = true; OnPropertyChanged(); }
        }

        private double _AccelerometerData_Z = 0;
        private bool _AccelerometerData_Z_set = false;
        public double AccelerometerData_Z
        {
            get { return _AccelerometerData_Z; }
            internal set { if (_AccelerometerData_Z_set && value == _AccelerometerData_Z) return; _AccelerometerData_Z = value; _AccelerometerData_Z_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometerData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(10, "AccelerometerData", cacheMode);
            if (result == null) return null;

            var datameaning = "I16^1000_/|FIXED|X I16^1000_/|FIXED|Y I16^1000_/|FIXED|Z";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            AccelerometerData_X = parseResult.ValueList.GetValue("X").AsDouble;

            AccelerometerData_Y = parseResult.ValueList.GetValue("Y").AsDouble;

            AccelerometerData_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; AccelerometerDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent AccelerometerDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyAccelerometerData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyAccelerometerDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[10];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyAccelerometerData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyAccelerometerData_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I16^1000_/|FIXED|X I16^1000_/|FIXED|Y I16^1000_/|FIXED|Z";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        AccelerometerData_X = parseResult.ValueList.GetValue("X").AsDouble;

                        AccelerometerData_Y = parseResult.ValueList.GetValue("Y").AsDouble;

                        AccelerometerData_Z = parseResult.ValueList.GetValue("Z").AsDouble;

                        AccelerometerDataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyAccelerometerData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyAccelerometerData: set notification", result);

            return true;
        }

        private double _AccelerometerPeriod = 0;
        private bool _AccelerometerPeriod_set = false;
        public double AccelerometerPeriod
        {
            get { return _AccelerometerPeriod; }
            internal set { if (_AccelerometerPeriod_set && value == _AccelerometerPeriod) return; _AccelerometerPeriod = value; _AccelerometerPeriod_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometerPeriod(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(11, "AccelerometerPeriod", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|AccelerometerPeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            AccelerometerPeriod = parseResult.ValueList.GetValue("AccelerometerPeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for AccelerometerPeriod
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccelerometerPeriod(UInt16 AccelerometerPeriod)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(AccelerometerPeriod);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(11, "AccelerometerPeriod", command, GattWriteOption.WriteWithResponse);
        }

        private double _ButtonDataA = 0;
        private bool _ButtonDataA_set = false;
        public double ButtonDataA
        {
            get { return _ButtonDataA; }
            internal set { if (_ButtonDataA_set && value == _ButtonDataA) return; _ButtonDataA = value; _ButtonDataA_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadButtonDataA(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(12, "ButtonDataA", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|ButtonA";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            ButtonDataA = parseResult.ValueList.GetValue("ButtonA").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ButtonDataAEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ButtonDataAEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyButtonDataA_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyButtonDataAAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[12];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyButtonDataA_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyButtonDataA_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "U8|HEX|ButtonA";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        ButtonDataA = parseResult.ValueList.GetValue("ButtonA").AsDouble;

                        ButtonDataAEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyButtonDataA: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyButtonDataA: set notification", result);

            return true;
        }

        private double _ButtonDataB = 0;
        private bool _ButtonDataB_set = false;
        public double ButtonDataB
        {
            get { return _ButtonDataB; }
            internal set { if (_ButtonDataB_set && value == _ButtonDataB) return; _ButtonDataB = value; _ButtonDataB_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadButtonDataB(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(13, "ButtonDataB", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|ButtonB";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            ButtonDataB = parseResult.ValueList.GetValue("ButtonB").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ButtonDataBEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ButtonDataBEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyButtonDataB_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyButtonDataBAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[13];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyButtonDataB_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyButtonDataB_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "U8|HEX|ButtonB";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        ButtonDataB = parseResult.ValueList.GetValue("ButtonB").AsDouble;

                        ButtonDataBEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyButtonDataB: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyButtonDataB: set notification", result);

            return true;
        }

        private double _PinAnalog = 0;
        private bool _PinAnalog_set = false;
        public double PinAnalog
        {
            get { return _PinAnalog; }
            internal set { if (_PinAnalog_set && value == _PinAnalog) return; _PinAnalog = value; _PinAnalog_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPinAnalog(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(14, "PinAnalog", cacheMode);
            if (result == null) return null;

            var datameaning = "U32|HEX|SetAnalog";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            PinAnalog = parseResult.ValueList.GetValue("SetAnalog").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for PinAnalog
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePinAnalog(UInt32 SetAnalog)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt32(SetAnalog);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(14, "PinAnalog", command, GattWriteOption.WriteWithResponse);
        }

        private double _PinInput = 0;
        private bool _PinInput_set = false;
        public double PinInput
        {
            get { return _PinInput; }
            internal set { if (_PinInput_set && value == _PinInput) return; _PinInput = value; _PinInput_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPinInput(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(15, "PinInput", cacheMode);
            if (result == null) return null;

            var datameaning = "U32|HEX|SetInput";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            PinInput = parseResult.ValueList.GetValue("SetInput").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for PinInput
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePinInput(UInt32 SetInput)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt32(SetInput);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(15, "PinInput", command, GattWriteOption.WriteWithResponse);
        }

        /// <summary>
        /// Writes data for PinPwm
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePinPwm(byte PinNumber1, UInt16 Value1, UInt32 Period1, byte PinNumber2, UInt16 Value2, UInt32 Period2)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(PinNumber1);
            dw.WriteUInt16(Value1);
            dw.WriteUInt32(Period1);
            dw.WriteByte(PinNumber2);
            dw.WriteUInt16(Value2);
            dw.WriteUInt32(Period2);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(16, "PinPwm", command, GattWriteOption.WriteWithResponse);
        }

        private double _PinData_PinNumber1 = 0;
        private bool _PinData_PinNumber1_set = false;
        public double PinData_PinNumber1
        {
            get { return _PinData_PinNumber1; }
            internal set { if (_PinData_PinNumber1_set && value == _PinData_PinNumber1) return; _PinData_PinNumber1 = value; _PinData_PinNumber1_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData1 = 0;
        private bool _PinData_DEPinData1_set = false;
        public double PinData_DEPinData1
        {
            get { return _PinData_DEPinData1; }
            internal set { if (_PinData_DEPinData1_set && value == _PinData_DEPinData1) return; _PinData_DEPinData1 = value; _PinData_DEPinData1_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber2 = 0;
        private bool _PinData_PinNumber2_set = false;
        public double PinData_PinNumber2
        {
            get { return _PinData_PinNumber2; }
            internal set { if (_PinData_PinNumber2_set && value == _PinData_PinNumber2) return; _PinData_PinNumber2 = value; _PinData_PinNumber2_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData2 = 0;
        private bool _PinData_DEPinData2_set = false;
        public double PinData_DEPinData2
        {
            get { return _PinData_DEPinData2; }
            internal set { if (_PinData_DEPinData2_set && value == _PinData_DEPinData2) return; _PinData_DEPinData2 = value; _PinData_DEPinData2_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber3 = 0;
        private bool _PinData_PinNumber3_set = false;
        public double PinData_PinNumber3
        {
            get { return _PinData_PinNumber3; }
            internal set { if (_PinData_PinNumber3_set && value == _PinData_PinNumber3) return; _PinData_PinNumber3 = value; _PinData_PinNumber3_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData3 = 0;
        private bool _PinData_DEPinData3_set = false;
        public double PinData_DEPinData3
        {
            get { return _PinData_DEPinData3; }
            internal set { if (_PinData_DEPinData3_set && value == _PinData_DEPinData3) return; _PinData_DEPinData3 = value; _PinData_DEPinData3_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber4 = 0;
        private bool _PinData_PinNumber4_set = false;
        public double PinData_PinNumber4
        {
            get { return _PinData_PinNumber4; }
            internal set { if (_PinData_PinNumber4_set && value == _PinData_PinNumber4) return; _PinData_PinNumber4 = value; _PinData_PinNumber4_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData4 = 0;
        private bool _PinData_DEPinData4_set = false;
        public double PinData_DEPinData4
        {
            get { return _PinData_DEPinData4; }
            internal set { if (_PinData_DEPinData4_set && value == _PinData_DEPinData4) return; _PinData_DEPinData4 = value; _PinData_DEPinData4_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber5 = 0;
        private bool _PinData_PinNumber5_set = false;
        public double PinData_PinNumber5
        {
            get { return _PinData_PinNumber5; }
            internal set { if (_PinData_PinNumber5_set && value == _PinData_PinNumber5) return; _PinData_PinNumber5 = value; _PinData_PinNumber5_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData5 = 0;
        private bool _PinData_DEPinData5_set = false;
        public double PinData_DEPinData5
        {
            get { return _PinData_DEPinData5; }
            internal set { if (_PinData_DEPinData5_set && value == _PinData_DEPinData5) return; _PinData_DEPinData5 = value; _PinData_DEPinData5_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber6 = 0;
        private bool _PinData_PinNumber6_set = false;
        public double PinData_PinNumber6
        {
            get { return _PinData_PinNumber6; }
            internal set { if (_PinData_PinNumber6_set && value == _PinData_PinNumber6) return; _PinData_PinNumber6 = value; _PinData_PinNumber6_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData6 = 0;
        private bool _PinData_DEPinData6_set = false;
        public double PinData_DEPinData6
        {
            get { return _PinData_DEPinData6; }
            internal set { if (_PinData_DEPinData6_set && value == _PinData_DEPinData6) return; _PinData_DEPinData6 = value; _PinData_DEPinData6_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber7 = 0;
        private bool _PinData_PinNumber7_set = false;
        public double PinData_PinNumber7
        {
            get { return _PinData_PinNumber7; }
            internal set { if (_PinData_PinNumber7_set && value == _PinData_PinNumber7) return; _PinData_PinNumber7 = value; _PinData_PinNumber7_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData7 = 0;
        private bool _PinData_DEPinData7_set = false;
        public double PinData_DEPinData7
        {
            get { return _PinData_DEPinData7; }
            internal set { if (_PinData_DEPinData7_set && value == _PinData_DEPinData7) return; _PinData_DEPinData7 = value; _PinData_DEPinData7_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber8 = 0;
        private bool _PinData_PinNumber8_set = false;
        public double PinData_PinNumber8
        {
            get { return _PinData_PinNumber8; }
            internal set { if (_PinData_PinNumber8_set && value == _PinData_PinNumber8) return; _PinData_PinNumber8 = value; _PinData_PinNumber8_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData8 = 0;
        private bool _PinData_DEPinData8_set = false;
        public double PinData_DEPinData8
        {
            get { return _PinData_DEPinData8; }
            internal set { if (_PinData_DEPinData8_set && value == _PinData_DEPinData8) return; _PinData_DEPinData8 = value; _PinData_DEPinData8_set = true; OnPropertyChanged(); }
        }

        private double _PinData_PinNumber9 = 0;
        private bool _PinData_PinNumber9_set = false;
        public double PinData_PinNumber9
        {
            get { return _PinData_PinNumber9; }
            internal set { if (_PinData_PinNumber9_set && value == _PinData_PinNumber9) return; _PinData_PinNumber9 = value; _PinData_PinNumber9_set = true; OnPropertyChanged(); }
        }

        private double _PinData_DEPinData9 = 0;
        private bool _PinData_DEPinData9_set = false;
        public double PinData_DEPinData9
        {
            get { return _PinData_DEPinData9; }
            internal set { if (_PinData_DEPinData9_set && value == _PinData_DEPinData9) return; _PinData_DEPinData9 = value; _PinData_DEPinData9_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPinData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(17, "PinData", cacheMode);
            if (result == null) return null;

            var datameaning = "OOPT U8|DEC|PinNumber1 U8|DEC|DEPinData1 U8|DEC|PinNumber2 U8|DEC|DEPinData2 U8|DEC|PinNumber3 U8|DEC|DEPinData3 U8|DEC|PinNumber4 U8|DEC|DEPinData4 U8|DEC|PinNumber5 U8|DEC|DEPinData5 U8|DEC|PinNumber6 U8|DEC|DEPinData6 U8|DEC|PinNumber7 U8|DEC|DEPinData7 U8|DEC|PinNumber8 U8|DEC|DEPinData8 U8|DEC|PinNumber9 U8|DEC|DEPinData9";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            PinData_PinNumber1 = parseResult.ValueList.GetValue("PinNumber1").AsDouble;

            PinData_DEPinData1 = parseResult.ValueList.GetValue("DEPinData1").AsDouble;

            PinData_PinNumber2 = parseResult.ValueList.GetValue("PinNumber2").AsDouble;

            PinData_DEPinData2 = parseResult.ValueList.GetValue("DEPinData2").AsDouble;

            PinData_PinNumber3 = parseResult.ValueList.GetValue("PinNumber3").AsDouble;

            PinData_DEPinData3 = parseResult.ValueList.GetValue("DEPinData3").AsDouble;

            PinData_PinNumber4 = parseResult.ValueList.GetValue("PinNumber4").AsDouble;

            PinData_DEPinData4 = parseResult.ValueList.GetValue("DEPinData4").AsDouble;

            PinData_PinNumber5 = parseResult.ValueList.GetValue("PinNumber5").AsDouble;

            PinData_DEPinData5 = parseResult.ValueList.GetValue("DEPinData5").AsDouble;

            PinData_PinNumber6 = parseResult.ValueList.GetValue("PinNumber6").AsDouble;

            PinData_DEPinData6 = parseResult.ValueList.GetValue("DEPinData6").AsDouble;

            PinData_PinNumber7 = parseResult.ValueList.GetValue("PinNumber7").AsDouble;

            PinData_DEPinData7 = parseResult.ValueList.GetValue("DEPinData7").AsDouble;

            PinData_PinNumber8 = parseResult.ValueList.GetValue("PinNumber8").AsDouble;

            PinData_DEPinData8 = parseResult.ValueList.GetValue("DEPinData8").AsDouble;

            PinData_PinNumber9 = parseResult.ValueList.GetValue("PinNumber9").AsDouble;

            PinData_DEPinData9 = parseResult.ValueList.GetValue("DEPinData9").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; PinDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent PinDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyPinData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyPinDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[17];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyPinData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyPinData_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "OOPT U8|DEC|PinNumber1 U8|DEC|DEPinData1 U8|DEC|PinNumber2 U8|DEC|DEPinData2 U8|DEC|PinNumber3 U8|DEC|DEPinData3 U8|DEC|PinNumber4 U8|DEC|DEPinData4 U8|DEC|PinNumber5 U8|DEC|DEPinData5 U8|DEC|PinNumber6 U8|DEC|DEPinData6 U8|DEC|PinNumber7 U8|DEC|DEPinData7 U8|DEC|PinNumber8 U8|DEC|DEPinData8 U8|DEC|PinNumber9 U8|DEC|DEPinData9";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        PinData_PinNumber1 = parseResult.ValueList.GetValue("PinNumber1").AsDouble;

                        PinData_DEPinData1 = parseResult.ValueList.GetValue("DEPinData1").AsDouble;

                        PinData_PinNumber2 = parseResult.ValueList.GetValue("PinNumber2").AsDouble;

                        PinData_DEPinData2 = parseResult.ValueList.GetValue("DEPinData2").AsDouble;

                        PinData_PinNumber3 = parseResult.ValueList.GetValue("PinNumber3").AsDouble;

                        PinData_DEPinData3 = parseResult.ValueList.GetValue("DEPinData3").AsDouble;

                        PinData_PinNumber4 = parseResult.ValueList.GetValue("PinNumber4").AsDouble;

                        PinData_DEPinData4 = parseResult.ValueList.GetValue("DEPinData4").AsDouble;

                        PinData_PinNumber5 = parseResult.ValueList.GetValue("PinNumber5").AsDouble;

                        PinData_DEPinData5 = parseResult.ValueList.GetValue("DEPinData5").AsDouble;

                        PinData_PinNumber6 = parseResult.ValueList.GetValue("PinNumber6").AsDouble;

                        PinData_DEPinData6 = parseResult.ValueList.GetValue("DEPinData6").AsDouble;

                        PinData_PinNumber7 = parseResult.ValueList.GetValue("PinNumber7").AsDouble;

                        PinData_DEPinData7 = parseResult.ValueList.GetValue("DEPinData7").AsDouble;

                        PinData_PinNumber8 = parseResult.ValueList.GetValue("PinNumber8").AsDouble;

                        PinData_DEPinData8 = parseResult.ValueList.GetValue("DEPinData8").AsDouble;

                        PinData_PinNumber9 = parseResult.ValueList.GetValue("PinNumber9").AsDouble;

                        PinData_DEPinData9 = parseResult.ValueList.GetValue("DEPinData9").AsDouble;

                        PinDataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyPinData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyPinData: set notification", result);

            return true;
        }

        /// <summary>
        /// Writes data for PinData
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePinData(byte PinNumber1, byte DEPinData1, byte PinNumber2, byte DEPinData2, byte PinNumber3, byte DEPinData3, byte PinNumber4, byte DEPinData4, byte PinNumber5, byte DEPinData5, byte PinNumber6, byte DEPinData6, byte PinNumber7, byte DEPinData7, byte PinNumber8, byte DEPinData8, byte PinNumber9, byte DEPinData9)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(PinNumber1);
            dw.WriteByte(DEPinData1);
            dw.WriteByte(PinNumber2);
            dw.WriteByte(DEPinData2);
            dw.WriteByte(PinNumber3);
            dw.WriteByte(DEPinData3);
            dw.WriteByte(PinNumber4);
            dw.WriteByte(DEPinData4);
            dw.WriteByte(PinNumber5);
            dw.WriteByte(DEPinData5);
            dw.WriteByte(PinNumber6);
            dw.WriteByte(DEPinData6);
            dw.WriteByte(PinNumber7);
            dw.WriteByte(DEPinData7);
            dw.WriteByte(PinNumber8);
            dw.WriteByte(DEPinData8);
            dw.WriteByte(PinNumber9);
            dw.WriteByte(DEPinData9);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(17, "PinData", command, GattWriteOption.WriteWithResponse);
        }

        private double _LedPattern_Row1 = 0;
        private bool _LedPattern_Row1_set = false;
        public double LedPattern_Row1
        {
            get { return _LedPattern_Row1; }
            internal set { if (_LedPattern_Row1_set && value == _LedPattern_Row1) return; _LedPattern_Row1 = value; _LedPattern_Row1_set = true; OnPropertyChanged(); }
        }

        private double _LedPattern_Row2 = 0;
        private bool _LedPattern_Row2_set = false;
        public double LedPattern_Row2
        {
            get { return _LedPattern_Row2; }
            internal set { if (_LedPattern_Row2_set && value == _LedPattern_Row2) return; _LedPattern_Row2 = value; _LedPattern_Row2_set = true; OnPropertyChanged(); }
        }

        private double _LedPattern_Row3 = 0;
        private bool _LedPattern_Row3_set = false;
        public double LedPattern_Row3
        {
            get { return _LedPattern_Row3; }
            internal set { if (_LedPattern_Row3_set && value == _LedPattern_Row3) return; _LedPattern_Row3 = value; _LedPattern_Row3_set = true; OnPropertyChanged(); }
        }

        private double _LedPattern_Row4 = 0;
        private bool _LedPattern_Row4_set = false;
        public double LedPattern_Row4
        {
            get { return _LedPattern_Row4; }
            internal set { if (_LedPattern_Row4_set && value == _LedPattern_Row4) return; _LedPattern_Row4 = value; _LedPattern_Row4_set = true; OnPropertyChanged(); }
        }

        private double _LedPattern_Row5 = 0;
        private bool _LedPattern_Row5_set = false;
        public double LedPattern_Row5
        {
            get { return _LedPattern_Row5; }
            internal set { if (_LedPattern_Row5_set && value == _LedPattern_Row5) return; _LedPattern_Row5 = value; _LedPattern_Row5_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLedPattern(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(18, "LedPattern", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Row1 U8|HEX|Row2 U8|HEX|Row3 U8|HEX|Row4 U8|HEX|Row5";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            LedPattern_Row1 = parseResult.ValueList.GetValue("Row1").AsDouble;

            LedPattern_Row2 = parseResult.ValueList.GetValue("Row2").AsDouble;

            LedPattern_Row3 = parseResult.ValueList.GetValue("Row3").AsDouble;

            LedPattern_Row4 = parseResult.ValueList.GetValue("Row4").AsDouble;

            LedPattern_Row5 = parseResult.ValueList.GetValue("Row5").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for LedPattern
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLedPattern(byte Row1, byte Row2, byte Row3, byte Row4, byte Row5)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Row1);
            dw.WriteByte(Row2);
            dw.WriteByte(Row3);
            dw.WriteByte(Row4);
            dw.WriteByte(Row5);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(18, "LedPattern", command, GattWriteOption.WriteWithResponse);
        }

        /// <summary>
        /// Writes data for LedText
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLedText(String LedText)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(LedText);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(19, "LedText", command, GattWriteOption.WriteWithResponse);
        }

        private double _LedScrollTime = 0;
        private bool _LedScrollTime_set = false;
        public double LedScrollTime
        {
            get { return _LedScrollTime; }
            internal set { if (_LedScrollTime_set && value == _LedScrollTime) return; _LedScrollTime = value; _LedScrollTime_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLedScrollTime(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(20, "LedScrollTime", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|ScrollTime";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            LedScrollTime = parseResult.ValueList.GetValue("ScrollTime").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for LedScrollTime
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLedScrollTime(UInt16 ScrollTime)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(ScrollTime);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(20, "LedScrollTime", command, GattWriteOption.WriteWithResponse);
        }

        private double _MagnetometerData_MagX = 0;
        private bool _MagnetometerData_MagX_set = false;
        public double MagnetometerData_MagX
        {
            get { return _MagnetometerData_MagX; }
            internal set { if (_MagnetometerData_MagX_set && value == _MagnetometerData_MagX) return; _MagnetometerData_MagX = value; _MagnetometerData_MagX_set = true; OnPropertyChanged(); }
        }

        private double _MagnetometerData_MagY = 0;
        private bool _MagnetometerData_MagY_set = false;
        public double MagnetometerData_MagY
        {
            get { return _MagnetometerData_MagY; }
            internal set { if (_MagnetometerData_MagY_set && value == _MagnetometerData_MagY) return; _MagnetometerData_MagY = value; _MagnetometerData_MagY_set = true; OnPropertyChanged(); }
        }

        private double _MagnetometerData_MagZ = 0;
        private bool _MagnetometerData_MagZ_set = false;
        public double MagnetometerData_MagZ
        {
            get { return _MagnetometerData_MagZ; }
            internal set { if (_MagnetometerData_MagZ_set && value == _MagnetometerData_MagZ) return; _MagnetometerData_MagZ = value; _MagnetometerData_MagZ_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMagnetometerData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(21, "MagnetometerData", cacheMode);
            if (result == null) return null;

            var datameaning = "I16|DEC|MagX I16|DEC|MagY I16|DEC|MagZ";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            MagnetometerData_MagX = parseResult.ValueList.GetValue("MagX").AsDouble;

            MagnetometerData_MagY = parseResult.ValueList.GetValue("MagY").AsDouble;

            MagnetometerData_MagZ = parseResult.ValueList.GetValue("MagZ").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; MagnetometerDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent MagnetometerDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMagnetometerData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMagnetometerDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[21];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMagnetometerData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMagnetometerData_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I16|DEC|MagX I16|DEC|MagY I16|DEC|MagZ";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        MagnetometerData_MagX = parseResult.ValueList.GetValue("MagX").AsDouble;

                        MagnetometerData_MagY = parseResult.ValueList.GetValue("MagY").AsDouble;

                        MagnetometerData_MagZ = parseResult.ValueList.GetValue("MagZ").AsDouble;

                        MagnetometerDataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMagnetometerData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMagnetometerData: set notification", result);

            return true;
        }

        private double _MagnetometerBearing = 0;
        private bool _MagnetometerBearing_set = false;
        public double MagnetometerBearing
        {
            get { return _MagnetometerBearing; }
            internal set { if (_MagnetometerBearing_set && value == _MagnetometerBearing) return; _MagnetometerBearing = value; _MagnetometerBearing_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMagnetometerBearing(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(22, "MagnetometerBearing", cacheMode);
            if (result == null) return null;

            var datameaning = "I16|DEC|Bearing";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            MagnetometerBearing = parseResult.ValueList.GetValue("Bearing").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; MagnetometerBearingEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent MagnetometerBearingEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMagnetometerBearing_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMagnetometerBearingAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[22];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMagnetometerBearing_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMagnetometerBearing_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I16|DEC|Bearing";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        MagnetometerBearing = parseResult.ValueList.GetValue("Bearing").AsDouble;

                        MagnetometerBearingEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMagnetometerBearing: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMagnetometerBearing: set notification", result);

            return true;
        }

        private double _MagnetometerPeriod = 0;
        private bool _MagnetometerPeriod_set = false;
        public double MagnetometerPeriod
        {
            get { return _MagnetometerPeriod; }
            internal set { if (_MagnetometerPeriod_set && value == _MagnetometerPeriod) return; _MagnetometerPeriod = value; _MagnetometerPeriod_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMagnetometerPeriod(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(23, "MagnetometerPeriod", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|MagnetometerPeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            MagnetometerPeriod = parseResult.ValueList.GetValue("MagnetometerPeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for MagnetometerPeriod
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMagnetometerPeriod(UInt16 MagnetometerPeriod)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(MagnetometerPeriod);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(23, "MagnetometerPeriod", command, GattWriteOption.WriteWithResponse);
        }

        private double _MagnetometerCalibration = 0;
        private bool _MagnetometerCalibration_set = false;
        public double MagnetometerCalibration
        {
            get { return _MagnetometerCalibration; }
            internal set { if (_MagnetometerCalibration_set && value == _MagnetometerCalibration) return; _MagnetometerCalibration = value; _MagnetometerCalibration_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMagnetometerCalibration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(24, "MagnetometerCalibration", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|MagnetometerCalibration";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            MagnetometerCalibration = parseResult.ValueList.GetValue("MagnetometerCalibration").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for MagnetometerCalibration
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMagnetometerCalibration(byte MagnetometerCalibration)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(MagnetometerCalibration);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(24, "MagnetometerCalibration", command, GattWriteOption.WriteWithResponse);
        }

        private double _TemperatureData = 0;
        private bool _TemperatureData_set = false;
        public double TemperatureData
        {
            get { return _TemperatureData; }
            internal set { if (_TemperatureData_set && value == _TemperatureData) return; _TemperatureData = value; _TemperatureData_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperatureData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(25, "TemperatureData", cacheMode);
            if (result == null) return null;

            var datameaning = "I8|DEC|Temperature|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            TemperatureData = parseResult.ValueList.GetValue("Temperature").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; TemperatureDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent TemperatureDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyTemperatureData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyTemperatureDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[25];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTemperatureData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTemperatureData_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I8|DEC|Temperature|C";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        TemperatureData = parseResult.ValueList.GetValue("Temperature").AsDouble;

                        TemperatureDataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyTemperatureData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyTemperatureData: set notification", result);

            return true;
        }

        private double _TemperaturePeriod = 0;
        private bool _TemperaturePeriod_set = false;
        public double TemperaturePeriod
        {
            get { return _TemperaturePeriod; }
            internal set { if (_TemperaturePeriod_set && value == _TemperaturePeriod) return; _TemperaturePeriod = value; _TemperaturePeriod_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperaturePeriod(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(26, "TemperaturePeriod", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|TemperaturePeriod";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            TemperaturePeriod = parseResult.ValueList.GetValue("TemperaturePeriod").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        /// <summary>
        /// Writes data for TemperaturePeriod
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTemperaturePeriod(UInt16 TemperaturePeriod)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(TemperaturePeriod);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(26, "TemperaturePeriod", command, GattWriteOption.WriteWithResponse);
        }

    }
}
