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
    /// The Nordic Thingy:52™ is an easy-to-use prototyping platform, designed to help in building prototypes and demos, without the need to build hardware or even write firmware. It is built around the nRF52832 Bluetooth 5 SoC.
    /// This class was automatically generated 2022-12-08::04:46
    /// </summary>

    public  class Nordic_Thingy : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://nordicsemiconductor.github.io/Nordic-Thingy52-FW/documentation/firmware_architecture.html#fw_arch_ble_services


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("EF680200-9B35-4933-9B10-52FFA9740042"),
           Guid.Parse("EF680400-9B35-4933-9B10-52FFA9740042"),
           Guid.Parse("EF680300-9B35-4933-9B10-52FFA9740042"),
           Guid.Parse("EF680500-9B35-4933-9B10-52FFA9740042"),
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("EF680100-9B35-4933-9B10-52FFA9740042"),

        };
        String[] ServiceNames = new string[] {
            "Environment",
            "Motion",
            "UI",
            "Audio",
            "Common Configuration",
            "Generic Service",
            "Battery",
            "Configuration",

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

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("EF680201-9B35-4933-9B10-52FFA9740042"), // #0 is Temperature (c)
            Guid.Parse("EF680202-9B35-4933-9B10-52FFA9740042"), // #1 is Pressure (hpa)
            Guid.Parse("EF680203-9B35-4933-9B10-52FFA9740042"), // #2 is Humidity (%)
            Guid.Parse("EF680204-9B35-4933-9B10-52FFA9740042"), // #3 is Air Quality eCOS TVOC
            Guid.Parse("EF680205-9B35-4933-9B10-52FFA9740042"), // #4 is Color RGB+Clear
            Guid.Parse("EF680206-9B35-4933-9B10-52FFA9740042"), // #5 is Environment Configuration
            Guid.Parse("EF680401-9B35-4933-9B10-52FFA9740042"), // #0 is Motion Configuration
            Guid.Parse("EF680402-9B35-4933-9B10-52FFA9740042"), // #1 is Taps
            Guid.Parse("EF680403-9B35-4933-9B10-52FFA9740042"), // #2 is Orientation
            Guid.Parse("EF680404-9B35-4933-9B10-52FFA9740042"), // #3 is Quaternions
            Guid.Parse("EF680405-9B35-4933-9B10-52FFA9740042"), // #4 is Step Counter
            Guid.Parse("EF680406-9B35-4933-9B10-52FFA9740042"), // #5 is Raw Motion
            Guid.Parse("EF680407-9B35-4933-9B10-52FFA9740042"), // #6 is Euler
            Guid.Parse("EF680408-9B35-4933-9B10-52FFA9740042"), // #7 is RotationMatrix
            Guid.Parse("EF680409-9B35-4933-9B10-52FFA9740042"), // #8 is Compass Heading
            Guid.Parse("EF68040A-9B35-4933-9B10-52FFA9740042"), // #9 is Gravity
            Guid.Parse("EF680301-9B35-4933-9B10-52FFA9740042"), // #0 is LED Characteristics
            Guid.Parse("EF680302-9B35-4933-9B10-52FFA9740042"), // #1 is Button
            Guid.Parse("EF680303-9B35-4933-9B10-52FFA9740042"), // #2 is External pin
            Guid.Parse("EF680501-9B35-4933-9B10-52FFA9740042"), // #0 is Audio Configuration
            Guid.Parse("EF680502-9B35-4933-9B10-52FFA9740042"), // #1 is Speaker Data
            Guid.Parse("EF680503-9B35-4933-9B10-52FFA9740042"), // #2 is Speaker Status
            Guid.Parse("EF680504-9B35-4933-9B10-52FFA9740042"), // #3 is Microphone Data
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #2 is Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #3 is Central Address Resolution
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #0 is Service Changes
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #0 is BatteryLevel
            Guid.Parse("EF680101-9B35-4933-9B10-52FFA9740042"), // #0 is Configuration Device Name
            Guid.Parse("EF680102-9B35-4933-9B10-52FFA9740042"), // #1 is Advertising Parameter
            Guid.Parse("EF680104-9B35-4933-9B10-52FFA9740042"), // #2 is Connection parameters
            Guid.Parse("EF680105-9B35-4933-9B10-52FFA9740042"), // #3 is Eddystone URL
            Guid.Parse("EF680106-9B35-4933-9B10-52FFA9740042"), // #4 is Cloud Token
            Guid.Parse("EF680107-9B35-4933-9B10-52FFA9740042"), // #5 is Firmware Version
            Guid.Parse("EF680108-9B35-4933-9B10-52FFA9740042"), // #6 is MTU Request
            Guid.Parse("EF680109-9B35-4933-9B10-52FFA9740042"), // #7 is NFC Tag

        };
        String[] CharacteristicNames = new string[] {
            "Temperature (c)", // #0 is EF680201-9B35-4933-9B10-52FFA9740042
            "Pressure (hpa)", // #1 is EF680202-9B35-4933-9B10-52FFA9740042
            "Humidity (%)", // #2 is EF680203-9B35-4933-9B10-52FFA9740042
            "Air Quality eCOS TVOC", // #3 is EF680204-9B35-4933-9B10-52FFA9740042
            "Color RGB+Clear", // #4 is EF680205-9B35-4933-9B10-52FFA9740042
            "Environment Configuration", // #5 is EF680206-9B35-4933-9B10-52FFA9740042
            "Motion Configuration", // #0 is EF680401-9B35-4933-9B10-52FFA9740042
            "Taps", // #1 is EF680402-9B35-4933-9B10-52FFA9740042
            "Orientation", // #2 is EF680403-9B35-4933-9B10-52FFA9740042
            "Quaternions", // #3 is EF680404-9B35-4933-9B10-52FFA9740042
            "Step Counter", // #4 is EF680405-9B35-4933-9B10-52FFA9740042
            "Raw Motion", // #5 is EF680406-9B35-4933-9B10-52FFA9740042
            "Euler", // #6 is EF680407-9B35-4933-9B10-52FFA9740042
            "RotationMatrix", // #7 is EF680408-9B35-4933-9B10-52FFA9740042
            "Compass Heading", // #8 is EF680409-9B35-4933-9B10-52FFA9740042
            "Gravity", // #9 is EF68040A-9B35-4933-9B10-52FFA9740042
            "LED Characteristics", // #0 is EF680301-9B35-4933-9B10-52FFA9740042
            "Button", // #1 is EF680302-9B35-4933-9B10-52FFA9740042
            "External pin", // #2 is EF680303-9B35-4933-9B10-52FFA9740042
            "Audio Configuration", // #0 is EF680501-9B35-4933-9B10-52FFA9740042
            "Speaker Data", // #1 is EF680502-9B35-4933-9B10-52FFA9740042
            "Speaker Status", // #2 is EF680503-9B35-4933-9B10-52FFA9740042
            "Microphone Data", // #3 is EF680504-9B35-4933-9B10-52FFA9740042
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #2 is 00002a04-0000-1000-8000-00805f9b34fb
            "Central Address Resolution", // #3 is 00002aa6-0000-1000-8000-00805f9b34fb
            "Service Changes", // #0 is 00002a05-0000-1000-8000-00805f9b34fb
            "BatteryLevel", // #0 is 00002a19-0000-1000-8000-00805f9b34fb
            "Configuration Device Name", // #0 is EF680101-9B35-4933-9B10-52FFA9740042
            "Advertising Parameter", // #1 is EF680102-9B35-4933-9B10-52FFA9740042
            "Connection parameters", // #2 is EF680104-9B35-4933-9B10-52FFA9740042
            "Eddystone URL", // #3 is EF680105-9B35-4933-9B10-52FFA9740042
            "Cloud Token", // #4 is EF680106-9B35-4933-9B10-52FFA9740042
            "Firmware Version", // #5 is EF680107-9B35-4933-9B10-52FFA9740042
            "MTU Request", // #6 is EF680108-9B35-4933-9B10-52FFA9740042
            "NFC Tag", // #7 is EF680109-9B35-4933-9B10-52FFA9740042

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
            null,
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3, 4, 5,  },
            new HashSet<int>(){ 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,  },
            new HashSet<int>(){ 16, 17, 18,  },
            new HashSet<int>(){ 19, 20, 21, 22,  },
            new HashSet<int>(){ 23, 24, 25, 26,  },
            new HashSet<int>(){ 27,  },
            new HashSet<int>(){ 28,  },
            new HashSet<int>(){ 29, 30, 31, 32, 33, 34, 35, 36,  },

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
            1, // Characteristic 10
            1, // Characteristic 11
            1, // Characteristic 12
            1, // Characteristic 13
            1, // Characteristic 14
            1, // Characteristic 15
            2, // Characteristic 16
            2, // Characteristic 17
            2, // Characteristic 18
            3, // Characteristic 19
            3, // Characteristic 20
            3, // Characteristic 21
            3, // Characteristic 22
            4, // Characteristic 23
            4, // Characteristic 24
            4, // Characteristic 25
            4, // Characteristic 26
            5, // Characteristic 27
            6, // Characteristic 28
            7, // Characteristic 29
            7, // Characteristic 30
            7, // Characteristic 31
            7, // Characteristic 32
            7, // Characteristic 33
            7, // Characteristic 34
            7, // Characteristic 35
            7, // Characteristic 36
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Temperature_c_Environment_enum = 0,
            Pressure_hpa_Environment_enum = 1,
            Humidity_Environment_enum = 2,
            Air_Quality_eCOS_TVOC_Environment_enum = 3,
            Color_RGB_Clear_Environment_enum = 4,
            Environment_Configuration_Environment_enum = 5,
            Motion_Configuration_Motion_enum = 6,
            Taps_Motion_enum = 7,
            Orientation_Motion_enum = 8,
            Quaternions_Motion_enum = 9,
            Step_Counter_Motion_enum = 10,
            Raw_Motion_Motion_enum = 11,
            Euler_Motion_enum = 12,
            RotationMatrix_Motion_enum = 13,
            Compass_Heading_Motion_enum = 14,
            Gravity_Motion_enum = 15,
            LED_Characteristics_UI_enum = 16,
            Button_UI_enum = 17,
            External_pin_UI_enum = 18,
            Audio_Configuration_Audio_enum = 19,
            Speaker_Data_Audio_enum = 20,
            Speaker_Status_Audio_enum = 21,
            Microphone_Data_Audio_enum = 22,
            Device_Name_Common_Configuration_enum = 23,
            Appearance_Common_Configuration_enum = 24,
            Connection_Parameter_Common_Configuration_enum = 25,
            Central_Address_Resolution_Common_Configuration_enum = 26,
            Service_Changes_Generic_Service_enum = 27,
            BatteryLevel_Battery_enum = 28,
            Configuration_Device_Name_Configuration_enum = 29,
            Advertising_Parameter_Configuration_enum = 30,
            Connection_parameters_Configuration_enum = 31,
            Eddystone_URL_Configuration_enum = 32,
            Cloud_Token_Configuration_enum = 33,
            Firmware_Version_Configuration_enum = 34,
            MTU_Request_Configuration_enum = 35,
            NFC_Tag_Configuration_enum = 36,

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



        private double _Temperature_c = 0.0;
        private bool _Temperature_c_set = false;
        public double Temperature_c
        {
            get { return _Temperature_c; }
            internal set { if (_Temperature_c_set && value == _Temperature_c) return; _Temperature_c = value; _Temperature_c_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Temperature_cEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Temperature_cEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyTemperature_c_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyTemperature_cAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_c_Environment_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Temperature_c_Environment_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTemperature_c_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTemperature_c_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyTemperature_cCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyTemperature_c: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyTemperature_c: set notification", result);

            return true;
        }

        private void NotifyTemperature_cCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "/I8/P8|FIXED|Temperature|C";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Temperature_c = parseResult.ValueList.GetValue("Temperature").AsDouble;

            Temperature_cEvent?.Invoke(parseResult);

        }

        public void NotifyTemperature_cRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Temperature_c_Environment_enum];
            if (ch == null) return;
            NotifyTemperature_c_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyTemperature_cCallback;
        }



        private double _Pressure_hpa = 0.0;
        private bool _Pressure_hpa_set = false;
        public double Pressure_hpa
        {
            get { return _Pressure_hpa; }
            internal set { if (_Pressure_hpa_set && value == _Pressure_hpa) return; _Pressure_hpa = value; _Pressure_hpa_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Pressure_hpaEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Pressure_hpaEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyPressure_hpa_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyPressure_hpaAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pressure_hpa_Environment_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Pressure_hpa_Environment_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyPressure_hpa_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyPressure_hpa_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyPressure_hpaCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyPressure_hpa: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyPressure_hpa: set notification", result);

            return true;
        }

        private void NotifyPressure_hpaCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "/I32/P8|FIXED|Pressure|hPA";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Pressure_hpa = parseResult.ValueList.GetValue("Pressure").AsDouble;

            Pressure_hpaEvent?.Invoke(parseResult);

        }

        public void NotifyPressure_hpaRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Pressure_hpa_Environment_enum];
            if (ch == null) return;
            NotifyPressure_hpa_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyPressure_hpaCallback;
        }



        private double _Humidity = 0;
        private bool _Humidity_set = false;
        public double Humidity
        {
            get { return _Humidity; }
            internal set { if (_Humidity_set && value == _Humidity) return; _Humidity = value; _Humidity_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; HumidityEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent HumidityEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyHumidity_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyHumidityAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Humidity_Environment_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Humidity_Environment_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyHumidity_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyHumidity_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyHumidityCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyHumidity: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyHumidity: set notification", result);

            return true;
        }

        private void NotifyHumidityCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|DEC|Humidity|%";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Humidity = parseResult.ValueList.GetValue("Humidity").AsDouble;

            HumidityEvent?.Invoke(parseResult);

        }

        public void NotifyHumidityRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Humidity_Environment_enum];
            if (ch == null) return;
            NotifyHumidity_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyHumidityCallback;
        }



        private double _Air_Quality_eCOS_TVOC_eCOS = 0;
        private bool _Air_Quality_eCOS_TVOC_eCOS_set = false;
        public double Air_Quality_eCOS_TVOC_eCOS
        {
            get { return _Air_Quality_eCOS_TVOC_eCOS; }
            internal set { if (_Air_Quality_eCOS_TVOC_eCOS_set && value == _Air_Quality_eCOS_TVOC_eCOS) return; _Air_Quality_eCOS_TVOC_eCOS = value; _Air_Quality_eCOS_TVOC_eCOS_set = true; OnPropertyChanged(); }
        }
        private double _Air_Quality_eCOS_TVOC_TVOC = 0;
        private bool _Air_Quality_eCOS_TVOC_TVOC_set = false;
        public double Air_Quality_eCOS_TVOC_TVOC
        {
            get { return _Air_Quality_eCOS_TVOC_TVOC; }
            internal set { if (_Air_Quality_eCOS_TVOC_TVOC_set && value == _Air_Quality_eCOS_TVOC_TVOC) return; _Air_Quality_eCOS_TVOC_TVOC = value; _Air_Quality_eCOS_TVOC_TVOC_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Air_Quality_eCOS_TVOCEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Air_Quality_eCOS_TVOCEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyAir_Quality_eCOS_TVOC_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyAir_Quality_eCOS_TVOCAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Air_Quality_eCOS_TVOC_Environment_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Air_Quality_eCOS_TVOC_Environment_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyAir_Quality_eCOS_TVOC_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyAir_Quality_eCOS_TVOC_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyAir_Quality_eCOS_TVOCCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyAir_Quality_eCOS_TVOC: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyAir_Quality_eCOS_TVOC: set notification", result);

            return true;
        }

        private void NotifyAir_Quality_eCOS_TVOCCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U16|DEC|eCOS|ppm U16|DEC|TVOC|ppb";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Air_Quality_eCOS_TVOC_eCOS = parseResult.ValueList.GetValue("eCOS").AsDouble;
            Air_Quality_eCOS_TVOC_TVOC = parseResult.ValueList.GetValue("TVOC").AsDouble;

            Air_Quality_eCOS_TVOCEvent?.Invoke(parseResult);

        }

        public void NotifyAir_Quality_eCOS_TVOCRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Air_Quality_eCOS_TVOC_Environment_enum];
            if (ch == null) return;
            NotifyAir_Quality_eCOS_TVOC_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyAir_Quality_eCOS_TVOCCallback;
        }



        private double _Color_RGB_Clear_Red = 0;
        private bool _Color_RGB_Clear_Red_set = false;
        public double Color_RGB_Clear_Red
        {
            get { return _Color_RGB_Clear_Red; }
            internal set { if (_Color_RGB_Clear_Red_set && value == _Color_RGB_Clear_Red) return; _Color_RGB_Clear_Red = value; _Color_RGB_Clear_Red_set = true; OnPropertyChanged(); }
        }
        private double _Color_RGB_Clear_Green = 0;
        private bool _Color_RGB_Clear_Green_set = false;
        public double Color_RGB_Clear_Green
        {
            get { return _Color_RGB_Clear_Green; }
            internal set { if (_Color_RGB_Clear_Green_set && value == _Color_RGB_Clear_Green) return; _Color_RGB_Clear_Green = value; _Color_RGB_Clear_Green_set = true; OnPropertyChanged(); }
        }
        private double _Color_RGB_Clear_Blue = 0;
        private bool _Color_RGB_Clear_Blue_set = false;
        public double Color_RGB_Clear_Blue
        {
            get { return _Color_RGB_Clear_Blue; }
            internal set { if (_Color_RGB_Clear_Blue_set && value == _Color_RGB_Clear_Blue) return; _Color_RGB_Clear_Blue = value; _Color_RGB_Clear_Blue_set = true; OnPropertyChanged(); }
        }
        private double _Color_RGB_Clear_Clear = 0;
        private bool _Color_RGB_Clear_Clear_set = false;
        public double Color_RGB_Clear_Clear
        {
            get { return _Color_RGB_Clear_Clear; }
            internal set { if (_Color_RGB_Clear_Clear_set && value == _Color_RGB_Clear_Clear) return; _Color_RGB_Clear_Clear = value; _Color_RGB_Clear_Clear_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Color_RGB_ClearEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Color_RGB_ClearEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyColor_RGB_Clear_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyColor_RGB_ClearAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Color_RGB_Clear_Environment_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Color_RGB_Clear_Environment_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyColor_RGB_Clear_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyColor_RGB_Clear_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyColor_RGB_ClearCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyColor_RGB_Clear: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyColor_RGB_Clear: set notification", result);

            return true;
        }

        private void NotifyColor_RGB_ClearCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U16|DEC|Red U16|DEC|Green U16|DEC|Blue U16|DEC|Clear";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Color_RGB_Clear_Red = parseResult.ValueList.GetValue("Red").AsDouble;
            Color_RGB_Clear_Green = parseResult.ValueList.GetValue("Green").AsDouble;
            Color_RGB_Clear_Blue = parseResult.ValueList.GetValue("Blue").AsDouble;
            Color_RGB_Clear_Clear = parseResult.ValueList.GetValue("Clear").AsDouble;

            Color_RGB_ClearEvent?.Invoke(parseResult);

        }

        public void NotifyColor_RGB_ClearRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Color_RGB_Clear_Environment_enum];
            if (ch == null) return;
            NotifyColor_RGB_Clear_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyColor_RGB_ClearCallback;
        }



        private double _Environment_Configuration_TempInterval = 0;
        private bool _Environment_Configuration_TempInterval_set = false;
        public double Environment_Configuration_TempInterval
        {
            get { return _Environment_Configuration_TempInterval; }
            internal set { if (_Environment_Configuration_TempInterval_set && value == _Environment_Configuration_TempInterval) return; _Environment_Configuration_TempInterval = value; _Environment_Configuration_TempInterval_set = true; OnPropertyChanged(); }
        }
        private double _Environment_Configuration_PressureInterval = 0;
        private bool _Environment_Configuration_PressureInterval_set = false;
        public double Environment_Configuration_PressureInterval
        {
            get { return _Environment_Configuration_PressureInterval; }
            internal set { if (_Environment_Configuration_PressureInterval_set && value == _Environment_Configuration_PressureInterval) return; _Environment_Configuration_PressureInterval = value; _Environment_Configuration_PressureInterval_set = true; OnPropertyChanged(); }
        }
        private double _Environment_Configuration_HumidityInterval = 0;
        private bool _Environment_Configuration_HumidityInterval_set = false;
        public double Environment_Configuration_HumidityInterval
        {
            get { return _Environment_Configuration_HumidityInterval; }
            internal set { if (_Environment_Configuration_HumidityInterval_set && value == _Environment_Configuration_HumidityInterval) return; _Environment_Configuration_HumidityInterval = value; _Environment_Configuration_HumidityInterval_set = true; OnPropertyChanged(); }
        }
        private double _Environment_Configuration_ColorInterval = 0;
        private bool _Environment_Configuration_ColorInterval_set = false;
        public double Environment_Configuration_ColorInterval
        {
            get { return _Environment_Configuration_ColorInterval; }
            internal set { if (_Environment_Configuration_ColorInterval_set && value == _Environment_Configuration_ColorInterval) return; _Environment_Configuration_ColorInterval = value; _Environment_Configuration_ColorInterval_set = true; OnPropertyChanged(); }
        }
        private double _Environment_Configuration_GasMode = 0;
        private bool _Environment_Configuration_GasMode_set = false;
        public double Environment_Configuration_GasMode
        {
            get { return _Environment_Configuration_GasMode; }
            internal set { if (_Environment_Configuration_GasMode_set && value == _Environment_Configuration_GasMode) return; _Environment_Configuration_GasMode = value; _Environment_Configuration_GasMode_set = true; OnPropertyChanged(); }
        }
        private double _Environment_Configuration_RedCalibration = 0;
        private bool _Environment_Configuration_RedCalibration_set = false;
        public double Environment_Configuration_RedCalibration
        {
            get { return _Environment_Configuration_RedCalibration; }
            internal set { if (_Environment_Configuration_RedCalibration_set && value == _Environment_Configuration_RedCalibration) return; _Environment_Configuration_RedCalibration = value; _Environment_Configuration_RedCalibration_set = true; OnPropertyChanged(); }
        }
        private double _Environment_Configuration_GreenCalibration = 0;
        private bool _Environment_Configuration_GreenCalibration_set = false;
        public double Environment_Configuration_GreenCalibration
        {
            get { return _Environment_Configuration_GreenCalibration; }
            internal set { if (_Environment_Configuration_GreenCalibration_set && value == _Environment_Configuration_GreenCalibration) return; _Environment_Configuration_GreenCalibration = value; _Environment_Configuration_GreenCalibration_set = true; OnPropertyChanged(); }
        }
        private double _Environment_Configuration_BlueCalibration = 0;
        private bool _Environment_Configuration_BlueCalibration_set = false;
        public double Environment_Configuration_BlueCalibration
        {
            get { return _Environment_Configuration_BlueCalibration; }
            internal set { if (_Environment_Configuration_BlueCalibration_set && value == _Environment_Configuration_BlueCalibration) return; _Environment_Configuration_BlueCalibration = value; _Environment_Configuration_BlueCalibration_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadEnvironment_Configuration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Environment_Configuration_Environment_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Environment_Configuration_Environment_enum, "Environment_Configuration", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|TempInterval|ms U16|DEC|PressureInterval|ms U16|DEC|HumidityInterval|ms U16|DEC|ColorInterval|ms U8|DEC|GasMode U8|DEC|RedCalibration U8|DEC|GreenCalibration U8|DEC|BlueCalibration";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Environment_Configuration_TempInterval = parseResult.ValueList.GetValue("TempInterval").AsDouble;
            Environment_Configuration_PressureInterval = parseResult.ValueList.GetValue("PressureInterval").AsDouble;
            Environment_Configuration_HumidityInterval = parseResult.ValueList.GetValue("HumidityInterval").AsDouble;
            Environment_Configuration_ColorInterval = parseResult.ValueList.GetValue("ColorInterval").AsDouble;
            Environment_Configuration_GasMode = parseResult.ValueList.GetValue("GasMode").AsDouble;
            Environment_Configuration_RedCalibration = parseResult.ValueList.GetValue("RedCalibration").AsDouble;
            Environment_Configuration_GreenCalibration = parseResult.ValueList.GetValue("GreenCalibration").AsDouble;
            Environment_Configuration_BlueCalibration = parseResult.ValueList.GetValue("BlueCalibration").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Environment_Configuration
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteEnvironment_Configuration(UInt16 TempInterval, UInt16 PressureInterval, UInt16 HumidityInterval, UInt16 ColorInterval, byte GasMode, byte RedCalibration, byte GreenCalibration, byte BlueCalibration)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Environment_Configuration_Environment_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(  TempInterval);
            dw.WriteUInt16(  PressureInterval);
            dw.WriteUInt16(  HumidityInterval);
            dw.WriteUInt16(  ColorInterval);
            dw.WriteByte(  GasMode);
            dw.WriteByte(  RedCalibration);
            dw.WriteByte(  GreenCalibration);
            dw.WriteByte(  BlueCalibration);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Environment_Configuration_Environment_enum, "Environment_Configuration", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Environment_Configuration_Environment_enum, "Environment_Configuration", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Motion_Configuration_param0 = 0;
        private bool _Motion_Configuration_param0_set = false;
        public double Motion_Configuration_param0
        {
            get { return _Motion_Configuration_param0; }
            internal set { if (_Motion_Configuration_param0_set && value == _Motion_Configuration_param0) return; _Motion_Configuration_param0 = value; _Motion_Configuration_param0_set = true; OnPropertyChanged(); }
        }
        private double _Motion_Configuration_param1 = 0;
        private bool _Motion_Configuration_param1_set = false;
        public double Motion_Configuration_param1
        {
            get { return _Motion_Configuration_param1; }
            internal set { if (_Motion_Configuration_param1_set && value == _Motion_Configuration_param1) return; _Motion_Configuration_param1 = value; _Motion_Configuration_param1_set = true; OnPropertyChanged(); }
        }
        private double _Motion_Configuration_param2 = 0;
        private bool _Motion_Configuration_param2_set = false;
        public double Motion_Configuration_param2
        {
            get { return _Motion_Configuration_param2; }
            internal set { if (_Motion_Configuration_param2_set && value == _Motion_Configuration_param2) return; _Motion_Configuration_param2 = value; _Motion_Configuration_param2_set = true; OnPropertyChanged(); }
        }
        private double _Motion_Configuration_param3 = 0;
        private bool _Motion_Configuration_param3_set = false;
        public double Motion_Configuration_param3
        {
            get { return _Motion_Configuration_param3; }
            internal set { if (_Motion_Configuration_param3_set && value == _Motion_Configuration_param3) return; _Motion_Configuration_param3 = value; _Motion_Configuration_param3_set = true; OnPropertyChanged(); }
        }
        private double _Motion_Configuration_param4 = 0;
        private bool _Motion_Configuration_param4_set = false;
        public double Motion_Configuration_param4
        {
            get { return _Motion_Configuration_param4; }
            internal set { if (_Motion_Configuration_param4_set && value == _Motion_Configuration_param4) return; _Motion_Configuration_param4 = value; _Motion_Configuration_param4_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMotion_Configuration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Motion_Configuration_Motion_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Motion_Configuration_Motion_enum, "Motion_Configuration", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC U16|DEC U16|DEC U16|DEC U8";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Motion_Configuration_param0 = parseResult.ValueList.GetValue("param0").AsDouble;
            Motion_Configuration_param1 = parseResult.ValueList.GetValue("param1").AsDouble;
            Motion_Configuration_param2 = parseResult.ValueList.GetValue("param2").AsDouble;
            Motion_Configuration_param3 = parseResult.ValueList.GetValue("param3").AsDouble;
            Motion_Configuration_param4 = parseResult.ValueList.GetValue("param4").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Motion_Configuration
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMotion_Configuration(UInt16 param0, UInt16 param1, UInt16 param2, UInt16 param3, byte param4)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Motion_Configuration_Motion_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(  param0);
            dw.WriteUInt16(  param1);
            dw.WriteUInt16(  param2);
            dw.WriteUInt16(  param3);
            dw.WriteByte(  param4);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Motion_Configuration_Motion_enum, "Motion_Configuration", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Motion_Configuration_Motion_enum, "Motion_Configuration", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Taps_param0 = 0;
        private bool _Taps_param0_set = false;
        public double Taps_param0
        {
            get { return _Taps_param0; }
            internal set { if (_Taps_param0_set && value == _Taps_param0) return; _Taps_param0 = value; _Taps_param0_set = true; OnPropertyChanged(); }
        }
        private double _Taps_param1 = 0;
        private bool _Taps_param1_set = false;
        public double Taps_param1
        {
            get { return _Taps_param1; }
            internal set { if (_Taps_param1_set && value == _Taps_param1) return; _Taps_param1 = value; _Taps_param1_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; TapsEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent TapsEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyTaps_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyTapsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Taps_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Taps_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTaps_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTaps_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyTapsCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyTaps: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyTaps: set notification", result);

            return true;
        }

        private void NotifyTapsCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8 U8|DEC";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Taps_param0 = parseResult.ValueList.GetValue("param0").AsDouble;
            Taps_param1 = parseResult.ValueList.GetValue("param1").AsDouble;

            TapsEvent?.Invoke(parseResult);

        }

        public void NotifyTapsRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Taps_Motion_enum];
            if (ch == null) return;
            NotifyTaps_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyTapsCallback;
        }



        private double _Orientation = 0;
        private bool _Orientation_set = false;
        public double Orientation
        {
            get { return _Orientation; }
            internal set { if (_Orientation_set && value == _Orientation) return; _Orientation = value; _Orientation_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; OrientationEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent OrientationEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyOrientation_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyOrientationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Orientation_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Orientation_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyOrientation_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyOrientation_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyOrientationCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyOrientation: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyOrientation: set notification", result);

            return true;
        }

        private void NotifyOrientationCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Orientation = parseResult.ValueList.GetValue("param0").AsDouble;

            OrientationEvent?.Invoke(parseResult);

        }

        public void NotifyOrientationRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Orientation_Motion_enum];
            if (ch == null) return;
            NotifyOrientation_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyOrientationCallback;
        }



        private double _Quaternions_W = 0.0;
        private bool _Quaternions_W_set = false;
        public double Quaternions_W
        {
            get { return _Quaternions_W; }
            internal set { if (_Quaternions_W_set && value == _Quaternions_W) return; _Quaternions_W = value; _Quaternions_W_set = true; OnPropertyChanged(); }
        }
        private double _Quaternions_X = 0.0;
        private bool _Quaternions_X_set = false;
        public double Quaternions_X
        {
            get { return _Quaternions_X; }
            internal set { if (_Quaternions_X_set && value == _Quaternions_X) return; _Quaternions_X = value; _Quaternions_X_set = true; OnPropertyChanged(); }
        }
        private double _Quaternions_Y = 0.0;
        private bool _Quaternions_Y_set = false;
        public double Quaternions_Y
        {
            get { return _Quaternions_Y; }
            internal set { if (_Quaternions_Y_set && value == _Quaternions_Y) return; _Quaternions_Y = value; _Quaternions_Y_set = true; OnPropertyChanged(); }
        }
        private double _Quaternions_Z = 0.0;
        private bool _Quaternions_Z_set = false;
        public double Quaternions_Z
        {
            get { return _Quaternions_Z; }
            internal set { if (_Quaternions_Z_set && value == _Quaternions_Z) return; _Quaternions_Z = value; _Quaternions_Z_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; QuaternionsEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent QuaternionsEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyQuaternions_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyQuaternionsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Quaternions_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Quaternions_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyQuaternions_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyQuaternions_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyQuaternionsCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyQuaternions: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyQuaternions: set notification", result);

            return true;
        }

        private void NotifyQuaternionsCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "Q2Q30|DEC|W Q2Q30|DEC|X Q2Q30|DEC|Y Q2Q30|DEC|Z";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Quaternions_W = parseResult.ValueList.GetValue("W").AsDouble;
            Quaternions_X = parseResult.ValueList.GetValue("X").AsDouble;
            Quaternions_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Quaternions_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            QuaternionsEvent?.Invoke(parseResult);

        }

        public void NotifyQuaternionsRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Quaternions_Motion_enum];
            if (ch == null) return;
            NotifyQuaternions_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyQuaternionsCallback;
        }



        private double _Step_Counter_Steps = 0;
        private bool _Step_Counter_Steps_set = false;
        public double Step_Counter_Steps
        {
            get { return _Step_Counter_Steps; }
            internal set { if (_Step_Counter_Steps_set && value == _Step_Counter_Steps) return; _Step_Counter_Steps = value; _Step_Counter_Steps_set = true; OnPropertyChanged(); }
        }
        private double _Step_Counter_Time = 0;
        private bool _Step_Counter_Time_set = false;
        public double Step_Counter_Time
        {
            get { return _Step_Counter_Time; }
            internal set { if (_Step_Counter_Time_set && value == _Step_Counter_Time) return; _Step_Counter_Time = value; _Step_Counter_Time_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Step_CounterEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Step_CounterEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyStep_Counter_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyStep_CounterAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Step_Counter_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Step_Counter_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyStep_Counter_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyStep_Counter_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyStep_CounterCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyStep_Counter: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyStep_Counter: set notification", result);

            return true;
        }

        private void NotifyStep_CounterCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U32|DEC|Steps U32|DEC|Time|ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Step_Counter_Steps = parseResult.ValueList.GetValue("Steps").AsDouble;
            Step_Counter_Time = parseResult.ValueList.GetValue("Time").AsDouble;

            Step_CounterEvent?.Invoke(parseResult);

        }

        public void NotifyStep_CounterRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Step_Counter_Motion_enum];
            if (ch == null) return;
            NotifyStep_Counter_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyStep_CounterCallback;
        }



        private double _Raw_Motion_AccelX = 0.0;
        private bool _Raw_Motion_AccelX_set = false;
        public double Raw_Motion_AccelX
        {
            get { return _Raw_Motion_AccelX; }
            internal set { if (_Raw_Motion_AccelX_set && value == _Raw_Motion_AccelX) return; _Raw_Motion_AccelX = value; _Raw_Motion_AccelX_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_AccelY = 0.0;
        private bool _Raw_Motion_AccelY_set = false;
        public double Raw_Motion_AccelY
        {
            get { return _Raw_Motion_AccelY; }
            internal set { if (_Raw_Motion_AccelY_set && value == _Raw_Motion_AccelY) return; _Raw_Motion_AccelY = value; _Raw_Motion_AccelY_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_AccelZ = 0.0;
        private bool _Raw_Motion_AccelZ_set = false;
        public double Raw_Motion_AccelZ
        {
            get { return _Raw_Motion_AccelZ; }
            internal set { if (_Raw_Motion_AccelZ_set && value == _Raw_Motion_AccelZ) return; _Raw_Motion_AccelZ = value; _Raw_Motion_AccelZ_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_GyroX = 0.0;
        private bool _Raw_Motion_GyroX_set = false;
        public double Raw_Motion_GyroX
        {
            get { return _Raw_Motion_GyroX; }
            internal set { if (_Raw_Motion_GyroX_set && value == _Raw_Motion_GyroX) return; _Raw_Motion_GyroX = value; _Raw_Motion_GyroX_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_GyroY = 0.0;
        private bool _Raw_Motion_GyroY_set = false;
        public double Raw_Motion_GyroY
        {
            get { return _Raw_Motion_GyroY; }
            internal set { if (_Raw_Motion_GyroY_set && value == _Raw_Motion_GyroY) return; _Raw_Motion_GyroY = value; _Raw_Motion_GyroY_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_GyroZ = 0.0;
        private bool _Raw_Motion_GyroZ_set = false;
        public double Raw_Motion_GyroZ
        {
            get { return _Raw_Motion_GyroZ; }
            internal set { if (_Raw_Motion_GyroZ_set && value == _Raw_Motion_GyroZ) return; _Raw_Motion_GyroZ = value; _Raw_Motion_GyroZ_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_CompassX = 0.0;
        private bool _Raw_Motion_CompassX_set = false;
        public double Raw_Motion_CompassX
        {
            get { return _Raw_Motion_CompassX; }
            internal set { if (_Raw_Motion_CompassX_set && value == _Raw_Motion_CompassX) return; _Raw_Motion_CompassX = value; _Raw_Motion_CompassX_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_CompassY = 0.0;
        private bool _Raw_Motion_CompassY_set = false;
        public double Raw_Motion_CompassY
        {
            get { return _Raw_Motion_CompassY; }
            internal set { if (_Raw_Motion_CompassY_set && value == _Raw_Motion_CompassY) return; _Raw_Motion_CompassY = value; _Raw_Motion_CompassY_set = true; OnPropertyChanged(); }
        }
        private double _Raw_Motion_CompassZ = 0.0;
        private bool _Raw_Motion_CompassZ_set = false;
        public double Raw_Motion_CompassZ
        {
            get { return _Raw_Motion_CompassZ; }
            internal set { if (_Raw_Motion_CompassZ_set && value == _Raw_Motion_CompassZ) return; _Raw_Motion_CompassZ = value; _Raw_Motion_CompassZ_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Raw_MotionEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Raw_MotionEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyRaw_Motion_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyRaw_MotionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Raw_Motion_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Raw_Motion_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyRaw_Motion_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyRaw_Motion_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyRaw_MotionCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyRaw_Motion: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyRaw_Motion: set notification", result);

            return true;
        }

        private void NotifyRaw_MotionCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "Q6Q10|HEX|AccelX|g Q6Q10|HEX|AccelY|g Q6Q10|HEX|AccelZ|g Q5Q11|HEX|GyroX|dps Q5Q11|HEX|GyroY|dps Q5Q11|HEX|GyroZ|dps Q12Q4|HEX|CompassX|microTesla Q12Q4|HEX|CompassY|microTesla Q12Q4|HEX|CompassZ|microTesla";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Raw_Motion_AccelX = parseResult.ValueList.GetValue("AccelX").AsDouble;
            Raw_Motion_AccelY = parseResult.ValueList.GetValue("AccelY").AsDouble;
            Raw_Motion_AccelZ = parseResult.ValueList.GetValue("AccelZ").AsDouble;
            Raw_Motion_GyroX = parseResult.ValueList.GetValue("GyroX").AsDouble;
            Raw_Motion_GyroY = parseResult.ValueList.GetValue("GyroY").AsDouble;
            Raw_Motion_GyroZ = parseResult.ValueList.GetValue("GyroZ").AsDouble;
            Raw_Motion_CompassX = parseResult.ValueList.GetValue("CompassX").AsDouble;
            Raw_Motion_CompassY = parseResult.ValueList.GetValue("CompassY").AsDouble;
            Raw_Motion_CompassZ = parseResult.ValueList.GetValue("CompassZ").AsDouble;

            Raw_MotionEvent?.Invoke(parseResult);

        }

        public void NotifyRaw_MotionRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Raw_Motion_Motion_enum];
            if (ch == null) return;
            NotifyRaw_Motion_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyRaw_MotionCallback;
        }



        private double _Euler_Roll = 0;
        private bool _Euler_Roll_set = false;
        public double Euler_Roll
        {
            get { return _Euler_Roll; }
            internal set { if (_Euler_Roll_set && value == _Euler_Roll) return; _Euler_Roll = value; _Euler_Roll_set = true; OnPropertyChanged(); }
        }
        private double _Euler_Pitch = 0;
        private bool _Euler_Pitch_set = false;
        public double Euler_Pitch
        {
            get { return _Euler_Pitch; }
            internal set { if (_Euler_Pitch_set && value == _Euler_Pitch) return; _Euler_Pitch = value; _Euler_Pitch_set = true; OnPropertyChanged(); }
        }
        private double _Euler_Yaw = 0;
        private bool _Euler_Yaw_set = false;
        public double Euler_Yaw
        {
            get { return _Euler_Yaw; }
            internal set { if (_Euler_Yaw_set && value == _Euler_Yaw) return; _Euler_Yaw = value; _Euler_Yaw_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; EulerEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent EulerEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyEuler_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyEulerAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Euler_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Euler_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyEuler_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyEuler_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyEulerCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyEuler: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyEuler: set notification", result);

            return true;
        }

        private void NotifyEulerCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I32|DEC|Roll|d I32|DEC|Pitch|d I32|DEC|Yaw|d";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Euler_Roll = parseResult.ValueList.GetValue("Roll").AsDouble;
            Euler_Pitch = parseResult.ValueList.GetValue("Pitch").AsDouble;
            Euler_Yaw = parseResult.ValueList.GetValue("Yaw").AsDouble;

            EulerEvent?.Invoke(parseResult);

        }

        public void NotifyEulerRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Euler_Motion_enum];
            if (ch == null) return;
            NotifyEuler_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyEulerCallback;
        }



        private double _RotationMatrix_param0 = 0;
        private bool _RotationMatrix_param0_set = false;
        public double RotationMatrix_param0
        {
            get { return _RotationMatrix_param0; }
            internal set { if (_RotationMatrix_param0_set && value == _RotationMatrix_param0) return; _RotationMatrix_param0 = value; _RotationMatrix_param0_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param1 = 0;
        private bool _RotationMatrix_param1_set = false;
        public double RotationMatrix_param1
        {
            get { return _RotationMatrix_param1; }
            internal set { if (_RotationMatrix_param1_set && value == _RotationMatrix_param1) return; _RotationMatrix_param1 = value; _RotationMatrix_param1_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param2 = 0;
        private bool _RotationMatrix_param2_set = false;
        public double RotationMatrix_param2
        {
            get { return _RotationMatrix_param2; }
            internal set { if (_RotationMatrix_param2_set && value == _RotationMatrix_param2) return; _RotationMatrix_param2 = value; _RotationMatrix_param2_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param3 = 0;
        private bool _RotationMatrix_param3_set = false;
        public double RotationMatrix_param3
        {
            get { return _RotationMatrix_param3; }
            internal set { if (_RotationMatrix_param3_set && value == _RotationMatrix_param3) return; _RotationMatrix_param3 = value; _RotationMatrix_param3_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param4 = 0;
        private bool _RotationMatrix_param4_set = false;
        public double RotationMatrix_param4
        {
            get { return _RotationMatrix_param4; }
            internal set { if (_RotationMatrix_param4_set && value == _RotationMatrix_param4) return; _RotationMatrix_param4 = value; _RotationMatrix_param4_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param5 = 0;
        private bool _RotationMatrix_param5_set = false;
        public double RotationMatrix_param5
        {
            get { return _RotationMatrix_param5; }
            internal set { if (_RotationMatrix_param5_set && value == _RotationMatrix_param5) return; _RotationMatrix_param5 = value; _RotationMatrix_param5_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param6 = 0;
        private bool _RotationMatrix_param6_set = false;
        public double RotationMatrix_param6
        {
            get { return _RotationMatrix_param6; }
            internal set { if (_RotationMatrix_param6_set && value == _RotationMatrix_param6) return; _RotationMatrix_param6 = value; _RotationMatrix_param6_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param7 = 0;
        private bool _RotationMatrix_param7_set = false;
        public double RotationMatrix_param7
        {
            get { return _RotationMatrix_param7; }
            internal set { if (_RotationMatrix_param7_set && value == _RotationMatrix_param7) return; _RotationMatrix_param7 = value; _RotationMatrix_param7_set = true; OnPropertyChanged(); }
        }
        private double _RotationMatrix_param8 = 0;
        private bool _RotationMatrix_param8_set = false;
        public double RotationMatrix_param8
        {
            get { return _RotationMatrix_param8; }
            internal set { if (_RotationMatrix_param8_set && value == _RotationMatrix_param8) return; _RotationMatrix_param8 = value; _RotationMatrix_param8_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; RotationMatrixEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent RotationMatrixEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyRotationMatrix_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyRotationMatrixAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.RotationMatrix_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.RotationMatrix_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyRotationMatrix_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyRotationMatrix_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyRotationMatrixCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyRotationMatrix: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyRotationMatrix: set notification", result);

            return true;
        }

        private void NotifyRotationMatrixCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I16 I16 I16 I16 I16 I16 I16 I16 I16";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            RotationMatrix_param0 = parseResult.ValueList.GetValue("param0").AsDouble;
            RotationMatrix_param1 = parseResult.ValueList.GetValue("param1").AsDouble;
            RotationMatrix_param2 = parseResult.ValueList.GetValue("param2").AsDouble;
            RotationMatrix_param3 = parseResult.ValueList.GetValue("param3").AsDouble;
            RotationMatrix_param4 = parseResult.ValueList.GetValue("param4").AsDouble;
            RotationMatrix_param5 = parseResult.ValueList.GetValue("param5").AsDouble;
            RotationMatrix_param6 = parseResult.ValueList.GetValue("param6").AsDouble;
            RotationMatrix_param7 = parseResult.ValueList.GetValue("param7").AsDouble;
            RotationMatrix_param8 = parseResult.ValueList.GetValue("param8").AsDouble;

            RotationMatrixEvent?.Invoke(parseResult);

        }

        public void NotifyRotationMatrixRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.RotationMatrix_Motion_enum];
            if (ch == null) return;
            NotifyRotationMatrix_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyRotationMatrixCallback;
        }



        private double _Compass_Heading = 0.0;
        private bool _Compass_Heading_set = false;
        public double Compass_Heading
        {
            get { return _Compass_Heading; }
            internal set { if (_Compass_Heading_set && value == _Compass_Heading) return; _Compass_Heading = value; _Compass_Heading_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Compass_HeadingEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Compass_HeadingEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyCompass_Heading_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyCompass_HeadingAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Compass_Heading_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Compass_Heading_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyCompass_Heading_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyCompass_Heading_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyCompass_HeadingCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyCompass_Heading: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyCompass_Heading: set notification", result);

            return true;
        }

        private void NotifyCompass_HeadingCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "Q16Q16|DEC|Heading|d";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Compass_Heading = parseResult.ValueList.GetValue("Heading").AsDouble;

            Compass_HeadingEvent?.Invoke(parseResult);

        }

        public void NotifyCompass_HeadingRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Compass_Heading_Motion_enum];
            if (ch == null) return;
            NotifyCompass_Heading_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyCompass_HeadingCallback;
        }



        private double _Gravity_X = 0.0;
        private bool _Gravity_X_set = false;
        public double Gravity_X
        {
            get { return _Gravity_X; }
            internal set { if (_Gravity_X_set && value == _Gravity_X) return; _Gravity_X = value; _Gravity_X_set = true; OnPropertyChanged(); }
        }
        private double _Gravity_Y = 0.0;
        private bool _Gravity_Y_set = false;
        public double Gravity_Y
        {
            get { return _Gravity_Y; }
            internal set { if (_Gravity_Y_set && value == _Gravity_Y) return; _Gravity_Y = value; _Gravity_Y_set = true; OnPropertyChanged(); }
        }
        private double _Gravity_Z = 0.0;
        private bool _Gravity_Z_set = false;
        public double Gravity_Z
        {
            get { return _Gravity_Z; }
            internal set { if (_Gravity_Z_set && value == _Gravity_Z) return; _Gravity_Z = value; _Gravity_Z_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; GravityEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent GravityEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyGravity_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyGravityAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Gravity_Motion_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Gravity_Motion_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyGravity_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyGravity_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyGravityCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyGravity: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyGravity: set notification", result);

            return true;
        }

        private void NotifyGravityCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "F32|FIXED|X|mpss F32|N3|Y|mpss F32|N3|Z|mpss";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Gravity_X = parseResult.ValueList.GetValue("X").AsDouble;
            Gravity_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Gravity_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            GravityEvent?.Invoke(parseResult);

        }

        public void NotifyGravityRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Gravity_Motion_enum];
            if (ch == null) return;
            NotifyGravity_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyGravityCallback;
        }



        private double _LED_Characteristics_param0 = 0;
        private bool _LED_Characteristics_param0_set = false;
        public double LED_Characteristics_param0
        {
            get { return _LED_Characteristics_param0; }
            internal set { if (_LED_Characteristics_param0_set && value == _LED_Characteristics_param0) return; _LED_Characteristics_param0 = value; _LED_Characteristics_param0_set = true; OnPropertyChanged(); }
        }
        private double _LED_Characteristics_param1 = 0;
        private bool _LED_Characteristics_param1_set = false;
        public double LED_Characteristics_param1
        {
            get { return _LED_Characteristics_param1; }
            internal set { if (_LED_Characteristics_param1_set && value == _LED_Characteristics_param1) return; _LED_Characteristics_param1 = value; _LED_Characteristics_param1_set = true; OnPropertyChanged(); }
        }
        private double _LED_Characteristics_param2 = 0;
        private bool _LED_Characteristics_param2_set = false;
        public double LED_Characteristics_param2
        {
            get { return _LED_Characteristics_param2; }
            internal set { if (_LED_Characteristics_param2_set && value == _LED_Characteristics_param2) return; _LED_Characteristics_param2 = value; _LED_Characteristics_param2_set = true; OnPropertyChanged(); }
        }
        private double _LED_Characteristics_param3 = 0;
        private bool _LED_Characteristics_param3_set = false;
        public double LED_Characteristics_param3
        {
            get { return _LED_Characteristics_param3; }
            internal set { if (_LED_Characteristics_param3_set && value == _LED_Characteristics_param3) return; _LED_Characteristics_param3 = value; _LED_Characteristics_param3_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLED_Characteristics(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.LED_Characteristics_UI_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.LED_Characteristics_UI_enum, "LED_Characteristics", cacheMode);
            if (result == null) return null;

            var datameaning = "U8 U8 U8 U8";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            LED_Characteristics_param0 = parseResult.ValueList.GetValue("param0").AsDouble;
            LED_Characteristics_param1 = parseResult.ValueList.GetValue("param1").AsDouble;
            LED_Characteristics_param2 = parseResult.ValueList.GetValue("param2").AsDouble;
            LED_Characteristics_param3 = parseResult.ValueList.GetValue("param3").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for LED_Characteristics
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLED_Characteristics(byte param0, byte param1, byte param2, byte param3)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.LED_Characteristics_UI_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  param0);
            dw.WriteByte(  param1);
            dw.WriteByte(  param2);
            dw.WriteByte(  param3);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.LED_Characteristics_UI_enum, "LED_Characteristics", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.LED_Characteristics_UI_enum, "LED_Characteristics", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Button = 0;
        private bool _Button_set = false;
        public double Button
        {
            get { return _Button; }
            internal set { if (_Button_set && value == _Button) return; _Button = value; _Button_set = true; OnPropertyChanged(); }
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
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Button_UI_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Button_UI_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyButton_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyButton_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyButtonCallback;
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

        private void NotifyButtonCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX|Press";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Button = parseResult.ValueList.GetValue("Press").AsDouble;

            ButtonEvent?.Invoke(parseResult);

        }

        public void NotifyButtonRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Button_UI_enum];
            if (ch == null) return;
            NotifyButton_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyButtonCallback;
        }



        private double _External_pin_param0 = 0;
        private bool _External_pin_param0_set = false;
        public double External_pin_param0
        {
            get { return _External_pin_param0; }
            internal set { if (_External_pin_param0_set && value == _External_pin_param0) return; _External_pin_param0 = value; _External_pin_param0_set = true; OnPropertyChanged(); }
        }
        private double _External_pin_param1 = 0;
        private bool _External_pin_param1_set = false;
        public double External_pin_param1
        {
            get { return _External_pin_param1; }
            internal set { if (_External_pin_param1_set && value == _External_pin_param1) return; _External_pin_param1 = value; _External_pin_param1_set = true; OnPropertyChanged(); }
        }
        private double _External_pin_param2 = 0;
        private bool _External_pin_param2_set = false;
        public double External_pin_param2
        {
            get { return _External_pin_param2; }
            internal set { if (_External_pin_param2_set && value == _External_pin_param2) return; _External_pin_param2 = value; _External_pin_param2_set = true; OnPropertyChanged(); }
        }
        private double _External_pin_param3 = 0;
        private bool _External_pin_param3_set = false;
        public double External_pin_param3
        {
            get { return _External_pin_param3; }
            internal set { if (_External_pin_param3_set && value == _External_pin_param3) return; _External_pin_param3 = value; _External_pin_param3_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadExternal_pin(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.External_pin_UI_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.External_pin_UI_enum, "External_pin", cacheMode);
            if (result == null) return null;

            var datameaning = "U8 U8 U8 U8";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            External_pin_param0 = parseResult.ValueList.GetValue("param0").AsDouble;
            External_pin_param1 = parseResult.ValueList.GetValue("param1").AsDouble;
            External_pin_param2 = parseResult.ValueList.GetValue("param2").AsDouble;
            External_pin_param3 = parseResult.ValueList.GetValue("param3").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for External_pin
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteExternal_pin(byte param0, byte param1, byte param2, byte param3)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.External_pin_UI_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  param0);
            dw.WriteByte(  param1);
            dw.WriteByte(  param2);
            dw.WriteByte(  param3);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.External_pin_UI_enum, "External_pin", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.External_pin_UI_enum, "External_pin", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Audio_Configuration_SpeakerMode = 0;
        private bool _Audio_Configuration_SpeakerMode_set = false;
        public double Audio_Configuration_SpeakerMode
        {
            get { return _Audio_Configuration_SpeakerMode; }
            internal set { if (_Audio_Configuration_SpeakerMode_set && value == _Audio_Configuration_SpeakerMode) return; _Audio_Configuration_SpeakerMode = value; _Audio_Configuration_SpeakerMode_set = true; OnPropertyChanged(); }
        }
        private double _Audio_Configuration_MicrophoneMode = 0;
        private bool _Audio_Configuration_MicrophoneMode_set = false;
        public double Audio_Configuration_MicrophoneMode
        {
            get { return _Audio_Configuration_MicrophoneMode; }
            internal set { if (_Audio_Configuration_MicrophoneMode_set && value == _Audio_Configuration_MicrophoneMode) return; _Audio_Configuration_MicrophoneMode = value; _Audio_Configuration_MicrophoneMode_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAudio_Configuration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Audio_Configuration_Audio_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Audio_Configuration_Audio_enum, "Audio_Configuration", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|SpeakerMode U8|HEX|MicrophoneMode";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Audio_Configuration_SpeakerMode = parseResult.ValueList.GetValue("SpeakerMode").AsDouble;
            Audio_Configuration_MicrophoneMode = parseResult.ValueList.GetValue("MicrophoneMode").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Audio_Configuration
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAudio_Configuration(byte SpeakerMode, byte MicrophoneMode)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Audio_Configuration_Audio_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  SpeakerMode);
            dw.WriteByte(  MicrophoneMode);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Audio_Configuration_Audio_enum, "Audio_Configuration", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Audio_Configuration_Audio_enum, "Audio_Configuration", subcommand, GattWriteOption.WriteWithResponse);
            }
        }





        /// <summary>
        /// Writes data for Speaker_Data
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteSpeaker_Data(byte[] Data)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Speaker_Data_Audio_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Data);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Speaker_Data_Audio_enum, "Speaker_Data", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Speaker_Data_Audio_enum, "Speaker_Data", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private double _Speaker_Status = 0;
        private bool _Speaker_Status_set = false;
        public double Speaker_Status
        {
            get { return _Speaker_Status; }
            internal set { if (_Speaker_Status_set && value == _Speaker_Status) return; _Speaker_Status = value; _Speaker_Status_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Speaker_StatusEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Speaker_StatusEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifySpeaker_Status_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifySpeaker_StatusAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Speaker_Status_Audio_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Speaker_Status_Audio_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifySpeaker_Status_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifySpeaker_Status_ValueChanged_Set = true;
                    ch.ValueChanged += NotifySpeaker_StatusCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifySpeaker_Status: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifySpeaker_Status: set notification", result);

            return true;
        }

        private void NotifySpeaker_StatusCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "U8|HEX|SpeakerStatus";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Speaker_Status = parseResult.ValueList.GetValue("SpeakerStatus").AsDouble;

            Speaker_StatusEvent?.Invoke(parseResult);

        }

        public void NotifySpeaker_StatusRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Speaker_Status_Audio_enum];
            if (ch == null) return;
            NotifySpeaker_Status_ValueChanged_Set = false;
            ch.ValueChanged -= NotifySpeaker_StatusCallback;
        }



        private string _Microphone_Data = null;
        private bool _Microphone_Data_set = false;
        public string Microphone_Data
        {
            get { return _Microphone_Data; }
            internal set { if (_Microphone_Data_set && value == _Microphone_Data) return; _Microphone_Data = value; _Microphone_Data_set = true; OnPropertyChanged(); }
        }


        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Microphone_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Microphone_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyMicrophone_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyMicrophone_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Microphone_Data_Audio_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Microphone_Data_Audio_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyMicrophone_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyMicrophone_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyMicrophone_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyMicrophone_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyMicrophone_Data: set notification", result);

            return true;
        }

        private void NotifyMicrophone_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|MicrophoneStatus";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Microphone_Data = parseResult.ValueList.GetValue("MicrophoneStatus").AsString;

            Microphone_DataEvent?.Invoke(parseResult);

        }

        public void NotifyMicrophone_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[(int)CharacteristicsEnum.Microphone_Data_Audio_enum];
            if (ch == null) return;
            NotifyMicrophone_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyMicrophone_DataCallback;
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



        private string _Configuration_Device_Name = "";
        private bool _Configuration_Device_Name_set = false;
        public string Configuration_Device_Name
        {
            get { return _Configuration_Device_Name; }
            internal set { if (_Configuration_Device_Name_set && value == _Configuration_Device_Name) return; _Configuration_Device_Name = value; _Configuration_Device_Name_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConfiguration_Device_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Configuration_Device_Name_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Configuration_Device_Name_Configuration_enum, "Configuration_Device_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Configuration_Device_Name = parseResult.ValueList.GetValue("Name").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Configuration_Device_Name
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteConfiguration_Device_Name(String Name)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Configuration_Device_Name_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(  Name);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Configuration_Device_Name_Configuration_enum, "Configuration_Device_Name", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Configuration_Device_Name_Configuration_enum, "Configuration_Device_Name", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Advertising_Parameter_Interval = 0;
        private bool _Advertising_Parameter_Interval_set = false;
        public double Advertising_Parameter_Interval
        {
            get { return _Advertising_Parameter_Interval; }
            internal set { if (_Advertising_Parameter_Interval_set && value == _Advertising_Parameter_Interval) return; _Advertising_Parameter_Interval = value; _Advertising_Parameter_Interval_set = true; OnPropertyChanged(); }
        }
        private double _Advertising_Parameter_Timeout = 0;
        private bool _Advertising_Parameter_Timeout_set = false;
        public double Advertising_Parameter_Timeout
        {
            get { return _Advertising_Parameter_Timeout; }
            internal set { if (_Advertising_Parameter_Timeout_set && value == _Advertising_Parameter_Timeout) return; _Advertising_Parameter_Timeout = value; _Advertising_Parameter_Timeout_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAdvertising_Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Advertising_Parameter_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Advertising_Parameter_Configuration_enum, "Advertising_Parameter", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|Interval|ms U8|DEC|Timeout|s";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Advertising_Parameter_Interval = parseResult.ValueList.GetValue("Interval").AsDouble;
            Advertising_Parameter_Timeout = parseResult.ValueList.GetValue("Timeout").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Advertising_Parameter
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAdvertising_Parameter(UInt16 Interval, byte Timeout)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Advertising_Parameter_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(  Interval);
            dw.WriteByte(  Timeout);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Advertising_Parameter_Configuration_enum, "Advertising_Parameter", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Advertising_Parameter_Configuration_enum, "Advertising_Parameter", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Connection_parameters_MinInterval = 0;
        private bool _Connection_parameters_MinInterval_set = false;
        public double Connection_parameters_MinInterval
        {
            get { return _Connection_parameters_MinInterval; }
            internal set { if (_Connection_parameters_MinInterval_set && value == _Connection_parameters_MinInterval) return; _Connection_parameters_MinInterval = value; _Connection_parameters_MinInterval_set = true; OnPropertyChanged(); }
        }
        private double _Connection_parameters_MaxInterval = 0;
        private bool _Connection_parameters_MaxInterval_set = false;
        public double Connection_parameters_MaxInterval
        {
            get { return _Connection_parameters_MaxInterval; }
            internal set { if (_Connection_parameters_MaxInterval_set && value == _Connection_parameters_MaxInterval) return; _Connection_parameters_MaxInterval = value; _Connection_parameters_MaxInterval_set = true; OnPropertyChanged(); }
        }
        private double _Connection_parameters_Latency = 0;
        private bool _Connection_parameters_Latency_set = false;
        public double Connection_parameters_Latency
        {
            get { return _Connection_parameters_Latency; }
            internal set { if (_Connection_parameters_Latency_set && value == _Connection_parameters_Latency) return; _Connection_parameters_Latency = value; _Connection_parameters_Latency_set = true; OnPropertyChanged(); }
        }
        private double _Connection_parameters_SupervisionTimeout = 0;
        private bool _Connection_parameters_SupervisionTimeout_set = false;
        public double Connection_parameters_SupervisionTimeout
        {
            get { return _Connection_parameters_SupervisionTimeout; }
            internal set { if (_Connection_parameters_SupervisionTimeout_set && value == _Connection_parameters_SupervisionTimeout) return; _Connection_parameters_SupervisionTimeout = value; _Connection_parameters_SupervisionTimeout_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConnection_parameters(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Connection_parameters_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Connection_parameters_Configuration_enum, "Connection_parameters", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|DEC|MinInterval U16|DEC|MaxInterval U16|DEC|Latency U16|DEC|SupervisionTimeout|10ms";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Connection_parameters_MinInterval = parseResult.ValueList.GetValue("MinInterval").AsDouble;
            Connection_parameters_MaxInterval = parseResult.ValueList.GetValue("MaxInterval").AsDouble;
            Connection_parameters_Latency = parseResult.ValueList.GetValue("Latency").AsDouble;
            Connection_parameters_SupervisionTimeout = parseResult.ValueList.GetValue("SupervisionTimeout").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Connection_parameters
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteConnection_parameters(UInt16 MinInterval, UInt16 MaxInterval, UInt16 Latency, UInt16 SupervisionTimeout)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Connection_parameters_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(  MinInterval);
            dw.WriteUInt16(  MaxInterval);
            dw.WriteUInt16(  Latency);
            dw.WriteUInt16(  SupervisionTimeout);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Connection_parameters_Configuration_enum, "Connection_parameters", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Connection_parameters_Configuration_enum, "Connection_parameters", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _Eddystone_URL = "";
        private bool _Eddystone_URL_set = false;
        public string Eddystone_URL
        {
            get { return _Eddystone_URL; }
            internal set { if (_Eddystone_URL_set && value == _Eddystone_URL) return; _Eddystone_URL = value; _Eddystone_URL_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadEddystone_URL(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Eddystone_URL_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Eddystone_URL_Configuration_enum, "Eddystone_URL", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|Eddystone|Eddystone";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Eddystone_URL = parseResult.ValueList.GetValue("Eddystone").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Eddystone_URL
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteEddystone_URL(String Eddystone)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Eddystone_URL_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(  Eddystone);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Eddystone_URL_Configuration_enum, "Eddystone_URL", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Eddystone_URL_Configuration_enum, "Eddystone_URL", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _Cloud_Token = null;
        private bool _Cloud_Token_set = false;
        public string Cloud_Token
        {
            get { return _Cloud_Token; }
            internal set { if (_Cloud_Token_set && value == _Cloud_Token) return; _Cloud_Token = value; _Cloud_Token_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadCloud_Token(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Cloud_Token_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Cloud_Token_Configuration_enum, "Cloud_Token", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|CloudToken";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Cloud_Token = parseResult.ValueList.GetValue("CloudToken").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Cloud_Token
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteCloud_Token(byte[] CloudToken)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Cloud_Token_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  CloudToken);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Cloud_Token_Configuration_enum, "Cloud_Token", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Cloud_Token_Configuration_enum, "Cloud_Token", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Firmware_Version_Major = 0;
        private bool _Firmware_Version_Major_set = false;
        public double Firmware_Version_Major
        {
            get { return _Firmware_Version_Major; }
            internal set { if (_Firmware_Version_Major_set && value == _Firmware_Version_Major) return; _Firmware_Version_Major = value; _Firmware_Version_Major_set = true; OnPropertyChanged(); }
        }
        private double _Firmware_Version_Minor = 0;
        private bool _Firmware_Version_Minor_set = false;
        public double Firmware_Version_Minor
        {
            get { return _Firmware_Version_Minor; }
            internal set { if (_Firmware_Version_Minor_set && value == _Firmware_Version_Minor) return; _Firmware_Version_Minor = value; _Firmware_Version_Minor_set = true; OnPropertyChanged(); }
        }
        private double _Firmware_Version_Patch = 0;
        private bool _Firmware_Version_Patch_set = false;
        public double Firmware_Version_Patch
        {
            get { return _Firmware_Version_Patch; }
            internal set { if (_Firmware_Version_Patch_set && value == _Firmware_Version_Patch) return; _Firmware_Version_Patch = value; _Firmware_Version_Patch_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadFirmware_Version(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Firmware_Version_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Firmware_Version_Configuration_enum, "Firmware_Version", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Major U8|DEC|Minor U8|DEC|Patch";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Firmware_Version_Major = parseResult.ValueList.GetValue("Major").AsDouble;
            Firmware_Version_Minor = parseResult.ValueList.GetValue("Minor").AsDouble;
            Firmware_Version_Patch = parseResult.ValueList.GetValue("Patch").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _MTU_Request_param0 = 0;
        private bool _MTU_Request_param0_set = false;
        public double MTU_Request_param0
        {
            get { return _MTU_Request_param0; }
            internal set { if (_MTU_Request_param0_set && value == _MTU_Request_param0) return; _MTU_Request_param0 = value; _MTU_Request_param0_set = true; OnPropertyChanged(); }
        }
        private double _MTU_Request_param1 = 0;
        private bool _MTU_Request_param1_set = false;
        public double MTU_Request_param1
        {
            get { return _MTU_Request_param1; }
            internal set { if (_MTU_Request_param1_set && value == _MTU_Request_param1) return; _MTU_Request_param1 = value; _MTU_Request_param1_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadMTU_Request(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MTU_Request_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.MTU_Request_Configuration_enum, "MTU_Request", cacheMode);
            if (result == null) return null;

            var datameaning = "U8 U16";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            MTU_Request_param0 = parseResult.ValueList.GetValue("param0").AsDouble;
            MTU_Request_param1 = parseResult.ValueList.GetValue("param1").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for MTU_Request
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteMTU_Request(byte param0, UInt16 param1)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.MTU_Request_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  param0);
            dw.WriteUInt16(  param1);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.MTU_Request_Configuration_enum, "MTU_Request", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.MTU_Request_Configuration_enum, "MTU_Request", subcommand, GattWriteOption.WriteWithResponse);
            }
        }








    }
}