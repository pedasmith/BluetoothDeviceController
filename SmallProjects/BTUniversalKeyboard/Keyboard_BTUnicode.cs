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
using BluetoothDeviceController.BleEditor;

namespace BluetoothProtocols
{
    /// <summary>
    /// The BT Unicode Keyboard protocol allows for advanced keyboards to be designed that do not require specialised keyboard mappings to be installed. These do require a specialized app to read in the data..
    /// This class was automatically generated 2023-04-21::08:20
    /// </summary>

    public  class Keyboard_BTUnicode : INotifyPropertyChanged
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
           Guid.Parse("b7b0a005-d6a5-41ed-892b-4ce97f8c0397"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("adaf0001-4369-7263-7569-74507974686e"),
           Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "BTKeyboard",
            "Common Configuration",
            "AdafruitControl2",
            "Device Info",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("b7b0a009-b23d-428f-985c-f6f26a80bf1f"), // #0 is KeyPress
            Guid.Parse("b7b0a07e-a995-4eae-9315-856e31bd7334"), // #1 is KeyCount
            Guid.Parse("b7b0a035-852f-4a31-bae4-fcd4510c444d"), // #2 is KeyVirtualCode
            Guid.Parse("b7b0a047-d291-41a3-8c2c-2f4bfa46fef9"), // #3 is KeyScanCode
            Guid.Parse("b7b0a074-e122-4a2d-ae7e-3c596cfcae3b"), // #4 is KeyUtf8
            Guid.Parse("b7b0a075-e122-4a2d-ae7e-3c596cfcae3b"), // #4 is KeyCommand //TODO: pick correct GUID
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #3 is Central Address Resolution
            // No characteristics for AdafruitControl2
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #0 is Manufacturer Name
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #1 is Software Revision
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #2 is Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #3 is Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #4 is Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #5 is Hardware Revision

        };
        String[] CharacteristicNames = new string[] {
            "KeyPress", // #0 is b7b0a009-b23d-428f-985c-f6f26a80bf1f
            "KeyCount", // #1 is b7b0a07e-a995-4eae-9315-856e31bd7334
            "KeyVirtualCode", // #2 is b7b0a035-852f-4a31-bae4-fcd4510c444d
            "KeyScanCode", // #3 is b7b0a047-d291-41a3-8c2c-2f4bfa46fef9
            "KeyUtf8", // #4 is b7b0a074-e122-4a2d-ae7e-3c596cfcae3b
            "KeyCommand", // #5 is b7b0a075-e122-4a2d-ae7e-3c596cfcae3b //TODO: correct GUID
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "Central Address Resolution", // #3 is 00002aa6-0000-1000-8000-00805f9b34fb
            // No characteristics for AdafruitControl2
            "Manufacturer Name", // #0 is 00002a29-0000-1000-8000-00805f9b34fb
            "Software Revision", // #1 is 00002a28-0000-1000-8000-00805f9b34fb
            "Model Number", // #2 is 00002a24-0000-1000-8000-00805f9b34fb
            "Serial Number", // #3 is 00002a25-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #4 is 00002a26-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #5 is 00002a27-0000-1000-8000-00805f9b34fb

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
            // No characteristics for AdafruitControl2
            null,
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3, 4, 5, },
            new HashSet<int>(){ 6, 7, 8,  9, },
            // No characteristics for AdafruitControl2
            new HashSet<int>(){ 10, 11, 12, 13, 14, 15 },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            0, // Characteristic 2
            0, // Characteristic 3
            0, // Characteristic 4
            0, // Characteristic 5
            1, // Characteristic 6
            1, // Characteristic 7
            1, // Characteristic 8
            1, // Characteristic 9
            // No characteristics for AdafruitControl2
            3, // Characteristic 10
            3, // Characteristic 11
            3, // Characteristic 12
            3, // Characteristic 13
            3, // Characteristic 14
            3, // Characteristic 15
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            KeyPress_BTKeyboard_enum = 0,
            KeyCount_BTKeyboard_enum = 1,
            KeyVirtualCode_BTKeyboard_enum = 2,
            KeyScanCode_BTKeyboard_enum = 3,
            KeyUtf8_BTKeyboard_enum = 4,
            KeyCommand_BTKeyboard_enum = 5,
            Device_Name_Common_Configuration_enum = 6,
            Appearance_Common_Configuration_enum = 7,
            Connection_Parameter_Common_Configuration_enum = 8,
            Central_Address_Resolution_Common_Configuration_enum = 9,
            // No characteristics for AdafruitControl2
            Manufacturer_Name_Device_Info_enum = 10,
            Software_Revision_Device_Info_enum = 11,
            Model_Number_Device_Info_enum = 12,
            Serial_Number_Device_Info_enum = 13,
            Firmware_Revision_Device_Info_enum = 14,
            Hardware_Revision_Device_Info_enum = 15,

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



        private double _KeyPress = 0;
        private bool _KeyPress_set = false;
        public double KeyPress
        {
            get { return _KeyPress; }
            internal set { if (_KeyPress_set && value == _KeyPress) return; _KeyPress = value; _KeyPress_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadKeyPress(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyPress_BTKeyboard_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.KeyPress_BTKeyboard_enum, "KeyPress", cacheMode);
            if (result == null) return null;

            var datameaning = "I32|HEX|Press";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            KeyPress = parseResult.ValueList.GetValue("Press").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; KeyPressEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent KeyPressEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyKeyPress_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyKeyPressAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyPress_BTKeyboard_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.KeyPress_BTKeyboard_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyKeyPress_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyKeyPress_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyKeyPressCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyKeyPress: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyKeyPress: set notification", result);

            return true;
        }

        private void NotifyKeyPressCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I32|HEX|Press";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            KeyPress = parseResult.ValueList.GetValue("Press").AsDouble;

            KeyPressEvent?.Invoke(parseResult);

        }

        public void NotifyKeyPressRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.KeyPress_BTKeyboard_enum];
            if (ch == null) return;
            NotifyKeyPress_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKeyPressCallback;
        }



        private double _KeyCount = 0;
        private bool _KeyCount_set = false;
        public double KeyCount
        {
            get { return _KeyCount; }
            internal set { if (_KeyCount_set && value == _KeyCount) return; _KeyCount = value; _KeyCount_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadKeyCount(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyCount_BTKeyboard_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.KeyCount_BTKeyboard_enum, "KeyCount", cacheMode);
            if (result == null) return null;

            var datameaning = "I32|HEX|PressCount";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            KeyCount = parseResult.ValueList.GetValue("PressCount").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; KeyCountEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent KeyCountEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyKeyCount_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyKeyCountAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyCount_BTKeyboard_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.KeyCount_BTKeyboard_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyKeyCount_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyKeyCount_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyKeyCountCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyKeyCount: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyKeyCount: set notification", result);

            return true;
        }

        private void NotifyKeyCountCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I32|HEX|PressCount";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            KeyCount = parseResult.ValueList.GetValue("PressCount").AsDouble;

            KeyCountEvent?.Invoke(parseResult);

        }

        public void NotifyKeyCountRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.KeyCount_BTKeyboard_enum];
            if (ch == null) return;
            NotifyKeyCount_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKeyCountCallback;
        }



        private double _KeyVirtualCode = 0;
        private bool _KeyVirtualCode_set = false;
        public double KeyVirtualCode
        {
            get { return _KeyVirtualCode; }
            internal set { if (_KeyVirtualCode_set && value == _KeyVirtualCode) return; _KeyVirtualCode = value; _KeyVirtualCode_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadKeyVirtualCode(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyVirtualCode_BTKeyboard_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.KeyVirtualCode_BTKeyboard_enum, "KeyVirtualCode", cacheMode);
            if (result == null) return null;

            var datameaning = "I32|HEX|VirtualCode";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            KeyVirtualCode = parseResult.ValueList.GetValue("VirtualCode").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; KeyVirtualCodeEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent KeyVirtualCodeEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyKeyVirtualCode_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyKeyVirtualCodeAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyVirtualCode_BTKeyboard_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.KeyVirtualCode_BTKeyboard_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyKeyVirtualCode_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyKeyVirtualCode_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyKeyVirtualCodeCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyKeyVirtualCode: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyKeyVirtualCode: set notification", result);

            return true;
        }

        private void NotifyKeyVirtualCodeCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I32|HEX|VirtualCode";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            KeyVirtualCode = parseResult.ValueList.GetValue("VirtualCode").AsDouble;

            KeyVirtualCodeEvent?.Invoke(parseResult);

        }

        public void NotifyKeyVirtualCodeRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.KeyVirtualCode_BTKeyboard_enum];
            if (ch == null) return;
            NotifyKeyVirtualCode_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKeyVirtualCodeCallback;
        }



        private double _KeyScanCode = 0;
        private bool _KeyScanCode_set = false;
        public double KeyScanCode
        {
            get { return _KeyScanCode; }
            internal set { if (_KeyScanCode_set && value == _KeyScanCode) return; _KeyScanCode = value; _KeyScanCode_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadKeyScanCode(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyScanCode_BTKeyboard_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.KeyScanCode_BTKeyboard_enum, "KeyScanCode", cacheMode);
            if (result == null) return null;

            var datameaning = "I32|HEX|ScanCode";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            KeyScanCode = parseResult.ValueList.GetValue("ScanCode").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; KeyScanCodeEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent KeyScanCodeEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyKeyScanCode_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyKeyScanCodeAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyScanCode_BTKeyboard_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.KeyScanCode_BTKeyboard_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyKeyScanCode_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyKeyScanCode_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyKeyScanCodeCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyKeyScanCode: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyKeyScanCode: set notification", result);

            return true;
        }

        private void NotifyKeyScanCodeCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I32|HEX|ScanCode";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            KeyScanCode = parseResult.ValueList.GetValue("ScanCode").AsDouble;

            KeyScanCodeEvent?.Invoke(parseResult);

        }

        public void NotifyKeyScanCodeRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.KeyScanCode_BTKeyboard_enum];
            if (ch == null) return;
            NotifyKeyScanCode_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKeyScanCodeCallback;
        }



        private string _KeyUtf8 = "";
        private bool _KeyUtf8_set = false;
        public string KeyUtf8
        {
            get { return _KeyUtf8; }
            internal set { if (_KeyUtf8_set && value == _KeyUtf8) return; _KeyUtf8 = value; _KeyUtf8_set = true; OnPropertyChanged(); }
        }



        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadKeyUtf8(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyUtf8_BTKeyboard_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.KeyUtf8_BTKeyboard_enum, "KeyUtf8", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Utf8";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            KeyUtf8 = parseResult.ValueList.GetValue("Utf8").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; KeyUtf8Event += _my function_
        /// </summary>
        public event BluetoothDataEvent KeyUtf8Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyKeyUtf8_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyKeyUtf8Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyUtf8_BTKeyboard_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.KeyUtf8_BTKeyboard_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyKeyUtf8_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyKeyUtf8_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyKeyUtf8Callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyKeyUtf8: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyKeyUtf8: set notification", result);

            return true;
        }

        private void NotifyKeyUtf8Callback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "STRING|ASCII|Utf8";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            KeyUtf8 = parseResult.ValueList.GetValue("Utf8").AsString;
            KeyUtf8 = BluetoothDeviceController.BleEditor.ValueParser.UnescapeString(KeyUtf8);
            KeyUtf8Event?.Invoke(parseResult);
        }

        public void NotifyKeyUtf8RemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.KeyUtf8_BTKeyboard_enum];
            if (ch == null) return;
            NotifyKeyUtf8_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKeyUtf8Callback;
        }


        private string _KeyCommand = "";
        private bool _KeyCommand_set = false;
        public string KeyCommand
        {
            get { return _KeyCommand; }
            internal set { if (_KeyCommand_set && value == _KeyCommand) return; _KeyCommand = value; _KeyCommand_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadKeyCommand(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyCommand_BTKeyboard_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.KeyCommand_BTKeyboard_enum, "KeyCommand", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Command";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            KeyCommand = parseResult.ValueList.GetValue("Command").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; KeyCommandEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent KeyCommandEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyKeyCommand_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyKeyCommandAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.KeyCommand_BTKeyboard_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.KeyCommand_BTKeyboard_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyKeyCommand_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyKeyCommand_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyKeyCommandCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyKeyCommand: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyKeyCommand: set notification", result);

            return true;
        }

        private void NotifyKeyCommandCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var datameaning = "BYTES|HEX|Command";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            KeyCommand = parseResult.ValueList.GetValue("Command").AsString;

            KeyCommandEvent?.Invoke(parseResult);

        }

        public void NotifyKeyCommandRemoveCharacteristicCallback()
        {
            var ch = Characteristics[(int)CharacteristicsEnum.KeyCommand_BTKeyboard_enum];
            if (ch == null) return;
            NotifyKeyCommand_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyKeyCommandCallback;
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




        private double _Connection_Parameter_Interval_Min = 0;
        private bool _Connection_Parameter_Interval_Min_set = false;
        public double Connection_Parameter_Interval_Min
        {
            get { return _Connection_Parameter_Interval_Min; }
            internal set { if (_Connection_Parameter_Interval_Min_set && value == _Connection_Parameter_Interval_Min) return; _Connection_Parameter_Interval_Min = value; _Connection_Parameter_Interval_Min_set = true; OnPropertyChanged(); }
        }
        private double _Connection_Parameter_Interval_Max = 0;
        private bool _Connection_Parameter_Interval_Max_set = false;
        public double Connection_Parameter_Interval_Max
        {
            get { return _Connection_Parameter_Interval_Max; }
            internal set { if (_Connection_Parameter_Interval_Max_set && value == _Connection_Parameter_Interval_Max) return; _Connection_Parameter_Interval_Max = value; _Connection_Parameter_Interval_Max_set = true; OnPropertyChanged(); }
        }
        private double _Connection_Parameter_Latency = 0;
        private bool _Connection_Parameter_Latency_set = false;
        public double Connection_Parameter_Latency
        {
            get { return _Connection_Parameter_Latency; }
            internal set { if (_Connection_Parameter_Latency_set && value == _Connection_Parameter_Latency) return; _Connection_Parameter_Latency = value; _Connection_Parameter_Latency_set = true; OnPropertyChanged(); }
        }
        private double _Connection_Parameter_Timeout = 0;
        private bool _Connection_Parameter_Timeout_set = false;
        public double Connection_Parameter_Timeout
        {
            get { return _Connection_Parameter_Timeout; }
            internal set { if (_Connection_Parameter_Timeout_set && value == _Connection_Parameter_Timeout) return; _Connection_Parameter_Timeout = value; _Connection_Parameter_Timeout_set = true; OnPropertyChanged(); }
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

            var datameaning = "U16^1.25_*|DEC|Interval_Min|ms U16^1.15_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Connection_Parameter_Interval_Min = parseResult.ValueList.GetValue("Interval_Min").AsDouble;
            Connection_Parameter_Interval_Max = parseResult.ValueList.GetValue("Interval_Max").AsDouble;
            Connection_Parameter_Latency = parseResult.ValueList.GetValue("Latency").AsDouble;
            Connection_Parameter_Timeout = parseResult.ValueList.GetValue("Timeout").AsDouble;

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




// No methods for AdafruitControl2
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





    }
}