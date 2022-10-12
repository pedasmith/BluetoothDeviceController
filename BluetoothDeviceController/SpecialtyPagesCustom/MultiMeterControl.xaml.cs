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
            await ConnectCallbacksAsync();
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
        bool MM_DataNotifySetup = false;
        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMM_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
#if NEVER_EER_DEFINED
        int MM_DataNotifyIndex = 0;
        private async Task DoNotifyMM_Data()
        {
            try
            {
                // Only set up the event callback once.
                if (!MM_DataNotifySetup)
                {
                    MM_DataNotifySetup = true;
                    bleDevice.MM_DataEvent += BleDevice_MM_DataEvent;
                }
                var notifyType = NotifyMM_DataSettings[MM_DataNotifyIndex];
                MM_DataNotifyIndex = (MM_DataNotifyIndex + 1) % NotifyMM_DataSettings.Length;
                var result = await bleDevice.NotifyMM_DataAsync(notifyType);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }
#endif
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
            uiMMSetting.Text = "Ω";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMCurrentAC(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "⏦ Amp AC";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMCurrentDC(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "⎓ Amp DC";
            uiMMValue.Text = e.Value.ToString("F2");
        }

        private void BleDevice_OnMMVoltDC(object sender, PokitProMeter.MMData e)
        {
            uiMMSetting.Text = "VDC";
            uiMMValue.Text = e.Value.ToString("F2");
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
                BtConnectionState = ConnectionState.Configuring;
                await bleDevice.WriteMM_Settings((byte)mode, range, interval);
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
}
