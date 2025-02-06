# PageCSharp FileName=[[CLASSNAME]]Page.xaml.cs DirName=SpecialtyPages SuppressFile=:SuppressXAML:


```
using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Lamps;
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
using static BluetoothProtocols.[[CLASSNAME]];

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[DeviceName]] device
    /// </summary>
    public sealed partial class [[CLASSNAME]]Page : Page, HasId, ISetHandleStatus
    {
        public [[CLASSNAME]]Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
[[EXTRAUI+XAML+CS+INIT]]
        }
        private string DeviceName = "[[CLASSNAME]]";
        private string DeviceNameUser = "[[DeviceName]]";

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
[[DOREADDEVICE+NAME]]
[[DOAUTONOTIFY]]
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


[[CS+SERVICE+LIST]]
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

## DOREADDEVICE+NAME If="[[HasReadDeviceName]] == true" Else="            await Task.Delay(0); // No Device_Name to read, but still need to have an async operation."

```
            await DoReadDevice_Name();
```

## DOAUTONOTIFY If="[[AutoNotify]] contains true" Type=list Source=ServicesByPriority/Characteristics CodeListSubZero="" CodeListZero=""

```
            await DoNotify[[CharacteristicName.dotNet]]();
```

## CS+CHARACTERISTIC+DATA+PROPERTY Type=list Source=ServicesByOriginalOrder/Characteristics/Properties ListOutput=parent
 Was the DATA1_LIST a.k.a. DATA1+LIST

```
            private [[VARIABLETYPE+DS]] _[[DATANAME]];
            public [[VARIABLETYPE+DS]] [[DATANAME]] { get { return _[[DATANAME]]; } set { if (value == _[[DATANAME]]) return; _[[DATANAME]] = value; OnPropertyChanged(); } }
```

## CS+CHARACTERISTIC+DATA+LIST Type=list Source=ServicesByOriginalOrder/Characteristics/Properties ListOutput=parent Trim=true

Was the DATA2_LIST a.k.a. the DATA2+LIST

```
,[[DATANAME]]
```

## CS+CHARACTERISTIC+DATA+VALUE+LIST Type=list Source=ServicesByOriginalOrder/Characteristics/Properties ListOutput=parent Trim=true
Was the DATA3_LIST a.k.a. the DATA3+LIST

```
,{row.[[DATANAME]]}
```




## CS+UPDATE+DATA+VALUE Type=list Source=ServicesByOriginalOrder/Characteristics/Properties ListOutput=parent

Was PageCSharp+CharacteristicNotify+DataTemplates

```
                var [[DATANAME]] = valueList.GetValue("[[DATANAME]]");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString || [[DATANAME]].IsArray)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE+DS]])[[DATANAME]].[[AS+DOUBLE+OR+STRING]];
                    [[CharacteristicName.dotNet]]_[[DATANAME]].Text = record.[[DATANAME]].[[DataToString.dotNet]];
                }
```

## CS+READ+FROM+VALUELIST Type=list Source=ServicesByOriginalOrder/Characteristics/Properties ListOutput=parent

Was DATA1_LIST a.k.a. DATA1+LIST
PageCSharp+CharacteristicRead+DataTemplates

```
                var [[DATANAME]] = valueList.GetValue("[[DATANAME]]");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString || [[DATANAME]].IsArray)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE+DS]])[[DATANAME]].[[AS+DOUBLE+OR+STRING]];
                    [[CharacteristicName.dotNet]]_[[DATANAME]].Text = record.[[DATANAME]].[[DataToString.dotNet]];
                }
```

## NOTIFYVALUELIST If="[[Verbs]] contains :InNo:" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child

```
            GattClientCharacteristicConfigurationDescriptorValue.Notify,
```

## CS+CHART+REDRAW+VERB+ALT If="[[UICHARTCOMMAND]] contains AddLineYTime" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child Trim=true

```
RedrawLineYTime
```

## CS+CHART+REDRAW+VERB+REGULAR If="[[UICHARTCOMMAND]] !contains AddLineYTime" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child Trim=true

```
RedrawYTime
```

## CS+CHART+REDRAW If="[[TableType]] contains standard" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child

The original code was a little bit more complex and also allowed (for the BBC MicroBit) this alternative:
[[CharacteristicName.dotNet]]Chart.RedrawLineYTime<[[CharacteristicName.dotNet]]Record>([[CharacteristicName.dotNet]]RecordData);
Note that the alternative calls RedrawLineYTime instead of RedrawYTime

TODO: Did the changes but no tests yet: This is based on the formula of whether or not the chartCommand includes the word 'AddLineYTime'.


```
[[CharacteristicName.dotNet]]Chart.[[CS+CHART+REDRAW+VERB+REGULAR]][[CS+CHART+REDRAW+VERB+ALT]]<[[CharacteristicName.dotNet]]Record>([[CharacteristicName.dotNet]]RecordData);
```

## CS+CHART+SETUP+PROPERTYLIST If="[[TableType]] contains standard" Type=list Source=ServicesByOriginalOrder/Characteristics/Properties ListOutput=parent

```
                    typeof([[CharacteristicName.dotNet]]Record).GetProperty("[[DATANAME]]"),
```

## CS+CHART+SETUP+NAMELIST If="[[TableType]] contains standard" Type=list Source=ServicesByOriginalOrder/Characteristics/Properties ListOutput=parent Trim=true

This was originally DATA5+LIST DATA5_LIST

```
"[[DATANAME]]",
```

## CS+CHART+SETUP If="[[TableType]] contains standard" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child

here!here TODO: Chart.SetDataProperties

```
                var EventTimeProperty = typeof([[CharacteristicName.dotNet]]Record).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
[[CS+CHART+SETUP+PROPERTYLIST]]
                };
                var propertiesWithEventTime = new System.Reflection.PropertyInfo>[]
                {
                    typeof(DataRecord).GetProperty("EventTime"),
[[CS+CHART+SETUP+PROPERTYLIST]]
                };
                var names = new List<string>()
                {[[CS+CHART+SETUP+NAMELIST]]
                };
                [[CharacteristicName.dotNet]]RecordData.TProperties = propertiesWithEventTime;
                [[CharacteristicName.dotNet]]Chart.SetDataProperties(properties, EventTimeProperty, names);
                [[CharacteristicName.dotNet]]Chart.SetTitle("[[CharacteristicName]] Chart");
                [[CharacteristicName.dotNet]]Chart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
[[UISPECS]]
;
```

## CS+CHART+UICHARTCOMMAND If="[[TableType]] contains standard" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child

```
[[CharacteristicName.dotNet]]Chart.[[UICHARTCOMMAND]];
```

## CS+CHARACTERISTIC+NOTIFY+METHOD If="[[Verbs]] contains :InNo:" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child

PageCSharp+CharacteristicNotifyTemplate
Will be like CS+CHARACTERISTIC+READ+METHOD

```
        GattClientCharacteristicConfigurationDescriptorValue[] Notify[[CharacteristicName.dotNet]]Settings = {
[[NOTIFYVALUELIST]]
            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int [[CharacteristicName.dotNet]]NotifyIndex = 0;
        bool [[CharacteristicName.dotNet]]NotifySetup = false;
        private async void OnNotify[[CharacteristicName.dotNet]](object sender, RoutedEventArgs e)
        {
            await DoNotify[[CharacteristicName.dotNet]]();
        }

        private async Task DoNotify[[CharacteristicName.dotNet]]()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (![[CharacteristicName.dotNet]]NotifySetup)
                {
                    [[CharacteristicName.dotNet]]NotifySetup = true;
                    bleDevice.[[CharacteristicName.dotNet]]Event += BleDevice_[[CharacteristicName.dotNet]]Event;
                }
                var notifyType = Notify[[CharacteristicName.dotNet]]Settings[[[CharacteristicName.dotNet]]NotifyIndex];
                [[CharacteristicName.dotNet]]NotifyIndex = ([[CharacteristicName.dotNet]]NotifyIndex + 1) % Notify[[CharacteristicName.dotNet]]Settings.Length;
                var result = await bleDevice.Notify[[CharacteristicName.dotNet]]Async(notifyType);
                [[NOTIFYCONFIGURE]]

[[CS+CHART+SETUP]]
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_[[CharacteristicName.dotNet]]Event(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                [[NOTIFYCONVERT]]
                var record = new [[CharacteristicName.dotNet]]Record();
[[CS+READ+FROM+VALUELIST]]
                var addResult = [[CharacteristicName.dotNet]]RecordData.AddRecord(record);
                
                [[CS+CHART+UICHARTCOMMAND]]
                // Original update was to make this CHART+COMMAND
                });
            }
        }
```



## CS+CHARACTERISTIC+READ+METHOD If="[[Verbs]] contains :Read:" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child

PageCSharp+CharacteristicReadTemplate
        // This is CS+CHARACTERISTIC+READ+METHOD for [[CharacteristicName]] for [[ServiceName]]

```
        private async void OnRead[[CharacteristicName.dotNet]](object sender, RoutedEventArgs e)
        {
            await DoRead[[CharacteristicName.dotNet]]();
        }

        private async Task DoRead[[CharacteristicName.dotNet]]()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.Read[[CharacteristicName.dotNet]]();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read [[CharacteristicName.dotNet]]");
                    return;
                }
                [[READCONVERT]]
                var record = new [[CharacteristicName.dotNet]]Record();
[[CS+UPDATE+DATA+VALUE]]
                [[CharacteristicName.dotNet]]RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }
```

## CS+CHARACTERISTIC+WRITE+ARGS+SETUP If="[[Verbs]] contains :WrWw:" Type=list Source=ServicesByOriginalOrder/Characteristics/WriteProperties ListOutput=parent CodeListSubZero="" CodeListZero="" 

This was DATA1+LIST DATA1_LIST

```
                [[VARIABLETYPE]] [[DATANAME]];
                // History: used to go into [[CharacteristicName.dotNet]]_[[DATANAME]].Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsed[[DATANAME]] = Utilities.Parsers.TryParse[[VARIABLETYPE]](values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out [[DATANAME]]);
                valueIndex++; // Change #5
                if (!parsed[[DATANAME]])
                {
                    parseError = "[[DATANAMEUSER]]";
                }
```

## CS+CHARACTERISTIC+WRITE+ARGS If="[[Verbs]] contains :WrWw:" Type=list Source=ServicesByOriginalOrder/Characteristics/WriteProperties ListOutput=parent CodeListSubZero="" CodeListZero="" CodeListSeparator=", " Trim=true

```
[[DATANAME]]
```

## CS+CHARACTERISTIC+WRITE+ARGS+XAML+LIST+SETUP If="[[Verbs]] contains :WrWw:" Type=list ListOutput=parent Source=ServicesByOriginalOrder/Characteristics/WriteProperties CodeListSubZero="" CodeListZero=""

```
				new UxTextValue([[CharacteristicName.dotNet]]_[[DataName.dotNet]].Text, [[DEC+OR+HEX]]),
```


## CS+CHARACTERISTIC+WRITE+METHOD If="[[Verbs]] contains :WrWw:" Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=child CodeListSubZero="" CodeListZero=""

PageCSharp+CharacteristicWriteTemplate

```
        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClick[[CharacteristicName.dotNet]](object sender, RoutedEventArgs e)
        {
			var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue([[DataName.First.dotNetSafe]].Text, [[DEC+OR+HEX]]),
[[CS+CHARACTERISTIC+WRITE+ARGS+XAML+LIST+SETUP]]
            };
            //var text = (sender as Button).Tag as String;
            await DoWrite[[CharacteristicName.dotNet]](values);

        }

        private async void OnWrite[[CharacteristicName.dotNet]](object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue([[DataName.First.dotNetSafe]].Text, [[DEC+OR+HEX]]),
[[CS+CHARACTERISTIC+WRITE+ARGS+XAML+LIST+SETUP]]
            };
            await DoWrite[[CharacteristicName.dotNet]](values);
			
        }

        private async Task DoWrite[[CharacteristicName.dotNet]](List<UxTextValue> values)
        {
            if (values.Count != [[WRITE+NARGS]]) return;
            int valueIndex = 0; // Change #3;
			
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

[[CS+CHARACTERISTIC+WRITE+ARGS+SETUP]]
                if (parseError == null)
                {
                    await bleDevice.Write[[CharacteristicName.dotNet]]([[CS+CHARACTERISTIC+WRITE+ARGS]]);
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


## CS+XAML+UILIST+LIST+CONTENT+VALUECHANGE+COMPUTETARGET If="[[ComputeTarget]] length> 0" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=child
```
            // computedTarget might be different from the computed value
            var commandWrite = bleDevice.[[CharacteristicName.dotNet]]_[[ComputeTarget]]_Init();
            var commandString = commandWrite.DoCompute();
            await bleDevice.Write[[CharacteristicName.dotNet]](commandString);
```

## ASYNC If="[[CS+XAML+UILIST+LIST+CONTENT+VALUECHANGE+COMPUTETARGET]] length> 10" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=child Trim=true
```
async
```


## CS+XAML+UILIST+LIST+CONTENT+BUTTON+SETLIST If="[[Set0]] length> 5" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=child Trim=true

The original concept of the "Set" field was that it would be a list of strings, where each string was a space-seperated
list of what to set. But that is actually overkill; nothing in the Elegoo ever used more than one. So the Set list
(technically an array) is flattened and just shows up as the Set0_Paremeter Set0_Value etc.

```
            commandWrite.Parameters["[[Set0_Parameter]]"].CurrValue = [[Set0_Value]]; // same as commandWrite.Parameters["[[Set0_Parameter]]"].ValueNames["[[Set0_ValueName]]"];
```

## CS+XAML+UILIST+LIST+CONTENT+BUTTON If="[[UIType]] contains ButtonFor" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=child


PageCSharp+Characteristic+ButtonClick


```
        private async void [[FunctionName]]_ButtonClick(object sender, RoutedEventArgs e)
        {
            var commandWrite = bleDevice.[[CharacteristicName.dotNet]]_[[Target0]]_Init();
[[CS+XAML+UILIST+LIST+CONTENT+BUTTON+SETLIST]]
            var commandString = commandWrite.DoCompute();
            await bleDevice.Write[[CharacteristicName.dotNet]](commandString);
        }

```

## CS+XAML+UILIST+LIST+CONTENT+COMBOBOX+LIST If="[[UIType]] contains ComboBoxFor" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=child 

PageCSharp+Characteristic+ComboChange


```
        private [[ASYNC]] void [[FunctionName]]_ComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            var commandSet = bleDevice.[[CharacteristicName.dotNet]]_[[Target0]]_Init();
            if (e.AddedItems.Count == 1
                && (double.TryParse((sender as FrameworkElement).Tag as string, out var value)))
            {
                commandSet.SetCurrDouble("[[Target1]]", value);
            }
[[CS+XAML+UILIST+LIST+CONTENT+VALUECHANGE+COMPUTETARGET]]
        }
```


## CS+XAML+UILIST+LIST+CONTENT+RADIO+LIST If="[[UIType]] contains RadioFor" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=child

PageCSharp+Characteristic+RadioChange


```
        private [[ASYNC]] void [[FunctionName]]_RadioCheck(object sender, RoutedEventArgs e)
        {
            var commandSet = bleDevice.[[CharacteristicName.dotNet]]_[[Target0]]_Init();
            if (double.TryParse((sender as FrameworkElement).Tag as string, out var value))
            {
                commandSet.SetCurrDouble("[[Target1]]", value);
            }
[[CS+XAML+UILIST+LIST+CONTENT+VALUECHANGE+COMPUTETARGET]]
        }
```

## CS+XAML+UILIST+LIST+CONTENT+SLIDER If="[[UIType]] contains SliderFor" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=child  

PageCSharp+Characteristic+SliderChange

```
        private [[ASYNC]] void [[FunctionName]]_SliderChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var commandSet = bleDevice.[[CharacteristicName.dotNet]]_[[Target0]]_Init();
            commandSet.SetCurrDouble("[[Target1]]", e.NewValue);
[[CS+XAML+UILIST+LIST+CONTENT+VALUECHANGE+COMPUTETARGET]]
        }
```

## CS+XAML+UILIST If="[[UIListType]] !contains None" Type=list Source=ServicesByOriginalOrder/Characteristics/UIList ListOutput=parent CodeListSubZero=""

```
[[CS+XAML+UILIST+LIST+CONTENT+BUTTON]][[CS+XAML+UILIST+LIST+CONTENT+COMBOBOX+LIST]][[CS+XAML+UILIST+LIST+CONTENT+RADIO+LIST]][[CS+XAML+UILIST+LIST+CONTENT+SLIDER]]
```

## CS+CHARACTERISTIC+LIST Type=list Source=ServicesByOriginalOrder/Characteristics ListOutput=parent

PageCSharp+CharacteristicRecordTemplate


```
        public class [[CharacteristicName.dotNet]]Record : INotifyPropertyChanged
        {
            public [[CharacteristicName.dotNet]]Record()
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

[[CS+CHARACTERISTIC+DATA+PROPERTY]]
            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }
    
    public DataCollection<[[CharacteristicName.dotNet]]Record> [[CharacteristicName.dotNet]]RecordData { get; } = new DataCollection<[[CharacteristicName.dotNet]]Record>();
    private void On[[CharacteristicName.dotNet]]_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if ([[CharacteristicName.dotNet]]RecordData.Count == 0)
            {
                [[CharacteristicName.dotNet]]RecordData.AddRecord(new [[CharacteristicName.dotNet]]Record());
            }
            [[CharacteristicName.dotNet]]RecordData[[[CharacteristicName.dotNet]]RecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCount[[CharacteristicName.dotNet]](object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        [[CharacteristicName.dotNet]]RecordData.MaxLength = value;

        [[CS+CHART+REDRAW]]
    }

    private void OnAlgorithm[[CharacteristicName.dotNet]](object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        [[CharacteristicName.dotNet]]RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopy[[CharacteristicName.dotNet]](object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime[[CS+CHARACTERISTIC+DATA+LIST]],Notes\n");
        foreach (var row in [[CharacteristicName.dotNet]]RecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24}[[CS+CHARACTERISTIC+DATA+VALUE+LIST]],{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

[[CS+CHARACTERISTIC+NOTIFY+METHOD]]
[[CS+CHARACTERISTIC+READ+METHOD]]
[[CS+CHARACTERISTIC+WRITE+METHOD]]
[[CS+XAML+UILIST]]

```

## CS+SERVICE+LIST Type=list Source=ServicesByOriginalOrder

```
        // Functions for [[ServiceName]]
[[CS+CHARACTERISTIC+LIST]]
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
typeof([[CharacteristicName.dotNet]]Record).GetProperty("[[DATANAME]]"),
```
", // DATA4+LIST
```TEST
"[[DATANAME]]",
```
", // DATA5+LIST




LIST!!



LIST!!!
## PageCSharp+CharacteristicWrite+DataTemplates
```
                [[VARIABLETYPE]] [[DATANAME]];
                // History: used to go into [[CharacteristicName.dotNet]]_[[DATANAME]].Text instead of using the variable
                // History: used to used [[DEC+OR+HEX]] for parsing instead of the newer dec_or_hex variable that's passed in
                var parsed[[DATANAME]] = Utilities.Parsers.TryParse[[VARIABLETYPE]](text, dec_or_hex, null, out [[DATANAME]]);
                if (!parsed[[DATANAME]])
                {
                    parseError = "[[DATANAMEUSER]]";
                }
```



## PageCSharp+Characteristic+SetValue 

```
            commandWrite.Parameters["[[PARAMETER]]"].CurrValue = [[VALUE]]; // same as commandWrite.Parameters["[[PARAMETER]]"].ValueNames["[[VALUENAME]]"];
```



## PageCSharp+Characteristic+ValueChangeComputeTarget
```
            // computedTarget might be different from the computed value
            var commandWrite = bleDevice.[[CharacteristicName.dotNet]]_[[TARGETCOMMAND]]_Init();
            var commandString = commandWrite.DoCompute();
            await bleDevice.Write[[CharacteristicName.dotNet]](commandString);
```



