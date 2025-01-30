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
    /// MATICOD under-counter 'stick' LED light with Bluetooth control. Can also be controlled with a small remote, or via a tiny controller built into the power cord. Seems to use the same protocol as the ELK-BLEDOM controller..
    /// This class was automatically generated 2025-01-29::18:31
    /// </summary>

    public partial class Elk_LedLight : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://www.amazon.com/dp/B0C1N8WSXF
    // Link: https://github.com/kquinsland/JACKYLED-BLE-RGB-LED-Strip-controller
    // Link: https://github.com/arduino12/ble_rgb_led_strip_controller
    // Link: https://github.com/8none1/elk-bledob
    // Link: https://github.com/lilgallon/DynamicLedStrips
    // Link: https://github.com/TheSylex/ELK-BLEDOM-bluetooth-led-strip-controller/blob/main/README.md
    // Link: https://github.com/FergusInLondon/ELK-BLEDOM/blob/master/proof-of-concept/demo.go
    // Link: https://linuxthings.co.uk/blog/control-an-elk-bledom-bluetooth-led-strip
    // Link: https://github.com/dave-code-ruiz/elkbledom
    // Link: https://github.com/dave-code-ruiz/elkbledom/issues/11


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Commands",
            "Common Configuration",
            "Generic Service",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("0000fff3-0000-1000-8000-00805f9b34fb"), // #0 is Command
            Guid.Parse("0000fff4-0000-1000-8000-00805f9b34fb"), // #1 is Response
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #0 is Service Changes

        };
        String[] CharacteristicNames = new string[] {
            "Command", // #0 is 0000fff3-0000-1000-8000-00805f9b34fb
            "Response", // #1 is 0000fff4-0000-1000-8000-00805f9b34fb
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "Service Changes", // #0 is 00002a05-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1,  },
            new HashSet<int>(){ 2, 3, 4,  },
            new HashSet<int>(){ 5,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            1, // Characteristic 2
            1, // Characteristic 3
            1, // Characteristic 4
            2, // Characteristic 5
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Command_Commands_enum = 0,
            Response_Commands_enum = 1,
            Device_Name_Common_Configuration_enum = 2,
            Appearance_Common_Configuration_enum = 3,
            Connection_Parameter_Common_Configuration_enum = 4,
            Service_Changes_Generic_Service_enum = 5,

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



        private double _Command_Start = 0;
        private bool _Command_Start_set = false;
        public double Command_Start
        {
            get { return _Command_Start; }
            internal set { if (_Command_Start_set && value == _Command_Start) return; _Command_Start = value; _Command_Start_set = true; OnPropertyChanged(); }
        }
        private double _Command_Counter = 0;
        private bool _Command_Counter_set = false;
        public double Command_Counter
        {
            get { return _Command_Counter; }
            internal set { if (_Command_Counter_set && value == _Command_Counter) return; _Command_Counter = value; _Command_Counter_set = true; OnPropertyChanged(); }
        }
        private double _Command_Command = 0;
        private bool _Command_Command_set = false;
        public double Command_Command
        {
            get { return _Command_Command; }
            internal set { if (_Command_Command_set && value == _Command_Command) return; _Command_Command = value; _Command_Command_set = true; OnPropertyChanged(); }
        }
        private double _Command_P1 = 0;
        private bool _Command_P1_set = false;
        public double Command_P1
        {
            get { return _Command_P1; }
            internal set { if (_Command_P1_set && value == _Command_P1) return; _Command_P1 = value; _Command_P1_set = true; OnPropertyChanged(); }
        }
        private double _Command_P2 = 0;
        private bool _Command_P2_set = false;
        public double Command_P2
        {
            get { return _Command_P2; }
            internal set { if (_Command_P2_set && value == _Command_P2) return; _Command_P2 = value; _Command_P2_set = true; OnPropertyChanged(); }
        }
        private double _Command_P3 = 0;
        private bool _Command_P3_set = false;
        public double Command_P3
        {
            get { return _Command_P3; }
            internal set { if (_Command_P3_set && value == _Command_P3) return; _Command_P3 = value; _Command_P3_set = true; OnPropertyChanged(); }
        }
        private double _Command_P4 = 0;
        private bool _Command_P4_set = false;
        public double Command_P4
        {
            get { return _Command_P4; }
            internal set { if (_Command_P4_set && value == _Command_P4) return; _Command_P4 = value; _Command_P4_set = true; OnPropertyChanged(); }
        }
        private double _Command_P5 = 0;
        private bool _Command_P5_set = false;
        public double Command_P5
        {
            get { return _Command_P5; }
            internal set { if (_Command_P5_set && value == _Command_P5) return; _Command_P5 = value; _Command_P5_set = true; OnPropertyChanged(); }
        }
        private double _Command_End = 0;
        private bool _Command_End_set = false;
        public double Command_End
        {
            get { return _Command_End; }
            internal set { if (_Command_End_set && value == _Command_End) return; _Command_End = value; _Command_End_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadCommand(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Command_Commands_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
            IBuffer result = await ReadAsync(CharacteristicsEnum.Command_Commands_enum, "Command", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Start||7E U8|HEX|Counter||00 U8|HEX|Command||01 U8|HEX|P1||FF U8|HEX|P2||FF U8|HEX|P3||00 U8|HEX|P4||00 U8|HEX|P5||00 U8|HEX|End||EF";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Command_Start = parseResult.ValueList.GetValue("Start").AsDouble;
            Command_Counter = parseResult.ValueList.GetValue("Counter").AsDouble;
            Command_Command = parseResult.ValueList.GetValue("Command").AsDouble;
            Command_P1 = parseResult.ValueList.GetValue("P1").AsDouble;
            Command_P2 = parseResult.ValueList.GetValue("P2").AsDouble;
            Command_P3 = parseResult.ValueList.GetValue("P3").AsDouble;
            Command_P4 = parseResult.ValueList.GetValue("P4").AsDouble;
            Command_P5 = parseResult.ValueList.GetValue("P5").AsDouble;
            Command_End = parseResult.ValueList.GetValue("End").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Command
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task<GattCommunicationStatus> WriteCommand(byte Start, byte Counter, byte Command, byte P1, byte P2, byte P3, byte P4, byte P5, byte End)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Command_Commands_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return ensureResult;
            }

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(Start);
            dw.WriteByte(Counter);
            dw.WriteByte(Command);
            dw.WriteByte(P1);
            dw.WriteByte(P2);
            dw.WriteByte(P3);
            dw.WriteByte(P4);
            dw.WriteByte(P5);
            dw.WriteByte(End);

            var command = dw.DetachBuffer().ToArray();
            
            var retval = await WriteCommandAsync(CharacteristicsEnum.Command_Commands_enum, "Command", command, GattWriteOption.WriteWithoutResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.Command_Commands_enum, "Command", command, GattWriteOption.WriteWithoutResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.Command_Commands_enum, "Command", subcommand, GattWriteOption.WriteWithoutResponse);
            //}
            return retval;
        }


        private string _Response = null;
        private bool _Response_set = false;
        public string Response
        {
            get { return _Response; }
            internal set { if (_Response_set && value == _Response) return; _Response = value; _Response_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ResponseEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ResponseEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyResponse_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyResponseAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Response_Commands_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.Response_Commands_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyResponse_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyResponse_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyResponseCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyResponse: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyResponse: set notification", result);

            return true;
        }

        private void NotifyResponseCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Response";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Response = parseResult.ValueList.GetValue("Response").AsString;

            ResponseEvent?.Invoke(parseResult);

        }

        public void NotifyResponseRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Response_Commands_enum];
            if (ch == null) return;
            NotifyResponse_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyResponseCallback;
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
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
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




        private double _Service_Changes_StartRange = 0;
        private bool _Service_Changes_StartRange_set = false;
        public double Service_Changes_StartRange
        {
            get { return _Service_Changes_StartRange; }
            internal set { if (_Service_Changes_StartRange_set && value == _Service_Changes_StartRange) return; _Service_Changes_StartRange = value; _Service_Changes_StartRange_set = true; OnPropertyChanged(); }
        }
        private double _Service_Changes_EndRange = 0;
        private bool _Service_Changes_EndRange_set = false;
        public double Service_Changes_EndRange
        {
            get { return _Service_Changes_EndRange; }
            internal set { if (_Service_Changes_EndRange_set && value == _Service_Changes_EndRange) return; _Service_Changes_EndRange = value; _Service_Changes_EndRange_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Service_ChangesEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Service_ChangesEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyService_Changes_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyService_ChangesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.Service_Changes_Generic_Service_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

            var ch = Characteristics[(int)CharacteristicsEnum.Service_Changes_Generic_Service_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyService_Changes_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyService_Changes_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyService_ChangesCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyService_Changes: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyService_Changes: set notification", result);

            return true;
        }

        private void NotifyService_ChangesCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U16|DEC|StartRange U16|DEC|EndRange";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Service_Changes_StartRange = parseResult.ValueList.GetValue("StartRange").AsDouble;
            Service_Changes_EndRange = parseResult.ValueList.GetValue("EndRange").AsDouble;

            Service_ChangesEvent?.Invoke(parseResult);

        }

        public void NotifyService_ChangesRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Service_Changes_Generic_Service_enum];
            if (ch == null) return;
            NotifyService_Changes_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyService_ChangesCallback;
        }




    }
}