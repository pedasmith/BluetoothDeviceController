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
    /// The TI 1350 and 2650 are the latest in the TI range of Sensor. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth.
    /// This class was automatically generated 2022-07-07::07:06
    /// </summary>

    public partial class TI_SensorTag_1350 : INotifyPropertyChanged
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
           Guid.Parse("{00001800-0000-1000-8000-00805f9b34fb}"),
           Guid.Parse("{00001801-0000-1000-8000-00805f9b34fb}"),
           Guid.Parse("{0000180a-0000-1000-8000-00805f9b34fb}"),
           Guid.Parse("{0000180f-0000-1000-8000-00805f9b34fb}"),
           Guid.Parse("{f000aa00-0451-4000-b000-000000000000}"),
           Guid.Parse("{f000aa20-0451-4000-b000-000000000000}"),
           Guid.Parse("{f000aa40-0451-4000-b000-000000000000}"),
           Guid.Parse("{f000aa80-0451-4000-b000-000000000000}"),
           Guid.Parse("{f000aa70-0451-4000-b000-000000000000}"),
           Guid.Parse("{0000ffe0-0000-1000-8000-00805f9b34fb}"),
           Guid.Parse("{f000aa64-0451-4000-b000-000000000000}"),
           Guid.Parse("{f000ac00-0451-4000-b000-000000000000}"),
           Guid.Parse("{f000ccc0-0451-4000-b000-000000000000}"),
           Guid.Parse("{f000ffc0-0451-4000-b000-000000000000}"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Generic Service",
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
            "Connection Control service",
            "OAD service",

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
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            // No characteristics for Generic Service
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #3 is System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #4 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #5 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #6 is Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #7 is Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #8 is Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #9 is Manufacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #10 is Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #11 is PnP ID
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #12 is BatteryLevel
            Guid.Parse("f000aa01-0451-4000-b000-000000000000"), // #13 is IR Data
            Guid.Parse("f000aa02-0451-4000-b000-000000000000"), // #14 is IR Service Config
            Guid.Parse("f000aa03-0451-4000-b000-000000000000"), // #15 is IR Service Period
            Guid.Parse("f000aa21-0451-4000-b000-000000000000"), // #16 is Humidity Data
            Guid.Parse("f000aa22-0451-4000-b000-000000000000"), // #17 is Humidity Config
            Guid.Parse("f000aa23-0451-4000-b000-000000000000"), // #18 is Humidity Period
            Guid.Parse("f000aa41-0451-4000-b000-000000000000"), // #19 is Barometer Data
            Guid.Parse("f000aa42-0451-4000-b000-000000000000"), // #20 is Barometer Config
            Guid.Parse("f000aa44-0451-4000-b000-000000000000"), // #21 is Barometer Period
            Guid.Parse("f000aa81-0451-4000-b000-000000000000"), // #22 is Accelerometer Data
            Guid.Parse("f000aa82-0451-4000-b000-000000000000"), // #23 is Accelerometer Config
            Guid.Parse("f000aa83-0451-4000-b000-000000000000"), // #24 is Accelerometer Period
            Guid.Parse("f000aa71-0451-4000-b000-000000000000"), // #25 is Optical Service Data
            Guid.Parse("f000aa72-0451-4000-b000-000000000000"), // #26 is Optical Service Config
            Guid.Parse("f000aa73-0451-4000-b000-000000000000"), // #27 is Optical Service Period
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #28 is Key Press State
            Guid.Parse("f000aa65-0451-4000-b000-000000000000"), // #29 is IO Service Data
            Guid.Parse("f000aa66-0451-4000-b000-000000000000"), // #30 is IO Service Config
            Guid.Parse("f000ac01-0451-4000-b000-000000000000"), // #31 is Register Data
            Guid.Parse("f000ac02-0451-4000-b000-000000000000"), // #32 is Register Address
            Guid.Parse("f000ac03-0451-4000-b000-000000000000"), // #33 is Register Device ID
            Guid.Parse("f000ccc1-0451-4000-b000-000000000000"), // #34 is Conn Parameters A
            Guid.Parse("f000ccc2-0451-4000-b000-000000000000"), // #35 is Conn Parameters B
            Guid.Parse("f000ccc3-0451-4000-b000-000000000000"), // #36 is Disconnect Request
            Guid.Parse("f000ffc1-0451-4000-b000-000000000000"), // #37 is Img Identify
            Guid.Parse("f000ffc2-0451-4000-b000-000000000000"), // #38 is Img Block
            Guid.Parse("f000ffc3-0451-4000-b000-000000000000"), // #39 is Img Count
            Guid.Parse("f000ffc4-0451-4000-b000-000000000000"), // #40 is Img Status

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            // No characteristics for Generic Service
            "System ID", // #3 is 00002a23-0000-1000-8000-00805f9b34fb
            "Model Number", // #4 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #5 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #6 is 00002a26-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #7 is 00002a27-0000-1000-8000-00805f9b34fb
            "Software Revision", // #8 is 00002a28-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #9 is 00002a29-0000-1000-8000-00805f9b34fb
            "Regulatory List", // #10 is 00002a2a-0000-1000-8000-00805f9b34fb
            "PnP ID", // #11 is 00002a50-0000-1000-8000-00805f9b34fb
            "BatteryLevel", // #12 is 00002a19-0000-1000-8000-00805f9b34fb
            "IR Data", // #13 is f000aa01-0451-4000-b000-000000000000
            "IR Service Config", // #14 is f000aa02-0451-4000-b000-000000000000
            "IR Service Period", // #15 is f000aa03-0451-4000-b000-000000000000
            "Humidity Data", // #16 is f000aa21-0451-4000-b000-000000000000
            "Humidity Config", // #17 is f000aa22-0451-4000-b000-000000000000
            "Humidity Period", // #18 is f000aa23-0451-4000-b000-000000000000
            "Barometer Data", // #19 is f000aa41-0451-4000-b000-000000000000
            "Barometer Config", // #20 is f000aa42-0451-4000-b000-000000000000
            "Barometer Period", // #21 is f000aa44-0451-4000-b000-000000000000
            "Accelerometer Data", // #22 is f000aa81-0451-4000-b000-000000000000
            "Accelerometer Config", // #23 is f000aa82-0451-4000-b000-000000000000
            "Accelerometer Period", // #24 is f000aa83-0451-4000-b000-000000000000
            "Optical Service Data", // #25 is f000aa71-0451-4000-b000-000000000000
            "Optical Service Config", // #26 is f000aa72-0451-4000-b000-000000000000
            "Optical Service Period", // #27 is f000aa73-0451-4000-b000-000000000000
            "Key Press State", // #28 is 0000ffe1-0000-1000-8000-00805f9b34fb
            "IO Service Data", // #29 is f000aa65-0451-4000-b000-000000000000
            "IO Service Config", // #30 is f000aa66-0451-4000-b000-000000000000
            "Register Data", // #31 is f000ac01-0451-4000-b000-000000000000
            "Register Address", // #32 is f000ac02-0451-4000-b000-000000000000
            "Register Device ID", // #33 is f000ac03-0451-4000-b000-000000000000
            "Conn Parameters A", // #34 is f000ccc1-0451-4000-b000-000000000000
            "Conn Parameters B", // #35 is f000ccc2-0451-4000-b000-000000000000
            "Disconnect Request", // #36 is f000ccc3-0451-4000-b000-000000000000
            "Img Identify", // #37 is f000ffc1-0451-4000-b000-000000000000
            "Img Block", // #38 is f000ffc2-0451-4000-b000-000000000000
            "Img Count", // #39 is f000ffc3-0451-4000-b000-000000000000
            "Img Status", // #40 is f000ffc4-0451-4000-b000-000000000000

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            // No characteristics for Generic Service
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
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2,  },
            // No characteristics for Generic Service
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
            new HashSet<int>(){ 34, 35, 36,  },
            new HashSet<int>(){ 37, 38, 39, 40,  },

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



        /// <summary>
        /// Reads data for Service=Common Configuration Characteristic=Device Name
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDevice Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(0, "Device Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Common Configuration Characteristic=Appearance
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
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Common Configuration Characteristic=Connection Parameter
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConnection Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(2, "Connection Parameter", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|ConnectionParameter";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
[[ERROR: Characteristics has no macros and no children]]        /// <summary>
        /// Reads data for Service=Device Info Characteristic=System ID
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSystem ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(3, "System ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=Model Number
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadModel Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(4, "Model Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=Serial Number
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSerial Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(5, "Serial Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=Firmware Revision
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadFirmware Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(6, "Firmware Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=Hardware Revision
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHardware Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(7, "Hardware Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=Software Revision
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSoftware Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(8, "Software Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=Manufacturer Name
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadManufacturer Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(9, "Manufacturer Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=Regulatory List
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegulatory List(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(10, "Regulatory List", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|BodyType U8|HEX|BodyStructure STRING|ASCII|Data";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Device Info Characteristic=PnP ID
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPnP ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(11, "PnP ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Battery Characteristic=BatteryLevel
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
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=IR Service Characteristic=IR Data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(13, "IR Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16^4_/_0.03125_*|FIXED|ObjTemp|C I16^4_/_0.03125_*|FIXED|AmbTemp|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=IR Service Characteristic=IR Service Config
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR Service Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(14, "IR Service Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=IR Service Characteristic=IR Service Period
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIR Service Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(15, "IR Service Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Humidity Characteristic=Humidity Data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(16, "Humidity Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U16^DU_3_ZE_175.72_*_65536_/_46.85_-_SW_2_JZ_PO_NP|FIXED|Temp U16^DU_3_ZE_125.0_*_65536_/_6.0_-_SW_2_JZ_PO_NP|FIXED|Humidity";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Humidity Characteristic=Humidity Config
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(17, "Humidity Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Humidity Characteristic=Humidity Period
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHumidity Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(18, "Humidity Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Barometer Characteristic=Barometer Data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(19, "Barometer Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I24^100_/|FIXED|Temp|C I24^100_/|FIXED|Pressure|hPa";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Barometer Characteristic=Barometer Config
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(20, "Barometer Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Barometer Characteristic=Barometer Period
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBarometer Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(21, "Barometer Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Accelerometer Characteristic=Accelerometer Data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(22, "Accelerometer Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16^500_*_65536_/|FIXED^N3|GyroX|dps I16^500_*_65536_/|FIXED^N3|GyroY|dps I16^500_*_65536_/|FIXED^N3|GyroZ|dps I16^8_*_32768_/|FIXED^N3|AccX|g I16^8_*_32768_/|FIXED^N3|AccY|g I16^8_*_32768_/|FIXED^N3|AccZ|g I16|DEC|MagnetometerX|microTesla I16|DEC|MagnetometerY|microTesla I16|DEC|MagnetometerZ|microTesla";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Accelerometer Characteristic=Accelerometer Config
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(23, "Accelerometer Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Accelerometer Characteristic=Accelerometer Period
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(24, "Accelerometer Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Optical Service Characteristic=Optical Service Data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadOptical Service Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(25, "Optical Service Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U16^DU_12_RS_15_AN_2_XY_0.01_*_SW_4095_AN_*|FIXED^N1|Lux";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Optical Service Characteristic=Optical Service Config
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadOptical Service Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(26, "Optical Service Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Optical Service Characteristic=Optical Service Period
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadOptical Service Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(27, "Optical Service Period", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Period|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Key Press Characteristic=Key Press State
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadKey Press State(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(28, "Key Press State", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=IO Service Characteristic=IO Service Data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIO Service Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(29, "IO Service Data", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=IO Service Characteristic=IO Service Config
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadIO Service Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(30, "IO Service Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Register service Characteristic=Register Data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegister Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(31, "Register Data", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Register service Characteristic=Register Address
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegister Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(32, "Register Address", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Register service Characteristic=Register Device ID
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadRegister Device ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(33, "Register Device ID", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Connection Control service Characteristic=Conn Parameters A
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConn Parameters A(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(34, "Conn Parameters A", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|ConnectionInterval U16|DEC|ServantLatency U16|DEC|SupervisionTimeout";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Connection Control service Characteristic=Conn Parameters B
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConn Parameters B(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(35, "Conn Parameters B", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|MinConnectionInterval U16|DEC|MaxConnectionInterval U16|DEC|ServentLatency U16|DEC|SupervisionTimeout";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=Connection Control service Characteristic=Disconnect Request
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDisconnect Request(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(36, "Disconnect Request", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=OAD service Characteristic=Img Identify
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadImg Identify(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(37, "Img Identify", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=OAD service Characteristic=Img Block
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadImg Block(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(38, "Img Block", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=OAD service Characteristic=Img Count
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadImg Count(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(39, "Img Count", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        /// <summary>
        /// Reads data for Service=OAD service Characteristic=Img Status
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadImg Status(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(40, "Img Status", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

    }
}