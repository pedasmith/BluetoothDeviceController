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
    /// .
    /// This class was automatically generated 2025-01-02::09:23
    /// </summary>

    public partial class Zengge_LedLight : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://github.com/8none1/zengge_lednetwf/blob/main/readme.md
    // Link: https://github.com/8none1/zengge_lednetwf/blob/main/ledwf_controller.py
    // Link: https://www.magiclightbulbs.com/product-page/60-watt-wifi-multi-color-bulb


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
           Guid.Parse("0000ffff-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "LED_Control",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("0000ff02-0000-1000-8000-00805f9b34fb"), // #0 is LED_Response
            Guid.Parse("0000ff01-0000-1000-8000-00805f9b34fb"), // #1 is LED_Write

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "LED_Response", // #0 is 0000ff02-0000-1000-8000-00805f9b34fb
            "LED_Write", // #1 is 0000ff01-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0,  },
            new HashSet<int>(){ 1, 2,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            1, // Characteristic 1
            1, // Characteristic 2
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Device_Name_Common_Configuration_enum = 0,
            LED_Response_LED_Control_enum = 1,
            LED_Write_LED_Control_enum = 2,

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




        private double _LED_Response_Junk10 = 0;
        private bool _LED_Response_Junk10_set = false;
        public double LED_Response_Junk10
        {
            get { return _LED_Response_Junk10; }
            internal set { if (_LED_Response_Junk10_set && value == _LED_Response_Junk10) return; _LED_Response_Junk10 = value; _LED_Response_Junk10_set = true; OnPropertyChanged(); }
        }
        private double _LED_Response_Counter = 0;
        private bool _LED_Response_Counter_set = false;
        public double LED_Response_Counter
        {
            get { return _LED_Response_Counter; }
            internal set { if (_LED_Response_Counter_set && value == _LED_Response_Counter) return; _LED_Response_Counter = value; _LED_Response_Counter_set = true; OnPropertyChanged(); }
        }
        private double _LED_Response_Junk11 = 0;
        private bool _LED_Response_Junk11_set = false;
        public double LED_Response_Junk11
        {
            get { return _LED_Response_Junk11; }
            internal set { if (_LED_Response_Junk11_set && value == _LED_Response_Junk11) return; _LED_Response_Junk11 = value; _LED_Response_Junk11_set = true; OnPropertyChanged(); }
        }
        private double _LED_Response_Junk12 = 0;
        private bool _LED_Response_Junk12_set = false;
        public double LED_Response_Junk12
        {
            get { return _LED_Response_Junk12; }
            internal set { if (_LED_Response_Junk12_set && value == _LED_Response_Junk12) return; _LED_Response_Junk12 = value; _LED_Response_Junk12_set = true; OnPropertyChanged(); }
        }
        private string _LED_Response_JsonResponse = "";
        private bool _LED_Response_JsonResponse_set = false;
        public string LED_Response_JsonResponse
        {
            get { return _LED_Response_JsonResponse; }
            internal set { if (_LED_Response_JsonResponse_set && value == _LED_Response_JsonResponse) return; _LED_Response_JsonResponse = value; _LED_Response_JsonResponse_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLED_Response(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.LED_Response_LED_Control_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.LED_Response_LED_Control_enum, "LED_Response", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Junk10 U8|HEX|Counter U24|HEX|Junk11 U24|HEX|Junk12 STRING|ASCII|JsonResponse";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            LED_Response_Junk10 = parseResult.ValueList.GetValue("Junk10").AsDouble;
            LED_Response_Counter = parseResult.ValueList.GetValue("Counter").AsDouble;
            LED_Response_Junk11 = parseResult.ValueList.GetValue("Junk11").AsDouble;
            LED_Response_Junk12 = parseResult.ValueList.GetValue("Junk12").AsDouble;
            LED_Response_JsonResponse = parseResult.ValueList.GetValue("JsonResponse").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; LED_ResponseEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent LED_ResponseEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyLED_Response_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyLED_ResponseAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.LED_Response_LED_Control_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.LED_Response_LED_Control_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyLED_Response_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyLED_Response_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyLED_ResponseCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyLED_Response: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyLED_Response: set notification", result);

            return true;
        }

        private void NotifyLED_ResponseCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX|Junk10 U8|HEX|Counter U24|HEX|Junk11 U24|HEX|Junk12 STRING|ASCII|JsonResponse";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            LED_Response_Junk10 = parseResult.ValueList.GetValue("Junk10").AsDouble;
            LED_Response_Counter = parseResult.ValueList.GetValue("Counter").AsDouble;
            LED_Response_Junk11 = parseResult.ValueList.GetValue("Junk11").AsDouble;
            LED_Response_Junk12 = parseResult.ValueList.GetValue("Junk12").AsDouble;
            LED_Response_JsonResponse = parseResult.ValueList.GetValue("JsonResponse").AsString;

            LED_ResponseEvent?.Invoke(parseResult);

        }

        public void NotifyLED_ResponseRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.LED_Response_LED_Control_enum];
            if (ch == null) return;
            NotifyLED_Response_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyLED_ResponseCallback;
        }






        /// <summary>
        /// Writes data for LED_Write
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLED_Write(UInt16 Counter, UInt32 Junk1, byte Len1, byte Len2, UInt32 Junk2, byte H, byte S, byte V, UInt16 White, byte[] Junk3)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.LED_Write_LED_Control_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(  Counter);
            dw.WriteUInt24(  Junk1);
            dw.WriteByte(  Len1);
            dw.WriteByte(  Len2);
            dw.WriteUInt24(  Junk2);
            dw.WriteByte(  H);
            dw.WriteByte(  S);
            dw.WriteByte(  V);
            dw.WriteUInt16(  White);
            dw.WriteBytes(  Junk3);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(CharacteristicsEnum.LED_Write_LED_Control_enum, "LED_Write", command, GattWriteOption.WriteWithoutResponse);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. Chcked the file history; this code has always been this way
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.LED_Write_LED_Control_enum, "LED_Write", command, GattWriteOption.WriteWithoutResponse);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.LED_Write_LED_Control_enum, "LED_Write", subcommand, GattWriteOption.WriteWithoutResponse);
            //}
        }



    }
}