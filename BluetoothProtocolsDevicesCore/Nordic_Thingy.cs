//From template: Protocol_Core_Body v2026-04-17 11:43
//From template: Protocol_Body v2022-07-02 9:54
using System;
using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Needed for CallerMemberNameAttribute
using System.Runtime.InteropServices.WindowsRuntime; // Needed for IBuffer.ToArray extension method
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    /// <summary>
    /// The Nordic Thingy:52™ is an easy-to-use prototyping platform, designed to help in building prototypes and demos, without the need to build hardware or even write firmware. It is built around the nRF52832 Bluetooth 5 SoC.
    /// This class was automatically generated 2026-04-25::17:11
    /// </summary>

    public  class Nordic_Thingy : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://nordicsemiconductor.github.io/Nordic-Thingy52-FW/documentation/firmware_architecture.html#fw_arch_ble_services

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /* Service and Characteristics summary for the device Thingy

        Environment service Guid=ef680200-9b35-4933-9b10-52ffa9740042
            Environment_Data (DataGroup record)
                Temperature (c) characteristic has Temperature (double-->double)  Guid=ef680201-9b35-4933-9b10-52ffa9740042
                Pressure (hpa) characteristic has Pressure (double-->double)  Guid=ef680202-9b35-4933-9b10-52ffa9740042
                Humidity (%) characteristic has Humidity (Byte-->double)  Guid=ef680203-9b35-4933-9b10-52ffa9740042
                Air Quality eCOS TVOC characteristic has eCOS (UInt16-->double) TVOC (UInt16-->double)  Guid=ef680204-9b35-4933-9b10-52ffa9740042

            EnvironmentColor_Data (DataGroup record)
                Color RGB+Clear characteristic has Red (UInt16-->double) Green (UInt16-->double) Blue (UInt16-->double) Clear (UInt16-->double)  Guid=ef680205-9b35-4933-9b10-52ffa9740042

            EnvironmentConfiguration_Data (DataGroup record)
                Environment Configuration characteristic has TempInterval (UInt16-->double) PressureInterval (UInt16-->double) HumidityInterval (UInt16-->double) ColorInterval (UInt16-->double) GasMode (Byte-->double) RedCalibration (Byte-->double) GreenCalibration (Byte-->double) BlueCalibration (Byte-->double)  Guid=ef680206-9b35-4933-9b10-52ffa9740042


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                BatteryLevel characteristic has BatteryLevel (SByte-->double)  Guid=2a19
        */

        public const string Temperature_cPropertyChangedName = "Temperature_c";
        public const string Pressure_hpaPropertyChangedName = "Pressure_hpa";
        public const string HumidityPropertyChangedName = "Humidity";
        public const string Air_Quality_eCOS_TVOCPropertyChangedName = "Air_Quality_eCOS_TVOC";
        public const string Color_RGB_ClearPropertyChangedName = "Color_RGB_Clear";
        public const string Environment_ConfigurationPropertyChangedName = "Environment_Configuration";
        public const string BatteryLevelPropertyChangedName = "BatteryLevel";


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Environment_index = 0,
            Battery_index = 1,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Environment_Temperature_c_index = 0,     // GUID EF680201-9B35-4933-9B10-52FFA9740042
            Environment_Pressure_hpa_index = 1,     // GUID EF680202-9B35-4933-9B10-52FFA9740042
            Environment_Humidity_index = 2,     // GUID EF680203-9B35-4933-9B10-52FFA9740042
            Environment_Air_Quality_eCOS_TVOC_index = 3,     // GUID EF680204-9B35-4933-9B10-52FFA9740042
            Environment_Color_RGB_Clear_index = 4,     // GUID EF680205-9B35-4933-9B10-52FFA9740042
            Environment_Environment_Configuration_index = 5,     // GUID EF680206-9B35-4933-9B10-52FFA9740042
            Battery_BatteryLevel_index = 6,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("EF680200-9B35-4933-9B10-52FFA9740042"), // #0 is Environment
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // #1 is Battery
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("EF680201-9B35-4933-9B10-52FFA9740042"), // #0 is Environment Temperature (c)
            Guid.Parse("EF680202-9B35-4933-9B10-52FFA9740042"), // #1 is Environment Pressure (hpa)
            Guid.Parse("EF680203-9B35-4933-9B10-52FFA9740042"), // #2 is Environment Humidity (%)
            Guid.Parse("EF680204-9B35-4933-9B10-52FFA9740042"), // #3 is Environment Air Quality eCOS TVOC
            Guid.Parse("EF680205-9B35-4933-9B10-52FFA9740042"), // #4 is Environment Color RGB+Clear
            Guid.Parse("EF680206-9B35-4933-9B10-52FFA9740042"), // #5 is Environment Environment Configuration
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #6 is Battery BatteryLevel
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null,  };


        /// <summary>
        /// Delegate for all Notify events. this is specific to this device (the indexes are all for this device only)
        /// but otherwise is generic.
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(IotNumberFormats.ValueParserResult data);

        private async Task<bool> Ensure_Characteristic_Async(ServiceIndex serviceIndex, string serviceName, CharacteristicIndex characteristicIndex, string characteristicName)
        {
            if (Characteristics[(int)characteristicIndex] == null)
            {
                if (ble == null) return false;
                if (Services[(int)serviceIndex] == null)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(Service_Guids[(int)serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {serviceName}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {serviceName}", serviceStatus);
                        return false;
                    }
                    Services[(int)serviceIndex] = serviceStatus.Services[0];
                }
                var service = Services[(int)serviceIndex];
                var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(Characteristic_Guids[(int)characteristicIndex]);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"unable to get characteristic for {characteristicName}", characteristicsStatus);
                    return false;
                }
                if (characteristicsStatus.Characteristics.Count == 0)
                {
                    Status.ReportStatus($"unable to get any characteristics for {characteristicName}", characteristicsStatus);
                    return false;
                }
                else if (characteristicsStatus.Characteristics.Count != 1)
                {
                    Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {characteristicName}", characteristicsStatus);
                    return false;
                }
                Characteristics[(int)characteristicIndex] = characteristicsStatus.Characteristics[0];
            }
            return true;
        }


        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(GattCharacteristic ch, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await ch.ReadValueAsync(cacheMode);
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


        private async Task<bool> SetupNotifyAsync(string name, 
            ServiceIndex serviceIndex, string serviceName, CharacteristicIndex index, 
            Windows.Foundation.TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> callback,
            GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            await Ensure_Characteristic_Async(serviceIndex, serviceName, index, name);
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return false;
            }
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyCharacteristic_ValueChanged_set[(int)index])
                {
                    // Only set the event callback once
                    NotifyCharacteristic_ValueChanged_set[(int)index] = true;
                    ch.ValueChanged += callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"Notify{name}: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"Notify{name}: set notification", result);

            return true;
        }

        //
        //
        // Start of the service + characteristic
        //
        //


        //
        // All services / characteristics and data structures
        //


        #region Service_Environment
                // Service Environment 
        /// <summary>
        /// Data from all of the characteristics in the Environment Service
        /// </summary>
        public class Environment_Data
        {
            public DateTimeOffset TimestampMostRecent {get; set; } = DateTimeOffset.MinValue;
            public DateTime TimestampMostRecentDT {get { return TimestampMostRecent.DateTime; }  }
            public double Temperature { get; set; } // From Environment and Temperature (c)
            public double Pressure { get; set; } // From Environment and Pressure (hpa)
            public double Humidity { get; set; } // From Environment and Humidity (%)
            public double eCOS { get; set; } // From Environment and Air Quality eCOS TVOC
            public double TVOC { get; set; } // From Environment and Air Quality eCOS TVOC
        }
        public Environment_Data CurrEnvironment_Data { get; set; } = new Environment_Data();

        // Per-characteristics methods for Environment Temperature_c
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTemperature_cAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Temperature_c", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Temperature_c_index, NotifyTemperature_cCallback, notifyType);
            return retval;
        }

        private void NotifyTemperature_cCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Temperature_c_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("/I8/P8|FIXED|Temperature|C");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.Temperature = vr.GetNextDouble();
            OnPropertyChanged(Temperature_cPropertyChangedName); // "Temperature_c"
        }
        // Per-characteristics methods for Environment Pressure_hpa
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPressure_hpaAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Pressure_hpa", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Pressure_hpa_index, NotifyPressure_hpaCallback, notifyType);
            return retval;
        }

        private void NotifyPressure_hpaCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Pressure_hpa_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("/I32/P8|FIXED|Pressure|hPA");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.Pressure = vr.GetNextDouble();
            OnPropertyChanged(Pressure_hpaPropertyChangedName); // "Pressure_hpa"
        }
        // Per-characteristics methods for Environment Humidity
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidityAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Humidity", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Humidity_index, NotifyHumidityCallback, notifyType);
            return retval;
        }

        private void NotifyHumidityCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Humidity_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Humidity|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged(HumidityPropertyChangedName); // "Humidity"
        }
        // Per-characteristics methods for Environment Air_Quality_eCOS_TVOC
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAir_Quality_eCOS_TVOCAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Air_Quality_eCOS_TVOC", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Air_Quality_eCOS_TVOC_index, NotifyAir_Quality_eCOS_TVOCCallback, notifyType);
            return retval;
        }

        private void NotifyAir_Quality_eCOS_TVOCCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Air_Quality_eCOS_TVOC_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|eCOS|ppm U16|DEC|TVOC|ppb");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.eCOS = vr.GetNextDouble();
            CurrEnvironment_Data.TVOC = vr.GetNextDouble();
            OnPropertyChanged(Air_Quality_eCOS_TVOCPropertyChangedName); // "Air_Quality_eCOS_TVOC"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadTemperature_c(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Temperature_c_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Temperature (c)");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Temperature (c)", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("/I8/P8|FIXED|Temperature|C");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.Temperature = vr.GetNextDouble();
            OnPropertyChanged(Temperature_cPropertyChangedName); // "Temperature_c"
            return CurrEnvironment_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadPressure_hpa(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Pressure_hpa_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Pressure (hpa)");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Pressure (hpa)", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("/I32/P8|FIXED|Pressure|hPA");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.Pressure = vr.GetNextDouble();
            OnPropertyChanged(Pressure_hpaPropertyChangedName); // "Pressure_hpa"
            return CurrEnvironment_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadHumidity(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Humidity_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Humidity (%)");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Humidity (%)", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Humidity|%");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged(HumidityPropertyChangedName); // "Humidity"
            return CurrEnvironment_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadAir_Quality_eCOS_TVOC(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Air_Quality_eCOS_TVOC_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Air Quality eCOS TVOC");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Air Quality eCOS TVOC", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|eCOS|ppm U16|DEC|TVOC|ppb");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.eCOS = vr.GetNextDouble();
            CurrEnvironment_Data.TVOC = vr.GetNextDouble();
            OnPropertyChanged(Air_Quality_eCOS_TVOCPropertyChangedName); // "Air_Quality_eCOS_TVOC"
            return CurrEnvironment_Data;
        }
        // Service Environment 
        /// <summary>
        /// Data from all of the characteristics in the Environment Service
        /// </summary>
        public class EnvironmentColor_Data
        {
            public DateTimeOffset TimestampMostRecent {get; set; } = DateTimeOffset.MinValue;
            public DateTime TimestampMostRecentDT {get { return TimestampMostRecent.DateTime; }  }
            public double Red { get; set; } // From Environment and Color RGB+Clear
            public double Green { get; set; } // From Environment and Color RGB+Clear
            public double Blue { get; set; } // From Environment and Color RGB+Clear
            public double Clear { get; set; } // From Environment and Color RGB+Clear
        }
        public EnvironmentColor_Data CurrEnvironmentColor_Data { get; set; } = new EnvironmentColor_Data();

        // Per-characteristics methods for Environment Color_RGB_Clear
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyColor_RGB_ClearAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Color_RGB_Clear", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Color_RGB_Clear_index, NotifyColor_RGB_ClearCallback, notifyType);
            return retval;
        }

        private void NotifyColor_RGB_ClearCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Color_RGB_Clear_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|Red U16|DEC|Green U16|DEC|Blue U16|DEC|Clear");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironmentColor_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironmentColor_Data.Red = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Green = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Blue = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Clear = vr.GetNextDouble();
            OnPropertyChanged(Color_RGB_ClearPropertyChangedName); // "Color_RGB_Clear"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>EnvironmentColor_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<EnvironmentColor_Data> ReadColor_RGB_Clear(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Color_RGB_Clear_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Color RGB+Clear");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Color RGB+Clear", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|Red U16|DEC|Green U16|DEC|Blue U16|DEC|Clear");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironmentColor_Data.Red = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Green = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Blue = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Clear = vr.GetNextDouble();
            OnPropertyChanged(Color_RGB_ClearPropertyChangedName); // "Color_RGB_Clear"
            return CurrEnvironmentColor_Data;
        }
        // Service Environment 
        /// <summary>
        /// Data from all of the characteristics in the Environment Service
        /// </summary>
        public class EnvironmentConfiguration_Data
        {
            public DateTimeOffset TimestampMostRecent {get; set; } = DateTimeOffset.MinValue;
            public DateTime TimestampMostRecentDT {get { return TimestampMostRecent.DateTime; }  }
            public double TempInterval { get; set; } // From Environment and Environment Configuration
            public double PressureInterval { get; set; } // From Environment and Environment Configuration
            public double HumidityInterval { get; set; } // From Environment and Environment Configuration
            public double ColorInterval { get; set; } // From Environment and Environment Configuration
            public double GasMode { get; set; } // From Environment and Environment Configuration
            public double RedCalibration { get; set; } // From Environment and Environment Configuration
            public double GreenCalibration { get; set; } // From Environment and Environment Configuration
            public double BlueCalibration { get; set; } // From Environment and Environment Configuration
        }
        public EnvironmentConfiguration_Data CurrEnvironmentConfiguration_Data { get; set; } = new EnvironmentConfiguration_Data();

        // Per-characteristics methods for Environment Environment_Configuration
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyEnvironment_ConfigurationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Environment_Configuration", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Environment_Configuration_index, NotifyEnvironment_ConfigurationCallback, notifyType);
            return retval;
        }

        private void NotifyEnvironment_ConfigurationCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Environment_Configuration_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|TempInterval|ms U16|DEC|PressureInterval|ms U16|DEC|HumidityInterval|ms U16|DEC|ColorInterval|ms U8|DEC|GasMode U8|DEC|RedCalibration U8|DEC|GreenCalibration U8|DEC|BlueCalibration");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironmentConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironmentConfiguration_Data.TempInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.PressureInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.HumidityInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.ColorInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GasMode = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.RedCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GreenCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.BlueCalibration = vr.GetNextDouble();
            OnPropertyChanged(Environment_ConfigurationPropertyChangedName); // "Environment_Configuration"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>EnvironmentConfiguration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<EnvironmentConfiguration_Data> ReadEnvironment_Configuration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Environment_Configuration_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Environment Configuration");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Environment Configuration", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|TempInterval|ms U16|DEC|PressureInterval|ms U16|DEC|HumidityInterval|ms U16|DEC|ColorInterval|ms U8|DEC|GasMode U8|DEC|RedCalibration U8|DEC|GreenCalibration U8|DEC|BlueCalibration");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironmentConfiguration_Data.TempInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.PressureInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.HumidityInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.ColorInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GasMode = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.RedCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GreenCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.BlueCalibration = vr.GetNextDouble();
            OnPropertyChanged(Environment_ConfigurationPropertyChangedName); // "Environment_Configuration"
            return CurrEnvironmentConfiguration_Data;
        }

        #endregion
//
        #region Service_Battery
                // Service Battery 
        /// <summary>
        /// Data from all of the characteristics in the Battery Service
        /// </summary>
        public class Battery_Data
        {
            public DateTimeOffset TimestampMostRecent {get; set; } = DateTimeOffset.MinValue;
            public DateTime TimestampMostRecentDT {get { return TimestampMostRecent.DateTime; }  }
            public double BatteryLevel { get; set; } // From Battery and BatteryLevel
        }
        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

        // Per-characteristics methods for Battery BatteryLevel
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBatteryLevelAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("BatteryLevel", ServiceIndex.Battery_index, "Battery", CharacteristicIndex.Battery_BatteryLevel_index, NotifyBatteryLevelCallback, notifyType);
            return retval;
        }

        private void NotifyBatteryLevelCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Battery_BatteryLevel_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.TimestampMostRecent = args.Timestamp;
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Battery_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadBatteryLevel(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_BatteryLevel_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "BatteryLevel");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "BatteryLevel", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
            return CurrBattery_Data;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}