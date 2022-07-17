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
    /// This class was automatically generated 2022-07-17::09:41
    /// </summary>

    public  class Samico_BloodPressure_BG512 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://github.com/glmck13/VitalBlues/blob/master/samico-bp.sh


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
           Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Device Info",
            "BloodPressure",
            "Battery",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #0 is System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #2 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #3 is Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #4 is Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #5 is Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #6 is Manufacturer Name
            Guid.Parse("0000fff4-0000-1000-8000-00805f9b34fb"), // #0 is Results
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #0 is BatteryLevel

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "System ID", // #0 is 00002a23-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #2 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #3 is 00002a26-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #4 is 00002a27-0000-1000-8000-00805f9b34fb
            "Software Revision", // #5 is 00002a28-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #6 is 00002a29-0000-1000-8000-00805f9b34fb
            "Results", // #0 is 0000fff4-0000-1000-8000-00805f9b34fb
            "BatteryLevel", // #0 is 00002a19-0000-1000-8000-00805f9b34fb

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

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0,  },
            new HashSet<int>(){ 1, 2, 3, 4, 5, 6, 7,  },
            new HashSet<int>(){ 8,  },
            new HashSet<int>(){ 9,  },

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
            IBuffer result = await ReadAsync(1, "System_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|System_Id";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            System_ID = parseResult.ValueList.GetValue("System_Id").AsString;

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
            IBuffer result = await ReadAsync(2, "Model_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Model_Number";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Model_Number = parseResult.ValueList.GetValue("Model_Number").AsString;

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
            IBuffer result = await ReadAsync(3, "Serial_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Serial_Number";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Serial_Number = parseResult.ValueList.GetValue("Serial_Number").AsString;

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
            IBuffer result = await ReadAsync(4, "Firmware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Firmware_Revision";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Firmware_Revision = parseResult.ValueList.GetValue("Firmware_Revision").AsString;

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
            IBuffer result = await ReadAsync(5, "Hardware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Hardware_Revision";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Hardware_Revision = parseResult.ValueList.GetValue("Hardware_Revision").AsString;

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
            IBuffer result = await ReadAsync(6, "Software_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Software_Revision";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Software_Revision = parseResult.ValueList.GetValue("Software_Revision").AsString;

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
            IBuffer result = await ReadAsync(7, "Manufacturer_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Manufacturer_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Manufacturer_Name = parseResult.ValueList.GetValue("Manufacturer_Name").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Results_CuffPressure = 0;
        private bool _Results_CuffPressure_set = false;
        public double Results_CuffPressure
        {
            get { return _Results_CuffPressure; }
            internal set { if (_Results_CuffPressure_set && value == _Results_CuffPressure) return; _Results_CuffPressure = value; _Results_CuffPressure_set = true; OnPropertyChanged(); }
        }
        private double _Results_SystolicInMMHg = 0;
        private bool _Results_SystolicInMMHg_set = false;
        public double Results_SystolicInMMHg
        {
            get { return _Results_SystolicInMMHg; }
            internal set { if (_Results_SystolicInMMHg_set && value == _Results_SystolicInMMHg) return; _Results_SystolicInMMHg = value; _Results_SystolicInMMHg_set = true; OnPropertyChanged(); }
        }
        private double _Results_DiastolicInMMHg = 0;
        private bool _Results_DiastolicInMMHg_set = false;
        public double Results_DiastolicInMMHg
        {
            get { return _Results_DiastolicInMMHg; }
            internal set { if (_Results_DiastolicInMMHg_set && value == _Results_DiastolicInMMHg) return; _Results_DiastolicInMMHg = value; _Results_DiastolicInMMHg_set = true; OnPropertyChanged(); }
        }
        private double _Results_PulseInBeatsPerMinute = 0;
        private bool _Results_PulseInBeatsPerMinute_set = false;
        public double Results_PulseInBeatsPerMinute
        {
            get { return _Results_PulseInBeatsPerMinute; }
            internal set { if (_Results_PulseInBeatsPerMinute_set && value == _Results_PulseInBeatsPerMinute) return; _Results_PulseInBeatsPerMinute = value; _Results_PulseInBeatsPerMinute_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ResultsEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ResultsEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyResults_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyResultsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[8];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyResults_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyResults_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyResultsCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyResults: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyResults: set notification", result);

            return true;
        }

        private void NotifyResultsCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "OEL U8|HEX|Cmd_Internal U8|DEC|CuffPressure U16|DEC|SystolicInMMHg U16|DEC|DiastolicInMMHg U16|DEC|Unused U16|DEC|PulseInBeatsPerMinute";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Results_CuffPressure = parseResult.ValueList.GetValue("CuffPressure").AsDouble;
            Results_SystolicInMMHg = parseResult.ValueList.GetValue("SystolicInMMHg").AsDouble;
            Results_DiastolicInMMHg = parseResult.ValueList.GetValue("DiastolicInMMHg").AsDouble;
            Results_PulseInBeatsPerMinute = parseResult.ValueList.GetValue("PulseInBeatsPerMinute").AsDouble;

            ResultsEvent?.Invoke(parseResult);

        }

        public void NotifyResultsRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[8];
            if (ch == null) return;
            NotifyResults_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyResultsCallback;
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
            IBuffer result = await ReadAsync(9, "BatteryLevel", cacheMode);
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
            var ch = Characteristics[9];
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
            var ch = Characteristics[9];
            if (ch == null) return;
            NotifyBatteryLevel_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBatteryLevelCallback;
        }




    }
}