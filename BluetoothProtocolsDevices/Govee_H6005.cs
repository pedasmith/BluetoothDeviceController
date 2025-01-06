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
    /// The Govee H6055 bulb is a standard lightbulb that accepts Bluetooth commands.
    /// This class was automatically generated 2025-01-05::15:26
    /// </summary>

    public partial class Govee_H6005 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://us.govee.com/products/govee-smart-bluetooth-rgbww-led-bulbs?_pos=1&_sid=58c8705e1&_ss=r
    // Link: https://github.com/chvolkmann/govee_btled/blob/master/govee_btled/bluetooth_led.py
    // Link: https://github.com/Beshelmek/govee_ble_lights/blob/master/custom_components/govee-ble-lights/light.py


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("00010203-0405-0607-0809-0a0b0c0d1910"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00010203-0405-0607-0809-0a0b0c0d1912"),

        };
        String[] ServiceNames = new string[] {
            "LED_Command",
            "Common Configuration",
            "Generic Service",
            "Device Info",
            "OtaCommand",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00010203-0405-0607-0809-0a0b0c0d2b10"), // #0 is Response
            Guid.Parse("00010203-0405-0607-0809-0a0b0c0d2b11"), // #1 is Send
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #0 is Service Changes
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #0 is PnP ID
            Guid.Parse("00010203-0405-0607-0809-0a0b0c0d2b12"), // #0 is OTA

        };
        String[] CharacteristicNames = new string[] {
            "Response", // #0 is 00010203-0405-0607-0809-0a0b0c0d2b10
            "Send", // #1 is 00010203-0405-0607-0809-0a0b0c0d2b11
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "Service Changes", // #0 is 00002a05-0000-1000-8000-00805f9b34fb
            "PnP ID", // #0 is 00002a50-0000-1000-8000-00805f9b34fb
            "OTA", // #0 is 00010203-0405-0607-0809-0a0b0c0d2b12

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

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1,  },
            new HashSet<int>(){ 2, 3, 4,  },
            new HashSet<int>(){ 5,  },
            new HashSet<int>(){ 6,  },
            new HashSet<int>(){ 7,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            1, // Characteristic 2
            1, // Characteristic 3
            1, // Characteristic 4
            2, // Characteristic 5
            3, // Characteristic 6
            4, // Characteristic 7
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Response_LED_Command_enum = 0,
            Send_LED_Command_enum = 1,
            Device_Name_Common_Configuration_enum = 2,
            Appearance_Common_Configuration_enum = 3,
            Connection_Parameter_Common_Configuration_enum = 4,
            Service_Changes_Generic_Service_enum = 5,
            PnP_ID_Device_Info_enum = 6,
            OTA_OtaCommand_enum = 7,

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



        private string _Response = null;
        private bool _Response_set = false;
        public string Response
        {
            get { return _Response; }
            internal set { if (_Response_set && value == _Response) return; _Response = value; _Response_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadResponse(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Response_LED_Command_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Response_LED_Command_enum, "Response", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Rx";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Response = parseResult.ValueList.GetValue("Rx").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Response_LED_Command_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Response_LED_Command_enum];
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
            var datameaning = "BYTES|HEX|Rx";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Response = parseResult.ValueList.GetValue("Rx").AsString;

            ResponseEvent?.Invoke(parseResult);

        }

        public void NotifyResponseRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Response_LED_Command_enum];
            if (ch == null) return;
            NotifyResponse_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyResponseCallback;
        }



        private double _Send_Start = 0;
        private bool _Send_Start_set = false;
        public double Send_Start
        {
            get { return _Send_Start; }
            internal set { if (_Send_Start_set && value == _Send_Start) return; _Send_Start = value; _Send_Start_set = true; OnPropertyChanged(); }
        }
        private double _Send_Command = 0;
        private bool _Send_Command_set = false;
        public double Send_Command
        {
            get { return _Send_Command; }
            internal set { if (_Send_Command_set && value == _Send_Command) return; _Send_Command = value; _Send_Command_set = true; OnPropertyChanged(); }
        }
        private double _Send_Mode = 0;
        private bool _Send_Mode_set = false;
        public double Send_Mode
        {
            get { return _Send_Mode; }
            internal set { if (_Send_Mode_set && value == _Send_Mode) return; _Send_Mode = value; _Send_Mode_set = true; OnPropertyChanged(); }
        }
        private double _Send_R = 0;
        private bool _Send_R_set = false;
        public double Send_R
        {
            get { return _Send_R; }
            internal set { if (_Send_R_set && value == _Send_R) return; _Send_R = value; _Send_R_set = true; OnPropertyChanged(); }
        }
        private double _Send_G = 0;
        private bool _Send_G_set = false;
        public double Send_G
        {
            get { return _Send_G; }
            internal set { if (_Send_G_set && value == _Send_G) return; _Send_G = value; _Send_G_set = true; OnPropertyChanged(); }
        }
        private double _Send_B = 0;
        private bool _Send_B_set = false;
        public double Send_B
        {
            get { return _Send_B; }
            internal set { if (_Send_B_set && value == _Send_B) return; _Send_B = value; _Send_B_set = true; OnPropertyChanged(); }
        }
        private string _Send_Blank = null;
        private bool _Send_Blank_set = false;
        public string Send_Blank
        {
            get { return _Send_Blank; }
            internal set { if (_Send_Blank_set && value == _Send_Blank) return; _Send_Blank = value; _Send_Blank_set = true; OnPropertyChanged(); }
        }
        private double _Send_CRC = 0;
        private bool _Send_CRC_set = false;
        public double Send_CRC
        {
            get { return _Send_CRC; }
            internal set { if (_Send_CRC_set && value == _Send_CRC) return; _Send_CRC = value; _Send_CRC_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSend(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Send_LED_Command_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Send_LED_Command_enum, "Send", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Start||33 U8|HEX|Command||05 U8|HEX|Mode||02 U8|HEX|R||FF U8|HEX|G||FF U8|HEX|B||00 BYTES|HEX|Blank||00_00_00_00_00_00_00_00_00_00_00_00_00 U8|HEX|CRC||00";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Send_Start = parseResult.ValueList.GetValue("Start").AsDouble;
            Send_Command = parseResult.ValueList.GetValue("Command").AsDouble;
            Send_Mode = parseResult.ValueList.GetValue("Mode").AsDouble;
            Send_R = parseResult.ValueList.GetValue("R").AsDouble;
            Send_G = parseResult.ValueList.GetValue("G").AsDouble;
            Send_B = parseResult.ValueList.GetValue("B").AsDouble;
            Send_Blank = parseResult.ValueList.GetValue("Blank").AsString;
            Send_CRC = parseResult.ValueList.GetValue("CRC").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Send
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteSend(byte Start, byte Command, byte Mode, byte R, byte G, byte B, byte[] Blank, byte CRC)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Send_LED_Command_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;

            // CHANGE: need to calculate the checksum.
            byte checksum = (byte)(Start ^ Command ^ Mode ^ R ^ G ^ B);
            CRC = checksum;



            dw.WriteByte(Start);
            dw.WriteByte(Command);
            dw.WriteByte(Mode);
            dw.WriteByte(R);
            dw.WriteByte(G);
            dw.WriteByte(B);
            dw.WriteBytes(Blank);
            dw.WriteByte(CRC);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(CharacteristicsEnum.Send_LED_Command_enum, "Send", command, GattWriteOption.WriteWithoutResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.Send_LED_Command_enum, "Send", command, GattWriteOption.WriteWithoutResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.Send_LED_Command_enum, "Send", subcommand, GattWriteOption.WriteWithoutResponse);
            //}
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

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Device_NameEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Device_NameEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyDevice_Name_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyDevice_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Device_Name_Common_Configuration_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyDevice_Name_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyDevice_Name_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyDevice_NameCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyDevice_Name: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyDevice_Name: set notification", result);

            return true;
        }

        private void NotifyDevice_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Device_Name = parseResult.ValueList.GetValue("Device_Name").AsString;

            Device_NameEvent?.Invoke(parseResult);

        }

        public void NotifyDevice_NameRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Device_Name_Common_Configuration_enum];
            if (ch == null) return;
            NotifyDevice_Name_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyDevice_NameCallback;
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Service_Changes_Generic_Service_enum)) return false;
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum, "PnP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            PnP_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _OTA = null;
        private bool _OTA_set = false;
        public string OTA
        {
            get { return _OTA; }
            internal set { if (_OTA_set && value == _OTA) return; _OTA = value; _OTA_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadOTA(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.OTA_OtaCommand_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.OTA_OtaCommand_enum, "OTA", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|OTA";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            OTA = parseResult.ValueList.GetValue("OTA").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for OTA
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteOTA(byte[] OTA)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.OTA_OtaCommand_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(OTA);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(CharacteristicsEnum.OTA_OtaCommand_enum, "OTA", command, GattWriteOption.WriteWithoutResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.OTA_OtaCommand_enum, "OTA", command, GattWriteOption.WriteWithoutResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.OTA_OtaCommand_enum, "OTA", subcommand, GattWriteOption.WriteWithoutResponse);
            //}
        }



    }
}