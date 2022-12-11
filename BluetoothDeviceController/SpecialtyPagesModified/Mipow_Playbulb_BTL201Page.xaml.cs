using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Names;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[NAME]] device
    /// </summary>
    public sealed partial class MIPOW_Playbulb_BTL201Page : Page, HasId, ISetHandleStatus
    {
        public MIPOW_Playbulb_BTL201Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += MIPOW_Playbulb_BTL201Page_Loaded;
        }
        bool isLoaded = false;
        bool isNavigated = false;
        int NWhiteValueChangeSupress = 0;

        private void MIPOW_Playbulb_BTL201Page_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
        }

        private string DeviceName = "MIPOW_Playbulb_BTL201";
        private string DeviceNameUser = "PLAYBULB smart bulb";

        int ncommand = 0;
        MIPOW_Playbulb_BTL201 bleDevice = new MIPOW_Playbulb_BTL201();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive(false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            isNavigated = true;

            DoReadDevice_Name();
        }

        public string GetId()
        {
            return bleDevice?.ble?.DeviceId ?? "";
        }

        public string GetPicturePath()
        {
            return $"/Assets/DevicePictures/{DeviceName}-175.PNG";
        }

        public string GetDeviceNameUser()
        {
            return $"{DeviceNameUser}";
        }

        private IHandleStatus ParentStatusHandler = null;

        public void SetHandleStatus(IHandleStatus handleStatus)
        {
            ParentStatusHandler = handleStatus;
        }

        private void SetStatus(string status)
        {
            // // // uiStatus.Text = status;
            ParentStatusHandler?.SetStatusText(status);
        }
        private void SetStatusActive(bool isActive)
        {
            // // // uiProgress.IsActive = isActive;
            ParentStatusHandler?.SetStatusActive(isActive);
        }

        private async void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                SetStatus(nowstr + ": " + status.AsStatusString);
                SetStatusActive(false);
            });
        }

        public void DoReadDevice_Name()
        {
            OnReadDevice_Name(null, null);
        }

        private async void OnReadDevice_Name(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDevice_Name();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Device_Name");
                    return;
                }

                // // // var record = new Device_NameRecord();

                var Device_Name = valueList.GetValue("Device_Name");
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    // // // record.Device_Name = (string)Device_Name.AsString;
                    // // // Device_Name_Device_Name.Text = record.Device_Name.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(MIPOW_Playbulb_BTL201.CharacteristicsEnum.All_enum, true);
            SetStatusActive(false);
        }

        // The Mipow bulb doesn't have an on/off.
        private async void OnPowerChecked(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;
            if (!isLoaded || !isNavigated) return;

            bool onoff = (sender as CheckBox).IsChecked.Value;
            if (onoff)
            {
                if (currUsingColorNotWhite)
                {
                    await bleDevice.WriteColor(0, currColor.R, currColor.G, currColor.B);
                }
                else
                {
                    await bleDevice.WriteColor(currWhite, 0, 0, 0);
                }
            }
            else
            {
                await bleDevice.WriteColor (0, 0, 0, 0);
            }
        }

        private async void OnWhiteValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            if (!isLoaded || !isNavigated) return;
            if (NWhiteValueChangeSupress-- > 0) return;
            currWhite = (byte)e.NewValue;
            currUsingColorNotWhite = false;
            await bleDevice.WriteColor (currWhite, 0, 0, 0);
        }

        byte currDemoSpeed = 0x40;
        byte currDemoMode = 0x00;
        Color currDemoColor = Colors.Green;
        Color defaultDemoColor = Colors.Green;
        Color candleDemoColor = Colors.OrangeRed;
        Color currColor = Colors.Blue;
        byte currWhite = 0;
        bool currUsingColorNotWhite = true;
        int nColorCommand = 0;

        private async void OnDemoSpeedChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            currDemoSpeed = (byte)e.NewValue;
            // Good for Triones but not for Mipow: if (currDemoMode == 0) return;
            if (!isLoaded || !isNavigated) return;

            await bleDevice.WriteEffect(0x00, currDemoColor.R, currDemoColor.G, currDemoColor.B, currDemoMode, 0, currDemoSpeed, currDemoSpeed);
        }

        private async void OnDemoSelect(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            currDemoMode = (byte)(int)(e.AddedItems[0]);
            currDemoColor = (currDemoMode == (byte)Mipow_Playbulb_BTL201_Custom.Modes.Candle) 
                ? candleDemoColor 
                : nColorCommand > 0 ? currColor : defaultDemoColor;
            await bleDevice.WriteEffect(0x00, currDemoColor.R, currDemoColor.G, currDemoColor.B, currDemoMode, 0, currDemoSpeed, currDemoSpeed);
        }

        private async void OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;
            nColorCommand++;

            currColor = args.NewColor;
            currUsingColorNotWhite = true;
            await bleDevice.WriteColor (0, currColor.R, currColor.G, currColor.B);
        }
    }
}
