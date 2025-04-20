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

using Utilities;

namespace BluetoothProtocols
{
    /// <summary>
    /// CozyTime Smart Wireless Thermo-Hygrometer.
    /// This class was automatically generated 2025-04-12::09:13
    /// </summary>

    public  class CozyTime_CT01 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://www.hypersynes.com/about-1?pgid=m3wtei7g-ed3b0674-2c92-4902-a7fc-c385678ee9c6
    // Link: https://play.google.com/store/apps/details?id=com.cozytime.haibosi&hl=en-US
    // Link: https://www.hypersynes.com/about-1


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("0000cec0-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Sensor_Service",
            "Common Configuration",
            "Device Info",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("0000cec1-0000-1000-8000-00805f9b34fb"), // #0 is Control
            Guid.Parse("0000cec2-0000-1000-8000-00805f9b34fb"), // #1 is Sensor_DataZZZ
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #0 is Manufacturer Name
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #2 is Firmware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #3 is Software Revision
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #4 is System ID
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #5 is PnP ID

        };
        String[] CharacteristicNames = new string[] {
            "Control", // #0 is 0000cec1-0000-1000-8000-00805f9b34fb
            "Sensor_DataZZZ", // #1 is 0000cec2-0000-1000-8000-00805f9b34fb
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #0 is 00002a29-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #2 is 00002a26-0000-1000-8000-00805f9b34fb
            "Software Revision", // #3 is 00002a28-0000-1000-8000-00805f9b34fb
            "System ID", // #4 is 00002a23-0000-1000-8000-00805f9b34fb
            "PnP ID", // #5 is 00002a50-0000-1000-8000-00805f9b34fb

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
            new HashSet<int>(){ 0, 1,  },
            new HashSet<int>(){ 2, 3,  },
            new HashSet<int>(){ 4, 5, 6, 7, 8, 9,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            1, // Characteristic 2
            1, // Characteristic 3
            2, // Characteristic 4
            2, // Characteristic 5
            2, // Characteristic 6
            2, // Characteristic 7
            2, // Characteristic 8
            2, // Characteristic 9
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Control_Sensor_Service_enum = 0,
            Sensor_DataZZZ_Sensor_Service_enum = 1,
            Device_Name_Common_Configuration_enum = 2,
            Appearance_Common_Configuration_enum = 3,
            Manufacturer_Name_Device_Info_enum = 4,
            Model_Number_Device_Info_enum = 5,
            Firmware_Revision_Device_Info_enum = 6,
            Software_Revision_Device_Info_enum = 7,
            System_ID_Device_Info_enum = 8,
            PnP_ID_Device_Info_enum = 9,

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
        public async Task<GattCommunicationStatus> EnsureCharacteristicAsync(CharacteristicsEnum characteristicIndex = CharacteristicsEnum.All_enum, bool forceReread = false)
        {
            if (Characteristics.Length == 0) return GattCommunicationStatus.Unreachable;
            if (ble == null) return GattCommunicationStatus.Unreachable; // might not be initialized yet

            if (characteristicIndex != CharacteristicsEnum.All_enum)
            {
                var serviceIndex = MapCharacteristicToService[(int)characteristicIndex];
                var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                if (serviceStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                    return serviceStatus.Status;
                }
                if (serviceStatus.Services.Count != 1)
                {
                    Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                    return GattCommunicationStatus.Unreachable;
                }
                var service = serviceStatus.Services[0];
                var characteristicsStatus = await EnsureCharacteristicOne(service, characteristicIndex);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    return characteristicsStatus.Status;
                }
                return GattCommunicationStatus.Success;
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
                        return GattCommunicationStatus.Unreachable;
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
                            return characteristicsStatus.Status;
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
            // Not quite right, but close.
            return readCharacteristics ? GattCommunicationStatus.Success : GattCommunicationStatus.Unreachable;
        }


        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="method" ></param>
        /// <param name="command" ></param>
        /// <returns></returns>
        private async Task<GattCommunicationStatus> WriteCommandAsync(CharacteristicsEnum characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
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
            return result;
        }
        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either success or failure
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



// method.list for Control
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Control = null;
        private bool _Control_set = false;
        public string Control
        {
            get { return _Control; }
            internal set { if (_Control_set && value == _Control) return; _Control = value; _Control_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadControl(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Control_Sensor_Service_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Control_Sensor_Service_enum, "Control", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Write_Reply";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Control = parseResult.ValueList.GetValue("Write_Reply").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ControlEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ControlEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyControl_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyControlAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Control_Sensor_Service_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.Control_Sensor_Service_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyControl_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyControl_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyControlCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyControl: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyControl: set notification", result);

            return true;
        }

        private void NotifyControlCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Write_Reply";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Control = parseResult.ValueList.GetValue("Write_Reply").AsString;

            ControlEvent?.Invoke(parseResult);

        }

        public void NotifyControlRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Control_Sensor_Service_enum];
            if (ch == null) return;
            NotifyControl_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyControlCallback;
        }

        /// <summary>
        /// Writes data for Control
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task<GattCommunicationStatus> WriteControl(byte[] Write_Reply)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Control_Sensor_Service_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return ensureResult;
            }

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(Write_Reply);

            var command = dw.DetachBuffer().ToArray();
            
            var retval = await WriteCommandAsync(CharacteristicsEnum.Control_Sensor_Service_enum, "Control", command, GattWriteOption.WriteWithResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.Control_Sensor_Service_enum, "Control", command, GattWriteOption.WriteWithResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.Control_Sensor_Service_enum, "Control", subcommand, GattWriteOption.WriteWithResponse);
            //}
            return retval;
        }


// method.list for Sensor_DataZZZ
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_STX = 0;
        private bool _Sensor_DataZZZ_STX_set = false;
        public double Sensor_DataZZZ_STX
        {
            get { return _Sensor_DataZZZ_STX; }
            internal set { if (_Sensor_DataZZZ_STX_set && value == _Sensor_DataZZZ_STX) return; _Sensor_DataZZZ_STX = value; _Sensor_DataZZZ_STX_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_Len = 0;
        private bool _Sensor_DataZZZ_Len_set = false;
        public double Sensor_DataZZZ_Len
        {
            get { return _Sensor_DataZZZ_Len; }
            internal set { if (_Sensor_DataZZZ_Len_set && value == _Sensor_DataZZZ_Len) return; _Sensor_DataZZZ_Len = value; _Sensor_DataZZZ_Len_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_Op = 0;
        private bool _Sensor_DataZZZ_Op_set = false;
        public double Sensor_DataZZZ_Op
        {
            get { return _Sensor_DataZZZ_Op; }
            internal set { if (_Sensor_DataZZZ_Op_set && value == _Sensor_DataZZZ_Op) return; _Sensor_DataZZZ_Op = value; _Sensor_DataZZZ_Op_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_Button = 0;
        private bool _Sensor_DataZZZ_Button_set = false;
        public double Sensor_DataZZZ_Button
        {
            get { return _Sensor_DataZZZ_Button; }
            internal set { if (_Sensor_DataZZZ_Button_set && value == _Sensor_DataZZZ_Button) return; _Sensor_DataZZZ_Button = value; _Sensor_DataZZZ_Button_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_Temperature = 0;
        private bool _Sensor_DataZZZ_Temperature_set = false;
        public double Sensor_DataZZZ_Temperature
        {
            get { return _Sensor_DataZZZ_Temperature; }
            internal set { if (_Sensor_DataZZZ_Temperature_set && value == _Sensor_DataZZZ_Temperature) return; _Sensor_DataZZZ_Temperature = value; _Sensor_DataZZZ_Temperature_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_Humidity = 0;
        private bool _Sensor_DataZZZ_Humidity_set = false;
        public double Sensor_DataZZZ_Humidity
        {
            get { return _Sensor_DataZZZ_Humidity; }
            internal set { if (_Sensor_DataZZZ_Humidity_set && value == _Sensor_DataZZZ_Humidity) return; _Sensor_DataZZZ_Humidity = value; _Sensor_DataZZZ_Humidity_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_Unknown1 = 0;
        private bool _Sensor_DataZZZ_Unknown1_set = false;
        public double Sensor_DataZZZ_Unknown1
        {
            get { return _Sensor_DataZZZ_Unknown1; }
            internal set { if (_Sensor_DataZZZ_Unknown1_set && value == _Sensor_DataZZZ_Unknown1) return; _Sensor_DataZZZ_Unknown1 = value; _Sensor_DataZZZ_Unknown1_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_DD = 0;
        private bool _Sensor_DataZZZ_DD_set = false;
        public double Sensor_DataZZZ_DD
        {
            get { return _Sensor_DataZZZ_DD; }
            internal set { if (_Sensor_DataZZZ_DD_set && value == _Sensor_DataZZZ_DD) return; _Sensor_DataZZZ_DD = value; _Sensor_DataZZZ_DD_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_HH = 0;
        private bool _Sensor_DataZZZ_HH_set = false;
        public double Sensor_DataZZZ_HH
        {
            get { return _Sensor_DataZZZ_HH; }
            internal set { if (_Sensor_DataZZZ_HH_set && value == _Sensor_DataZZZ_HH) return; _Sensor_DataZZZ_HH = value; _Sensor_DataZZZ_HH_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_MM = 0;
        private bool _Sensor_DataZZZ_MM_set = false;
        public double Sensor_DataZZZ_MM
        {
            get { return _Sensor_DataZZZ_MM; }
            internal set { if (_Sensor_DataZZZ_MM_set && value == _Sensor_DataZZZ_MM) return; _Sensor_DataZZZ_MM = value; _Sensor_DataZZZ_MM_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_SS = 0;
        private bool _Sensor_DataZZZ_SS_set = false;
        public double Sensor_DataZZZ_SS
        {
            get { return _Sensor_DataZZZ_SS; }
            internal set { if (_Sensor_DataZZZ_SS_set && value == _Sensor_DataZZZ_SS) return; _Sensor_DataZZZ_SS = value; _Sensor_DataZZZ_SS_set = true; OnPropertyChanged(); }
        }
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Sensor_DataZZZ_Final = 0;
        private bool _Sensor_DataZZZ_Final_set = false;
        public double Sensor_DataZZZ_Final
        {
            get { return _Sensor_DataZZZ_Final; }
            internal set { if (_Sensor_DataZZZ_Final_set && value == _Sensor_DataZZZ_Final) return; _Sensor_DataZZZ_Final = value; _Sensor_DataZZZ_Final_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Sensor_DataZZZEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Sensor_DataZZZEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifySensor_DataZZZ_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifySensor_DataZZZAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Sensor_DataZZZ_Sensor_Service_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.Sensor_DataZZZ_Sensor_Service_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifySensor_DataZZZ_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifySensor_DataZZZ_ValueChanged_Set = true;
                    ch.ValueChanged += NotifySensor_DataZZZCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifySensor_DataZZZ: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifySensor_DataZZZ: set notification", result);

            return true;
        }

        private void NotifySensor_DataZZZCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX^^HIDDEN|STX U8|DEC^^HIDDEN|Len U8|HEX^^HIDDEN|Op U8|HEX|Button U16^400_-_10.94_/|FIXED|Temperature|c U8|DEC|Humidity U32|HEX|Unknown1 U8|DEC|DD U8|DEC|HH U8|DEC|MM U8|DEC|SS U8|HEX^^HIDDEN|Final";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Sensor_DataZZZ_STX = parseResult.ValueList.GetValue("STX").AsDouble;
            Sensor_DataZZZ_Len = parseResult.ValueList.GetValue("Len").AsDouble;
            Sensor_DataZZZ_Op = parseResult.ValueList.GetValue("Op").AsDouble;
            Sensor_DataZZZ_Button = parseResult.ValueList.GetValue("Button").AsDouble;
            Sensor_DataZZZ_Temperature = parseResult.ValueList.GetValue("Temperature").AsDouble;
            Sensor_DataZZZ_Humidity = parseResult.ValueList.GetValue("Humidity").AsDouble;
            Sensor_DataZZZ_Unknown1 = parseResult.ValueList.GetValue("Unknown1").AsDouble;
            Sensor_DataZZZ_DD = parseResult.ValueList.GetValue("DD").AsDouble;
            Sensor_DataZZZ_HH = parseResult.ValueList.GetValue("HH").AsDouble;
            Sensor_DataZZZ_MM = parseResult.ValueList.GetValue("MM").AsDouble;
            Sensor_DataZZZ_SS = parseResult.ValueList.GetValue("SS").AsDouble;
            Sensor_DataZZZ_Final = parseResult.ValueList.GetValue("Final").AsDouble;

            Sensor_DataZZZEvent?.Invoke(parseResult);

        }

        public void NotifySensor_DataZZZRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Sensor_DataZZZ_Sensor_Service_enum];
            if (ch == null) return;
            NotifySensor_DataZZZ_ValueChanged_Set = false;
            ch.ValueChanged -= NotifySensor_DataZZZCallback;
        }



// method.list for Device_Name
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Device_Name = "";
        private bool _Device_Name_set = false;
        public string Device_Name
        {
            get { return _Device_Name; }
            internal set { if (_Device_Name_set && value == _Device_Name) return; _Device_Name = value; _Device_Name_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDevice_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Device_Name = parseResult.ValueList.GetValue("Device_Name").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




// method.list for Appearance
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private double _Appearance = 0;
        private bool _Appearance_set = false;
        public double Appearance
        {
            get { return _Appearance; }
            internal set { if (_Appearance_set && value == _Appearance) return; _Appearance = value; _Appearance_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAppearance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum, "Appearance", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|Speciality^Appearance|Appearance";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Appearance = parseResult.ValueList.GetValue("Appearance").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




// method.list for Manufacturer_Name
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Manufacturer_Name = "";
        private bool _Manufacturer_Name_set = false;
        public string Manufacturer_Name
        {
            get { return _Manufacturer_Name; }
            internal set { if (_Manufacturer_Name_set && value == _Manufacturer_Name) return; _Manufacturer_Name = value; _Manufacturer_Name_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadManufacturer_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Manufacturer_Name_Device_Info_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Manufacturer_Name_Device_Info_enum, "Manufacturer_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Manufacturer_Name = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




// method.list for Model_Number
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Model_Number = "";
        private bool _Model_Number_set = false;
        public string Model_Number
        {
            get { return _Model_Number; }
            internal set { if (_Model_Number_set && value == _Model_Number) return; _Model_Number = value; _Model_Number_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadModel_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Model_Number_Device_Info_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Model_Number_Device_Info_enum, "Model_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Model_Number = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




// method.list for Firmware_Revision
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Firmware_Revision = "";
        private bool _Firmware_Revision_set = false;
        public string Firmware_Revision
        {
            get { return _Firmware_Revision; }
            internal set { if (_Firmware_Revision_set && value == _Firmware_Revision) return; _Firmware_Revision = value; _Firmware_Revision_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadFirmware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Firmware_Revision_Device_Info_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Firmware_Revision_Device_Info_enum, "Firmware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Firmware_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




// method.list for Software_Revision
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _Software_Revision = "";
        private bool _Software_Revision_set = false;
        public string Software_Revision
        {
            get { return _Software_Revision; }
            internal set { if (_Software_Revision_set && value == _Software_Revision) return; _Software_Revision = value; _Software_Revision_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSoftware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Software_Revision_Device_Info_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Software_Revision_Device_Info_enum, "Software_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Software_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




// method.list for System_ID
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _System_ID = "";
        private bool _System_ID_set = false;
        public string System_ID
        {
            get { return _System_ID; }
            internal set { if (_System_ID_set && value == _System_ID) return; _System_ID = value; _System_ID_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSystem_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.System_ID_Device_Info_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.System_ID_Device_Info_enum, "System_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            System_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




// method.list for PnP_ID
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
        private string _PnP_ID = "";
        private bool _PnP_ID_set = false;
        public string PnP_ID
        {
            get { return _PnP_ID; }
            internal set { if (_PnP_ID_set && value == _PnP_ID) return; _PnP_ID = value; _PnP_ID_set = true; OnPropertyChanged(); }
        }

        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPnP_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum, "PnP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            PnP_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }





    }
}