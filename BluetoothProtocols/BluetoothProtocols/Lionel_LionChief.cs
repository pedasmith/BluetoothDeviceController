﻿//From template: Protocol_Body
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
    /// This class was automatically generated 2/1/2022 10:07 PM
    /// </summary>

    public class Lionel_LionChief : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://github.com/Property404/lionchief-controller


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Guid[] ServiceGuids = new Guid[] {
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("e20a39f4-73f5-4bc4-a12f-17d1ad07a961"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Generic Service",
            "Device Info",
            "LionChief",

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
            Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb"), // #2 is Peripheral Privacy Flag
            Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb"), // #3 is Reconnect Address
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #4 is Connection Parameter
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #0 is Service Changes
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #0 is System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #2 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #3 is Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #4 is Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #5 is Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #6 is Manufacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #7 is Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #8 is PnP ID
            Guid.Parse("08590f7e-db05-467e-8757-72f6faeb13d4"), // #0 is LionelCommand
            Guid.Parse("08590f7e-db05-467e-8757-72f6faeb14d3"), // #1 is LionelData

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Peripheral Privacy Flag", // #2 is 00002a02-0000-1000-8000-00805f9b34fb
            "Reconnect Address", // #3 is 00002a03-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #4 is 00002a04-0000-1000-8000-00805f9b34fb
            "Service Changes", // #0 is 00002a05-0000-1000-8000-00805f9b34fb
            "System ID", // #0 is 00002a23-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #2 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #3 is 00002a26-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #4 is 00002a27-0000-1000-8000-00805f9b34fb
            "Software Revision", // #5 is 00002a28-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #6 is 00002a29-0000-1000-8000-00805f9b34fb
            "Regulatory List", // #7 is 00002a2a-0000-1000-8000-00805f9b34fb
            "PnP ID", // #8 is 00002a50-0000-1000-8000-00805f9b34fb
            "LionelCommand", // #0 is 08590f7e-db05-467e-8757-72f6faeb13d4
            "LionelData", // #1 is 08590f7e-db05-467e-8757-72f6faeb14d3

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

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3, 4,  },
            new HashSet<int>(){ 5,  },
            new HashSet<int>(){ 6, 7, 8, 9, 10, 11, 12, 13, 14,  },
            new HashSet<int>(){ 15, 16,  },

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

        private double _Peripheral_Privacy_Flag = 0;
        private bool _Peripheral_Privacy_Flag_set = false;
        public double Peripheral_Privacy_Flag
        {
            get { return _Peripheral_Privacy_Flag; }
            internal set { if (_Peripheral_Privacy_Flag_set && value == _Peripheral_Privacy_Flag) return; _Peripheral_Privacy_Flag = value; _Peripheral_Privacy_Flag_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPeripheral_Privacy_Flag(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(2, "Peripheral_Privacy_Flag", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Flag";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            Peripheral_Privacy_Flag = parseResult.ValueList.GetValue("Flag").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Peripheral_Privacy_Flag
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePeripheral_Privacy_Flag(byte Flag)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Flag);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(2, "Peripheral_Privacy_Flag", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Reconnect_Address
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteReconnect_Address(byte[] ReconnectAddress)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(ReconnectAddress);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(3, "Reconnect_Address", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
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
            IBuffer result = await ReadAsync(4, "Connection_Parameter", cacheMode);
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
            IBuffer result = await ReadAsync(6, "System_ID", cacheMode);
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
            IBuffer result = await ReadAsync(7, "Model_Number", cacheMode);
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
            IBuffer result = await ReadAsync(8, "Serial_Number", cacheMode);
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
            IBuffer result = await ReadAsync(9, "Firmware_Revision", cacheMode);
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
            IBuffer result = await ReadAsync(10, "Hardware_Revision", cacheMode);
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
            IBuffer result = await ReadAsync(11, "Software_Revision", cacheMode);
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
            IBuffer result = await ReadAsync(12, "Manufacturer_Name", cacheMode);
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
            IBuffer result = await ReadAsync(13, "Regulatory_List", cacheMode);
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
            IBuffer result = await ReadAsync(14, "PnP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);

            PnP_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for LionelCommand
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLionelCommand(byte Zero, byte Command, byte[] Parameters, byte Checksum)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Zero);
            dw.WriteByte(Command);
            dw.WriteBytes(Parameters);
            dw.WriteByte(Checksum);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();

                // Write to console
                var str = "COMMAND: ";
                foreach (var b in subcommand)
                {
                    str += $"{b:X2} ";
                }
                System.Diagnostics.Debug.WriteLine(str);

                await WriteCommandAsync(15, "LionelCommand", subcommand, GattWriteOption.WriteWithResponse);
            }
            // original: await DoWriteAsync(data);
        }

        private string _LionelData = null;
        private bool _LionelData_set = false;
        public string LionelData
        {
            get { return _LionelData; }
            internal set { if (_LionelData_set && value == _LionelData) return; _LionelData = value; _LionelData_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; LionelDataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent LionelDataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyLionelData_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyLionelDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[16];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyLionelData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyLionelData_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|TrainData";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        LionelData = parseResult.ValueList.GetValue("TrainData").AsString;

                        LionelDataEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyLionelData: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyLionelData: set notification", result);

            return true;
        }

    }
}