using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Names;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utilities;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static BluetoothProtocols.Keyboard_BTUnicode;
using Windows.UI.Input.Preview.Injection;
using Windows.Foundation.Diagnostics;
using System.Threading;
using BTUniversalKeyboard;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the BTUnicode Keyboard device
    /// </summary>
    // CHANGE: base class is UserControl
    public sealed partial class Keyboard_BTUnicodePage : UserControl, HasId, ISetHandleStatus
    {
        public Keyboard_BTUnicodePage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Keyboard_BTUnicode";
        private string DeviceNameUser = "BTUnicode Keyboard";

        int ncommand = 0;
        Keyboard_BTUnicode bleDevice = new Keyboard_BTUnicode();
        // CHANGE: all of the input stuff!
        InputInjector CurrInputInjector = null;

        // CHANGE: from an override of OnNavigatedTo to a new method
        // CHANGE: rename OnNavigateTo to DoInitializeAync
        // CHANGE: public, not private
        public async Task DoInitializeAsync(DeviceInformationWrapper di)
        {
            SetStatusActive (true);
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive (false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;

            // CHANGE: no need for reading the name?
            await DoReadDevice_Name();

            // CHANGE: get a notify for keypress
            await DoNotifyKeyPress();
            await DoNotifyKeyCount();
            await DoNotifyKeyScanCode();
            await DoNotifyKeyUtf8();
            await DoNotifyKeyVirtualCode();
            await DoNotifyKeyCommand();

        }

        // CHANGE: added Log()
        private void Log(string str)
        {
            Console.WriteLine(str + "(console)");
            System.Diagnostics.Debug.WriteLine(str + "(debug)");
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
            uiStatus.Text = status;
            ParentStatusHandler?.SetStatusText(status);
            Log(status); //CHANGE: add this log statement for debuggin
        }
        private void SetStatusActive (bool isActive)
        {
            uiProgress.IsActive = isActive;
            ParentStatusHandler?.SetStatusActive(isActive);
        }

        private async void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                SetStatus(nowstr + ": " + status.AsStatusString);
                SetStatusActive (false);
            });
        }


        // Functions for Common Configuration
        public class Device_NameRecord : INotifyPropertyChanged
        {
            public Device_NameRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _Device_Name;
            public string Device_Name { get { return _Device_Name; } set { if (value == _Device_Name) return; _Device_Name = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Device_NameRecord> Device_NameRecordData { get; } = new DataCollection<Device_NameRecord>();


        private async Task DoReadDevice_Name()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDevice_Name();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Device_Name");
                    return;
                }
                
                var record = new Device_NameRecord();
                var Device_Name = valueList.GetValue("Device_Name");
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString || Device_Name.IsArray)
                {
                    record.Device_Name = (string)Device_Name.AsString;
                }

                Device_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class AppearanceRecord : INotifyPropertyChanged
        {
            public AppearanceRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Appearance;
            public double Appearance { get { return _Appearance; } set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<AppearanceRecord> AppearanceRecordData { get; } = new DataCollection<AppearanceRecord>();

        private async Task DoReadAppearance()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAppearance();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Appearance");
                    return;
                }
                
                var record = new AppearanceRecord();
                var Appearance = valueList.GetValue("Appearance");
                if (Appearance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Appearance.CurrentType == BCBasic.BCValue.ValueType.IsString || Appearance.IsArray)
                {
                    record.Appearance = (double)Appearance.AsDouble;
                }

                AppearanceRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class Connection_ParameterRecord : INotifyPropertyChanged
        {
            public Connection_ParameterRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Interval_Min;
            public double Interval_Min { get { return _Interval_Min; } set { if (value == _Interval_Min) return; _Interval_Min = value; OnPropertyChanged(); } }
            private double _Interval_Max;
            public double Interval_Max { get { return _Interval_Max; } set { if (value == _Interval_Max) return; _Interval_Max = value; OnPropertyChanged(); } }
            private double _Latency;
            public double Latency { get { return _Latency; } set { if (value == _Latency) return; _Latency = value; OnPropertyChanged(); } }
            private double _Timeout;
            public double Timeout { get { return _Timeout; } set { if (value == _Timeout) return; _Timeout = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Connection_ParameterRecord> Connection_ParameterRecordData { get; } = new DataCollection<Connection_ParameterRecord>();

        private async Task DoReadConnection_Parameter()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadConnection_Parameter();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Connection_Parameter");
                    return;
                }
                
                var record = new Connection_ParameterRecord();
                var Interval_Min = valueList.GetValue("Interval_Min");
                if (Interval_Min.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Interval_Min.CurrentType == BCBasic.BCValue.ValueType.IsString || Interval_Min.IsArray)
                {
                    record.Interval_Min = (double)Interval_Min.AsDouble;
                }
                var Interval_Max = valueList.GetValue("Interval_Max");
                if (Interval_Max.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Interval_Max.CurrentType == BCBasic.BCValue.ValueType.IsString || Interval_Max.IsArray)
                {
                    record.Interval_Max = (double)Interval_Max.AsDouble;
                }
                var Latency = valueList.GetValue("Latency");
                if (Latency.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Latency.CurrentType == BCBasic.BCValue.ValueType.IsString || Latency.IsArray)
                {
                    record.Latency = (double)Latency.AsDouble;
                }
                var Timeout = valueList.GetValue("Timeout");
                if (Timeout.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Timeout.CurrentType == BCBasic.BCValue.ValueType.IsString || Timeout.IsArray)
                {
                    record.Timeout = (double)Timeout.AsDouble;
                }

                Connection_ParameterRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class Central_Address_ResolutionRecord : INotifyPropertyChanged
        {
            public Central_Address_ResolutionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _AddressResolutionSupported;
            public double AddressResolutionSupported { get { return _AddressResolutionSupported; } set { if (value == _AddressResolutionSupported) return; _AddressResolutionSupported = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Central_Address_ResolutionRecord> Central_Address_ResolutionRecordData { get; } = new DataCollection<Central_Address_ResolutionRecord>();

        private async Task DoReadCentral_Address_Resolution()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadCentral_Address_Resolution();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Central_Address_Resolution");
                    return;
                }
                
                var record = new Central_Address_ResolutionRecord();
                var AddressResolutionSupported = valueList.GetValue("AddressResolutionSupported");
                if (AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsString || AddressResolutionSupported.IsArray)
                {
                    record.AddressResolutionSupported = (double)AddressResolutionSupported.AsDouble;
                }

                Central_Address_ResolutionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }




        // Functions for AdafruitControl2
        // Functions for Device Info
        public class Manufacturer_NameRecord : INotifyPropertyChanged
        {
            public Manufacturer_NameRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Manufacturer_NameRecord> Manufacturer_NameRecordData { get; } = new DataCollection<Manufacturer_NameRecord>();



        private async Task DoReadManufacturer_Name()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadManufacturer_Name();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Manufacturer_Name");
                    return;
                }
                
                var record = new Manufacturer_NameRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                }

                Manufacturer_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class Software_RevisionRecord : INotifyPropertyChanged
        {
            public Software_RevisionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Software_RevisionRecord> Software_RevisionRecordData { get; } = new DataCollection<Software_RevisionRecord>();

        private async Task DoReadSoftware_Revision()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSoftware_Revision();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Software_Revision");
                    return;
                }
                
                var record = new Software_RevisionRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                }

                Software_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class Model_NumberRecord : INotifyPropertyChanged
        {
            public Model_NumberRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Model_NumberRecord> Model_NumberRecordData { get; } = new DataCollection<Model_NumberRecord>();
 
        private async Task DoReadModel_Number()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadModel_Number();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Model_Number");
                    return;
                }
                
                var record = new Model_NumberRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                }

                Model_NumberRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class Serial_NumberRecord : INotifyPropertyChanged
        {
            public Serial_NumberRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Serial_NumberRecord> Serial_NumberRecordData { get; } = new DataCollection<Serial_NumberRecord>();
 
        private async Task DoReadSerial_Number()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSerial_Number();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Serial_Number");
                    return;
                }
                
                var record = new Serial_NumberRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                }

                Serial_NumberRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class Firmware_RevisionRecord : INotifyPropertyChanged
        {
            public Firmware_RevisionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Firmware_RevisionRecord> Firmware_RevisionRecordData { get; } = new DataCollection<Firmware_RevisionRecord>();
    
        private async Task DoReadFirmware_Revision()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadFirmware_Revision();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Firmware_Revision");
                    return;
                }
                
                var record = new Firmware_RevisionRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                }

                Firmware_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class Hardware_RevisionRecord : INotifyPropertyChanged
        {
            public Hardware_RevisionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Hardware_RevisionRecord> Hardware_RevisionRecordData { get; } = new DataCollection<Hardware_RevisionRecord>();
    
        private async Task DoReadHardware_Revision()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHardware_Revision();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Hardware_Revision");
                    return;
                }
                
                var record = new Hardware_RevisionRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                }

                Hardware_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }




        // Functions for BTKeyboard
        public class KeyPressRecord : INotifyPropertyChanged
        {
            public KeyPressRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Press;
            public double Press { get { return _Press; } set { if (value == _Press) return; _Press = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<KeyPressRecord> KeyPressRecordData { get; } = new DataCollection<KeyPressRecord>();


    // Functions called from the expander
    private void OnKeepCountKeyPress(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyPressRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmKeyPress(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyPressRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }


        GattClientCharacteristicConfigurationDescriptorValue[] NotifyKeyPressSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int KeyPressNotifyIndex = 0;
        bool KeyPressNotifySetup = false;
        private async void OnNotifyKeyPress(object sender, RoutedEventArgs e)
        {
            await DoNotifyKeyPress();
        }

        private async Task DoNotifyKeyPress()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!KeyPressNotifySetup)
                {
                    KeyPressNotifySetup = true;
                    bleDevice.KeyPressEvent += BleDevice_KeyPressEvent;
                }
                var notifyType = NotifyKeyPressSettings[KeyPressNotifyIndex];
                KeyPressNotifyIndex = (KeyPressNotifyIndex + 1) % NotifyKeyPressSettings.Length;
                var result = await bleDevice.NotifyKeyPressAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_KeyPressEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new KeyPressRecord();
                var Press = valueList.GetValue("Press");
                if (Press.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Press.CurrentType == BCBasic.BCValue.ValueType.IsString || Press.IsArray)
                {
                    record.Press = (double)Press.AsDouble;
                    KeyPress_Press.Text = record.Press.ToString("N0");
                }

                var addResult = KeyPressRecordData.AddRecord(record);

                    // CHANGE: handle key press. Find the actual value.
                    // Press is 0 for release 1+ for press
                    if (record.Press >= 1)
                    {
                        // TODO: inject the Command!
                        //var utf8 = bleDevice.KeyUtf8;
                        //InjectString(utf8);
                        InjectList(KeyCommandList);

                    }

                    // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadKeyPress(object sender, RoutedEventArgs e)
        {
            await DoReadKeyPress();
        }

        private async Task DoReadKeyPress()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadKeyPress();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read KeyPress");
                    return;
                }
                
                var record = new KeyPressRecord();
                var Press = valueList.GetValue("Press");
                if (Press.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Press.CurrentType == BCBasic.BCValue.ValueType.IsString || Press.IsArray)
                {
                    record.Press = (double)Press.AsDouble;
                    KeyPress_Press.Text = record.Press.ToString("N0");
                }

                KeyPressRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class KeyCountRecord : INotifyPropertyChanged
        {
            public KeyCountRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _PressCount;
            public double PressCount { get { return _PressCount; } set { if (value == _PressCount) return; _PressCount = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<KeyCountRecord> KeyCountRecordData { get; } = new DataCollection<KeyCountRecord>();


    // Functions called from the expander
    private void OnKeepCountKeyCount(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyCountRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmKeyCount(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyCountRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }


        GattClientCharacteristicConfigurationDescriptorValue[] NotifyKeyCountSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int KeyCountNotifyIndex = 0;
        bool KeyCountNotifySetup = false;
        private async void OnNotifyKeyCount(object sender, RoutedEventArgs e)
        {
            await DoNotifyKeyCount();
        }

        private async Task DoNotifyKeyCount()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!KeyCountNotifySetup)
                {
                    KeyCountNotifySetup = true;
                    bleDevice.KeyCountEvent += BleDevice_KeyCountEvent;
                }
                var notifyType = NotifyKeyCountSettings[KeyCountNotifyIndex];
                KeyCountNotifyIndex = (KeyCountNotifyIndex + 1) % NotifyKeyCountSettings.Length;
                var result = await bleDevice.NotifyKeyCountAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_KeyCountEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new KeyCountRecord();
                var PressCount = valueList.GetValue("PressCount");
                if (PressCount.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PressCount.CurrentType == BCBasic.BCValue.ValueType.IsString || PressCount.IsArray)
                {
                    record.PressCount = (double)PressCount.AsDouble;
                    KeyCount_PressCount.Text = record.PressCount.ToString("N0");
                }

                var addResult = KeyCountRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadKeyCount(object sender, RoutedEventArgs e)
        {
            await DoReadKeyCount();
        }

        private async Task DoReadKeyCount()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadKeyCount();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read KeyCount");
                    return;
                }
                
                var record = new KeyCountRecord();
                var PressCount = valueList.GetValue("PressCount");
                if (PressCount.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PressCount.CurrentType == BCBasic.BCValue.ValueType.IsString || PressCount.IsArray)
                {
                    record.PressCount = (double)PressCount.AsDouble;
                    KeyCount_PressCount.Text = record.PressCount.ToString("N0");
                }

                KeyCountRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class KeyVirtualCodeRecord : INotifyPropertyChanged
        {
            public KeyVirtualCodeRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _VirtualCode;
            public double VirtualCode { get { return _VirtualCode; } set { if (value == _VirtualCode) return; _VirtualCode = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<KeyVirtualCodeRecord> KeyVirtualCodeRecordData { get; } = new DataCollection<KeyVirtualCodeRecord>();


    // Functions called from the expander
    private void OnKeepCountKeyVirtualCode(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyVirtualCodeRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmKeyVirtualCode(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyVirtualCodeRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }


        GattClientCharacteristicConfigurationDescriptorValue[] NotifyKeyVirtualCodeSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int KeyVirtualCodeNotifyIndex = 0;
        bool KeyVirtualCodeNotifySetup = false;
        private async void OnNotifyKeyVirtualCode(object sender, RoutedEventArgs e)
        {
            await DoNotifyKeyVirtualCode();
        }

        private async Task DoNotifyKeyVirtualCode()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!KeyVirtualCodeNotifySetup)
                {
                    KeyVirtualCodeNotifySetup = true;
                    bleDevice.KeyVirtualCodeEvent += BleDevice_KeyVirtualCodeEvent;
                }
                var notifyType = NotifyKeyVirtualCodeSettings[KeyVirtualCodeNotifyIndex];
                KeyVirtualCodeNotifyIndex = (KeyVirtualCodeNotifyIndex + 1) % NotifyKeyVirtualCodeSettings.Length;
                var result = await bleDevice.NotifyKeyVirtualCodeAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_KeyVirtualCodeEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new KeyVirtualCodeRecord();
                var VirtualCode = valueList.GetValue("VirtualCode");
                if (VirtualCode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || VirtualCode.CurrentType == BCBasic.BCValue.ValueType.IsString || VirtualCode.IsArray)
                {
                    record.VirtualCode = (double)VirtualCode.AsDouble;
                    KeyVirtualCode_VirtualCode.Text = record.VirtualCode.ToString("N0");
                }

                var addResult = KeyVirtualCodeRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadKeyVirtualCode(object sender, RoutedEventArgs e)
        {
            await DoReadKeyVirtualCode();
        }

        private async Task DoReadKeyVirtualCode()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadKeyVirtualCode();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read KeyVirtualCode");
                    return;
                }
                
                var record = new KeyVirtualCodeRecord();
                var VirtualCode = valueList.GetValue("VirtualCode");
                if (VirtualCode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || VirtualCode.CurrentType == BCBasic.BCValue.ValueType.IsString || VirtualCode.IsArray)
                {
                    record.VirtualCode = (double)VirtualCode.AsDouble;
                    KeyVirtualCode_VirtualCode.Text = record.VirtualCode.ToString("N0");
                }

                KeyVirtualCodeRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class KeyScanCodeRecord : INotifyPropertyChanged
        {
            public KeyScanCodeRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _ScanCode;
            public double ScanCode { get { return _ScanCode; } set { if (value == _ScanCode) return; _ScanCode = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<KeyScanCodeRecord> KeyScanCodeRecordData { get; } = new DataCollection<KeyScanCodeRecord>();


    // Functions called from the expander
    private void OnKeepCountKeyScanCode(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyScanCodeRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmKeyScanCode(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyScanCodeRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }


        GattClientCharacteristicConfigurationDescriptorValue[] NotifyKeyScanCodeSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int KeyScanCodeNotifyIndex = 0;
        bool KeyScanCodeNotifySetup = false;
        private async void OnNotifyKeyScanCode(object sender, RoutedEventArgs e)
        {
            await DoNotifyKeyScanCode();
        }

        private async Task DoNotifyKeyScanCode()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!KeyScanCodeNotifySetup)
                {
                    KeyScanCodeNotifySetup = true;
                    bleDevice.KeyScanCodeEvent += BleDevice_KeyScanCodeEvent;
                }
                var notifyType = NotifyKeyScanCodeSettings[KeyScanCodeNotifyIndex];
                KeyScanCodeNotifyIndex = (KeyScanCodeNotifyIndex + 1) % NotifyKeyScanCodeSettings.Length;
                var result = await bleDevice.NotifyKeyScanCodeAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_KeyScanCodeEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new KeyScanCodeRecord();
                var ScanCode = valueList.GetValue("ScanCode");
                if (ScanCode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ScanCode.CurrentType == BCBasic.BCValue.ValueType.IsString || ScanCode.IsArray)
                {
                    record.ScanCode = (double)ScanCode.AsDouble;
                    KeyScanCode_ScanCode.Text = record.ScanCode.ToString("N0");
                }

                var addResult = KeyScanCodeRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadKeyScanCode(object sender, RoutedEventArgs e)
        {
            await DoReadKeyScanCode();
        }

        private async Task DoReadKeyScanCode()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadKeyScanCode();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read KeyScanCode");
                    return;
                }
                
                var record = new KeyScanCodeRecord();
                var ScanCode = valueList.GetValue("ScanCode");
                if (ScanCode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ScanCode.CurrentType == BCBasic.BCValue.ValueType.IsString || ScanCode.IsArray)
                {
                    record.ScanCode = (double)ScanCode.AsDouble;
                    KeyScanCode_ScanCode.Text = record.ScanCode.ToString("N0");
                }

                KeyScanCodeRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class KeyUtf8Record : INotifyPropertyChanged
        {
            public KeyUtf8Record()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _Utf8;
            public string Utf8 { get { return _Utf8; } set { if (value == _Utf8) return; _Utf8 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<KeyUtf8Record> KeyUtf8RecordData { get; } = new DataCollection<KeyUtf8Record>();
    private void OnKeyUtf8_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (KeyUtf8RecordData.Count == 0)
            {
                KeyUtf8RecordData.AddRecord(new KeyUtf8Record());
            }
            KeyUtf8RecordData[KeyUtf8RecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountKeyUtf8(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyUtf8RecordData.MaxLength = value;

        
    }

    private void OnAlgorithmKeyUtf8(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        KeyUtf8RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }


        GattClientCharacteristicConfigurationDescriptorValue[] NotifyKeyUtf8Settings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int KeyUtf8NotifyIndex = 0;
        bool KeyUtf8NotifySetup = false;
        private async void OnNotifyKeyUtf8(object sender, RoutedEventArgs e)
        {
            await DoNotifyKeyUtf8();
        }

        private async Task DoNotifyKeyUtf8()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!KeyUtf8NotifySetup)
                {
                    KeyUtf8NotifySetup = true;
                    bleDevice.KeyUtf8Event += BleDevice_KeyUtf8Event;
                }
                var notifyType = NotifyKeyUtf8Settings[KeyUtf8NotifyIndex];
                KeyUtf8NotifyIndex = (KeyUtf8NotifyIndex + 1) % NotifyKeyUtf8Settings.Length;
                var result = await bleDevice.NotifyKeyUtf8Async(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_KeyUtf8Event(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new KeyUtf8Record();
                var Utf8 = valueList.GetValue("Utf8");
                if (Utf8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Utf8.CurrentType == BCBasic.BCValue.ValueType.IsString || Utf8.IsArray)
                {
                    record.Utf8 = (string)Utf8.AsString;
                    KeyUtf8_Utf8.Text = record.Utf8.ToString();
                }

                var addResult = KeyUtf8RecordData.AddRecord(record);




                    // Original update was to make this CHART+COMMAND
                });
            }
        }


        private async void OnReadKeyUtf8(object sender, RoutedEventArgs e)
        {
            await DoReadKeyUtf8();
        }

        private async Task DoReadKeyUtf8()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadKeyUtf8();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read KeyUtf8");
                    return;
                }
                
                var record = new KeyUtf8Record();
                var Utf8 = valueList.GetValue("Utf8");
                if (Utf8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Utf8.CurrentType == BCBasic.BCValue.ValueType.IsString || Utf8.IsArray)
                {
                    record.Utf8 = (string)Utf8.AsString;
                    KeyUtf8_Utf8.Text = record.Utf8.ToString();
                }

                KeyUtf8RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        //
        // CHANGE: add all this Command stuff
        //
        public class KeyCommandRecord : INotifyPropertyChanged
        {
            public KeyCommandRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _Command;
            public string Command { get { return _Command; } set { if (value == _Command) return; _Command = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<KeyCommandRecord> KeyCommandRecordData { get; } = new DataCollection<KeyCommandRecord>();
        private void OnKeyCommand_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (KeyCommandRecordData.Count == 0)
                {
                    KeyCommandRecordData.AddRecord(new KeyCommandRecord());
                }
                KeyCommandRecordData[KeyCommandRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountKeyCommand(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            KeyCommandRecordData.MaxLength = value;


        }

        private void OnAlgorithmKeyCommand(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            KeyCommandRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }


        GattClientCharacteristicConfigurationDescriptorValue[] NotifyKeyCommandSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int KeyCommandNotifyIndex = 0;
        bool KeyCommandNotifySetup = false;
        private async void OnNotifyKeyCommand(object sender, RoutedEventArgs e)
        {
            await DoNotifyKeyCommand();
        }

        private async Task DoNotifyKeyCommand()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!KeyCommandNotifySetup)
                {
                    KeyCommandNotifySetup = true;
                    bleDevice.KeyCommandEvent += BleDevice_KeyCommandEvent;
                }
                var notifyType = NotifyKeyCommandSettings[KeyCommandNotifyIndex];
                KeyCommandNotifyIndex = (KeyCommandNotifyIndex + 1) % NotifyKeyCommandSettings.Length;
                var result = await bleDevice.NotifyKeyCommandAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        // CHANGE: new list
        List<Object> KeyCommandList = new List<Object>();
        private async void BleDevice_KeyCommandEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new KeyCommandRecord();
                    var Command = valueList.GetValue("Command");
                    if (Command.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Command.CurrentType == BCBasic.BCValue.ValueType.IsString || Command.IsArray)
                    {
                        record.Command = (string)Command.AsString;
                        KeyCommand_Command.Text = record.Command.ToString();
                    }

                    var addResult = KeyCommandRecordData.AddRecord(record);

                    // Command.AsArray is the data in about the most memory-intensive format possible.
                    var array = Command.AsArray.AsByteArray(); // Handy conversion
                    BtUniversalKeyboardCommand.Parse(array, KeyCommandList);
                    // Original update was to make this CHART+COMMAND
                });
            }
        }


        private async void OnReadKeyCommand(object sender, RoutedEventArgs e)
        {
            await DoReadKeyCommand();
        }

        private async Task DoReadKeyCommand()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadKeyCommand();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read KeyCommand");
                    return;
                }

                var record = new KeyCommandRecord();
                var Command = valueList.GetValue("Command");
                if (Command.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Command.CurrentType == BCBasic.BCValue.ValueType.IsString || Command.IsArray)
                {
                    record.Command = (string)Command.AsString;
                    KeyCommand_Command.Text = record.Command.ToString();
                }

                KeyCommandRecordData.Add(record);

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
            await bleDevice.EnsureCharacteristicAsync(CharacteristicsEnum.All_enum, true);
            SetStatusActive(false);
        }


        // CHANGE: this new method

        private async Task InjectKeyboardListIfNeededAsync(List<InjectedInputKeyboardInfo> list, int minCount)
        {
            if (list.Count > minCount)
            {
                try
                {
                    CurrInputInjector.InjectKeyboardInput(list);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: injection: exception={e.Message}");
                }
                list.Clear();
                await Task.Delay(0); // DBG: do I need this?
            }
        }

        private async void InjectList(List<object> value)
        {
            if (CurrInputInjector == null)
            {
                CurrInputInjector = InputInjector.TryCreate();
            }
            if (CurrInputInjector != null)
            {
                var inputKeyboardList = new List<InjectedInputKeyboardInfo>();
                bool prevWasKeyboard = false;
                foreach (var item in value)
                {
                    if (item is InjectedInputKeyboardInfo iiki)
                    {
                        inputKeyboardList.Add(iiki);
                        await InjectKeyboardListIfNeededAsync(inputKeyboardList, 10);
                        prevWasKeyboard = true;
                    }
                }
                value.Clear();
                await InjectKeyboardListIfNeededAsync(inputKeyboardList, 0);
            }
        }


        private async void InjectString(string str)
        {
            //CHANGE: inject input
            if (CurrInputInjector == null)
            {
                CurrInputInjector = InputInjector.TryCreate();
            }
            if (CurrInputInjector != null)
            {
                var inputList = new List<InjectedInputKeyboardInfo>();
                foreach (var uchar in str)
                {
                    var ch = uchar;
                    var chtype = InjectedInputKeyOptions.Unicode;
                    bool isVKey = false;
                    switch (ch)
                    {
                        case '\n':
                            ch = (char)0x0d; // VK_ENTER
                            chtype = InjectedInputKeyOptions.None;
                            isVKey = true;
                            break;
                    }
                    var info = new InjectedInputKeyboardInfo()
                    {
                        KeyOptions = chtype,
                        ScanCode = isVKey ? (char)0 : ch,
                        VirtualKey = isVKey ? ch : (char)0,
                    };
                    inputList.Add(info);
                    info = new InjectedInputKeyboardInfo()
                    {
                        KeyOptions = chtype | InjectedInputKeyOptions.KeyUp,
                        ScanCode = isVKey ? (char)0 : ch,
                        VirtualKey = isVKey ? ch : (char)0,
                    };
                    inputList.Add(info);
                    if (inputList.Count >= 10)
                    {
                        try
                        {
                            // DBG: not right now! adding Command handling! CurrInputInjector.InjectKeyboardInput(list);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine($"ERROR: injection: exception={e.Message}");
                        }
                        inputList.Clear();
                        await Task.Delay(0); // DBG: do I need this?
                    }
                }
                try
                {
                    if (inputList.Count > 0)
                    {
                        // DBG: not right now! adding Command handling: CurrInputInjector.InjectKeyboardInput(list);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: injection: exception={e.Message}");
                }
            }
        }

    }
}