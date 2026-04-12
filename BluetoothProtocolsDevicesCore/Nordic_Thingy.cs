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

        enum ServiceIndex
        {
            Battery_index = 0,
            Environment_index = 1,
        }
        enum CharacteristicIndex
        {
            Battery_BatteryLevel_index = 0,
            Environment_Temperature_c_index = 1,
            Environment_Pressure_hpa_index = 2,
            Environment_Humidity_index = 3,
            Environment_TVOC_index = 4,
            Environment_Color_index = 5,
        }

        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // Battery
            Guid.Parse("EF680200-9B35-4933-9B10-52FFA9740042"), // Environment
        };

        List<GattDeviceService> Services = new List<GattDeviceService>(2) { null, null, };

        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // Battery_BatteryLevel is "I8|DEC|BatteryLevel|%"
            Guid.Parse("EF680201-9B35-4933-9B10-52FFA9740042"), // Environment_Temperature_c
            Guid.Parse("EF680202-9B35-4933-9B10-52FFA9740042"), // Environment_Pressure_hpa
            Guid.Parse("EF680203-9B35-4933-9B10-52FFA9740042"), // Environment_Humidity
            Guid.Parse("EF680204-9B35-4933-9B10-52FFA9740042"), // Environment_TVOC
            Guid.Parse("EF680205-9B35-4933-9B10-52FFA9740042"), // Environment_Color
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, };

        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() { null, null, null, null, null, null, };

        /// <summary>
        /// Delegate for all Notify events
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

        #region Service_Battery
        /// <summary>
        /// Data from all of the characteristics in the Battery Service; currently just BatteryLevel
        /// </summary>
        public class Battery_Data
        {
            public double BatteryLevel { get; set; }
        }
        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%"); // TODO: should be done ahead of time!
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged("BatteryLevel");
        }


        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadBatteryLevel(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_BatteryLevel_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "Battery_Level");
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
            OnPropertyChanged("BatteryLevel"); // TODO: really do the property changed?
            return CurrBattery_Data;
        }
        #endregion

        #region Service_Environment
        /// <summary>
        /// Data from all of the characteristics in the Environment Service
        /// </summary>
        public class Environment_Data
        {
            public double Temperature_c { get; set; }
            public double Pressure_hpa { get; set; }
            public double Humidity { get; set; }
            public double TVOC { get; set; }
            public double eCOS { get; set; }
            public double Red { get; set; }
            public double Green { get; set; }
            public double Blue { get; set; }
            public double Clear { get; set; }

        }
        public Environment_Data CurrEnvironment_Data { get; set; } = new Environment_Data();


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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("/I8/P8|FIXED|Temperature|C"); // TODO: should be done ahead of time!
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.Temperature_c = vr.GetNextDouble();
            OnPropertyChanged("Temperature_c");
        }













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
            CurrEnvironment_Data.Pressure_hpa = vr.GetNextDouble();
            OnPropertyChanged("Pressure_hpa");
        }



        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidityAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (ble == null) return false; // too early to calll this 
            var retval = await SetupNotifyAsync("Humidity", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Humidity_index, NotifyHumidityCallback, notifyType);
            return retval;
        }

        private void NotifyHumidityCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Humidity_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Humidity|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged("Humidity");
        }


        // TVOC is eCOS + TVOC+ 


        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTVOCAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("TVOC", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_TVOC_index, NotifyTVOCCallback, notifyType);
            return retval;
        }

        private void NotifyTVOCCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_TVOC_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|eCOS|ppm U16|DEC|TVOC|ppb");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.eCOS = vr.GetNextDouble();
            CurrEnvironment_Data.TVOC = vr.GetNextDouble();
            OnPropertyChanged("TVOC"); // name of the characteristic, not the individual fields.
        }





        // Color is RGB+Clear



        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyColorAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Color", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Color_index, NotifyColorCallback, notifyType);
            return retval;
        }

        private void NotifyColorCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Color_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|Red U16|DEC|Green U16|DEC|Blue U16|DEC|Clear");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.Red = vr.GetNextDouble();
            CurrEnvironment_Data.Green = vr.GetNextDouble();
            CurrEnvironment_Data.Blue = vr.GetNextDouble();
            CurrEnvironment_Data.Clear = vr.GetNextDouble();
            OnPropertyChanged("Color"); // name of the characteristic, not the individual fields.
        }



        #endregion
    }
}