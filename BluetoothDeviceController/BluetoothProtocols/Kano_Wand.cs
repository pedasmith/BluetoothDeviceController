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
    /// This class was automatically generated 10/12/2019 8:48 PM
    /// </summary>

    public class Kano_Wand : INotifyPropertyChanged
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
            Guid.Parse("64a70010-f691-4b93-a6f4-0968f5b648f8"),
            Guid.Parse("64a70012-f691-4b93-a6f4-0968f5b648f8"),
            Guid.Parse("64a70011-f691-4b93-a6f4-0968f5b648f8"),
            Guid.Parse("0000fe59-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "WandSoftwareInfo",
            "HardwareControl",
            "WandData",
            "FirmwareUpdate",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
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
            Guid.Parse("8ec90003-f315-4f60-9fb8-838830daea50"), // #0 is DeviceFirmwareUpdate

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
            "DeviceFirmwareUpdate", // #0 is 8ec90003-f315-4f60-9fb8-838830daea50

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

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3,  },
            new HashSet<int>(){ 4, 5, 6,  },
            new HashSet<int>(){ 7, 8, 9, 10, 11,  },
            new HashSet<int>(){ 12, 13, 14, 15, 16, 17,  },
            new HashSet<int>(){ 18,  },

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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(4, "Maker_Name", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(5, "Version", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(6, "HardwareDescription", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(7, "Battery", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[7];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyBattery_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyBattery_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "U8|DEC|Battery";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Battery = parseResult.ValueList.GetValue("Battery").AsDouble;

                        BatteryEvent?.Invoke(parseResult);
                    };
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(8, "Vibration", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Vibration);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(8, "Vibration", command, GattWriteOption.WriteWithResponse);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(9, "LED_Control", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(OnOff);
            dw.WriteUInt16(R5G6B5);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(9, "LED_Control", command, GattWriteOption.WriteWithResponse);
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
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync(10, "Button", cacheMode);
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[10];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyButton_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyButton_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "U8|HEX|Button0";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Button = parseResult.ValueList.GetValue("Button0").AsDouble;

                        ButtonEvent?.Invoke(parseResult);
                    };
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

        /// <summary>
        /// Writes data for Keepalive
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteKeepalive(byte[] Keepalive)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Keepalive);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(11, "Keepalive", command, GattWriteOption.WriteWithResponse);
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[12];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyWandData_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyWandData_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "I16|DEC|Angle I16|DEC|LeftRight I16|DEC|Twist I16|DEC|Pitch";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        WandData_Angle = parseResult.ValueList.GetValue("Angle").AsDouble;

                        WandData_LeftRight = parseResult.ValueList.GetValue("LeftRight").AsDouble;

                        WandData_Twist = parseResult.ValueList.GetValue("Twist").AsDouble;

                        WandData_Pitch = parseResult.ValueList.GetValue("Pitch").AsDouble;

                        WandDataEvent?.Invoke(parseResult);
                    };
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

        /// <summary>
        /// Writes data for RstQuaternions
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteRstQuaternions(byte[] RawQuaternions)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(RawQuaternions);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(13, "RstQuaternions", command, GattWriteOption.WriteWithResponse);
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[14];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyRaw9Axis_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyRaw9Axis_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|RawAxisData";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Raw9Axis = parseResult.ValueList.GetValue("RawAxisData").AsString;

                        Raw9AxisEvent?.Invoke(parseResult);
                    };
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[15];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyLinear_Acc_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyLinear_Acc_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|RawLinearAcceleration";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Linear_Acc = parseResult.ValueList.GetValue("RawLinearAcceleration").AsString;

                        Linear_AccEvent?.Invoke(parseResult);
                    };
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[16];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTemperature_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTemperature_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|Temperature";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Temperature = parseResult.ValueList.GetValue("Temperature").AsString;

                        TemperatureEvent?.Invoke(parseResult);
                    };
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
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[17];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMagnetometerCalibration_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMagnetometerCalibration_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "BYTES|HEX|Calibration";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        MagnetometerCalibration = parseResult.ValueList.GetValue("Calibration").AsString;

                        MagnetometerCalibrationEvent?.Invoke(parseResult);
                    };
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

        /// <summary>
        /// Writes data for MagnetometerCalibration
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMagnetometerCalibration(byte[] Calibration)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Calibration);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(17, "MagnetometerCalibration", command, GattWriteOption.WriteWithResponse);
        }

        /// <summary>
        /// Writes data for DeviceFirmwareUpdate
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteDeviceFirmwareUpdate(byte[] FirmwareData)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(FirmwareData);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(18, "DeviceFirmwareUpdate", command, GattWriteOption.WriteWithResponse);
        }

    }
}
