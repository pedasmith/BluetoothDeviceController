using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothProtocols
{
    /// <summary>
    /// The TI 1350 and 2650 are the latest in the TI range of Sensor. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth]].
    /// This class was automatically generated 8/13/2019 9:54 PM
    /// </summary>

    public partial class TI_SensorTag_1350 : INotifyPropertyChanged
    {

        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        public async Task WriteIR_Service_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteIR_Service_Config(1);
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteIR_Service_Config(0);
                    break;
            }
        }

        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        public async Task WriteHumidity_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteHumidity_Config(1);
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteHumidity_Config(0);
                    break;
            }
        }

        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        /// 
        public async Task WriteAccelerometer_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteAccelerometer_Config(0xFFFF); // always just write the same value. 
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteAccelerometer_Config(0);
                    break;
            }
        }
        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        public async Task WriteBarometer_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteBarometer_Config(1);
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteBarometer_Config(0);
                    break;
            }
        }

        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        public async Task WriteOptical_Service_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteOptical_Service_Config(1);
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteOptical_Service_Config(0);
                    break;
            }
        }
    }
}