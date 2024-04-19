using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.SpecialtyPagesCustom
{
    public sealed partial class MultiMeterControl : UserControl
    {
        public enum ConnectionState {  Off, Configuring, Configured, GotData, GotDataStale, Deconfiguring, Failed };
        public ConnectionState _BtConnectionState = ConnectionState.Off;
        private void UpdateConnectionState()
        {
            uiState.Text = BtConnectionState.ToString();
        }
        public ConnectionState BtConnectionState { 
            get { return _BtConnectionState; }
            internal set
            {
                if (_BtConnectionState == value) return;
                _BtConnectionState = value;
                UIThreadHelper.CallOnUIThread(UpdateConnectionState);
            }
        }
        PokitProMeter bleDevice = null;

        public MultiMeterControl()
        {
            this.InitializeComponent();
            this.Loaded += MultiMeterControl_Loaded;
        }


        public async Task SetMeter(PokitProMeter value)
        {
            BtConnectionState = ConnectionState.Off; // Assume the new value isn't connected
            bleDevice = value;
        }

        private void MultiMeterControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateConnectionState();
        }

        private void SetStatus(string text)
        {
            ;
        }

        private async Task ConnectCallbacksAsync()
        {
            if (BtConnectionState != ConnectionState.Off && BtConnectionState != ConnectionState.Failed)
            {
                return; 
            }
            if (bleDevice == null)
            {
                return;
            }
            // Change: set up events for MM and always ask for MM notifications
            bleDevice.OnMMVoltDC += BleDevice_OnMMVoltDC;
            bleDevice.OnMMVoltAC += BleDevice_OnMMVoltAC;
            bleDevice.OnMMCurrentDC += BleDevice_OnMMCurrentDC;
            bleDevice.OnMMCurrentAC += BleDevice_OnMMCurrentAC;
            bleDevice.OnMMResistance += BleDevice_OnMMResistance;
            bleDevice.OnMMDiode += BleDevice_OnMMDiode;
            bleDevice.OnMMContinuity += BleDevice_OnMMContinuity;
            bleDevice.OnMMTemperature += BleDevice_OnMMTemperature;
            bleDevice.OnMMOther += BleDevice_OnMMOther;

            // Copied from the Pokit_ProPage.xaml.cs
            bleDevice.MM_DataEvent += BleDevice_MM_DataEvent;
            BtConnectionState = ConnectionState.Configuring;
            var result = await bleDevice.NotifyMM_DataAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            BtConnectionState = result ? ConnectionState.Configured : ConnectionState.Failed;

        }
        private async Task RemoveCallbacksAsync()
        {
            if (bleDevice == null)
            {
                return;
            }
            // Change: set up events for MM and always ask for MM notifications
            bleDevice.OnMMVoltDC -= BleDevice_OnMMVoltDC;
            bleDevice.OnMMVoltAC -= BleDevice_OnMMVoltAC;
            bleDevice.OnMMCurrentDC -= BleDevice_OnMMCurrentDC;
            bleDevice.OnMMCurrentAC -= BleDevice_OnMMCurrentAC;
            bleDevice.OnMMResistance -= BleDevice_OnMMResistance;
            bleDevice.OnMMDiode -= BleDevice_OnMMDiode;
            bleDevice.OnMMContinuity -= BleDevice_OnMMContinuity;
            bleDevice.OnMMTemperature -= BleDevice_OnMMTemperature;
            bleDevice.OnMMOther -= BleDevice_OnMMOther;

            bleDevice.MM_DataEvent -= BleDevice_MM_DataEvent;
            var result = await bleDevice.NotifyMM_DataAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            BtConnectionState = result ? ConnectionState.Off : ConnectionState.Failed;
        }


        // Copied from the Pokit_ProPage.xaml.cs file
       //TODO: bool MM_DataNotifySetup = false;
        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMM_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        private void BleDevice_MM_DataEvent(BleEditor.ValueParserResult data)
        {
            BtConnectionState = ConnectionState.GotData;
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                UIThreadHelper.CallOnUIThread(bleDevice.HandleMMMessageCustom);
            }
        }
        private void BleDevice_OnMMOther(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "??";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMTemperature(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "Temp";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMContinuity(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "Cont";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMDiode(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "Diode";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMResistance(object sender, PokitProMeter.MMData e)
        {
            BtConnectionState = ConnectionState.GotData;
            var value = MathUtilities.Significant(e.Value);

            double[] limits = { 1_000_000, 1_000 };
            string[] limitstring = { "MΩ", "KΩ" };
            string units = "Ω";

            for (int i=0; i<limits.Length; i++)
            {
                if (value > limits[i])
                {
                    value = value / limits[i];
                    units = limitstring[i];
                    break;
                }
            }

            uiMMSetting.Text = units;
            uiMMValue.Text = value.ToString("F");
        }

        private void BleDevice_OnMMCurrentAC(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "⏦ Amp AC";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMCurrentDC(object sender, PokitProMeter.MMData e)
        {
            BtConnectionState = ConnectionState.GotData;
            var value = MathUtilities.Significant(e.Value);

            double[] limits = { 1, 0.001 };
            string[] limitstring = { "⎓ Amp DC", "⎓ mA DC" };
            string units = "⎓ Amp DC";

            for (int i = 0; i < limits.Length; i++)
            {
                if (value > limits[i])
                {
                    value = value / limits[i];
                    units = limitstring[i];
                    break;
                }
            }


            uiMMSetting.Text = units;
            uiMMValue.Text = value.ToString("F2");
        }

        private void BleDevice_OnMMVoltDC(object sender, PokitProMeter.MMData e)
        {
            var value = MathUtilities.Significant(e.Value);

            double[] limits = { 0.1, 0.001 };
            double[] limitsDivide = { 1, 0.001 }; // if the value is > 0.1 volts, just output as volts
            string[] limitstring = { "V DC", "mV DC" };
            string units = "VDC";

            for (int i = 0; i < limits.Length; i++)
            {
                if (value > limits[i])
                {
                    value = value / limitsDivide[i];
                    units = limitstring[i];
                    break;
                }
            }


            uiMMSetting.Text = units;
            uiMMValue.Text = value.ToString("F2");

        }

        private void BleDevice_OnMMVoltAC(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "VAC";
            uiMMValue.Text = e.Value.ToString("F2");
        }


        private async void OnMMRunClick(object sender, RoutedEventArgs e)
        {
            var mode = GetCurrentMMMode(PokitProMeter.MMMode.VoltAC);
            var start = (sender as ToggleButton).IsChecked.Value;
            byte range = 255; // autorange for all settings
            UInt32 interval = 100; // ms ; TODO: should be settable?
            if (bleDevice == null) return;
            if (start)
            {
                await ConnectCallbacksAsync();
                BtConnectionState = ConnectionState.Configuring;
                await bleDevice.WriteMM_Settings((byte)mode, range, interval);
                //var ok = await bleDevice.NotifyMM_DataAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                BtConnectionState = ConnectionState.Configured;
            }
            else
            {
                BtConnectionState = ConnectionState.Deconfiguring;
                uiMMSetting.Text = "...";
                uiMMValue.Text = "...";
                await bleDevice.WriteMM_Settings((byte)PokitProMeter.MMMode.Idle, range, interval);
                await RemoveCallbacksAsync();
            }
        }

        /// <summary>
        /// Gets the current mode the user requested (e.g., VoltDC, etc.)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private PokitProMeter.MMMode GetCurrentMMMode(PokitProMeter.MMMode defaultValue)
        {
            if (uiMMModeVDC.IsChecked.Value) return PokitProMeter.MMMode.VoltDC;
            if (uiMMModeVAC.IsChecked.Value) return PokitProMeter.MMMode.VoltAC;
            if (uiMMModeCDC.IsChecked.Value) return PokitProMeter.MMMode.CurrentDC;
            if (uiMMModeCAC.IsChecked.Value) return PokitProMeter.MMMode.CurrentAC;
            if (uiMMModeRes.IsChecked.Value) return PokitProMeter.MMMode.Resistance;
            if (uiMMModeDio.IsChecked.Value) return PokitProMeter.MMMode.Diode;
            if (uiMMModeCon.IsChecked.Value) return PokitProMeter.MMMode.Continuity;
            return defaultValue;
        }

        /// <summary>
        /// Called when the user 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMMModeChecked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) return;

            if (uiMMRunButton.IsChecked.Value)
            {
                BtConnectionState = ConnectionState.Configuring;
                OnMMRunClick(uiMMRunButton, null);
            }
            else
            {
                uiMMRunButton.IsChecked = true;
            }
        }

    }

    static class MathUtilities
    {
        // https://stackoverflow.com/questions/374316/round-a-double-to-x-significant-figures

        /// <summary>
        /// Rounds to a certain number of significant figures
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double Significant(this double value, int digits = 3)
        {
            if (value == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
            var retval = scale * Math.Round(value / scale, digits);
            return retval;
        }
    }
}
