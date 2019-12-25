using BluetoothDeviceController.BleEditor;
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
    public partial class TI_SensorTag_2541 : INotifyPropertyChanged
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
                    await WriteIR_Service_Configure(1);
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteIR_Service_Configure(0);
                    break;
            }
        }

        private double _IR_ObjTemp = 0;
        private bool _IR_ObjTemp_set = false;
        public double IR_ObjTemp
        {
            get { return _IR_ObjTemp; }
            internal set { if (_IR_ObjTemp_set && value == _IR_ObjTemp) return; _IR_ObjTemp = value; _IR_ObjTemp_set = true; OnPropertyChanged(); }
        }

        private double _IR_AmbientTemp = 0;
        private bool _IR_AmbientTemp_set = false;
        public double IR_AmbientTemp
        {
            get { return _IR_AmbientTemp; }
            internal set { if (_IR_AmbientTemp_set && value == _IR_AmbientTemp) return; _IR_AmbientTemp = value; _IR_AmbientTemp_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; IREvent += _my function_
        /// </summary>
        public event BluetoothDataEvent IREvent = null;

        /// <summary>
        /// Converts the raw IR data into directly usable data. Will set the values, trigger an event, and will
        /// also return a ValueParserResult which can be directly used.
        /// </summary>
        /// <returns></returns>
        public BCBasic.BCValueList ConvertIR()
        {
            // Standard conversion function swiped from the Best Calculator values (and those in
            // turn are from the Best TI SensorTag program and those in turn are from the Java code
            // that TI provides.
            double ambTempC = (double)(IR_Data_AmbientTemp / 128.0); // in degrees C?
            double Tdie = ambTempC + 273.15;

            double Vobj2 = (double)(IR_Data_ObjTemp); // Pretty magical value
            Vobj2 *= 0.00000015625;

            double S0 = 5.593E-14;  // Calibration factor
            double a1 = 1.75E-3;
            double a2 = -1.678E-5;
            double b0 = -2.94E-5;
            double b1 = -5.7E-7;
            double b2 = 4.63E-9;
            double c2 = 13.4;
            double Tref = 298.15;
            double S = S0 * (1 + a1 * (Tdie - Tref) + a2 * Math.Pow((Tdie - Tref), 2));
            double Vos = b0 + b1 * (Tdie - Tref) + b2 * Math.Pow((Tdie - Tref), 2);
            double fObj = (Vobj2 - Vos) + c2 * Math.Pow((Vobj2 - Vos), 2);
            double tObj = Math.Pow(Math.Pow(Tdie, 4) + (fObj / S), .25);

            double objTemp = tObj - 273.15;

            // Now set those values
            IR_ObjTemp = objTemp;
            IR_AmbientTemp = ambTempC;
            var parseResult = new BCBasic.BCValueList();
            parseResult.AddPropertyAllowDuplicates("ObjTemp", new BCBasic.BCValue(IR_ObjTemp));
            parseResult.AddPropertyAllowDuplicates("AmbientTemp", new BCBasic.BCValue(IR_AmbientTemp));
            IREvent?.Invoke(new ValueParserResult() { Result = ValueParserResult.ResultValues.Ok, ValueList = parseResult });

            return parseResult;
        }

        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        public async Task WriteAccelerometer_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteAccelerometer_Configure(1); // always just write the same value. 
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteAccelerometer_Configure(0);
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
                    await WriteHumidity_Configure(1);
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteHumidity_Configure(0);
                    break;
            }
        }

        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        public async Task WriteMagnetometer_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteMagnetometer_Configure(1); // always just write the same value. 
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteMagnetometer_Configure(0);
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
                    await WriteBarometer_Configure(1);
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteBarometer_Configure(0);
                    break;
            }
        }

        /// <summary>
        /// Convenience class for writing the config value.
        /// </summary>
        /// <param name="Enable"></param>
        /// <returns></returns>
        public async Task WriteGyroscope_ConfigNotify(GattClientCharacteristicConfigurationDescriptorValue Enable)
        {
            switch (Enable)
            {
                case GattClientCharacteristicConfigurationDescriptorValue.Notify:
                    await WriteGyroscope_Configure(7); // always just write the same value to enable all three axis
                    break;
                case GattClientCharacteristicConfigurationDescriptorValue.None:
                    await WriteGyroscope_Configure(0);
                    break;
            }
        }
    }
}

