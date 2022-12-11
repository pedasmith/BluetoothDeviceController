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
    /// This class was automatically generated 2022-12-10::04:56
    /// </summary>

    public  class MIPOW_Playbulb_BTL201 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: http://mipow.com/
    // Link: https://pdominique.wordpress.com/2015/01/02/hacking-playbulb-candles/
    // Link: https://github.com/Heckie75/Mipow-Playbulb-BTL201
    // Link: https://github.com/Phhere/Playbulb/tree/master/protocols


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001016-d102-11e1-9b23-00025b00a5a5"),
           Guid.Parse("0000ff0d-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000fef1-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Generic Service",
            "Common Configuration",
            "DeviceInformationService",
            "MipowBulb",
            "Battery",
            "AirCableSmartMeshService",
            "Device Info",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,
            null,
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #0 is Service Changes
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00001013-d102-11e1-9b23-00025b00a5a5"), // #0 is ApplicationNumber
            Guid.Parse("00001018-d102-11e1-9b23-00025b00a5a5"), // #1 is GetKeyBlock
            Guid.Parse("00001014-d102-11e1-9b23-00025b00a5a5"), // #2 is XferCharacteristics
            Guid.Parse("00001011-d102-11e1-9b23-00025b00a5a5"), // #3 is GetVersion
            Guid.Parse("00002a37-0000-1000-8000-00805f9b34fb"), // #0 is BulbHeartRate
            Guid.Parse("00001234-0000-1000-8000-00805f9b34fb"), // #1 is Unknown1
            Guid.Parse("0000fff7-0000-1000-8000-00805f9b34fb"), // #2 is PINPassword
            Guid.Parse("0000fff8-0000-1000-8000-00805f9b34fb"), // #3 is TimerEffects
            Guid.Parse("0000fff9-0000-1000-8000-00805f9b34fb"), // #4 is SecurityMode
            Guid.Parse("0000fffb-0000-1000-8000-00805f9b34fb"), // #5 is Effect
            Guid.Parse("0000fffc-0000-1000-8000-00805f9b34fb"), // #6 is Color
            Guid.Parse("0000fffd-0000-1000-8000-00805f9b34fb"), // #7 is Reset
            Guid.Parse("0000fffe-0000-1000-8000-00805f9b34fb"), // #8 is Timer
            Guid.Parse("0000ffff-0000-1000-8000-00805f9b34fb"), // #9 is GivenName
            Guid.Parse("0000fff5-0000-1000-8000-00805f9b34fb"), // #10 is Unknown10
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #0 is BatteryLevel
            Guid.Parse("c4edc000-9daf-11e3-8000-00025b000b00"), // #0 is NetworkKey
            Guid.Parse("c4edc000-9daf-11e3-8001-00025b000b00"), // #1 is DeviceUuid
            Guid.Parse("c4edc000-9daf-11e3-8002-00025b000b00"), // #2 is DeviceId
            Guid.Parse("c4edc000-9daf-11e3-8003-00025b000b00"), // #3 is MtlContinuationCpUuid
            Guid.Parse("c4edc000-9daf-11e3-8004-00025b000b00"), // #4 is MtlCompleteCpUuid
            Guid.Parse("c4edc000-9daf-11e3-8005-00025b000b00"), // #5 is MtlTtlUuid
            Guid.Parse("c4edc000-9daf-11e3-8006-00025b000b00"), // #6 is MeshAppearanceUuid
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #0 is Firmware Revision
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #1 is PnP ID

        };
        String[] CharacteristicNames = new string[] {
            "Service Changes", // #0 is 00002a05-0000-1000-8000-00805f9b34fb
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "ApplicationNumber", // #0 is 00001013-d102-11e1-9b23-00025b00a5a5
            "GetKeyBlock", // #1 is 00001018-d102-11e1-9b23-00025b00a5a5
            "XferCharacteristics", // #2 is 00001014-d102-11e1-9b23-00025b00a5a5
            "GetVersion", // #3 is 00001011-d102-11e1-9b23-00025b00a5a5
            "BulbHeartRate", // #0 is 00002a37-0000-1000-8000-00805f9b34fb
            "Unknown1", // #1 is 00001234-0000-1000-8000-00805f9b34fb
            "PINPassword", // #2 is 0000fff7-0000-1000-8000-00805f9b34fb
            "TimerEffects", // #3 is 0000fff8-0000-1000-8000-00805f9b34fb
            "SecurityMode", // #4 is 0000fff9-0000-1000-8000-00805f9b34fb
            "Effect", // #5 is 0000fffb-0000-1000-8000-00805f9b34fb
            "Color", // #6 is 0000fffc-0000-1000-8000-00805f9b34fb
            "Reset", // #7 is 0000fffd-0000-1000-8000-00805f9b34fb
            "Timer", // #8 is 0000fffe-0000-1000-8000-00805f9b34fb
            "GivenName", // #9 is 0000ffff-0000-1000-8000-00805f9b34fb
            "Unknown10", // #10 is 0000fff5-0000-1000-8000-00805f9b34fb
            "BatteryLevel", // #0 is 00002a19-0000-1000-8000-00805f9b34fb
            "NetworkKey", // #0 is c4edc000-9daf-11e3-8000-00025b000b00
            "DeviceUuid", // #1 is c4edc000-9daf-11e3-8001-00025b000b00
            "DeviceId", // #2 is c4edc000-9daf-11e3-8002-00025b000b00
            "MtlContinuationCpUuid", // #3 is c4edc000-9daf-11e3-8003-00025b000b00
            "MtlCompleteCpUuid", // #4 is c4edc000-9daf-11e3-8004-00025b000b00
            "MtlTtlUuid", // #5 is c4edc000-9daf-11e3-8005-00025b000b00
            "MeshAppearanceUuid", // #6 is c4edc000-9daf-11e3-8006-00025b000b00
            "Firmware Revision", // #0 is 00002a26-0000-1000-8000-00805f9b34fb
            "PnP ID", // #1 is 00002a50-0000-1000-8000-00805f9b34fb

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

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0,  },
            new HashSet<int>(){ 1, 2, 3,  },
            new HashSet<int>(){ 4, 5, 6, 7,  },
            new HashSet<int>(){ 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,  },
            new HashSet<int>(){ 19,  },
            new HashSet<int>(){ 20, 21, 22, 23, 24, 25, 26,  },
            new HashSet<int>(){ 27, 28,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            1, // Characteristic 1
            1, // Characteristic 2
            1, // Characteristic 3
            2, // Characteristic 4
            2, // Characteristic 5
            2, // Characteristic 6
            2, // Characteristic 7
            3, // Characteristic 8
            3, // Characteristic 9
            3, // Characteristic 10
            3, // Characteristic 11
            3, // Characteristic 12
            3, // Characteristic 13
            3, // Characteristic 14
            3, // Characteristic 15
            3, // Characteristic 16
            3, // Characteristic 17
            3, // Characteristic 18
            4, // Characteristic 19
            5, // Characteristic 20
            5, // Characteristic 21
            5, // Characteristic 22
            5, // Characteristic 23
            5, // Characteristic 24
            5, // Characteristic 25
            5, // Characteristic 26
            6, // Characteristic 27
            6, // Characteristic 28
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Service_Changes_Generic_Service_enum = 0,
            Device_Name_Common_Configuration_enum = 1,
            Appearance_Common_Configuration_enum = 2,
            Connection_Parameter_Common_Configuration_enum = 3,
            ApplicationNumber_DeviceInformationService_enum = 4,
            GetKeyBlock_DeviceInformationService_enum = 5,
            XferCharacteristics_DeviceInformationService_enum = 6,
            GetVersion_DeviceInformationService_enum = 7,
            BulbHeartRate_MipowBulb_enum = 8,
            Unknown1_MipowBulb_enum = 9,
            PINPassword_MipowBulb_enum = 10,
            TimerEffects_MipowBulb_enum = 11,
            SecurityMode_MipowBulb_enum = 12,
            Effect_MipowBulb_enum = 13,
            Color_MipowBulb_enum = 14,
            Reset_MipowBulb_enum = 15,
            Timer_MipowBulb_enum = 16,
            GivenName_MipowBulb_enum = 17,
            Unknown10_MipowBulb_enum = 18,
            BatteryLevel_Battery_enum = 19,
            NetworkKey_AirCableSmartMeshService_enum = 20,
            DeviceUuid_AirCableSmartMeshService_enum = 21,
            DeviceId_AirCableSmartMeshService_enum = 22,
            MtlContinuationCpUuid_AirCableSmartMeshService_enum = 23,
            MtlCompleteCpUuid_AirCableSmartMeshService_enum = 24,
            MtlTtlUuid_AirCableSmartMeshService_enum = 25,
            MeshAppearanceUuid_AirCableSmartMeshService_enum = 26,
            Firmware_Revision_Device_Info_enum = 27,
            PnP_ID_Device_Info_enum = 28,

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


        /// <summary>
        /// Writes data for Device_Name
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteDevice_Name(String Device_Name)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(  Device_Name);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", subcommand, GattWriteOption.WriteWithResponse);
            }
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum, "Connection_Parameter", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|ConnectionParameter";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Connection_Parameter = parseResult.ValueList.GetValue("ConnectionParameter").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _ApplicationNumber = null;
        private bool _ApplicationNumber_set = false;
        public string ApplicationNumber
        {
            get { return _ApplicationNumber; }
            internal set { if (_ApplicationNumber_set && value == _ApplicationNumber) return; _ApplicationNumber = value; _ApplicationNumber_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadApplicationNumber(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.ApplicationNumber_DeviceInformationService_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.ApplicationNumber_DeviceInformationService_enum, "ApplicationNumber", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|ApplicationNumber";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            ApplicationNumber = parseResult.ValueList.GetValue("ApplicationNumber").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for ApplicationNumber
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteApplicationNumber(byte[] ApplicationNumber)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.ApplicationNumber_DeviceInformationService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  ApplicationNumber);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.ApplicationNumber_DeviceInformationService_enum, "ApplicationNumber", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.ApplicationNumber_DeviceInformationService_enum, "ApplicationNumber", subcommand, GattWriteOption.WriteWithResponse);
            }
        }





        /// <summary>
        /// Writes data for GetKeyBlock
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteGetKeyBlock(byte[] KeyBlock)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.GetKeyBlock_DeviceInformationService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  KeyBlock);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.GetKeyBlock_DeviceInformationService_enum, "GetKeyBlock", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.GetKeyBlock_DeviceInformationService_enum, "GetKeyBlock", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _XferCharacteristics = null;
        private bool _XferCharacteristics_set = false;
        public string XferCharacteristics
        {
            get { return _XferCharacteristics; }
            internal set { if (_XferCharacteristics_set && value == _XferCharacteristics) return; _XferCharacteristics = value; _XferCharacteristics_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadXferCharacteristics(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.XferCharacteristics_DeviceInformationService_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.XferCharacteristics_DeviceInformationService_enum, "XferCharacteristics", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|XferCharacteristics";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            XferCharacteristics = parseResult.ValueList.GetValue("XferCharacteristics").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; XferCharacteristicsEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent XferCharacteristicsEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyXferCharacteristics_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyXferCharacteristicsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.XferCharacteristics_DeviceInformationService_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.XferCharacteristics_DeviceInformationService_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyXferCharacteristics_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyXferCharacteristics_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyXferCharacteristicsCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyXferCharacteristics: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyXferCharacteristics: set notification", result);

            return true;
        }

        private void NotifyXferCharacteristicsCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|XferCharacteristics";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            XferCharacteristics = parseResult.ValueList.GetValue("XferCharacteristics").AsString;

            XferCharacteristicsEvent?.Invoke(parseResult);

        }

        public void NotifyXferCharacteristicsRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.XferCharacteristics_DeviceInformationService_enum];
            if (ch == null) return;
            NotifyXferCharacteristics_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyXferCharacteristicsCallback;
        }



        private string _GetVersion = null;
        private bool _GetVersion_set = false;
        public string GetVersion
        {
            get { return _GetVersion; }
            internal set { if (_GetVersion_set && value == _GetVersion) return; _GetVersion = value; _GetVersion_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadGetVersion(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.GetVersion_DeviceInformationService_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.GetVersion_DeviceInformationService_enum, "GetVersion", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Version";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            GetVersion = parseResult.ValueList.GetValue("Version").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _BulbHeartRate = null;
        private bool _BulbHeartRate_set = false;
        public string BulbHeartRate
        {
            get { return _BulbHeartRate; }
            internal set { if (_BulbHeartRate_set && value == _BulbHeartRate) return; _BulbHeartRate = value; _BulbHeartRate_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; BulbHeartRateEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent BulbHeartRateEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyBulbHeartRate_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyBulbHeartRateAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BulbHeartRate_MipowBulb_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.BulbHeartRate_MipowBulb_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyBulbHeartRate_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyBulbHeartRate_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyBulbHeartRateCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyBulbHeartRate: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyBulbHeartRate: set notification", result);

            return true;
        }

        private void NotifyBulbHeartRateCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|BulbHeartRate";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            BulbHeartRate = parseResult.ValueList.GetValue("BulbHeartRate").AsString;

            BulbHeartRateEvent?.Invoke(parseResult);

        }

        public void NotifyBulbHeartRateRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.BulbHeartRate_MipowBulb_enum];
            if (ch == null) return;
            NotifyBulbHeartRate_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBulbHeartRateCallback;
        }






        /// <summary>
        /// Writes data for Unknown1
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUnknown1(byte[] Unknown1)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown1_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown1);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Unknown1_MipowBulb_enum, "Unknown1", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Unknown1_MipowBulb_enum, "Unknown1", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _PINPassword = "";
        private bool _PINPassword_set = false;
        public string PINPassword
        {
            get { return _PINPassword; }
            internal set { if (_PINPassword_set && value == _PINPassword) return; _PINPassword = value; _PINPassword_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPINPassword(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.PINPassword_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.PINPassword_MipowBulb_enum, "PINPassword", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Password";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            PINPassword = parseResult.ValueList.GetValue("Password").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for PINPassword
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePINPassword(String Password)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.PINPassword_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(  Password);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.PINPassword_MipowBulb_enum, "PINPassword", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.PINPassword_MipowBulb_enum, "PINPassword", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _TimerEffects_W1 = 0;
        private bool _TimerEffects_W1_set = false;
        public double TimerEffects_W1
        {
            get { return _TimerEffects_W1; }
            internal set { if (_TimerEffects_W1_set && value == _TimerEffects_W1) return; _TimerEffects_W1 = value; _TimerEffects_W1_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_R1 = 0;
        private bool _TimerEffects_R1_set = false;
        public double TimerEffects_R1
        {
            get { return _TimerEffects_R1; }
            internal set { if (_TimerEffects_R1_set && value == _TimerEffects_R1) return; _TimerEffects_R1 = value; _TimerEffects_R1_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_G1 = 0;
        private bool _TimerEffects_G1_set = false;
        public double TimerEffects_G1
        {
            get { return _TimerEffects_G1; }
            internal set { if (_TimerEffects_G1_set && value == _TimerEffects_G1) return; _TimerEffects_G1 = value; _TimerEffects_G1_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_B1 = 0;
        private bool _TimerEffects_B1_set = false;
        public double TimerEffects_B1
        {
            get { return _TimerEffects_B1; }
            internal set { if (_TimerEffects_B1_set && value == _TimerEffects_B1) return; _TimerEffects_B1 = value; _TimerEffects_B1_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_Time1 = 0;
        private bool _TimerEffects_Time1_set = false;
        public double TimerEffects_Time1
        {
            get { return _TimerEffects_Time1; }
            internal set { if (_TimerEffects_Time1_set && value == _TimerEffects_Time1) return; _TimerEffects_Time1 = value; _TimerEffects_Time1_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_W2 = 0;
        private bool _TimerEffects_W2_set = false;
        public double TimerEffects_W2
        {
            get { return _TimerEffects_W2; }
            internal set { if (_TimerEffects_W2_set && value == _TimerEffects_W2) return; _TimerEffects_W2 = value; _TimerEffects_W2_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_R2 = 0;
        private bool _TimerEffects_R2_set = false;
        public double TimerEffects_R2
        {
            get { return _TimerEffects_R2; }
            internal set { if (_TimerEffects_R2_set && value == _TimerEffects_R2) return; _TimerEffects_R2 = value; _TimerEffects_R2_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_G2 = 0;
        private bool _TimerEffects_G2_set = false;
        public double TimerEffects_G2
        {
            get { return _TimerEffects_G2; }
            internal set { if (_TimerEffects_G2_set && value == _TimerEffects_G2) return; _TimerEffects_G2 = value; _TimerEffects_G2_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_B2 = 0;
        private bool _TimerEffects_B2_set = false;
        public double TimerEffects_B2
        {
            get { return _TimerEffects_B2; }
            internal set { if (_TimerEffects_B2_set && value == _TimerEffects_B2) return; _TimerEffects_B2 = value; _TimerEffects_B2_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_Time2 = 0;
        private bool _TimerEffects_Time2_set = false;
        public double TimerEffects_Time2
        {
            get { return _TimerEffects_Time2; }
            internal set { if (_TimerEffects_Time2_set && value == _TimerEffects_Time2) return; _TimerEffects_Time2 = value; _TimerEffects_Time2_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_W3 = 0;
        private bool _TimerEffects_W3_set = false;
        public double TimerEffects_W3
        {
            get { return _TimerEffects_W3; }
            internal set { if (_TimerEffects_W3_set && value == _TimerEffects_W3) return; _TimerEffects_W3 = value; _TimerEffects_W3_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_R3 = 0;
        private bool _TimerEffects_R3_set = false;
        public double TimerEffects_R3
        {
            get { return _TimerEffects_R3; }
            internal set { if (_TimerEffects_R3_set && value == _TimerEffects_R3) return; _TimerEffects_R3 = value; _TimerEffects_R3_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_G3 = 0;
        private bool _TimerEffects_G3_set = false;
        public double TimerEffects_G3
        {
            get { return _TimerEffects_G3; }
            internal set { if (_TimerEffects_G3_set && value == _TimerEffects_G3) return; _TimerEffects_G3 = value; _TimerEffects_G3_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_B3 = 0;
        private bool _TimerEffects_B3_set = false;
        public double TimerEffects_B3
        {
            get { return _TimerEffects_B3; }
            internal set { if (_TimerEffects_B3_set && value == _TimerEffects_B3) return; _TimerEffects_B3 = value; _TimerEffects_B3_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_Time3 = 0;
        private bool _TimerEffects_Time3_set = false;
        public double TimerEffects_Time3
        {
            get { return _TimerEffects_Time3; }
            internal set { if (_TimerEffects_Time3_set && value == _TimerEffects_Time3) return; _TimerEffects_Time3 = value; _TimerEffects_Time3_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_W4 = 0;
        private bool _TimerEffects_W4_set = false;
        public double TimerEffects_W4
        {
            get { return _TimerEffects_W4; }
            internal set { if (_TimerEffects_W4_set && value == _TimerEffects_W4) return; _TimerEffects_W4 = value; _TimerEffects_W4_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_R4 = 0;
        private bool _TimerEffects_R4_set = false;
        public double TimerEffects_R4
        {
            get { return _TimerEffects_R4; }
            internal set { if (_TimerEffects_R4_set && value == _TimerEffects_R4) return; _TimerEffects_R4 = value; _TimerEffects_R4_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_G4 = 0;
        private bool _TimerEffects_G4_set = false;
        public double TimerEffects_G4
        {
            get { return _TimerEffects_G4; }
            internal set { if (_TimerEffects_G4_set && value == _TimerEffects_G4) return; _TimerEffects_G4 = value; _TimerEffects_G4_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_B4 = 0;
        private bool _TimerEffects_B4_set = false;
        public double TimerEffects_B4
        {
            get { return _TimerEffects_B4; }
            internal set { if (_TimerEffects_B4_set && value == _TimerEffects_B4) return; _TimerEffects_B4 = value; _TimerEffects_B4_set = true; OnPropertyChanged(); }
        }
        private double _TimerEffects_Time4 = 0;
        private bool _TimerEffects_Time4_set = false;
        public double TimerEffects_Time4
        {
            get { return _TimerEffects_Time4; }
            internal set { if (_TimerEffects_Time4_set && value == _TimerEffects_Time4) return; _TimerEffects_Time4 = value; _TimerEffects_Time4_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTimerEffects(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.TimerEffects_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.TimerEffects_MipowBulb_enum, "TimerEffects", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|W1 U8|HEX|R1 U8|HEX|G1 U8|HEX|B1 U8|DEC|Time1|Minutes U8|HEX|W2 U8|HEX|R2 U8|HEX|G2 U8|HEX|B2 U8|DEC|Time2|Minutes U8|HEX|W3 U8|HEX|R3 U8|HEX|G3 U8|HEX|B3 U8|DEC|Time3|Minutes U8|HEX|W4 U8|HEX|R4 U8|HEX|G4 U8|HEX|B4 U8|DEC|Time4|Minutes";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            TimerEffects_W1 = parseResult.ValueList.GetValue("W1").AsDouble;
            TimerEffects_R1 = parseResult.ValueList.GetValue("R1").AsDouble;
            TimerEffects_G1 = parseResult.ValueList.GetValue("G1").AsDouble;
            TimerEffects_B1 = parseResult.ValueList.GetValue("B1").AsDouble;
            TimerEffects_Time1 = parseResult.ValueList.GetValue("Time1").AsDouble;
            TimerEffects_W2 = parseResult.ValueList.GetValue("W2").AsDouble;
            TimerEffects_R2 = parseResult.ValueList.GetValue("R2").AsDouble;
            TimerEffects_G2 = parseResult.ValueList.GetValue("G2").AsDouble;
            TimerEffects_B2 = parseResult.ValueList.GetValue("B2").AsDouble;
            TimerEffects_Time2 = parseResult.ValueList.GetValue("Time2").AsDouble;
            TimerEffects_W3 = parseResult.ValueList.GetValue("W3").AsDouble;
            TimerEffects_R3 = parseResult.ValueList.GetValue("R3").AsDouble;
            TimerEffects_G3 = parseResult.ValueList.GetValue("G3").AsDouble;
            TimerEffects_B3 = parseResult.ValueList.GetValue("B3").AsDouble;
            TimerEffects_Time3 = parseResult.ValueList.GetValue("Time3").AsDouble;
            TimerEffects_W4 = parseResult.ValueList.GetValue("W4").AsDouble;
            TimerEffects_R4 = parseResult.ValueList.GetValue("R4").AsDouble;
            TimerEffects_G4 = parseResult.ValueList.GetValue("G4").AsDouble;
            TimerEffects_B4 = parseResult.ValueList.GetValue("B4").AsDouble;
            TimerEffects_Time4 = parseResult.ValueList.GetValue("Time4").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _SecurityMode_SecurityCommand = 0;
        private bool _SecurityMode_SecurityCommand_set = false;
        public double SecurityMode_SecurityCommand
        {
            get { return _SecurityMode_SecurityCommand; }
            internal set { if (_SecurityMode_SecurityCommand_set && value == _SecurityMode_SecurityCommand) return; _SecurityMode_SecurityCommand = value; _SecurityMode_SecurityCommand_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityCurrentMinute = 0;
        private bool _SecurityMode_SecurityCurrentMinute_set = false;
        public double SecurityMode_SecurityCurrentMinute
        {
            get { return _SecurityMode_SecurityCurrentMinute; }
            internal set { if (_SecurityMode_SecurityCurrentMinute_set && value == _SecurityMode_SecurityCurrentMinute) return; _SecurityMode_SecurityCurrentMinute = value; _SecurityMode_SecurityCurrentMinute_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityCurrentHour = 0;
        private bool _SecurityMode_SecurityCurrentHour_set = false;
        public double SecurityMode_SecurityCurrentHour
        {
            get { return _SecurityMode_SecurityCurrentHour; }
            internal set { if (_SecurityMode_SecurityCurrentHour_set && value == _SecurityMode_SecurityCurrentHour) return; _SecurityMode_SecurityCurrentHour = value; _SecurityMode_SecurityCurrentHour_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityStartingMinute = 0;
        private bool _SecurityMode_SecurityStartingMinute_set = false;
        public double SecurityMode_SecurityStartingMinute
        {
            get { return _SecurityMode_SecurityStartingMinute; }
            internal set { if (_SecurityMode_SecurityStartingMinute_set && value == _SecurityMode_SecurityStartingMinute) return; _SecurityMode_SecurityStartingMinute = value; _SecurityMode_SecurityStartingMinute_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityStartingHour = 0;
        private bool _SecurityMode_SecurityStartingHour_set = false;
        public double SecurityMode_SecurityStartingHour
        {
            get { return _SecurityMode_SecurityStartingHour; }
            internal set { if (_SecurityMode_SecurityStartingHour_set && value == _SecurityMode_SecurityStartingHour) return; _SecurityMode_SecurityStartingHour = value; _SecurityMode_SecurityStartingHour_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityEndingMinute = 0;
        private bool _SecurityMode_SecurityEndingMinute_set = false;
        public double SecurityMode_SecurityEndingMinute
        {
            get { return _SecurityMode_SecurityEndingMinute; }
            internal set { if (_SecurityMode_SecurityEndingMinute_set && value == _SecurityMode_SecurityEndingMinute) return; _SecurityMode_SecurityEndingMinute = value; _SecurityMode_SecurityEndingMinute_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityEndingHour = 0;
        private bool _SecurityMode_SecurityEndingHour_set = false;
        public double SecurityMode_SecurityEndingHour
        {
            get { return _SecurityMode_SecurityEndingHour; }
            internal set { if (_SecurityMode_SecurityEndingHour_set && value == _SecurityMode_SecurityEndingHour) return; _SecurityMode_SecurityEndingHour = value; _SecurityMode_SecurityEndingHour_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityMinInterval = 0;
        private bool _SecurityMode_SecurityMinInterval_set = false;
        public double SecurityMode_SecurityMinInterval
        {
            get { return _SecurityMode_SecurityMinInterval; }
            internal set { if (_SecurityMode_SecurityMinInterval_set && value == _SecurityMode_SecurityMinInterval) return; _SecurityMode_SecurityMinInterval = value; _SecurityMode_SecurityMinInterval_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityMaxInterval = 0;
        private bool _SecurityMode_SecurityMaxInterval_set = false;
        public double SecurityMode_SecurityMaxInterval
        {
            get { return _SecurityMode_SecurityMaxInterval; }
            internal set { if (_SecurityMode_SecurityMaxInterval_set && value == _SecurityMode_SecurityMaxInterval) return; _SecurityMode_SecurityMaxInterval = value; _SecurityMode_SecurityMaxInterval_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityW = 0;
        private bool _SecurityMode_SecurityW_set = false;
        public double SecurityMode_SecurityW
        {
            get { return _SecurityMode_SecurityW; }
            internal set { if (_SecurityMode_SecurityW_set && value == _SecurityMode_SecurityW) return; _SecurityMode_SecurityW = value; _SecurityMode_SecurityW_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityR = 0;
        private bool _SecurityMode_SecurityR_set = false;
        public double SecurityMode_SecurityR
        {
            get { return _SecurityMode_SecurityR; }
            internal set { if (_SecurityMode_SecurityR_set && value == _SecurityMode_SecurityR) return; _SecurityMode_SecurityR = value; _SecurityMode_SecurityR_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityG = 0;
        private bool _SecurityMode_SecurityG_set = false;
        public double SecurityMode_SecurityG
        {
            get { return _SecurityMode_SecurityG; }
            internal set { if (_SecurityMode_SecurityG_set && value == _SecurityMode_SecurityG) return; _SecurityMode_SecurityG = value; _SecurityMode_SecurityG_set = true; OnPropertyChanged(); }
        }
        private double _SecurityMode_SecurityB = 0;
        private bool _SecurityMode_SecurityB_set = false;
        public double SecurityMode_SecurityB
        {
            get { return _SecurityMode_SecurityB; }
            internal set { if (_SecurityMode_SecurityB_set && value == _SecurityMode_SecurityB) return; _SecurityMode_SecurityB = value; _SecurityMode_SecurityB_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSecurityMode(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.SecurityMode_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.SecurityMode_MipowBulb_enum, "SecurityMode", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|SecurityCommand U8|HEX|SecurityCurrentMinute U8|HEX|SecurityCurrentHour U8|HEX|SecurityStartingMinute U8|HEX|SecurityStartingHour U8|HEX|SecurityEndingMinute U8|HEX|SecurityEndingHour U8|DEC|SecurityMinInterval|Minutes U8|DEC|SecurityMaxInterval|Minutes U8|HEX|SecurityW U8|HEX|SecurityR U8|HEX|SecurityG U8|HEX|SecurityB";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            SecurityMode_SecurityCommand = parseResult.ValueList.GetValue("SecurityCommand").AsDouble;
            SecurityMode_SecurityCurrentMinute = parseResult.ValueList.GetValue("SecurityCurrentMinute").AsDouble;
            SecurityMode_SecurityCurrentHour = parseResult.ValueList.GetValue("SecurityCurrentHour").AsDouble;
            SecurityMode_SecurityStartingMinute = parseResult.ValueList.GetValue("SecurityStartingMinute").AsDouble;
            SecurityMode_SecurityStartingHour = parseResult.ValueList.GetValue("SecurityStartingHour").AsDouble;
            SecurityMode_SecurityEndingMinute = parseResult.ValueList.GetValue("SecurityEndingMinute").AsDouble;
            SecurityMode_SecurityEndingHour = parseResult.ValueList.GetValue("SecurityEndingHour").AsDouble;
            SecurityMode_SecurityMinInterval = parseResult.ValueList.GetValue("SecurityMinInterval").AsDouble;
            SecurityMode_SecurityMaxInterval = parseResult.ValueList.GetValue("SecurityMaxInterval").AsDouble;
            SecurityMode_SecurityW = parseResult.ValueList.GetValue("SecurityW").AsDouble;
            SecurityMode_SecurityR = parseResult.ValueList.GetValue("SecurityR").AsDouble;
            SecurityMode_SecurityG = parseResult.ValueList.GetValue("SecurityG").AsDouble;
            SecurityMode_SecurityB = parseResult.ValueList.GetValue("SecurityB").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for SecurityMode
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteSecurityMode(byte SecurityCommand, byte SecurityCurrentMinute, byte SecurityCurrentHour, byte SecurityStartingMinute, byte SecurityStartingHour, byte SecurityEndingMinute, byte SecurityEndingHour, byte SecurityMinInterval, byte SecurityMaxInterval, byte SecurityW, byte SecurityR, byte SecurityG, byte SecurityB)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.SecurityMode_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  SecurityCommand);
            dw.WriteByte(  SecurityCurrentMinute);
            dw.WriteByte(  SecurityCurrentHour);
            dw.WriteByte(  SecurityStartingMinute);
            dw.WriteByte(  SecurityStartingHour);
            dw.WriteByte(  SecurityEndingMinute);
            dw.WriteByte(  SecurityEndingHour);
            dw.WriteByte(  SecurityMinInterval);
            dw.WriteByte(  SecurityMaxInterval);
            dw.WriteByte(  SecurityW);
            dw.WriteByte(  SecurityR);
            dw.WriteByte(  SecurityG);
            dw.WriteByte(  SecurityB);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.SecurityMode_MipowBulb_enum, "SecurityMode", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.SecurityMode_MipowBulb_enum, "SecurityMode", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Effect_W = 0;
        private bool _Effect_W_set = false;
        public double Effect_W
        {
            get { return _Effect_W; }
            internal set { if (_Effect_W_set && value == _Effect_W) return; _Effect_W = value; _Effect_W_set = true; OnPropertyChanged(); }
        }
        private double _Effect_R = 0;
        private bool _Effect_R_set = false;
        public double Effect_R
        {
            get { return _Effect_R; }
            internal set { if (_Effect_R_set && value == _Effect_R) return; _Effect_R = value; _Effect_R_set = true; OnPropertyChanged(); }
        }
        private double _Effect_G = 0;
        private bool _Effect_G_set = false;
        public double Effect_G
        {
            get { return _Effect_G; }
            internal set { if (_Effect_G_set && value == _Effect_G) return; _Effect_G = value; _Effect_G_set = true; OnPropertyChanged(); }
        }
        private double _Effect_B = 0;
        private bool _Effect_B_set = false;
        public double Effect_B
        {
            get { return _Effect_B; }
            internal set { if (_Effect_B_set && value == _Effect_B) return; _Effect_B = value; _Effect_B_set = true; OnPropertyChanged(); }
        }
        private double _Effect_Effect = 0;
        private bool _Effect_Effect_set = false;
        public double Effect_Effect
        {
            get { return _Effect_Effect; }
            internal set { if (_Effect_Effect_set && value == _Effect_Effect) return; _Effect_Effect = value; _Effect_Effect_set = true; OnPropertyChanged(); }
        }
        private double _Effect_Junk = 0;
        private bool _Effect_Junk_set = false;
        public double Effect_Junk
        {
            get { return _Effect_Junk; }
            internal set { if (_Effect_Junk_set && value == _Effect_Junk) return; _Effect_Junk = value; _Effect_Junk_set = true; OnPropertyChanged(); }
        }
        private double _Effect_Delay1 = 0;
        private bool _Effect_Delay1_set = false;
        public double Effect_Delay1
        {
            get { return _Effect_Delay1; }
            internal set { if (_Effect_Delay1_set && value == _Effect_Delay1) return; _Effect_Delay1 = value; _Effect_Delay1_set = true; OnPropertyChanged(); }
        }
        private double _Effect_Delay2 = 0;
        private bool _Effect_Delay2_set = false;
        public double Effect_Delay2
        {
            get { return _Effect_Delay2; }
            internal set { if (_Effect_Delay2_set && value == _Effect_Delay2) return; _Effect_Delay2 = value; _Effect_Delay2_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadEffect(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Effect_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Effect_MipowBulb_enum, "Effect", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|W U8|HEX|R U8|HEX|G U8|HEX|B U8|HEX|Effect U8|HEX|Junk U8|HEX|Delay1 U8|HEX|Delay2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Effect_W = parseResult.ValueList.GetValue("W").AsDouble;
            Effect_R = parseResult.ValueList.GetValue("R").AsDouble;
            Effect_G = parseResult.ValueList.GetValue("G").AsDouble;
            Effect_B = parseResult.ValueList.GetValue("B").AsDouble;
            Effect_Effect = parseResult.ValueList.GetValue("Effect").AsDouble;
            Effect_Junk = parseResult.ValueList.GetValue("Junk").AsDouble;
            Effect_Delay1 = parseResult.ValueList.GetValue("Delay1").AsDouble;
            Effect_Delay2 = parseResult.ValueList.GetValue("Delay2").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Effect
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteEffect(byte W, byte R, byte G, byte B, byte Effect, byte Junk, byte Delay1, byte Delay2)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Effect_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  W);
            dw.WriteByte(  R);
            dw.WriteByte(  G);
            dw.WriteByte(  B);
            dw.WriteByte(  Effect);
            dw.WriteByte(  Junk);
            dw.WriteByte(  Delay1);
            dw.WriteByte(  Delay2);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Effect_MipowBulb_enum, "Effect", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Effect_MipowBulb_enum, "Effect", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private double _Color_W = 0;
        private bool _Color_W_set = false;
        public double Color_W
        {
            get { return _Color_W; }
            internal set { if (_Color_W_set && value == _Color_W) return; _Color_W = value; _Color_W_set = true; OnPropertyChanged(); }
        }
        private double _Color_R = 0;
        private bool _Color_R_set = false;
        public double Color_R
        {
            get { return _Color_R; }
            internal set { if (_Color_R_set && value == _Color_R) return; _Color_R = value; _Color_R_set = true; OnPropertyChanged(); }
        }
        private double _Color_G = 0;
        private bool _Color_G_set = false;
        public double Color_G
        {
            get { return _Color_G; }
            internal set { if (_Color_G_set && value == _Color_G) return; _Color_G = value; _Color_G_set = true; OnPropertyChanged(); }
        }
        private double _Color_B = 0;
        private bool _Color_B_set = false;
        public double Color_B
        {
            get { return _Color_B; }
            internal set { if (_Color_B_set && value == _Color_B) return; _Color_B = value; _Color_B_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadColor(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Color_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Color_MipowBulb_enum, "Color", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|W U8|HEX|R U8|HEX|G U8|HEX|B";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Color_W = parseResult.ValueList.GetValue("W").AsDouble;
            Color_R = parseResult.ValueList.GetValue("R").AsDouble;
            Color_G = parseResult.ValueList.GetValue("G").AsDouble;
            Color_B = parseResult.ValueList.GetValue("B").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Color
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteColor(byte W, byte R, byte G, byte B)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Color_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  W);
            dw.WriteByte(  R);
            dw.WriteByte(  G);
            dw.WriteByte(  B);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Color_MipowBulb_enum, "Color", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Color_MipowBulb_enum, "Color", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private double _Reset_ResetNow = 0;
        private bool _Reset_ResetNow_set = false;
        public double Reset_ResetNow
        {
            get { return _Reset_ResetNow; }
            internal set { if (_Reset_ResetNow_set && value == _Reset_ResetNow) return; _Reset_ResetNow = value; _Reset_ResetNow_set = true; OnPropertyChanged(); }
        }
        private string _Reset_ResetAdditional = null;
        private bool _Reset_ResetAdditional_set = false;
        public string Reset_ResetAdditional
        {
            get { return _Reset_ResetAdditional; }
            internal set { if (_Reset_ResetAdditional_set && value == _Reset_ResetAdditional) return; _Reset_ResetAdditional = value; _Reset_ResetAdditional_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadReset(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Reset_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Reset_MipowBulb_enum, "Reset", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|ResetNow BYTES|HEX|ResetAdditional";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Reset_ResetNow = parseResult.ValueList.GetValue("ResetNow").AsDouble;
            Reset_ResetAdditional = parseResult.ValueList.GetValue("ResetAdditional").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Reset
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteReset(byte ResetNow, byte[] ResetAdditional)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Reset_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  ResetNow);
            dw.WriteBytes(  ResetAdditional);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Reset_MipowBulb_enum, "Reset", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Reset_MipowBulb_enum, "Reset", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Timer_TimerIndex = 0;
        private bool _Timer_TimerIndex_set = false;
        public double Timer_TimerIndex
        {
            get { return _Timer_TimerIndex; }
            internal set { if (_Timer_TimerIndex_set && value == _Timer_TimerIndex) return; _Timer_TimerIndex = value; _Timer_TimerIndex_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerType = 0;
        private bool _Timer_TimerType_set = false;
        public double Timer_TimerType
        {
            get { return _Timer_TimerType; }
            internal set { if (_Timer_TimerType_set && value == _Timer_TimerType) return; _Timer_TimerType = value; _Timer_TimerType_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerSeconds = 0;
        private bool _Timer_TimerSeconds_set = false;
        public double Timer_TimerSeconds
        {
            get { return _Timer_TimerSeconds; }
            internal set { if (_Timer_TimerSeconds_set && value == _Timer_TimerSeconds) return; _Timer_TimerSeconds = value; _Timer_TimerSeconds_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerMinutes = 0;
        private bool _Timer_TimerMinutes_set = false;
        public double Timer_TimerMinutes
        {
            get { return _Timer_TimerMinutes; }
            internal set { if (_Timer_TimerMinutes_set && value == _Timer_TimerMinutes) return; _Timer_TimerMinutes = value; _Timer_TimerMinutes_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerHours = 0;
        private bool _Timer_TimerHours_set = false;
        public double Timer_TimerHours
        {
            get { return _Timer_TimerHours; }
            internal set { if (_Timer_TimerHours_set && value == _Timer_TimerHours) return; _Timer_TimerHours = value; _Timer_TimerHours_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerRun = 0;
        private bool _Timer_TimerRun_set = false;
        public double Timer_TimerRun
        {
            get { return _Timer_TimerRun; }
            internal set { if (_Timer_TimerRun_set && value == _Timer_TimerRun) return; _Timer_TimerRun = value; _Timer_TimerRun_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerMinutesStart = 0;
        private bool _Timer_TimerMinutesStart_set = false;
        public double Timer_TimerMinutesStart
        {
            get { return _Timer_TimerMinutesStart; }
            internal set { if (_Timer_TimerMinutesStart_set && value == _Timer_TimerMinutesStart) return; _Timer_TimerMinutesStart = value; _Timer_TimerMinutesStart_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerHoursStart = 0;
        private bool _Timer_TimerHoursStart_set = false;
        public double Timer_TimerHoursStart
        {
            get { return _Timer_TimerHoursStart; }
            internal set { if (_Timer_TimerHoursStart_set && value == _Timer_TimerHoursStart) return; _Timer_TimerHoursStart = value; _Timer_TimerHoursStart_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerColorW = 0;
        private bool _Timer_TimerColorW_set = false;
        public double Timer_TimerColorW
        {
            get { return _Timer_TimerColorW; }
            internal set { if (_Timer_TimerColorW_set && value == _Timer_TimerColorW) return; _Timer_TimerColorW = value; _Timer_TimerColorW_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerColorR = 0;
        private bool _Timer_TimerColorR_set = false;
        public double Timer_TimerColorR
        {
            get { return _Timer_TimerColorR; }
            internal set { if (_Timer_TimerColorR_set && value == _Timer_TimerColorR) return; _Timer_TimerColorR = value; _Timer_TimerColorR_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerColorG = 0;
        private bool _Timer_TimerColorG_set = false;
        public double Timer_TimerColorG
        {
            get { return _Timer_TimerColorG; }
            internal set { if (_Timer_TimerColorG_set && value == _Timer_TimerColorG) return; _Timer_TimerColorG = value; _Timer_TimerColorG_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerColorB = 0;
        private bool _Timer_TimerColorB_set = false;
        public double Timer_TimerColorB
        {
            get { return _Timer_TimerColorB; }
            internal set { if (_Timer_TimerColorB_set && value == _Timer_TimerColorB) return; _Timer_TimerColorB = value; _Timer_TimerColorB_set = true; OnPropertyChanged(); }
        }
        private double _Timer_TimerRuntime = 0;
        private bool _Timer_TimerRuntime_set = false;
        public double Timer_TimerRuntime
        {
            get { return _Timer_TimerRuntime; }
            internal set { if (_Timer_TimerRuntime_set && value == _Timer_TimerRuntime) return; _Timer_TimerRuntime = value; _Timer_TimerRuntime_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTimer(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Timer_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Timer_MipowBulb_enum, "Timer", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|TimerIndex U8|DEC|TimerType U8|HEX|TimerSeconds U8|HEX|TimerMinutes U8|HEX|TimerHours U8|HEX|TimerRun U8|HEX|TimerMinutesStart U8|HEX|TimerHoursStart U8|HEX|TimerColorW U8|HEX|TimerColorR U8|HEX|TimerColorG U8|HEX|TimerColorB U8|HEX|TimerRuntime|Minutes";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Timer_TimerIndex = parseResult.ValueList.GetValue("TimerIndex").AsDouble;
            Timer_TimerType = parseResult.ValueList.GetValue("TimerType").AsDouble;
            Timer_TimerSeconds = parseResult.ValueList.GetValue("TimerSeconds").AsDouble;
            Timer_TimerMinutes = parseResult.ValueList.GetValue("TimerMinutes").AsDouble;
            Timer_TimerHours = parseResult.ValueList.GetValue("TimerHours").AsDouble;
            Timer_TimerRun = parseResult.ValueList.GetValue("TimerRun").AsDouble;
            Timer_TimerMinutesStart = parseResult.ValueList.GetValue("TimerMinutesStart").AsDouble;
            Timer_TimerHoursStart = parseResult.ValueList.GetValue("TimerHoursStart").AsDouble;
            Timer_TimerColorW = parseResult.ValueList.GetValue("TimerColorW").AsDouble;
            Timer_TimerColorR = parseResult.ValueList.GetValue("TimerColorR").AsDouble;
            Timer_TimerColorG = parseResult.ValueList.GetValue("TimerColorG").AsDouble;
            Timer_TimerColorB = parseResult.ValueList.GetValue("TimerColorB").AsDouble;
            Timer_TimerRuntime = parseResult.ValueList.GetValue("TimerRuntime").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Timer
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTimer(byte TimerIndex, byte TimerType, byte TimerSeconds, byte TimerMinutes, byte TimerHours, byte TimerRun, byte TimerMinutesStart, byte TimerHoursStart, byte TimerColorW, byte TimerColorR, byte TimerColorG, byte TimerColorB, byte TimerRuntime)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Timer_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  TimerIndex);
            dw.WriteByte(  TimerType);
            dw.WriteByte(  TimerSeconds);
            dw.WriteByte(  TimerMinutes);
            dw.WriteByte(  TimerHours);
            dw.WriteByte(  TimerRun);
            dw.WriteByte(  TimerMinutesStart);
            dw.WriteByte(  TimerHoursStart);
            dw.WriteByte(  TimerColorW);
            dw.WriteByte(  TimerColorR);
            dw.WriteByte(  TimerColorG);
            dw.WriteByte(  TimerColorB);
            dw.WriteByte(  TimerRuntime);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Timer_MipowBulb_enum, "Timer", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Timer_MipowBulb_enum, "Timer", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _GivenName = "";
        private bool _GivenName_set = false;
        public string GivenName
        {
            get { return _GivenName; }
            internal set { if (_GivenName_set && value == _GivenName) return; _GivenName = value; _GivenName_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadGivenName(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.GivenName_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.GivenName_MipowBulb_enum, "GivenName", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|GivenName";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            GivenName = parseResult.ValueList.GetValue("GivenName").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for GivenName
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteGivenName(String GivenName)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.GivenName_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(  GivenName);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.GivenName_MipowBulb_enum, "GivenName", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.GivenName_MipowBulb_enum, "GivenName", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _Unknown10 = null;
        private bool _Unknown10_set = false;
        public string Unknown10
        {
            get { return _Unknown10; }
            internal set { if (_Unknown10_set && value == _Unknown10) return; _Unknown10 = value; _Unknown10_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUnknown10(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown10_MipowBulb_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Unknown10_MipowBulb_enum, "Unknown10", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown10";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Unknown10 = parseResult.ValueList.GetValue("Unknown10").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Unknown10
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUnknown10(byte[] Unknown10)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown10_MipowBulb_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown10);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Unknown10_MipowBulb_enum, "Unknown10", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Unknown10_MipowBulb_enum, "Unknown10", subcommand, GattWriteOption.WriteWithResponse);
            }
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BatteryLevel_Battery_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.BatteryLevel_Battery_enum, "BatteryLevel", cacheMode);
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BatteryLevel_Battery_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.BatteryLevel_Battery_enum];
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
            var ch = Characteristics[(int)CharacteristicsEnum.BatteryLevel_Battery_enum];
            if (ch == null) return;
            NotifyBatteryLevel_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBatteryLevelCallback;
        }






        /// <summary>
        /// Writes data for NetworkKey
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteNetworkKey(byte[] NetworkKey)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.NetworkKey_AirCableSmartMeshService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  NetworkKey);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.NetworkKey_AirCableSmartMeshService_enum, "NetworkKey", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.NetworkKey_AirCableSmartMeshService_enum, "NetworkKey", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _DeviceUuid = null;
        private bool _DeviceUuid_set = false;
        public string DeviceUuid
        {
            get { return _DeviceUuid; }
            internal set { if (_DeviceUuid_set && value == _DeviceUuid) return; _DeviceUuid = value; _DeviceUuid_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDeviceUuid(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.DeviceUuid_AirCableSmartMeshService_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.DeviceUuid_AirCableSmartMeshService_enum, "DeviceUuid", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|DeviceUuid";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            DeviceUuid = parseResult.ValueList.GetValue("DeviceUuid").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _DeviceId = null;
        private bool _DeviceId_set = false;
        public string DeviceId
        {
            get { return _DeviceId; }
            internal set { if (_DeviceId_set && value == _DeviceId) return; _DeviceId = value; _DeviceId_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDeviceId(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.DeviceId_AirCableSmartMeshService_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.DeviceId_AirCableSmartMeshService_enum, "DeviceId", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|DeviceId";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            DeviceId = parseResult.ValueList.GetValue("DeviceId").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for DeviceId
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteDeviceId(byte[] DeviceId)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.DeviceId_AirCableSmartMeshService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  DeviceId);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.DeviceId_AirCableSmartMeshService_enum, "DeviceId", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.DeviceId_AirCableSmartMeshService_enum, "DeviceId", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _MtlContinuationCpUuid = null;
        private bool _MtlContinuationCpUuid_set = false;
        public string MtlContinuationCpUuid
        {
            get { return _MtlContinuationCpUuid; }
            internal set { if (_MtlContinuationCpUuid_set && value == _MtlContinuationCpUuid) return; _MtlContinuationCpUuid = value; _MtlContinuationCpUuid_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; MtlContinuationCpUuidEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent MtlContinuationCpUuidEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMtlContinuationCpUuid_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMtlContinuationCpUuidAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MtlContinuationCpUuid_AirCableSmartMeshService_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.MtlContinuationCpUuid_AirCableSmartMeshService_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMtlContinuationCpUuid_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMtlContinuationCpUuid_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyMtlContinuationCpUuidCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMtlContinuationCpUuid: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMtlContinuationCpUuid: set notification", result);

            return true;
        }

        private void NotifyMtlContinuationCpUuidCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|MtlContinuationCpUuid";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            MtlContinuationCpUuid = parseResult.ValueList.GetValue("MtlContinuationCpUuid").AsString;

            MtlContinuationCpUuidEvent?.Invoke(parseResult);

        }

        public void NotifyMtlContinuationCpUuidRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.MtlContinuationCpUuid_AirCableSmartMeshService_enum];
            if (ch == null) return;
            NotifyMtlContinuationCpUuid_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyMtlContinuationCpUuidCallback;
        }

        /// <summary>
        /// Writes data for MtlContinuationCpUuid
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMtlContinuationCpUuid(byte[] MtlContinuationCpUuid)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MtlContinuationCpUuid_AirCableSmartMeshService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  MtlContinuationCpUuid);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.MtlContinuationCpUuid_AirCableSmartMeshService_enum, "MtlContinuationCpUuid", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.MtlContinuationCpUuid_AirCableSmartMeshService_enum, "MtlContinuationCpUuid", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _MtlCompleteCpUuid = null;
        private bool _MtlCompleteCpUuid_set = false;
        public string MtlCompleteCpUuid
        {
            get { return _MtlCompleteCpUuid; }
            internal set { if (_MtlCompleteCpUuid_set && value == _MtlCompleteCpUuid) return; _MtlCompleteCpUuid = value; _MtlCompleteCpUuid_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; MtlCompleteCpUuidEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent MtlCompleteCpUuidEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMtlCompleteCpUuid_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMtlCompleteCpUuidAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MtlCompleteCpUuid_AirCableSmartMeshService_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.MtlCompleteCpUuid_AirCableSmartMeshService_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMtlCompleteCpUuid_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMtlCompleteCpUuid_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyMtlCompleteCpUuidCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMtlCompleteCpUuid: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMtlCompleteCpUuid: set notification", result);

            return true;
        }

        private void NotifyMtlCompleteCpUuidCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|MtlCompleteCpUuid";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            MtlCompleteCpUuid = parseResult.ValueList.GetValue("MtlCompleteCpUuid").AsString;

            MtlCompleteCpUuidEvent?.Invoke(parseResult);

        }

        public void NotifyMtlCompleteCpUuidRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.MtlCompleteCpUuid_AirCableSmartMeshService_enum];
            if (ch == null) return;
            NotifyMtlCompleteCpUuid_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyMtlCompleteCpUuidCallback;
        }

        /// <summary>
        /// Writes data for MtlCompleteCpUuid
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMtlCompleteCpUuid(byte[] MtlCompleteCpUuid)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MtlCompleteCpUuid_AirCableSmartMeshService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  MtlCompleteCpUuid);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.MtlCompleteCpUuid_AirCableSmartMeshService_enum, "MtlCompleteCpUuid", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.MtlCompleteCpUuid_AirCableSmartMeshService_enum, "MtlCompleteCpUuid", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _MtlTtlUuid = null;
        private bool _MtlTtlUuid_set = false;
        public string MtlTtlUuid
        {
            get { return _MtlTtlUuid; }
            internal set { if (_MtlTtlUuid_set && value == _MtlTtlUuid) return; _MtlTtlUuid = value; _MtlTtlUuid_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMtlTtlUuid(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MtlTtlUuid_AirCableSmartMeshService_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.MtlTtlUuid_AirCableSmartMeshService_enum, "MtlTtlUuid", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|MtlTtlUuid";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            MtlTtlUuid = parseResult.ValueList.GetValue("MtlTtlUuid").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for MtlTtlUuid
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMtlTtlUuid(byte[] MtlTtlUuid)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MtlTtlUuid_AirCableSmartMeshService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  MtlTtlUuid);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.MtlTtlUuid_AirCableSmartMeshService_enum, "MtlTtlUuid", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.MtlTtlUuid_AirCableSmartMeshService_enum, "MtlTtlUuid", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _MeshAppearanceUuid = null;
        private bool _MeshAppearanceUuid_set = false;
        public string MeshAppearanceUuid
        {
            get { return _MeshAppearanceUuid; }
            internal set { if (_MeshAppearanceUuid_set && value == _MeshAppearanceUuid) return; _MeshAppearanceUuid = value; _MeshAppearanceUuid_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMeshAppearanceUuid(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MeshAppearanceUuid_AirCableSmartMeshService_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.MeshAppearanceUuid_AirCableSmartMeshService_enum, "MeshAppearanceUuid", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|MeshAppearanceUuid";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            MeshAppearanceUuid = parseResult.ValueList.GetValue("MeshAppearanceUuid").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for MeshAppearanceUuid
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMeshAppearanceUuid(byte[] MeshAppearanceUuid)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MeshAppearanceUuid_AirCableSmartMeshService_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  MeshAppearanceUuid);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.MeshAppearanceUuid_AirCableSmartMeshService_enum, "MeshAppearanceUuid", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.MeshAppearanceUuid_AirCableSmartMeshService_enum, "MeshAppearanceUuid", subcommand, GattWriteOption.WriteWithResponse);
            }
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





    }
}