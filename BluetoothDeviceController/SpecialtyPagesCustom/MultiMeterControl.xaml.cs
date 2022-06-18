using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        public MultiMeterControl()
        {
            this.InitializeComponent();
            this.Loaded += MultiMeterControl_Loaded;
        }


        public async Task SetMeter(PokitProMeter value)
        {
            bleDevice = value;
            await ConnectCallbacksAsync();
        }

        PokitProMeter bleDevice = null;
        private void MultiMeterControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void SetStatus(string text)
        {
            ;
        }

        private async Task ConnectCallbacksAsync()
        {
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
            var result = await bleDevice.NotifyMM_DataAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

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

        }


        // Copied from the Pokit_ProPage.xaml.cs file
        bool MM_DataNotifySetup = false;
        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMM_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
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
        private async void BleDevice_MM_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    bleDevice.HandleMMMessageCustom(); // CHANGE: divert for event processing

                });
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
            UInt32 interval = 100; // ms 
            if (start)
            {
                await bleDevice.WriteMM_Settings((byte)mode, range, interval);
            }
            else
            {
                uiMMSetting.Text = "...";
                uiMMValue.Text = "updating";
                await bleDevice.WriteMM_Settings((byte)PokitProMeter.MMMode.Idle, range, interval);
                await RemoveCallbacksAsync()
            }
        }

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

        private void OnMMModeChecked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) return;

            if (uiMMRunButton.IsChecked.Value)
            {
                OnMMRunClick(uiMMRunButton, null);
            }
            else
            {
                uiMMRunButton.IsChecked = true;
            }
        }

    }
}
