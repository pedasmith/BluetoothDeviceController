using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Names
{
    class Generate_CSharp_Templates
    {

        public static string Protocol_BodyTemplate = @"using System;
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
    /// [[DESCRIPTION]].
    /// This class was automatically generated [[CURRTIME]]
    /// </summary>

    public [[CLASSMODIFIERS]] class [[CLASSNAME]] : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
[[LINKS]]

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
[[SERVICE_CHARACTERISTIC_LIST]]

        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;

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
                        Status.ReportStatus($""Unable to get service {ServiceNames[serviceIndex]}"", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($""Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}"", serviceStatus);
                        continue; //return false;
                    }
                    var service = serviceStatus.Services[0];
                    var characteristicIndexSet = MapServiceToCharacteristic[serviceIndex];
                    foreach (var characteristicIndex in characteristicIndexSet)
                    {
                        var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[characteristicIndex]);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            Status.ReportStatus($""unable to get characteristic for {CharacteristicNames[characteristicIndex]}"", characteristicsStatus);
                            return false;
                        }
                        if (characteristicsStatus.Characteristics.Count == 0)
                        {
                            Status.ReportStatus($""unable to get any characteristics for {CharacteristicNames[characteristicIndex]}"", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else if (characteristicsStatus.Characteristics.Count != 1)
                        {
                            Status.ReportStatus($""unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[characteristicIndex]}"", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else
                        {
                            Characteristics[characteristicIndex] = characteristicsStatus.Characteristics[0];
                            lastResult = characteristicsStatus;
                        }
                        lastResult = characteristicsStatus;
                    }
                }
                // Do not call ReportStatus on OK -- the actual read/write/etc. call will
                // call ReportStatus for them. It's important that for any one actual call
                // (public method) that there's only one ReportStatus.
                //Status.ReportStatus(""OK: Connected to device"", lastResult);
                readCharacteristics = true;
            }
            return readCharacteristics;
        }

        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name=""method"" ></param>
        /// <param name=""command"" ></param>
        /// <returns></returns>
        private async Task WriteCommandAsync(int characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
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
        /// <param name=""characteristicIndex"">Index number of the characteristic</param>
        /// <param name=""method"" >Name of the actual method; is just used for logging</param>
        /// <param name=""cacheMode"" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(int characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[characteristicIndex].ReadValueAsync(cacheMode);
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
        /// <param name=""data""></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);



        [[METHOD_LIST]]
    }
}
";

        public static string Protocol_ServiceListTemplate = @"
        Guid[] ServiceGuids = new Guid[] {
[[SERVICE_GUID_LIST]]
        };
        String[] ServiceNames = new string[] {
[[SERVICE_NAME_LIST]]
        };
        GattDeviceService[] Services = new GattDeviceService[] {
[[SERVICE_LIST]]
        };
        Guid[] CharacteristicGuids = new Guid[] {
[[CHARACTERISTIC_GUID_LIST]]
        };
        String[] CharacteristicNames = new string[] {
[[CHARACTERISTIC_NAME_LIST]]
        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
[[CHARACTERISTIC_LIST]]
        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
[[HASH_LIST]]
        };
";

        public static string Protocol_NotifyMethodTemplate = @"
        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; [[CHARACTERISTICNAME]]Event += _my function_
        /// </summary>
        public event BluetoothDataEvent [[CHARACTERISTICNAME]]Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>
        
        private bool Notify[[CHARACTERISTICNAME]]_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name=""notifyType""></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> Notify[[CHARACTERISTICNAME]]Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[[[CHARACTERISTICINDEX]]];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!Notify[[CHARACTERISTICNAME]]_ValueChanged_Set)
                {
                    // Only set the event callback once
                    Notify[[CHARACTERISTICNAME]]_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = ""[[CHARACTERISTICTYPE]]"";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
[[SET_PROPERTY_VALUES]]
                        [[CHARACTERISTICNAME]]Event?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($""Notify[[CHARACTERISTICNAME]]: {e.Message}"", result);
                return false;
            }
            Status.ReportStatus($""Notify[[CHARACTERISTICNAME]]: set notification"", result);

            return true;
        }
";

        public static string Protocol_ReadMethodTemplate = @"
        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name=""cacheMode"">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> Read[[CHARACTERISTICNAME]](BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync([[CHARACTERISTICINDEX]], ""[[CHARACTERISTICNAME]]"", cacheMode);
            if (result == null) return null;

            var datameaning = ""[[CHARACTERISTICTYPE]]"";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET_PROPERTY_VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue(""LightRaw"").AsDouble;
            return parseResult.ValueList;
        }
";

        public static string Protocol_WriteMethodTemplate = @"
        /// <summary>
        /// Writes data for [[CHARACTERISTICNAME]]
        /// </summary>
        /// <param name=""Period""></param>
        /// <returns></returns>
        public async Task Write[[CHARACTERISTICNAME]]([[PARAMS]])
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
[[ARGS]]
            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync([[CHARACTERISTICINDEX]], ""[[CHARACTERISTICNAME]]"", subcommand, [[WRITEMODE]]);
            }
            // original: await DoWriteAsync(data);
        }
";

        public static string Protocol_DataPropertySetTemplate = @"
            [[CHDATANAME]] = parseResult.ValueList.GetValue(""[[DATANAME]]"").[[AS_DOUBLE_OR_STRING]];
";

        public static string Protocol_DataPropertyTemplate = @"
        private [[VARIABLETYPE_DS]] _[[CHDATANAME]] = [[DOUBLE_OR_STRING_DEFAULT]];
        private bool _[[CHDATANAME]]_set = false;
        public [[VARIABLETYPE_DS]] [[CHDATANAME]]
        {
            get { return _[[CHDATANAME]]; }
            internal set { if (_[[CHDATANAME]]_set && value == _[[CHDATANAME]]) return; _[[CHDATANAME]] = value; _[[CHDATANAME]]_set = true; OnPropertyChanged(); }
        }
";

        public static string PageXaml_BodyTemplate = @"
<Page
    x:Class=""BluetoothDeviceController.SpecialtyPages.[[CLASSNAME]]Page""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:BluetoothDeviceController.SpecialtyPages""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls""
    xmlns:charts=""using:BluetoothDeviceController.Charts""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d""
    Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
    <Page.Resources>
        <Style TargetType=""Button"">
            <Setter Property=""MinWidth"" Value=""60"" />
            <Setter Property=""VerticalAlignment"" Value=""Bottom"" />
            <Setter Property=""FontFamily"" Value=""Segoe UI,Segoe MDL2 Assets"" />
            <Setter Property=""Margin"" Value=""10,5,0,0"" />
        </Style>
        <Style TargetType=""Line"">
            <Setter Property=""Margin"" Value=""0,15,0,0"" />
            <Setter Property=""Stroke"" Value=""ForestGreen"" />
        </Style>
        <Style x:Key=""TitleStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""30"" />
        </Style>
        <Style x:Key=""HeaderStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""20"" />
        </Style>
        <Style x:Key=""HeaderStyleExpander"" TargetType=""controls:Expander"">
            <Setter Property=""MinWidth"" Value=""550"" />
            <Setter Property=""HorizontalAlignment"" Value=""Left"" />
            <Setter Property=""HorizontalContentAlignment"" Value=""Left"" />
        </Style>
        <Style x:Key=""SubheaderStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""16"" />
        </Style>
        <Style x:Key=""AboutStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""12"" />
            <Setter Property=""TextWrapping"" Value=""Wrap"" />
        </Style>
        <Style x:Key=""ChacteristicListStyle"" TargetType=""StackPanel"">
            <Setter Property=""Background"" Value=""WhiteSmoke"" />
            <Setter Property=""Margin"" Value=""18,0,0,0"" />
        </Style>
        <Style x:Key=""HEXStyle"" TargetType=""TextBox"">
            <Setter Property=""MinWidth"" Value=""90"" />
            <Setter Property=""FontSize"" Value=""12"" />
            <Setter Property=""Margin"" Value=""5,0,0,0"" />
        </Style>
        <Style x:Key=""TableStyle"" TargetType=""controls:DataGrid"">
            <Setter Property = ""Background"" Value=""BlanchedAlmond"" />
            <Setter Property = ""FontSize"" Value=""12"" />
            <Setter Property = ""Height"" Value=""200"" />
            <Setter Property = ""HorizontalAlignment"" Value=""Center"" />
            <Setter Property = ""Width"" Value=""500"" />
        </Style>
    </Page.Resources>
    
    <StackPanel>
        <Grid MaxWidth=""550"" HorizontalAlignment=""Left"">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""*"" />
                <ColumnDefinition Width=""auto"" />
            </Grid.ColumnDefinitions>
            <StackPanel Grid.Column=""0"">
                <TextBlock Style=""{StaticResource TitleStyle}"">[[DEVICENAMEUSER]] device</TextBlock>
                <TextBlock Style=""{StaticResource AboutStyle}"">
                    [[DESCRIPTION]]
                </TextBlock>
            </StackPanel>
            <controls:ImageEx Grid.Column=""1"" Style=""{StaticResource ImageStyle}""  Source=""/Assets/DevicePictures/[[CLASSNAME]].PNG"" />
        </Grid>
        <ProgressRing x:Name=""uiProgress"" />
        <TextBlock x:Name=""uiStatus"" />
[[SERVICE_LIST]]
        <Button Content=""REREAD"" Click=""OnRereadDevice"" />
    </StackPanel>
</Page>
";

        public static string PageXaml_ServiceTemplate = @"
        <controls:Expander Header=""[[SERVICENAMEUSER]]"" IsExpanded=""[[SERVICEISEXPANDED]]"" Style=""{StaticResource HeaderStyleExpander}"">
            <StackPanel Style=""{StaticResource ChacteristicListStyle}"">
[[CHARACTERISTIC_LIST]]
            </StackPanel>
        </controls:Expander>
";

        public static string PageXaml_CharacteristicTemplate = @"
                <TextBlock Style=""{StaticResource SubheaderStyle}"">[[CHARACTERISTICNAMEUSER]]</TextBlock>
                <StackPanel Orientation=""Horizontal"">
[[DATA1_LIST]]
[[READWRITE_BUTTON_LIST]]
                </StackPanel>
[[ENUM_BUTTON_LIST_PANEL]]
[[TABLE]]
";
        public static string PageXamlCharacteristicDataTemplate = @"
                    <TextBox IsReadOnly=""[[IS_READ_ONLY]]"" x:Name=""[[CHARACTERISTICNAME]]_[[DATANAME]]"" Text=""*"" Header=""[[DATANAMEUSER]]"" Style=""{StaticResource HEXStyle}""/>
";
        public static string PageXamlCharacteristicEnumButtonTemplate =
@"                    <Button Content=""[[ENUM_NAME]]"" Tag=""[[ENUM_VALUE]]"" Click=""OnClick[[CHARACTERISTICNAME]]"" />
"; 
        
        public static string PageXamlCharacteristicEnumButtonPanelTemplate =
@"                <VariableSizedWrapGrid Orientation=""Horizontal"" MaximumRowsOrColumns=""[[MAXCOLUMNS]]"">
[[ENUM_BUTTON_LIST]]                </VariableSizedWrapGrid>	";

        public static string PageXamlCharacteristicButtonTemplate = @"
                    <Button Content=""[[BUTTONTYPE]]"" Click=""On[[BUTTONTYPE]][[CHARACTERISTICNAME]]"" />
";

        public static string PageXamlCharacteristicTableTemplate = @"
                    <controls:Expander Header=""[[CHARACTERISTICNAMEUSER]] Data tracker"" IsExpanded=""false"" MinWidth=""550"" HorizontalAlignment=""Left"">
                        <StackPanel MinWidth=""550"">
                            [[XAMLCHART]]
                            <controls:DataGrid Style=""{StaticResource TableStyle}"" x:Name=""[[CHARACTERISTICNAME]]Grid"" ItemsSource=""{Binding [[CHARACTERISTICNAME]]RecordData}"" />
                            <TextBox  x:Name=""[[CHARACTERISTICNAME]]_Notebox"" KeyDown=""On[[CHARACTERISTICNAME]]_NoteKeyDown"" />
                            <StackPanel Orientation=""Horizontal"" HorizontalAlignment=""Left"">
                                <ComboBox SelectionChanged=""OnKeepCount[[CHARACTERISTICNAME]]"" Header=""Keep how many items?"" SelectedIndex=""2"">
                                    <ComboBoxItem Tag=""10"">10</ComboBoxItem>
                                    <ComboBoxItem Tag=""100"">100</ComboBoxItem>
                                    <ComboBoxItem Tag=""1000"">1,000</ComboBoxItem>
                                    <ComboBoxItem Tag=""10000"">10K</ComboBoxItem>
                                </ComboBox>
                                <Rectangle Width=""5"" />
                                <ComboBox SelectionChanged=""OnAlgorithm[[CHARACTERISTICNAME]]"" Header=""Remove algorithm?"" SelectedIndex=""0"">
                                    <ComboBoxItem Tag=""1"" ToolTipService.ToolTip=""Keep a random sample of data"">Keep random sample</ComboBoxItem>
                                    <ComboBoxItem Tag=""0"" ToolTipService.ToolTip=""Keep the most recent data"">Keep latest data</ComboBoxItem>
                                </ComboBox>
                                <Button Content = ""Copy"" Click=""OnCopy[[CHARACTERISTICNAME]]"" />
                            </StackPanel>
                        </StackPanel>
                    </controls:Expander>
";
        public static string PageCSharp_BodyTemplate =
@"using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Names;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;using Utilities;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[NAME]] device
    /// </summary>
    public sealed partial class [[CLASSNAME]]Page : Page, HasId, ISetHandleStatus
    {
        public [[CLASSNAME]]Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = ""[[CLASSNAME]]"";
        private string DeviceNameUser = ""[[DEVICENAMEUSER]]"";

        int ncommand = 0;
        [[CLASSNAME]] bleDevice = new [[CLASSNAME]]();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive (true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive (false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
[[DOREADDEVICE_NAME]]
        }

        public string GetId()
        {
            return bleDevice?.ble?.DeviceId ?? """";
        }

        public string GetPicturePath()
        {
            return $""/Assets/DevicePictures/{DeviceName}-175.PNG"";
        }

        public string GetDeviceNameUser()
        {
            return $""{DeviceNameUser}"";
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
        }
        private void SetStatusActive (bool isActive)
        {
            uiProgress.IsActive = isActive;
            ParentStatusHandler?.SetStatusActive(isActive);
        }

        private async void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $""{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}"";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                SetStatus(nowstr + "": "" + status.AsStatusString);
                SetStatusActive (false);
            });
        }
[[SERVICE_LIST]]
        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus(""Reading device"");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(true);
            SetStatusActive(false);
        }
    }
}
";
        public static string PageCSharp_ServiceTemplate = @"
// Functions for [[SERVICENAMEUSER]]

[[CHARACTERISTIC_LIST]]
";

        public static string PageCSharp_CharacteristicRecordTemplate = @"
        public class [[CHARACTERISTICNAME]]Record: INotifyPropertyChanged
        {
            public [[CHARACTERISTICNAME]]Record()
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
[[DATA1_LIST]]
            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }
    
    public DataCollection<[[CHARACTERISTICNAME]]Record> [[CHARACTERISTICNAME]]RecordData { get; } = new DataCollection<[[CHARACTERISTICNAME]]Record>();
    private void On[[CHARACTERISTICNAME]]_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = """";
            // Add the text to the notes section
            if ([[CHARACTERISTICNAME]]RecordData.Count == 0)
            {
                [[CHARACTERISTICNAME]]RecordData.AddRecord(new [[CHARACTERISTICNAME]]Record());
            }
            [[CHARACTERISTICNAME]]RecordData[[[CHARACTERISTICNAME]]RecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCount[[CHARACTERISTICNAME]](object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        [[CHARACTERISTICNAME]]RecordData.MaxLength = value;

        [[CHART_UPDATE_COMMAND]]
    }

    private void OnAlgorithm[[CHARACTERISTICNAME]](object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        [[CHARACTERISTICNAME]]RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopy[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append(""EventDate,EventTime[[DATA2_LIST]],Notes\n"");
        foreach (var row in [[CHARACTERISTICNAME]]RecordData)
        {
            var time24 = row.EventTime.ToString(""HH:mm:ss.f"");
            sb.Append($""{row.EventTime.ToShortDateString()},{time24}[[DATA3_LIST]],{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n"");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }
";
        public static string[] PageCSharp_CharacteristicRecord_DataTemplates =
        {
@"
            private [[VARIABLETYPE_DS]] _[[DATANAME]];
            public [[VARIABLETYPE_DS]] [[DATANAME]] { get { return _[[DATANAME]]; } set { if (value == _[[DATANAME]]) return; _[[DATANAME]] = value; OnPropertyChanged(); } }
", // DATA1_LIST
@",[[DATANAME]]", // DATA2_LIST
@",{row.[[DATANAME]]}", // DATA3_LIST
@"
typeof([[CHARACTERISTICNAME]]Record).GetProperty(""[[DATANAME]]""),", // DATA4_LIST
@"
""[[DATANAME]]"",", // DATA5_LIST
};


        public static string PageCSharp_CharacteristicReadTemplate = @"
        private async void OnRead[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            await DoRead[[CHARACTERISTICNAME]]();
        }

        private async Task DoRead[[CHARACTERISTICNAME]]()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.Read[[CHARACTERISTICNAME]]();
                if (valueList == null)
                {
                    SetStatus ($""Error: unable to read [[CHARACTERISTICNAME]]"");
                    return;
                }
                [[READCONVERT]]
                var record = new [[CHARACTERISTICNAME]]Record();
[[DATA1_LIST]]

                [[CHARACTERISTICNAME]]RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($""Error: exception: {ex.Message}"");
            }
        }
";
        public static string[] PageCSharp_CharacteristicRead_DataTemplates = new string[] { @"
                var [[DATANAME]] = valueList.GetValue(""[[DATANAME]]"");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE_DS]])[[DATANAME]].[[AS_DOUBLE_OR_STRING]];
                    [[CHARACTERISTICNAME]]_[[DATANAME]].Text = record.[[DATANAME]].ToString(); // ""N0""); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
" };

        public static string PageCSharp_CharacteristicWriteTemplate = @"
        // OK to include this method even if there are no defined buttons
        private async void OnClick[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWrite[[CHARACTERISTICNAME]] (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWrite[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            var text = [[CHARACTERISTICNAME]]_[[DATANAME]].Text;
            await DoWrite[[CHARACTERISTICNAME]] (text, [[DEC_OR_HEX]]);
        }

        private async Task DoWrite[[CHARACTERISTICNAME]](string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;
[[DATA1_LIST]]
                if (parseError == null)
                {
                    await bleDevice.Write[[CHARACTERISTICNAME]]([[ARG_LIST]]);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($""Error: could not parse {parseError}"");
                }
            }
            catch (Exception ex)
            {
                SetStatus($""Error: exception: {ex.Message}"");
            }
        }
";
        public static string[] PageCSharp_CharacteristicWrite_DataTemplates = new string[] { @"
                [[VARIABLETYPE]] [[DATANAME]];
                // History: used to go into [[CHARACTERISTICNAME]]_[[DATANAME]].Text instead of using the variable
                // History: used to used [[DEC_OR_HEX]] for parsing instead of the newer dec_or_hex variable that's passed in
                var parsed[[DATANAME]] = Utilities.Parsers.TryParse[[VARIABLETYPE]](text, dec_or_hex, null, out [[DATANAME]]);
                if (!parsed[[DATANAME]])
                {
                    parseError = ""[[DATANAMEUSER]]"";
                }
" };

        public static string PageCSharp_CharacteristicNotifyTemplate = @"
        GattClientCharacteristicConfigurationDescriptorValue[] Notify[[CHARACTERISTICNAME]]Settings = {
[[NOTIFYVALUELIST]]
            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int [[CHARACTERISTICNAME]]NotifyIndex = 0;
        bool [[CHARACTERISTICNAME]]NotifySetup = false;
        private async void OnNotify[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            await DoNotify[[CHARACTERISTICNAME]]();
        }

        private async Task DoNotify[[CHARACTERISTICNAME]]()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (![[CHARACTERISTICNAME]]NotifySetup)
                {
                    [[CHARACTERISTICNAME]]NotifySetup = true;
                    bleDevice.[[CHARACTERISTICNAME]]Event += BleDevice_[[CHARACTERISTICNAME]]Event;
                }
                var notifyType = Notify[[CHARACTERISTICNAME]]Settings[[[CHARACTERISTICNAME]]NotifyIndex];
                [[CHARACTERISTICNAME]]NotifyIndex = ([[CHARACTERISTICNAME]]NotifyIndex + 1) % Notify[[CHARACTERISTICNAME]]Settings.Length;
                var result = await bleDevice.Notify[[CHARACTERISTICNAME]]Async(notifyType);
                [[NOTIFYCONFIGURE]]

[[CHART_SETUP]]
            }
            catch (Exception ex)
            {
                SetStatus($""Error: exception: {ex.Message}"");
            }
        }

        private async void BleDevice_[[CHARACTERISTICNAME]]Event(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                [[NOTIFYCONVERT]]
                var record = new [[CHARACTERISTICNAME]]Record();
[[DATA1_LIST]]
                var addResult = [[CHARACTERISTICNAME]]RecordData.AddRecord(record);
                [[CHART_COMMAND]]
                });
            }
        }";

        public static string PageCSharp_Characteristic_ChartSetup_Template = @"
                var EventTimeProperty = typeof([[CHARACTERISTICNAME]]Record).GetProperty(""EventTime"");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {[[DATA4_LIST]]
                };
                var names = new List<string>()
                {[[DATA5_LIST]]
                };
                [[CHARACTERISTICNAME]]Chart.SetDataProperties(properties, EventTimeProperty, names);
                [[CHARACTERISTICNAME]]Chart.SetTitle(""[[CHARACTERISTICNAMEUSER]] Chart"");
                [[CHARACTERISTICNAME]]Chart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
[[UISPECS]]
;
";

        public static string[] PageCSharp_CharacteristicNotify_DataTemplates = new string[] { @"
                var [[DATANAME]] = valueList.GetValue(""[[DATANAME]]"");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE_DS]])[[DATANAME]].[[AS_DOUBLE_OR_STRING]];
                    [[CHARACTERISTICNAME]]_[[DATANAME]].Text = record.[[DATANAME]].ToString(); // ""N0""); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
" };

    }
}
