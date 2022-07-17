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
    /// Hand-held multimeter and oscilloscope that uses a phone (or PC) as the display..
    /// This class was automatically generated 6/12/2022 10:01 PM
    /// </summary>

    public partial class PokitProMeter : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://help.pokitmeter.com/hc/en-us/community/posts/360023523213-Bluetooth-API-Documentation
        // Link: https://www.pokitinnovations.com/wp-content/uploads/D0005250-PokitMeter-Bluetooth-API-Documentation-0_02.pdf
        // Link: https://github.com/pcolby/pokit/blob/6937d011c528b3a5f00c6e867ec91a71eaad5bcb/src/lib/uuids.h
        // Link: https://github.com/pcolby/qtpokit


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Guid[] ServiceGuids = new Guid[] {
            Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("57d3a771-267c-4394-8872-78223e92aec5"),
            Guid.Parse("e7481d2f-5781-442e-bb9a-fd4e3441dadc"),
            Guid.Parse("1569801e-1425-4a7a-b617-a4f4ed719de6"),
            Guid.Parse("a5ff3566-1fd8-4e10-8362-590a578a4121"),
            Guid.Parse("6f53be2f-780b-49b8-a7c3-e8a052b3ae2c"),
            Guid.Parse("1d14d6ee-fd63-4fa1-bfa4-8f47b42119f0"),

        };
        String[] ServiceNames = new string[] {
            "Generic Service",
            "Common Configuration",
            "Device Info",
            "Service_Status",
            "Multimeter",
            "DSO_Oscilloscope",
            "DataLogger_Dlog",
            "CalibrationService",
            "Silabs_Service_OTA",

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
            Guid.Parse("00002803-0000-1000-8000-00805f9b34fb"), // #0 is Server_Supported_Features
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #1 is Service Changes
            Guid.Parse("00002b29-0000-1000-8000-00805f9b34fb"), // #2 is Client_Supported_Features
            Guid.Parse("00002b2a-0000-1000-8000-00805f9b34fb"), // #3 is Database Hash
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #0 is Manufacturer Name
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #2 is Firmware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #3 is Software Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #4 is Hardware Revision
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #5 is Serial Number
            Guid.Parse("6974f5e5-0e54-45c3-97dd-29e4b5fb0849"), // #0 is Status_Device
            Guid.Parse("3dba36e1-6120-4706-8dfd-ed9c16e569b6"), // #1 is Status_Status
            Guid.Parse("7f0375de-077e-4555-8f78-800494509cc3"), // #2 is Status_Device_Name
            Guid.Parse("ec9bb1f3-05a9-4277-8dd0-60a7896f0d6e"), // #3 is Status_Flash_LED
            Guid.Parse("aaf3f6d5-43d4-4a83-9510-dff3d858d4cc"), // #4 is Status_Light_LED
            Guid.Parse("8fe5b5a9-b5b4-4a7b-8ff2-87224b970f89"), // #5 is UnknownStatusValues
            Guid.Parse("53dc9a7a-bc19-4280-b76b-002d0e23b078"), // #0 is MM_Settings
            Guid.Parse("047d3559-8bee-423a-b229-4417fa603b90"), // #1 is MM_Data
            Guid.Parse("a81af1b6-b8b3-4244-8859-3da368d2be39"), // #0 is DSO_Settings
            Guid.Parse("98e14f8e-536e-4f24-b4f4-1debfed0a99e"), // #1 is DSO_Reading
            Guid.Parse("970f00ba-f46f-4825-96a8-153a5cd0cda9"), // #2 is DSO_Metadata
            Guid.Parse("5f97c62b-a83b-46c6-b9cd-cac59e130a78"), // #0 is DataLogger_Settings
            Guid.Parse("3c669dab-fc86-411c-9498-4f9415049cc0"), // #1 is DataLogger_Reading
            Guid.Parse("9acada2e-3936-430b-a8f7-da407d97ca6e"), // #2 is DataLogger_MetaData
            Guid.Parse("0cd0f713-f5aa-4572-9e23-f8049f6bcaaa"), // #0 is CalbrateTemperature
            Guid.Parse("b6728f91-409c-4d6c-864e-272a6a7a0204"), // #1 is CalibrateUnknown1
            Guid.Parse("5588e47b-cb81-4f7b-acc4-6029a3f39f72"), // #2 is CalibrrateUnknown2
            Guid.Parse("f7bf3564-fb6d-4e53-88a4-5e37e0326063"), // #0 is OTA_Control
            Guid.Parse("984227f3-34fc-4045-a5d0-2c581f81a153"), // #1 is OTA_Data

        };
        String[] CharacteristicNames = new string[] {
            "Server_Supported_Features", // #0 is 00002803-0000-1000-8000-00805f9b34fb
            "Service Changes", // #1 is 00002a05-0000-1000-8000-00805f9b34fb
            "Client_Supported_Features", // #2 is 00002b29-0000-1000-8000-00805f9b34fb
            "Database Hash", // #3 is 00002b2a-0000-1000-8000-00805f9b34fb
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #0 is 00002a29-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #2 is 00002a26-0000-1000-8000-00805f9b34fb
            "Software Revision", // #3 is 00002a28-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #4 is 00002a27-0000-1000-8000-00805f9b34fb
            "Serial Number", // #5 is 00002a25-0000-1000-8000-00805f9b34fb
            "Status_Device", // #0 is 6974f5e5-0e54-45c3-97dd-29e4b5fb0849
            "Status_Status", // #1 is 3dba36e1-6120-4706-8dfd-ed9c16e569b6
            "Status_Device_Name", // #2 is 7f0375de-077e-4555-8f78-800494509cc3
            "Status_Flash_LED", // #3 is ec9bb1f3-05a9-4277-8dd0-60a7896f0d6e
            "Status_Light_LED", // #4 is aaf3f6d5-43d4-4a83-9510-dff3d858d4cc
            "UnknownStatusValues", // #5 is 8fe5b5a9-b5b4-4a7b-8ff2-87224b970f89
            "MM_Settings", // #0 is 53dc9a7a-bc19-4280-b76b-002d0e23b078
            "MM_Data", // #1 is 047d3559-8bee-423a-b229-4417fa603b90
            "DSO_Settings", // #0 is a81af1b6-b8b3-4244-8859-3da368d2be39
            "DSO_Reading", // #1 is 98e14f8e-536e-4f24-b4f4-1debfed0a99e
            "DSO_Metadata", // #2 is 970f00ba-f46f-4825-96a8-153a5cd0cda9
            "DataLogger_Settings", // #0 is 5f97c62b-a83b-46c6-b9cd-cac59e130a78
            "DataLogger_Reading", // #1 is 3c669dab-fc86-411c-9498-4f9415049cc0
            "DataLogger_MetaData", // #2 is 9acada2e-3936-430b-a8f7-da407d97ca6e
            "CalbrateTemperature", // #0 is 0cd0f713-f5aa-4572-9e23-f8049f6bcaaa
            "CalibrateUnknown1", // #1 is b6728f91-409c-4d6c-864e-272a6a7a0204
            "CalibrrateUnknown2", // #2 is 5588e47b-cb81-4f7b-acc4-6029a3f39f72
            "OTA_Control", // #0 is f7bf3564-fb6d-4e53-88a4-5e37e0326063
            "OTA_Data", // #1 is 984227f3-34fc-4045-a5d0-2c581f81a153

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

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3,  },
            new HashSet<int>(){ 4, 5,  },
            new HashSet<int>(){ 6, 7, 8, 9, 10, 11,  },
            new HashSet<int>(){ 12, 13, 14, 15, 16, 17,  },
            new HashSet<int>(){ 18, 19,  },
            new HashSet<int>(){ 20, 21, 22,  },
            new HashSet<int>(){ 23, 24, 25,  },
            new HashSet<int>(){ 26, 27, 28,  },
            new HashSet<int>(){ 29, 30,  },

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




        private double _Server_Supported_Features = 0;
        private bool _Server_Supported_Features_set = false;
        public double Server_Supported_Features
        {
            get { return _Server_Supported_Features; }
            internal set { if (_Server_Supported_Features_set && value == _Server_Supported_Features) return; _Server_Supported_Features = value; _Server_Supported_Features_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadServer_Supported_Features(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(0, "Server_Supported_Features", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|FeatureBitmap0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Server_Supported_Features = parseResult.ValueList.GetValue("FeatureBitmap0").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Server_Supported_Features
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteServer_Supported_Features(byte FeatureBitmap0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(FeatureBitmap0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(0, "Server_Supported_Features", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Client_Supported_Features = 0;
        private bool _Client_Supported_Features_set = false;
        public double Client_Supported_Features
        {
            get { return _Client_Supported_Features; }
            internal set { if (_Client_Supported_Features_set && value == _Client_Supported_Features) return; _Client_Supported_Features = value; _Client_Supported_Features_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadClient_Supported_Features(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(2, "Client_Supported_Features", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|FeatureBitmap0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Client_Supported_Features = parseResult.ValueList.GetValue("FeatureBitmap0").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Client_Supported_Features
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteClient_Supported_Features(byte FeatureBitmap0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(FeatureBitmap0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(2, "Client_Supported_Features", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Database_Hash_Hash0 = 0;
        private bool _Database_Hash_Hash0_set = false;
        public double Database_Hash_Hash0
        {
            get { return _Database_Hash_Hash0; }
            internal set { if (_Database_Hash_Hash0_set && value == _Database_Hash_Hash0) return; _Database_Hash_Hash0 = value; _Database_Hash_Hash0_set = true; OnPropertyChanged(); }
        }

        private double _Database_Hash_Hash1 = 0;
        private bool _Database_Hash_Hash1_set = false;
        public double Database_Hash_Hash1
        {
            get { return _Database_Hash_Hash1; }
            internal set { if (_Database_Hash_Hash1_set && value == _Database_Hash_Hash1) return; _Database_Hash_Hash1 = value; _Database_Hash_Hash1_set = true; OnPropertyChanged(); }
        }

        private double _Database_Hash_Hash2 = 0;
        private bool _Database_Hash_Hash2_set = false;
        public double Database_Hash_Hash2
        {
            get { return _Database_Hash_Hash2; }
            internal set { if (_Database_Hash_Hash2_set && value == _Database_Hash_Hash2) return; _Database_Hash_Hash2 = value; _Database_Hash_Hash2_set = true; OnPropertyChanged(); }
        }

        private double _Database_Hash_Hash3 = 0;
        private bool _Database_Hash_Hash3_set = false;
        public double Database_Hash_Hash3
        {
            get { return _Database_Hash_Hash3; }
            internal set { if (_Database_Hash_Hash3_set && value == _Database_Hash_Hash3) return; _Database_Hash_Hash3 = value; _Database_Hash_Hash3_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDatabase_Hash(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(3, "Database_Hash", cacheMode);
            if (result == null) return null;

            var datameaning = "U32|HEX|Hash0 U32|HEX|Hash1 U32|HEX|Hash2 U32|HEX|Hash3";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Database_Hash_Hash0 = parseResult.ValueList.GetValue("Hash0").AsDouble;

            Database_Hash_Hash1 = parseResult.ValueList.GetValue("Hash1").AsDouble;

            Database_Hash_Hash2 = parseResult.ValueList.GetValue("Hash2").AsDouble;

            Database_Hash_Hash3 = parseResult.ValueList.GetValue("Hash3").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(4, "Device_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Device_Name = parseResult.ValueList.GetValue("Device_Name").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
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
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(4, "Device_Name", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            IBuffer result = await ReadAsync(5, "Appearance", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|Speciality^Appearance|Appearance";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Appearance = parseResult.ValueList.GetValue("Appearance").AsDouble;

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
            IBuffer result = await ReadAsync(6, "Manufacturer_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Manufacturer_Name = parseResult.ValueList.GetValue("param0").AsString;

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
            IBuffer result = await ReadAsync(7, "Model_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Model_Number = parseResult.ValueList.GetValue("param0").AsString;

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

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Firmware_Revision = parseResult.ValueList.GetValue("param0").AsString;

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
            IBuffer result = await ReadAsync(9, "Software_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Software_Revision = parseResult.ValueList.GetValue("param0").AsString;

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
            IBuffer result = await ReadAsync(10, "Hardware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Hardware_Revision = parseResult.ValueList.GetValue("param0").AsString;

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
            IBuffer result = await ReadAsync(11, "Serial_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Serial_Number = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        private double _Status_Device_FirmwareMajor = 0;
        private bool _Status_Device_FirmwareMajor_set = false;
        public double Status_Device_FirmwareMajor
        {
            get { return _Status_Device_FirmwareMajor; }
            internal set { if (_Status_Device_FirmwareMajor_set && value == _Status_Device_FirmwareMajor) return; _Status_Device_FirmwareMajor = value; _Status_Device_FirmwareMajor_set = true; OnPropertyChanged(); }
        }

        private double _Status_Device_FirmwareMinor = 0;
        private bool _Status_Device_FirmwareMinor_set = false;
        public double Status_Device_FirmwareMinor
        {
            get { return _Status_Device_FirmwareMinor; }
            internal set { if (_Status_Device_FirmwareMinor_set && value == _Status_Device_FirmwareMinor) return; _Status_Device_FirmwareMinor = value; _Status_Device_FirmwareMinor_set = true; OnPropertyChanged(); }
        }

        private double _Status_Device_MaxInputVoltage = 0;
        private bool _Status_Device_MaxInputVoltage_set = false;
        public double Status_Device_MaxInputVoltage
        {
            get { return _Status_Device_MaxInputVoltage; }
            internal set { if (_Status_Device_MaxInputVoltage_set && value == _Status_Device_MaxInputVoltage) return; _Status_Device_MaxInputVoltage = value; _Status_Device_MaxInputVoltage_set = true; OnPropertyChanged(); }
        }

        private double _Status_Device_MaxInputCurrent = 0;
        private bool _Status_Device_MaxInputCurrent_set = false;
        public double Status_Device_MaxInputCurrent
        {
            get { return _Status_Device_MaxInputCurrent; }
            internal set { if (_Status_Device_MaxInputCurrent_set && value == _Status_Device_MaxInputCurrent) return; _Status_Device_MaxInputCurrent = value; _Status_Device_MaxInputCurrent_set = true; OnPropertyChanged(); }
        }

        private double _Status_Device_MaxInputResistance = 0;
        private bool _Status_Device_MaxInputResistance_set = false;
        public double Status_Device_MaxInputResistance
        {
            get { return _Status_Device_MaxInputResistance; }
            internal set { if (_Status_Device_MaxInputResistance_set && value == _Status_Device_MaxInputResistance) return; _Status_Device_MaxInputResistance = value; _Status_Device_MaxInputResistance_set = true; OnPropertyChanged(); }
        }

        private double _Status_Device_MaxSamplingRate = 0;
        private bool _Status_Device_MaxSamplingRate_set = false;
        public double Status_Device_MaxSamplingRate
        {
            get { return _Status_Device_MaxSamplingRate; }
            internal set { if (_Status_Device_MaxSamplingRate_set && value == _Status_Device_MaxSamplingRate) return; _Status_Device_MaxSamplingRate = value; _Status_Device_MaxSamplingRate_set = true; OnPropertyChanged(); }
        }

        private double _Status_Device_DeviceBufferSize = 0;
        private bool _Status_Device_DeviceBufferSize_set = false;
        public double Status_Device_DeviceBufferSize
        {
            get { return _Status_Device_DeviceBufferSize; }
            internal set { if (_Status_Device_DeviceBufferSize_set && value == _Status_Device_DeviceBufferSize) return; _Status_Device_DeviceBufferSize = value; _Status_Device_DeviceBufferSize_set = true; OnPropertyChanged(); }
        }

        private double _Status_Device_Reserved01 = 0;
        private bool _Status_Device_Reserved01_set = false;
        public double Status_Device_Reserved01
        {
            get { return _Status_Device_Reserved01; }
            internal set { if (_Status_Device_Reserved01_set && value == _Status_Device_Reserved01) return; _Status_Device_Reserved01 = value; _Status_Device_Reserved01_set = true; OnPropertyChanged(); }
        }

        private string _Status_Device_MacAddress = null;
        private bool _Status_Device_MacAddress_set = false;
        public string Status_Device_MacAddress
        {
            get { return _Status_Device_MacAddress; }
            internal set { if (_Status_Device_MacAddress_set && value == _Status_Device_MacAddress) return; _Status_Device_MacAddress = value; _Status_Device_MacAddress_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadStatus_Device(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(12, "Status_Device", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|FirmwareMajor U8|HEX|FirmwareMinor U16|HEX|MaxInputVoltage U16|HEX|MaxInputCurrent U16|HEX|MaxInputResistance U16|HEX|MaxSamplingRate U16|HEX|DeviceBufferSize U16|HEX|Reserved01 BYTES|HEX|MacAddress";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Status_Device_FirmwareMajor = parseResult.ValueList.GetValue("FirmwareMajor").AsDouble;

            Status_Device_FirmwareMinor = parseResult.ValueList.GetValue("FirmwareMinor").AsDouble;

            Status_Device_MaxInputVoltage = parseResult.ValueList.GetValue("MaxInputVoltage").AsDouble;

            Status_Device_MaxInputCurrent = parseResult.ValueList.GetValue("MaxInputCurrent").AsDouble;

            Status_Device_MaxInputResistance = parseResult.ValueList.GetValue("MaxInputResistance").AsDouble;

            Status_Device_MaxSamplingRate = parseResult.ValueList.GetValue("MaxSamplingRate").AsDouble;

            Status_Device_DeviceBufferSize = parseResult.ValueList.GetValue("DeviceBufferSize").AsDouble;

            Status_Device_Reserved01 = parseResult.ValueList.GetValue("Reserved01").AsDouble;

            Status_Device_MacAddress = parseResult.ValueList.GetValue("MacAddress").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        private double _Status_Status_DeviceStatus = 0;
        private bool _Status_Status_DeviceStatus_set = false;
        public double Status_Status_DeviceStatus
        {
            get { return _Status_Status_DeviceStatus; }
            internal set { if (_Status_Status_DeviceStatus_set && value == _Status_Status_DeviceStatus) return; _Status_Status_DeviceStatus = value; _Status_Status_DeviceStatus_set = true; OnPropertyChanged(); }
        }

        private double _Status_Status_BatteryLevel = 0.0;
        private bool _Status_Status_BatteryLevel_set = false;
        public double Status_Status_BatteryLevel
        {
            get { return _Status_Status_BatteryLevel; }
            internal set { if (_Status_Status_BatteryLevel_set && value == _Status_Status_BatteryLevel) return; _Status_Status_BatteryLevel = value; _Status_Status_BatteryLevel_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadStatus_Status(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(13, "Status_Status", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|DeviceStatus F32|FIXED|BatteryLevel";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Status_Status_DeviceStatus = parseResult.ValueList.GetValue("DeviceStatus").AsDouble;

            Status_Status_BatteryLevel = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Status_StatusEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Status_StatusEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyStatus_Status_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyStatus_StatusAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[13];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyStatus_Status_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyStatus_Status_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyStatus_StatusCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyStatus_Status: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyStatus_Status: set notification", result);

            return true;
        }

        private void NotifyStatus_StatusCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "U8|HEX|DeviceStatus F32|FIXED|BatteryLevel";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            Status_Status_DeviceStatus = parseResult.ValueList.GetValue("DeviceStatus").AsDouble;

            Status_Status_BatteryLevel = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

            Status_StatusEvent?.Invoke(parseResult);

        }

        public void NotifyStatus_StatusRemoveCharacteristicCallback()
        {
            var ch = Characteristics[13];
            if (ch == null) return;
            NotifyStatus_Status_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyStatus_StatusCallback;
        }


        private string _Status_Device_Name = "";
        private bool _Status_Device_Name_set = false;
        public string Status_Device_Name
        {
            get { return _Status_Device_Name; }
            internal set { if (_Status_Device_Name_set && value == _Status_Device_Name) return; _Status_Device_Name = value; _Status_Device_Name_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadStatus_Device_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(14, "Status_Device_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Status_Device_Name = parseResult.ValueList.GetValue("Device_Name").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Status_Device_Name
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteStatus_Device_Name(String Device_Name)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(Device_Name);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(14, "Status_Device_Name", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Status_Flash_LED_Red = 0;
        private bool _Status_Flash_LED_Red_set = false;
        public double Status_Flash_LED_Red
        {
            get { return _Status_Flash_LED_Red; }
            internal set { if (_Status_Flash_LED_Red_set && value == _Status_Flash_LED_Red) return; _Status_Flash_LED_Red = value; _Status_Flash_LED_Red_set = true; OnPropertyChanged(); }
        }

        private double _Status_Flash_LED_Green = 0;
        private bool _Status_Flash_LED_Green_set = false;
        public double Status_Flash_LED_Green
        {
            get { return _Status_Flash_LED_Green; }
            internal set { if (_Status_Flash_LED_Green_set && value == _Status_Flash_LED_Green) return; _Status_Flash_LED_Green = value; _Status_Flash_LED_Green_set = true; OnPropertyChanged(); }
        }

        private double _Status_Flash_LED_Blue = 0;
        private bool _Status_Flash_LED_Blue_set = false;
        public double Status_Flash_LED_Blue
        {
            get { return _Status_Flash_LED_Blue; }
            internal set { if (_Status_Flash_LED_Blue_set && value == _Status_Flash_LED_Blue) return; _Status_Flash_LED_Blue = value; _Status_Flash_LED_Blue_set = true; OnPropertyChanged(); }
        }

        private double _Status_Flash_LED_Beep = 0;
        private bool _Status_Flash_LED_Beep_set = false;
        public double Status_Flash_LED_Beep
        {
            get { return _Status_Flash_LED_Beep; }
            internal set { if (_Status_Flash_LED_Beep_set && value == _Status_Flash_LED_Beep) return; _Status_Flash_LED_Beep = value; _Status_Flash_LED_Beep_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadStatus_Flash_LED(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(15, "Status_Flash_LED", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Red U8|HEX|Green U8|HEX|Blue U8|HEX|Beep";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Status_Flash_LED_Red = parseResult.ValueList.GetValue("Red").AsDouble;

            Status_Flash_LED_Green = parseResult.ValueList.GetValue("Green").AsDouble;

            Status_Flash_LED_Blue = parseResult.ValueList.GetValue("Blue").AsDouble;

            Status_Flash_LED_Beep = parseResult.ValueList.GetValue("Beep").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Status_Flash_LED
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteStatus_Flash_LED(byte Red, byte Green, byte Blue, byte Beep)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Red);
            dw.WriteByte(Green);
            dw.WriteByte(Blue);
            dw.WriteByte(Beep);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(15, "Status_Flash_LED", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _Status_Light_LED = 0;
        private bool _Status_Light_LED_set = false;
        public double Status_Light_LED
        {
            get { return _Status_Light_LED; }
            internal set { if (_Status_Light_LED_set && value == _Status_Light_LED) return; _Status_Light_LED = value; _Status_Light_LED_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadStatus_Light_LED(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(16, "Status_Light_LED", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Light";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Status_Light_LED = parseResult.ValueList.GetValue("Light").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Status_Light_LEDEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Status_Light_LEDEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyStatus_Light_LED_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyStatus_Light_LEDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[16];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyStatus_Light_LED_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyStatus_Light_LED_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyStatus_Light_LEDCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyStatus_Light_LED: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyStatus_Light_LED: set notification", result);

            return true;
        }

        private void NotifyStatus_Light_LEDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "U8|HEX|Light";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            Status_Light_LED = parseResult.ValueList.GetValue("Light").AsDouble;

            Status_Light_LEDEvent?.Invoke(parseResult);

        }

        public void NotifyStatus_Light_LEDRemoveCharacteristicCallback()
        {
            var ch = Characteristics[16];
            if (ch == null) return;
            NotifyStatus_Light_LED_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyStatus_Light_LEDCallback;
        }

        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Status_Light_LED
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteStatus_Light_LED(byte Light)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Light);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(16, "Status_Light_LED", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _UnknownStatusValues = null;
        private bool _UnknownStatusValues_set = false;
        public string UnknownStatusValues
        {
            get { return _UnknownStatusValues; }
            internal set { if (_UnknownStatusValues_set && value == _UnknownStatusValues) return; _UnknownStatusValues = value; _UnknownStatusValues_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUnknownStatusValues(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(17, "UnknownStatusValues", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|StatusUnknown5";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            UnknownStatusValues = parseResult.ValueList.GetValue("StatusUnknown5").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; UnknownStatusValuesEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent UnknownStatusValuesEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyUnknownStatusValues_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyUnknownStatusValuesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[17];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyUnknownStatusValues_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyUnknownStatusValues_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyUnknownStatusValuesCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyUnknownStatusValues: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyUnknownStatusValues: set notification", result);

            return true;
        }

        private void NotifyUnknownStatusValuesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "BYTES|HEX|StatusUnknown5";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            UnknownStatusValues = parseResult.ValueList.GetValue("StatusUnknown5").AsString;

            UnknownStatusValuesEvent?.Invoke(parseResult);

        }

        public void NotifyUnknownStatusValuesRemoveCharacteristicCallback()
        {
            var ch = Characteristics[17];
            if (ch == null) return;
            NotifyUnknownStatusValues_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyUnknownStatusValuesCallback;
        }

        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for UnknownStatusValues
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUnknownStatusValues(byte[] StatusUnknown5)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(StatusUnknown5);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(17, "UnknownStatusValues", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for MM_Settings
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMM_Settings(byte Mode, byte Range, UInt32 Interval)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Mode);
            dw.WriteByte(Range);
            dw.WriteUInt32(Interval);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(18, "MM_Settings", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private double _MM_Data_Status = 0;
        private bool _MM_Data_Status_set = false;
        public double MM_Data_Status
        {
            get { return _MM_Data_Status; }
            internal set { if (_MM_Data_Status_set && value == _MM_Data_Status) return; _MM_Data_Status = value; _MM_Data_Status_set = true; OnPropertyChanged(); }
        }

        private double _MM_Data_Data = 0.0;
        private bool _MM_Data_Data_set = false;
        public double MM_Data_Data
        {
            get { return _MM_Data_Data; }
            internal set { if (_MM_Data_Data_set && value == _MM_Data_Data) return; _MM_Data_Data = value; _MM_Data_Data_set = true; OnPropertyChanged(); }
        }

        private double _MM_Data_OperationMode = 0;
        private bool _MM_Data_OperationMode_set = false;
        public double MM_Data_OperationMode
        {
            get { return _MM_Data_OperationMode; }
            internal set { if (_MM_Data_OperationMode_set && value == _MM_Data_OperationMode) return; _MM_Data_OperationMode = value; _MM_Data_OperationMode_set = true; OnPropertyChanged(); }
        }

        private double _MM_Data_Range = 0;
        private bool _MM_Data_Range_set = false;
        public double MM_Data_Range
        {
            get { return _MM_Data_Range; }
            internal set { if (_MM_Data_Range_set && value == _MM_Data_Range) return; _MM_Data_Range = value; _MM_Data_Range_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMM_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(19, "MM_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Status F32|FIXED|Data U8|HEX|OperationMode U8|HEX|Range";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            MM_Data_Status = parseResult.ValueList.GetValue("Status").AsDouble;

            MM_Data_Data = parseResult.ValueList.GetValue("Data").AsDouble;

            MM_Data_OperationMode = parseResult.ValueList.GetValue("OperationMode").AsDouble;

            MM_Data_Range = parseResult.ValueList.GetValue("Range").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; MM_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent MM_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMM_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMM_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[19];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMM_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMM_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyMM_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMM_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMM_Data: set notification", result);

            return true;
        }

        private void NotifyMM_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "U8|HEX|Status F32|FIXED|Data U8|HEX|OperationMode U8|HEX|Range";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            MM_Data_Status = parseResult.ValueList.GetValue("Status").AsDouble;

            MM_Data_Data = parseResult.ValueList.GetValue("Data").AsDouble;

            MM_Data_OperationMode = parseResult.ValueList.GetValue("OperationMode").AsDouble;

            MM_Data_Range = parseResult.ValueList.GetValue("Range").AsDouble;

            MM_DataEvent?.Invoke(parseResult);

        }

        public void NotifyMM_DataRemoveCharacteristicCallback()
        {
            var ch = Characteristics[19];
            if (ch == null) return;
            NotifyMM_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyMM_DataCallback;
        }

        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for DSO_Settings
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteDSO_Settings(byte DsoTriggerType, float DsoTriggerLevel, byte DsoMode, byte DsoRange, UInt32 DsoSamplingWindow, UInt16 DsoNSamples)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(DsoTriggerType);
            dw.WriteSingle(DsoTriggerLevel);
            dw.WriteByte(DsoMode);
            dw.WriteByte(DsoRange);
            dw.WriteUInt32(DsoSamplingWindow);
            dw.WriteUInt16(DsoNSamples);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(20, "DSO_Settings", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _DSO_Reading = null;
        private bool _DSO_Reading_set = false;
        public string DSO_Reading
        {
            get { return _DSO_Reading; }
            internal set { if (_DSO_Reading_set && value == _DSO_Reading) return; _DSO_Reading = value; _DSO_Reading_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; DSO_ReadingEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent DSO_ReadingEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyDSO_Reading_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDSO_ReadingAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[21];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyDSO_Reading_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyDSO_Reading_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDSO_ReadingCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyDSO_Reading: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyDSO_Reading: set notification", result);

            return true;
        }

        private void NotifyDSO_ReadingCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "BYTES|HEX|DsoDataRaw";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            DSO_Reading = parseResult.ValueList.GetValue("DsoDataRaw").AsString;

            DSO_ReadingEvent?.Invoke(parseResult);

        }

        public void NotifyDSO_ReadingRemoveCharacteristicCallback()
        {
            var ch = Characteristics[21];
            if (ch == null) return;
            NotifyDSO_Reading_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDSO_ReadingCallback;
        }


        private double _DSO_Metadata_DsoStatus = 0;
        private bool _DSO_Metadata_DsoStatus_set = false;
        public double DSO_Metadata_DsoStatus
        {
            get { return _DSO_Metadata_DsoStatus; }
            internal set { if (_DSO_Metadata_DsoStatus_set && value == _DSO_Metadata_DsoStatus) return; _DSO_Metadata_DsoStatus = value; _DSO_Metadata_DsoStatus_set = true; OnPropertyChanged(); }
        }

        private double _DSO_Metadata_DsoDataScale = 0.0;
        private bool _DSO_Metadata_DsoDataScale_set = false;
        public double DSO_Metadata_DsoDataScale
        {
            get { return _DSO_Metadata_DsoDataScale; }
            internal set { if (_DSO_Metadata_DsoDataScale_set && value == _DSO_Metadata_DsoDataScale) return; _DSO_Metadata_DsoDataScale = value; _DSO_Metadata_DsoDataScale_set = true; OnPropertyChanged(); }
        }

        private double _DSO_Metadata_DsoDataMode = 0;
        private bool _DSO_Metadata_DsoDataMode_set = false;
        public double DSO_Metadata_DsoDataMode
        {
            get { return _DSO_Metadata_DsoDataMode; }
            internal set { if (_DSO_Metadata_DsoDataMode_set && value == _DSO_Metadata_DsoDataMode) return; _DSO_Metadata_DsoDataMode = value; _DSO_Metadata_DsoDataMode_set = true; OnPropertyChanged(); }
        }

        private double _DSO_Metadata_DsoDataRange = 0;
        private bool _DSO_Metadata_DsoDataRange_set = false;
        public double DSO_Metadata_DsoDataRange
        {
            get { return _DSO_Metadata_DsoDataRange; }
            internal set { if (_DSO_Metadata_DsoDataRange_set && value == _DSO_Metadata_DsoDataRange) return; _DSO_Metadata_DsoDataRange = value; _DSO_Metadata_DsoDataRange_set = true; OnPropertyChanged(); }
        }

        private double _DSO_Metadata_DsoDataSamplingWindow = 0;
        private bool _DSO_Metadata_DsoDataSamplingWindow_set = false;
        public double DSO_Metadata_DsoDataSamplingWindow
        {
            get { return _DSO_Metadata_DsoDataSamplingWindow; }
            internal set { if (_DSO_Metadata_DsoDataSamplingWindow_set && value == _DSO_Metadata_DsoDataSamplingWindow) return; _DSO_Metadata_DsoDataSamplingWindow = value; _DSO_Metadata_DsoDataSamplingWindow_set = true; OnPropertyChanged(); }
        }

        private double _DSO_Metadata_DsoDataNsamples = 0;
        private bool _DSO_Metadata_DsoDataNsamples_set = false;
        public double DSO_Metadata_DsoDataNsamples
        {
            get { return _DSO_Metadata_DsoDataNsamples; }
            internal set { if (_DSO_Metadata_DsoDataNsamples_set && value == _DSO_Metadata_DsoDataNsamples) return; _DSO_Metadata_DsoDataNsamples = value; _DSO_Metadata_DsoDataNsamples_set = true; OnPropertyChanged(); }
        }

        private double _DSO_Metadata_DsoSamplingRate = 0;
        private bool _DSO_Metadata_DsoSamplingRate_set = false;
        public double DSO_Metadata_DsoSamplingRate
        {
            get { return _DSO_Metadata_DsoSamplingRate; }
            internal set { if (_DSO_Metadata_DsoSamplingRate_set && value == _DSO_Metadata_DsoSamplingRate) return; _DSO_Metadata_DsoSamplingRate = value; _DSO_Metadata_DsoSamplingRate_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDSO_Metadata(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(22, "DSO_Metadata", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|DsoStatus F32|FIXED|DsoDataScale U8|HEX|DsoDataMode U8|HEX|DsoDataRange U32|DEC|DsoDataSamplingWindow U16|DEC|DsoDataNsamples U32|DEC|DsoSamplingRate|hz";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            DSO_Metadata_DsoStatus = parseResult.ValueList.GetValue("DsoStatus").AsDouble;

            DSO_Metadata_DsoDataScale = parseResult.ValueList.GetValue("DsoDataScale").AsDouble;

            DSO_Metadata_DsoDataMode = parseResult.ValueList.GetValue("DsoDataMode").AsDouble;

            DSO_Metadata_DsoDataRange = parseResult.ValueList.GetValue("DsoDataRange").AsDouble;

            DSO_Metadata_DsoDataSamplingWindow = parseResult.ValueList.GetValue("DsoDataSamplingWindow").AsDouble;

            DSO_Metadata_DsoDataNsamples = parseResult.ValueList.GetValue("DsoDataNsamples").AsDouble;

            DSO_Metadata_DsoSamplingRate = parseResult.ValueList.GetValue("DsoSamplingRate").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; DSO_MetadataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent DSO_MetadataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyDSO_Metadata_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDSO_MetadataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[22];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyDSO_Metadata_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyDSO_Metadata_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDSO_MetadataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyDSO_Metadata: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyDSO_Metadata: set notification", result);

            return true;
        }

        private void NotifyDSO_MetadataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "U8|HEX|DsoStatus F32|FIXED|DsoDataScale U8|HEX|DsoDataMode U8|HEX|DsoDataRange U32|DEC|DsoDataSamplingWindow U16|DEC|DsoDataNsamples U32|DEC|DsoSamplingRate|hz";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            DSO_Metadata_DsoStatus = parseResult.ValueList.GetValue("DsoStatus").AsDouble;

            DSO_Metadata_DsoDataScale = parseResult.ValueList.GetValue("DsoDataScale").AsDouble;

            DSO_Metadata_DsoDataMode = parseResult.ValueList.GetValue("DsoDataMode").AsDouble;

            DSO_Metadata_DsoDataRange = parseResult.ValueList.GetValue("DsoDataRange").AsDouble;

            DSO_Metadata_DsoDataSamplingWindow = parseResult.ValueList.GetValue("DsoDataSamplingWindow").AsDouble;

            DSO_Metadata_DsoDataNsamples = parseResult.ValueList.GetValue("DsoDataNsamples").AsDouble;

            DSO_Metadata_DsoSamplingRate = parseResult.ValueList.GetValue("DsoSamplingRate").AsDouble;

            DSO_MetadataEvent?.Invoke(parseResult);

        }

        public void NotifyDSO_MetadataRemoveCharacteristicCallback()
        {
            var ch = Characteristics[22];
            if (ch == null) return;
            NotifyDSO_Metadata_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDSO_MetadataCallback;
        }

        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for DataLogger_Settings
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteDataLogger_Settings(byte DlogCommand, UInt16 DlogReserved1, byte DlogMode, byte DlogRange, UInt16 DlogUpdateInterval, UInt32 DlogTimestamp)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(DlogCommand);
            dw.WriteUInt16(DlogReserved1);
            dw.WriteByte(DlogMode);
            dw.WriteByte(DlogRange);
            dw.WriteUInt16(DlogUpdateInterval);
            dw.WriteUInt32(DlogTimestamp);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(23, "DataLogger_Settings", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _DataLogger_Reading = null;
        private bool _DataLogger_Reading_set = false;
        public string DataLogger_Reading
        {
            get { return _DataLogger_Reading; }
            internal set { if (_DataLogger_Reading_set && value == _DataLogger_Reading) return; _DataLogger_Reading = value; _DataLogger_Reading_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; DataLogger_ReadingEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent DataLogger_ReadingEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyDataLogger_Reading_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDataLogger_ReadingAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[24];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyDataLogger_Reading_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyDataLogger_Reading_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDataLogger_ReadingCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyDataLogger_Reading: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyDataLogger_Reading: set notification", result);

            return true;
        }

        private void NotifyDataLogger_ReadingCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "BYTES|HEX|DlogData";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            DataLogger_Reading = parseResult.ValueList.GetValue("DlogData").AsString;

            DataLogger_ReadingEvent?.Invoke(parseResult);

        }

        public void NotifyDataLogger_ReadingRemoveCharacteristicCallback()
        {
            var ch = Characteristics[24];
            if (ch == null) return;
            NotifyDataLogger_Reading_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDataLogger_ReadingCallback;
        }


        private double _DataLogger_MetaData_DlogStatus = 0;
        private bool _DataLogger_MetaData_DlogStatus_set = false;
        public double DataLogger_MetaData_DlogStatus
        {
            get { return _DataLogger_MetaData_DlogStatus; }
            internal set { if (_DataLogger_MetaData_DlogStatus_set && value == _DataLogger_MetaData_DlogStatus) return; _DataLogger_MetaData_DlogStatus = value; _DataLogger_MetaData_DlogStatus_set = true; OnPropertyChanged(); }
        }

        private double _DataLogger_MetaData_DlogScale = 0.0;
        private bool _DataLogger_MetaData_DlogScale_set = false;
        public double DataLogger_MetaData_DlogScale
        {
            get { return _DataLogger_MetaData_DlogScale; }
            internal set { if (_DataLogger_MetaData_DlogScale_set && value == _DataLogger_MetaData_DlogScale) return; _DataLogger_MetaData_DlogScale = value; _DataLogger_MetaData_DlogScale_set = true; OnPropertyChanged(); }
        }

        private double _DataLogger_MetaData_DlogMode = 0;
        private bool _DataLogger_MetaData_DlogMode_set = false;
        public double DataLogger_MetaData_DlogMode
        {
            get { return _DataLogger_MetaData_DlogMode; }
            internal set { if (_DataLogger_MetaData_DlogMode_set && value == _DataLogger_MetaData_DlogMode) return; _DataLogger_MetaData_DlogMode = value; _DataLogger_MetaData_DlogMode_set = true; OnPropertyChanged(); }
        }

        private double _DataLogger_MetaData_DlogRange = 0;
        private bool _DataLogger_MetaData_DlogRange_set = false;
        public double DataLogger_MetaData_DlogRange
        {
            get { return _DataLogger_MetaData_DlogRange; }
            internal set { if (_DataLogger_MetaData_DlogRange_set && value == _DataLogger_MetaData_DlogRange) return; _DataLogger_MetaData_DlogRange = value; _DataLogger_MetaData_DlogRange_set = true; OnPropertyChanged(); }
        }

        private double _DataLogger_MetaData_DlogCurrLogging = 0;
        private bool _DataLogger_MetaData_DlogCurrLogging_set = false;
        public double DataLogger_MetaData_DlogCurrLogging
        {
            get { return _DataLogger_MetaData_DlogCurrLogging; }
            internal set { if (_DataLogger_MetaData_DlogCurrLogging_set && value == _DataLogger_MetaData_DlogCurrLogging) return; _DataLogger_MetaData_DlogCurrLogging = value; _DataLogger_MetaData_DlogCurrLogging_set = true; OnPropertyChanged(); }
        }

        private double _DataLogger_MetaData_DlogCurrNSample = 0;
        private bool _DataLogger_MetaData_DlogCurrNSample_set = false;
        public double DataLogger_MetaData_DlogCurrNSample
        {
            get { return _DataLogger_MetaData_DlogCurrNSample; }
            internal set { if (_DataLogger_MetaData_DlogCurrNSample_set && value == _DataLogger_MetaData_DlogCurrNSample) return; _DataLogger_MetaData_DlogCurrNSample = value; _DataLogger_MetaData_DlogCurrNSample_set = true; OnPropertyChanged(); }
        }

        private double _DataLogger_MetaData_DlogCurrTimestamp = 0;
        private bool _DataLogger_MetaData_DlogCurrTimestamp_set = false;
        public double DataLogger_MetaData_DlogCurrTimestamp
        {
            get { return _DataLogger_MetaData_DlogCurrTimestamp; }
            internal set { if (_DataLogger_MetaData_DlogCurrTimestamp_set && value == _DataLogger_MetaData_DlogCurrTimestamp) return; _DataLogger_MetaData_DlogCurrTimestamp = value; _DataLogger_MetaData_DlogCurrTimestamp_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDataLogger_MetaData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(25, "DataLogger_MetaData", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|DlogStatus F32|FIXED|DlogScale U8|HEx|DlogMode U8|HEX|DlogRange U16|DEC|DlogCurrLogging|us U16|DEC|DlogCurrNSample U32|DEC|DlogCurrTimestamp";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            DataLogger_MetaData_DlogStatus = parseResult.ValueList.GetValue("DlogStatus").AsDouble;

            DataLogger_MetaData_DlogScale = parseResult.ValueList.GetValue("DlogScale").AsDouble;

            DataLogger_MetaData_DlogMode = parseResult.ValueList.GetValue("DlogMode").AsDouble;

            DataLogger_MetaData_DlogRange = parseResult.ValueList.GetValue("DlogRange").AsDouble;

            DataLogger_MetaData_DlogCurrLogging = parseResult.ValueList.GetValue("DlogCurrLogging").AsDouble;

            DataLogger_MetaData_DlogCurrNSample = parseResult.ValueList.GetValue("DlogCurrNSample").AsDouble;

            DataLogger_MetaData_DlogCurrTimestamp = parseResult.ValueList.GetValue("DlogCurrTimestamp").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; DataLogger_MetaDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent DataLogger_MetaDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyDataLogger_MetaData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDataLogger_MetaDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[25];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyDataLogger_MetaData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyDataLogger_MetaData_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDataLogger_MetaDataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyDataLogger_MetaData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyDataLogger_MetaData: set notification", result);

            return true;
        }

        private void NotifyDataLogger_MetaDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "U8|HEX|DlogStatus F32|FIXED|DlogScale U8|HEx|DlogMode U8|HEX|DlogRange U16|DEC|DlogCurrLogging|us U16|DEC|DlogCurrNSample U32|DEC|DlogCurrTimestamp";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

            DataLogger_MetaData_DlogStatus = parseResult.ValueList.GetValue("DlogStatus").AsDouble;

            DataLogger_MetaData_DlogScale = parseResult.ValueList.GetValue("DlogScale").AsDouble;

            DataLogger_MetaData_DlogMode = parseResult.ValueList.GetValue("DlogMode").AsDouble;

            DataLogger_MetaData_DlogRange = parseResult.ValueList.GetValue("DlogRange").AsDouble;

            DataLogger_MetaData_DlogCurrLogging = parseResult.ValueList.GetValue("DlogCurrLogging").AsDouble;

            DataLogger_MetaData_DlogCurrNSample = parseResult.ValueList.GetValue("DlogCurrNSample").AsDouble;

            DataLogger_MetaData_DlogCurrTimestamp = parseResult.ValueList.GetValue("DlogCurrTimestamp").AsDouble;

            DataLogger_MetaDataEvent?.Invoke(parseResult);

        }

        public void NotifyDataLogger_MetaDataRemoveCharacteristicCallback()
        {
            var ch = Characteristics[25];
            if (ch == null) return;
            NotifyDataLogger_MetaData_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDataLogger_MetaDataCallback;
        }

        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for CalbrateTemperature
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteCalbrateTemperature(byte[] CalibrateUnknown0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(CalibrateUnknown0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(26, "CalbrateTemperature", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _CalibrateUnknown1 = null;
        private bool _CalibrateUnknown1_set = false;
        public string CalibrateUnknown1
        {
            get { return _CalibrateUnknown1; }
            internal set { if (_CalibrateUnknown1_set && value == _CalibrateUnknown1) return; _CalibrateUnknown1 = value; _CalibrateUnknown1_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadCalibrateUnknown1(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(27, "CalibrateUnknown1", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|CalibrateUnknown1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            CalibrateUnknown1 = parseResult.ValueList.GetValue("CalibrateUnknown1").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for CalibrateUnknown1
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteCalibrateUnknown1(byte[] CalibrateUnknown1)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(CalibrateUnknown1);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(27, "CalibrateUnknown1", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for CalibrrateUnknown2
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteCalibrrateUnknown2(byte[] CalibrrateUnknown2)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(CalibrrateUnknown2);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(28, "CalibrrateUnknown2", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for OTA_Control
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteOTA_Control(byte[] Unknown0)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Unknown0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(29, "OTA_Control", subcommand, GattWriteOption.WriteWithoutResponse);
            }
            // original: await DoWriteAsync(data);
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for OTA_Data
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteOTA_Data(byte[] Unknown1)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Unknown1);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(30, "OTA_Data", subcommand, GattWriteOption.WriteWithoutResponse);
            }
            // original: await DoWriteAsync(data);
        }

    }
}
