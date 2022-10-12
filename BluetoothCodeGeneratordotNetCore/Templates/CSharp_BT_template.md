# Protocol FileName=[[CLASSNAME]].cs

```
//From template: Protocol_Body v2022-07-02 9:54
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using BluetoothDeviceController.Names;


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
[[SERVICE+CHARACTERISTIC+LIST]]

        public async Task<GattCharacteristicsResult> EnsureCharacteristicOne(GattDeviceService service, CharacteristicsEnum characteristicIndex)
        {
            var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[(int)characteristicIndex]);
            Characteristics[(int)characteristicIndex] = null;
            if (characteristicsStatus.Status != GattCommunicationStatus.Success)
            {
                Status.ReportStatus($"unable to get characteristic for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
                return null;
            }
            if (characteristicsStatus.Characteristics.Count == 0)
            {
                Status.ReportStatus($"unable to get any characteristics for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
            }
            else if (characteristicsStatus.Characteristics.Count != 1)
            {
                Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
            }
            else
            {
                Characteristics[(int)characteristicIndex] = characteristicsStatus.Characteristics[0];
            }
            return characteristicsStatus;
        }

        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(CharacteristicsEnum characteristicIndex = CharacteristicsEnum.All_enum, bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;
            if (ble == null) return false; // might not be initialized yet

            if (characteristicIndex != CharacteristicsEnum.All_enum)
            {
                var serviceIndex = MapCharacteristicToService[(int)characteristicIndex];
                var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                if (serviceStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                    return false;
                }
                if (serviceStatus.Services.Count != 1)
                {
                    Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                    return false;
                }
                var service = serviceStatus.Services[0];
                var characteristicsStatus = await EnsureCharacteristicOne(service, characteristicIndex);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    return false;
                }
                return true;
            }

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
                        Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                        continue; //return false;
                    }
                    var service = serviceStatus.Services[0];
                    var characteristicIndexSet = MapServiceToCharacteristic[serviceIndex];
                    foreach (var index in characteristicIndexSet)
                    {
                        var characteristicsStatus = await EnsureCharacteristicOne(service, (CharacteristicsEnum)index);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            return false;
                        }
                        lastResult = characteristicsStatus;
                    }
                }
                // Do not call ReportStatus on OK -- the actual read/write/etc. call will
                // call ReportStatus for them. It's important that for any one actual call
                // (public method) that there's only one ReportStatus.
                //Status.ReportStatus("OK: Connected to device", lastResult);
                readCharacteristics = true;
            }
            return readCharacteristics;
        }


        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="method" ></param>
        /// <param name="command" ></param>
        /// <returns></returns>
        private async Task WriteCommandAsync(CharacteristicsEnum characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[(int)characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
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
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(CharacteristicsEnum characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[(int)characteristicIndex].ReadValueAsync(cacheMode);
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
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);



[[METHOD+LIST]]
    }
}
```

## LINKS Type=list Source=LINKS CodeListZero="        // No links for this device"

```
    // Link: [[TEXT]]
```



## SERVICE+CHARACTERISTIC+LIST

```
        Guid[] ServiceGuids = new Guid[] {
[[SERVICE+GUID+LIST]]
        };
        String[] ServiceNames = new string[] {
[[SERVICE+NAME+LIST]]
        };
        GattDeviceService[] Services = new GattDeviceService[] {
[[SERVICE+LIST]]
        };
        Guid[] CharacteristicGuids = new Guid[] {
[[CHARACTERISTIC+GUID+LIST]]
        };
        String[] CharacteristicNames = new string[] {
[[CHARACTERISTIC+NAME+LIST]]
        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
[[CHARACTERISTIC+LIST]]
        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
[[HASH+LIST]]
        };
        List<int> MapCharacteristicToService = new List<int>() {
[[HASH+LIST+REVERSE]]            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
[[CHARACTERISTIC+ENUM+LIST]]
        };
```

## SERVICE+GUID+LIST Type=list Source=Services Code="           Guid.Parse(\"[[UUID]]\"),"
## SERVICE+NAME+LIST Type=list Source=Services Code="            \"[[Name]]\","
## SERVICE+LIST Type=list Source=Services Code="            null,"
## CHARACTERISTIC+ENUM+LIST Type=list Source=Services/Characteristics CodeListSubZero="            // No characteristics for [[Name]]"
```
            [[Name.dotNet]]_[[../Name.dotNet]]_enum = [[COUNT]],
```
## CHARACTERISTIC+GUID+LIST Type=list Source=Services/Characteristics Code="            Guid.Parse(\"[[UUID]]\"), // #[[Count.Child]] is [[Name]]" CodeListSubZero="            // No characteristics for [[Name]]"
## CHARACTERISTIC+NAME+LIST Type=list Source=Services/Characteristics Code="            \"[[Name]]\", // #[[Count.Child]] is [[UUID]]" CodeListSubZero="            // No characteristics for [[Name]]"
## CHARACTERISTIC+LIST Type=list Source=Services/Characteristics Code="            null," CodeListSubZero="            // No characteristics for [[Name]]"
## HASH+LIST Type=list Source=Services/Characteristics Code="[[COUNT]], " Trim=true CodeWrap="            new HashSet<int>(){ [[TEXT]] }," CodeListSubZero="            // No characteristics for [[Name]]"
## HASH+LIST+REVERSE Type=list Source=Services/Characteristics Code="            [[../COUNT]], // Characteristic [[COUNT]]" CodeListSubZero="            // No characteristics for [[Name]]"


## SET+PROPERTY+VALUES Type=list Source=Services/Characteristics/Properties ListOutput=parent CodeListZero="            // No properties for this characteristic" CodeListSubZero="            // No properties for this characteristic"

```
            [[CHDATANAME]] = parseResult.ValueList.GetValue("[[DATANAME]]").[[AS+DOUBLE+OR+STRING]];
```



## METHOD+NOTIFY If="[[Verbs]] contains :InNo:" Type=list ListOutput=child Source=Services/Characteristics CodeListSubZero=""

```
        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; [[Name.dotNet]]Event += _my function_
        /// </summary>
        public event BluetoothDataEvent [[Name.dotNet]]Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>
        
        private bool Notify[[Name.dotNet]]_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> Notify[[Name.dotNet]]Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!Notify[[Name.dotNet]]_ValueChanged_Set)
                {
                    // Only set the event callback once
                    Notify[[Name.dotNet]]_ValueChanged_Set = true;
                    ch.ValueChanged += Notify[[Name.dotNet]]Callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"Notify[[Name.dotNet]]: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"Notify[[Name.dotNet]]: set notification", result);

            return true;
        }

        private void Notify[[Name.dotNet]]Callback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "[[CHARACTERISTICTYPE]]";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
[[SET+PROPERTY+VALUES]]
            [[Name.dotNet]]Event?.Invoke(parseResult);

        }

        public void Notify[[Name.dotNet]]RemoveCharacteristicCallback() 
        {
            var ch = Characteristics[CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum];
            if (ch == null) return;
            Notify[[Name.dotNet]]_ValueChanged_Set = false;
            ch.ValueChanged -= Notify[[Name.dotNet]]Callback;
        }


```

## METHOD+PROPERTY Type=list If="[[Verbs]] contains :RdInNo:" Type=list ListOutput=parent Source=Services/Characteristics/Properties CodeListSubZero=""

```
        private [[VARIABLETYPE+DS]] _[[CHDATANAME]] = [[DOUBLE+OR+STRING+DEFAULT]];
        private bool _[[CHDATANAME]]_set = false;
        public [[VARIABLETYPE+DS]] [[CHDATANAME]]
        {
            get { return _[[CHDATANAME]]; }
            internal set { if (_[[CHDATANAME]]_set && value == _[[CHDATANAME]]) return; _[[CHDATANAME]] = value; _[[CHDATANAME]]_set = true; OnPropertyChanged(); }
        }
```

## METHOD+READ If="[[Verbs]] contains :Read:" Type=list ListOutput=child Source=Services/Characteristics CodeListSubZero=""

Replace the simple Reads Data comment with this better snippet.
 TODO:         /// Reads data for Characteristic=[[Name]] Service=[[../Name]] 

```
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> Read[[Name.dotNet]](BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum, "[[Name.dotNet]]", cacheMode);
            if (result == null) return null;

            var datameaning = "[[Type]]";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET+PROPERTY+VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }
```

## WRITE+PARAMS If="[[Verbs]] contains :WrWw:" Type=list ListOutput=parent Source=Services/Characteristics/WriteParams CodeListSubZero="" CodeListSeparator=", " Trim=true 
```
[[VARIABLETYPEPARAM]] [[DATANAME]]
```

The parameter list for writing data to the device.

## METHOD+WRITE If="[[Verbs]] contains :WrWw:" Type=list ListOutput=child Source=Services/Characteristics CodeListSubZero=""
```
        /// <summary>
        /// Writes data for [[Name.dotNet]]
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task Write[[Name.dotNet]]([[WRITE+PARAMS]])
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
[[DATAWRITER]]
            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum, "[[Name.dotNet]]", command, [[WRITEMODE]]);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum, "[[Name.dotNet]]", subcommand, [[WRITEMODE]]);
            }
        }
```

## DATAWRITER If="[[Verbs]] contains :WrWw:" Type=list ListOutput=parent Source=Services/Characteristics/WriteParams CodeListSubZero="" Trim=false
```
            [[ARGDWCALL]]( [[ARGDWCALLCAST]] [[DATANAME]]);
```

## FUNCTION+ENUM+INITS Type=list ListOutput=parent Source=Services/Characteristics/Commands/Parameters/ValueNames CodeListSubZero=""
```
            [[NAME]] = [[TEXT]],
```

## FUNCTION+ENUMS Type=list ListOutput=parent Source=Services/Characteristics/Commands/Parameters CodeListSubZero="" CodeListZero="//No enums" If="[[FUNCTION+ENUM+INITS]] length> 0"
```
        public enum Command_[[FUNCTIONNAME]]_[[Name.dotNet]]
        {
[[FUNCTION+ENUM+INITS]]
        }
```

## FUNCTION+PARAMS Type=list ListOutput=parent Source=Services/Characteristics/Commands/Parameters Trim=true CodeListSubZero="" CodeListSeparator=", "
```
[[VARIABLETYPE]] [[Name.dotNet]]
```

## FUNCTION+ADDVARIABLES Type=list ListOutput=parent Source=Services/Characteristics/Commands/Parameters CodeListSubZero=""
```
                command.Parameters.Add("[[Name.dotNet]]",
                    new VariableDescription()
                    {
                        Init = [[FUNCTIONPARAMINIT]],
                    });
```

## FUNCTION+SETVARIABLES Type=list ListOutput=parent Source=Services/Characteristics/Commands/Parameters CodeListSubZero=""
```
            command.Parameters["[[Name.dotNet]]"].CurrValue = (double)[[Name.dotNet]];
```


## METHOD+COMMANDS Type=list ListOutput=parent Source=Services/Characteristics/Commands CodeListZero="//No commands" CodeListSubZero=""
```
        //From template:Protocol_FunctionTemplate
[[FUNCTION+ENUMS]]
        Command command_[[../Name.dotNet]]_[[FUNCTIONNAME]] = null;
        public Command [[../Name.dotNet]]_[[FUNCTIONNAME]]_Init()
        {
            if (command_[[../Name.dotNet]]_[[FUNCTIONNAME]] == null)
            {
                var command = new Command();
[[FUNCTION+ADDVARIABLES]]
                command.InitVariables();
                command.Compute = "[[Compute]]";
                command_[[../Name.dotNet]]_[[FUNCTIONNAME]] = command;
            }
            return command_[[../Name.dotNet]]_[[FUNCTIONNAME]];
        }
        public async Task [[../Name.dotNet]]_[[FUNCTIONNAME]]([[FUNCTION+PARAMS]])
        {
            var command = [[../Name.dotNet]]_[[FUNCTIONNAME]]_Init();
[[FUNCTION+SETVARIABLES]]
            var computed_string = command.DoCompute();
            await Write[[../Name.dotNet]](computed_string);
        }
```
## METHOD+LIST Type=list Source=Services/Characteristics CodeListSubZero="// No methods for [[Name]]"

In my **TODO:** list
- Each characteristic might have read, write and notify; these have to be conditionally applied
- How should I handle the argument lists?

```
[[METHOD+PROPERTY]]
[[METHOD+READ]]
[[METHOD+NOTIFY]]
[[METHOD+WRITE]]
[[METHOD+COMMANDS]]

```



## ZZZ_MOVED_Protocol+DataPropertySetTemplate
```
            [[CHDATANAME]] = parseResult.ValueList.GetValue("[[DATANAME]]").[[AS+DOUBLE+OR+STRING]];
```





## ZZZ+Protocol+Function+AddVariableTemplate
```
                command.Parameters.Add("[[FUNCTIONPARAMNAME]]",
                    new VariableDescription()
                    {
                        Init = [[FUNCTIONPARAMINIT]],
                    });
```

## ZZZ+Protocol+Function+SetVariableTemplate
```
            command.Parameters["[[FUNCTIONPARAMNAME]]"].CurrValue = (double)[[FUNCTIONPARAMNAME]];";
```
## ZZZ+OLD+Protocol+Function+Enum
```
        public enum [[CHARACTERISTICNAME]]_[[FUNCTIONNAME]]_[[FUNCTIONPARAMNAME]]
        {
[[FUNCTION+ENUM+INITS]]
        }
```
// FUNCTION_ENUM_INITS is e.g. "            Left = 0,"


# PageXaml
## PageXaml+BodyTemplate
```
<Page
    x:Class="BluetoothDeviceController.SpecialtyPages.[[CLASSNAME]]Page"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:BluetoothDeviceController.SpecialtyPages"
    xmlns:controls="using:Microsoft.Toolkit.Uwp.UI.Controls"
    xmlns:charts="using:BluetoothDeviceController.Charts"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <Page.Resources>
        <Style TargetType="Button">
            <Setter Property="MinWidth" Value="60" />
            <Setter Property="VerticalAlignment" Value="Bottom" />
            <Setter Property="FontFamily" Value="Segoe UI,Segoe MDL2 Assets" />
            <Setter Property="Margin" Value="10,5,0,0" />
        </Style>
        <Style TargetType="Slider">
            <Setter Property="MinWidth" Value="200" />
            <Setter Property="FontFamily" Value="Segoe UI,Segoe MDL2 Assets" />
            <Setter Property="Margin" Value="10,5,10,0" />
        </Style>
        <Style TargetType="Line">
            <Setter Property="Margin" Value="0,15,0,0" />
            <Setter Property="Stroke" Value="ForestGreen" />
        </Style>
        <Style x:Key="TitleStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="30" />
        </Style>
        <Style x:Key="HeaderStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="20" />
        </Style>
        <Style x:Key="HeaderStyleExpander" TargetType="controls:Expander">
            <Setter Property="MinWidth" Value="550" />
            <Setter Property="HorizontalAlignment" Value="Left" />
            <Setter Property="HorizontalContentAlignment" Value="Left" />
        </Style>
        <Style x:Key="SubheaderStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="16" />
        </Style>
        <Style x:Key="AboutStyle" TargetType="TextBlock">
            <Setter Property="FontSize" Value="12" />
            <Setter Property="TextWrapping" Value="Wrap" />
        </Style>
        <Style x:Key="ChacteristicListStyle" TargetType="StackPanel">
            <Setter Property="Background" Value="WhiteSmoke" />
            <Setter Property="Margin" Value="18,0,0,0" />
        </Style>
        <Style x:Key="HEXStyle" TargetType="TextBox">
            <Setter Property="MinWidth" Value="90" />
            <Setter Property="FontSize" Value="12" />
            <Setter Property="Margin" Value="5,0,0,0" />
        </Style>
        <Style x:Key="TableStyle" TargetType="controls:DataGrid">
            <Setter Property = "Background" Value="BlanchedAlmond" />
            <Setter Property = "FontSize" Value="12" />
            <Setter Property = "Height" Value="200" />
            <Setter Property = "HorizontalAlignment" Value="Center" />
            <Setter Property = "Width" Value="500" />
        </Style>
    </Page.Resources>
    
    <StackPanel>
        <Grid MaxWidth="550" HorizontalAlignment="Left">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="auto" />
            </Grid.ColumnDefinitions>
            <StackPanel Grid.Column="0">
                <TextBlock Style="{StaticResource TitleStyle}">[[DEVICENAMEUSER]] device</TextBlock>
                <TextBlock Style="{StaticResource AboutStyle}">
                    [[DESCRIPTION]]
                </TextBlock>
            </StackPanel>
            <controls:ImageEx Grid.Column="1" Style="{StaticResource ImageStyle}"  Source="/Assets/DevicePictures/[[CLASSNAME]].PNG" />
        </Grid>
        <ProgressRing x:Name="uiProgress" />
        <TextBlock x:Name="uiStatus" />
[[SERVICE+LIST]]
        <Button Content="REREAD" Click="OnRereadDevice" />
    </StackPanel>
</Page>
```

## PageXaml+ServiceTemplate
```
        <controls:Expander Header="[[SERVICENAMEUSER]]" IsExpanded="[[SERVICEISEXPANDED]]" Style="{StaticResource HeaderStyleExpander}">
            <StackPanel Style="{StaticResource ChacteristicListStyle}">
[[CHARACTERISTIC+LIST]]
            </StackPanel>
        </controls:Expander>
```

## PageXaml+CharacteristicTemplate
```
                <TextBlock Style="{StaticResource SubheaderStyle}">[[CHARACTERISTICNAMEUSER]]</TextBlock>
                <StackPanel Orientation="Horizontal">
[[DATA1+LIST]]
[[READWRITE+BUTTON+LIST]]
                </StackPanel>
[[ENUM+BUTTON+LIST+PANEL]]
[[FUNCTIONUI+LIST+PANEL]]
[[TABLE]]
```

## PageXamlCharacteristicDataTemplate
```
                    <TextBox IsReadOnly="[[IS+READ+ONLY]]" x:Name="[[CHARACTERISTICNAME]]_[[DATANAME]]" Text="*" Header="[[DATANAMEUSER]]" Style="{StaticResource HEXStyle}"/>
```

## PageXamlCharacteristicEnumButtonTemplate
```
                    <Button Content="[[ENUM+NAME]]" Tag="[[ENUM+VALUE]]" Click="OnClick[[CHARACTERISTICNAME]]" />
```
        
## PageXamlCharacteristicEnumButtonPanelTemplate
```
                <VariableSizedWrapGrid Orientation="Horizontal" MaximumRowsOrColumns="[[MAXCOLUMNS]]">
[[ENUM+BUTTON+LIST]]                </VariableSizedWrapGrid>	
```

## PageXamlCharacteristicButtonTemplate
```
                    <Button Content="[[BUTTONTYPE]]" Click="On[[BUTTONTYPE]][[CHARACTERISTICNAME]]" />
```

## PageXamlCharacteristicTableTemplate
```
                    <controls:Expander Header="[[CHARACTERISTICNAMEUSER]] Data tracker" IsExpanded="false" MinWidth="550" HorizontalAlignment="Left">
                        <StackPanel MinWidth="550">
                            [[XAMLCHART]]
                            <controls:DataGrid Style="{StaticResource TableStyle}" x:Name="[[CHARACTERISTICNAME]]Grid" ItemsSource="{Binding [[CHARACTERISTICNAME]]RecordData}" />
                            <TextBox  x:Name="[[CHARACTERISTICNAME]]_Notebox" KeyDown="On[[CHARACTERISTICNAME]]_NoteKeyDown" />
                            <StackPanel Orientation="Horizontal" HorizontalAlignment="Left">
                                <ComboBox SelectionChanged="OnKeepCount[[CHARACTERISTICNAME]]" Header="Keep how many items?" SelectedIndex="2">
                                    <ComboBoxItem Tag="10">10</ComboBoxItem>
                                    <ComboBoxItem Tag="100">100</ComboBoxItem>
                                    <ComboBoxItem Tag="1000">1,000</ComboBoxItem>
                                    <ComboBoxItem Tag="10000">10K</ComboBoxItem>
                                </ComboBox>
                                <Rectangle Width="5" />
                                <ComboBox SelectionChanged="OnAlgorithm[[CHARACTERISTICNAME]]" Header="Remove algorithm?" SelectedIndex="0">
                                    <ComboBoxItem Tag="1" ToolTipService.ToolTip="Keep a random sample of data">Keep random sample</ComboBoxItem>
                                    <ComboBoxItem Tag="0" ToolTipService.ToolTip="Keep the most recent data">Keep latest data</ComboBoxItem>
                                </ComboBox>
                                <Button Content = "Copy" Click="OnCopy[[CHARACTERISTICNAME]]" />
                            </StackPanel>
                        </StackPanel>
                    </controls:Expander>
```


## PageXamlFunctionUIListPanelTemplate 
```
[[TAB]]<StackPanel>
[[FUNCTIONUILIST]]
[[TAB]]</StackPanel>
```

## PageXamlFunctionComboBoxTemplate 
```
[[TAB]]<ComboBox Header="[[LABEL]]" [[COMBOINIT]] MinWidth="140" SelectionChanged="[[COMMAND]]_[[PARAM]]_ComboBoxChanged">
[[COMBOBOXLIST]]
[[TAB]]</ComboBox>
```


## PageXamlFunctionButtonTemplate 
```
[[TAB]]<Button Content="[[LABEL]]" Click="[[FUNCTIONNAME]]_ButtonClick" />
```
## PageXamlFunctionSliderTemplate
```
[[TAB]]<Slider Header="[[LABEL]]" Value="[[INIT]]" Minimum="[[MIN]]" Maximum="[[MAX]]" ValueChanged="[[COMMAND]]_[[PARAM]]_SliderChanged" />
```

# PageCSharp
## PageCSharp+BodyTemplate
```
using BluetoothDeviceController.Charts;
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
        private string DeviceName = "[[CLASSNAME]]";
        private string DeviceNameUser = "[[DEVICENAMEUSER]]";

        int ncommand = 0;
        [[CLASSNAME]] bleDevice = new [[CLASSNAME]]();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive (true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive (false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice+OnBluetoothStatus;
[[DOREADDEVICE_NAME]]
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
[[SERVICE+LIST]]
        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(CharacteristicsEnum.All_enum, true);
            SetStatusActive(false);
        }
    }
}
```

## PageCSharp+ServiceTemplate
```
// Functions for [[SERVICENAMEUSER]]

[[CHARACTERISTIC+LIST]]
```

## PageCSharp+CharacteristicRecordTemplate
```
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
[[DATA1+LIST]]
            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }
    
    public DataCollection<[[CHARACTERISTICNAME]]Record> [[CHARACTERISTICNAME]]RecordData { get; } = new DataCollection<[[CHARACTERISTICNAME]]Record>();
    private void On[[CHARACTERISTICNAME]]_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
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

        [[CHART+UPDATE+COMMAND]]
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
        sb.Append("EventDate,EventTime[[DATA2+LIST]],Notes\n");
        foreach (var row in [[CHARACTERISTICNAME]]RecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24}[[DATA3+LIST]],{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }
```

## PageCSharp+CharacteristicRecord+DataTemplates

```
            private [[VARIABLETYPE+DS]] _[[DATANAME]];
            public [[VARIABLETYPE+DS]] [[DATANAME]] { get { return _[[DATANAME]]; } set { if (value == _[[DATANAME]]) return; _[[DATANAME]] = value; OnPropertyChanged(); } }
``` 
", // DATA1+LIST
```TEST
,[[DATANAME]]
```
", // DATA2+LIST

```TEST
,{row.[[DATANAME]]}
```
", // DATA3+LIST

```TEST
typeof([[CHARACTERISTICNAME]]Record).GetProperty("[[DATANAME]]"),
```
", // DATA4+LIST
```TEST
"[[DATANAME]]",
```
", // DATA5+LIST



## PageCSharp+CharacteristicReadTemplate
```
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
                    SetStatus ($"Error: unable to read [[CHARACTERISTICNAME]]");
                    return;
                }
                [[READCONVERT]]
                var record = new [[CHARACTERISTICNAME]]Record();
[[DATA1+LIST]]

                [[CHARACTERISTICNAME]]RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }
```

LIST!!
## PageCSharp+CharacteristicRead+DataTemplates
```
                var [[DATANAME]] = valueList.GetValue("[[DATANAME]]");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString || [[DATANAME]].IsArray)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE+DS]])[[DATANAME]].[[AS+DOUBLE+OR+STRING]];
                    [[CHARACTERISTICNAME]]_[[DATANAME]].Text = record.[[DATANAME]].ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
```

## PageCSharp+CharacteristicWriteTemplate
```
        // OK to include this method even if there are no defined buttons
        private async void OnClick[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWrite[[CHARACTERISTICNAME]] (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWrite[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            var text = [[CHARACTERISTICNAME]]_[[DATANAME]].Text;
            await DoWrite[[CHARACTERISTICNAME]] (text, [[DEC+OR+HEX]]);
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
[[DATA1+LIST]]
                if (parseError == null)
                {
                    await bleDevice.Write[[CHARACTERISTICNAME]]([[ARG+LIST]]);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }
```

LIST!!!
## PageCSharp+CharacteristicWrite+DataTemplates
```
                [[VARIABLETYPE]] [[DATANAME]];
                // History: used to go into [[CHARACTERISTICNAME]]_[[DATANAME]].Text instead of using the variable
                // History: used to used [[DEC+OR+HEX]] for parsing instead of the newer dec_or_hex variable that's passed in
                var parsed[[DATANAME]] = Utilities.Parsers.TryParse[[VARIABLETYPE]](text, dec_or_hex, null, out [[DATANAME]]);
                if (!parsed[[DATANAME]])
                {
                    parseError = "[[DATANAMEUSER]]";
                }
```

## PageCSharp+CharacteristicNotifyTemplate
```
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

[[CHART+SETUP]]
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
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
[[DATA1+LIST]]
                var addResult = [[CHARACTERISTICNAME]]RecordData.AddRecord(record);
                [[CHART+COMMAND]]
                });
            }
        }
```

## PageCSharp+Characteristic+ButtonClick
```
        private async void [[FUNCTIONNAME]]_ButtonClick(object sender, RoutedEventArgs e)
        {
            var commandWrite = bleDevice.[[CHARACTERISTICNAME]]_[[COMMAND]]_Init();
[[SETLIST]]
            var commandString = commandWrite.DoCompute();
            await bleDevice.Write[[CHARACTERISTICNAME]](commandString);
        }
```

## PageCSharp+Characteristic+ComboChange
```
        private [[ASYNC]]void [[COMMAND]]_[[PARAM]]_ComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            var commandSet = bleDevice.[[CHARACTERISTICNAME]]_[[COMMAND]]_Init();
            if (e.AddedItems.Count == 1
                && (double.TryParse((sender as FrameworkElement).Tag as string, out var value)))
            {
                commandSet.SetCurrDouble("[[PARAM]]", value);
            }
[[VALUECHANGE+COMPUTETARGET]]
        }
```

## PageCSharp+Characteristic+SetValue 

```
            commandWrite.Parameters["[[PARAMETER]]"].CurrValue = [[VALUE]]; // same as commandWrite.Parameters["[[PARAMETER]]"].ValueNames["[[VALUENAME]]"];
```

## PageCSharp+Characteristic+RadioChange
```
        private void [[COMMAND]]_[[PARAM]]_RadioCheck(object sender, RoutedEventArgs e)
        {
            var commandSet = bleDevice.[[CHARACTERISTICNAME]]_[[COMMAND]]_Init();
            if (double.TryParse((sender as FrameworkElement).Tag as string, out var value))
            {
                commandSet.SetCurrDouble("[[PARAM]]", value);
            }
[[VALUECHANGE+COMPUTETARGET]]
        }
```

## PageCSharp+Characteristic+SliderChange
```
        private [[ASYNC]]void [[COMMAND]]_[[PARAM]]_SliderChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var commandSet = bleDevice.[[CHARACTERISTICNAME]]_[[COMMAND]]_Init();
            commandSet.SetCurrDouble("[[PARAM]]", e.NewValue);
[[VALUECHANGE+COMPUTETARGET]]
        }
```

## PageCSharp+Characteristic+ValueChangeComputeTarget
```
            // computedTarget might be different from the computed value
            var commandWrite = bleDevice.[[CHARACTERISTICNAME]]_[[TARGETCOMMAND]]_Init();
            var commandString = commandWrite.DoCompute();
            await bleDevice.Write[[CHARACTERISTICNAME]](commandString);
```

## PageCSharp+Characteristic+ChartSetup+Template
```
                var EventTimeProperty = typeof([[CHARACTERISTICNAME]]Record).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {[[DATA4+LIST]]
                };
                var names = new List<string>()
                {[[DATA5+LIST]]
                };
                [[CHARACTERISTICNAME]]Chart.SetDataProperties(properties, EventTimeProperty, names);
                [[CHARACTERISTICNAME]]Chart.SetTitle("[[CHARACTERISTICNAMEUSER]] Chart");
                [[CHARACTERISTICNAME]]Chart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
[[UISPECS]]
;
```

## PageCSharp+CharacteristicNotify+DataTemplates
```
                var [[DATANAME]] = valueList.GetValue("[[DATANAME]]");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString || [[DATANAME]].IsArray)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE+DS]])[[DATANAME]].[[AS+DOUBLE+OR+STRING]];
                    [[CHARACTERISTICNAME]]+[[DATANAME]].Text = record.[[DATANAME]].ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
```
