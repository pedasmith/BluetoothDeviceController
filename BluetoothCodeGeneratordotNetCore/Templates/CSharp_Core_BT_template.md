# ProtocolCore FileName=[[CLASSNAME]].cs DirName=BluetoothProtocolsDevicesCore SuppressFile=:SuppressCSharpProtocol:

```
//From template: Protocol_Core_Body v2026-04-16 15:43
//From template: Protocol_Body v2022-07-02 9:54
using System;
using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Needed for CallerMemberNameAttribute
using System.Runtime.InteropServices.WindowsRuntime; // Needed for IBuffer.ToArray extension method
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    /// <summary>
    /// [[DESCRIPTION]].
    /// This class was automatically generated [[CURRTIME]]
    /// </summary>

    public [[CLASSMODIFIERS]] class [[CLASSNAME]] : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
[[Links]]

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /* Service and Characteristics summary for the device [[DeviceName]]

[[ServiceSummary]]
        */

        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
[[ServiceIndexList]]
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
[[CharacteristicIndexList]]
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
[[ServiceGuidsList]]
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { [[SERVICES+NULL+LIST]]};

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
[[CharacteristicGuidsList]]
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { [[CHARACTERISTICS+NULL+LIST]] };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { [[CHARACTERISTICS+FALSE+LIST]] };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  [[CHARACTERISTICS+NULL+LIST]] };


        /// <summary>
        /// Delegate for all Notify events
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(IotNumberFormats.ValueParserResult data);

        private async Task<bool> Ensure_Characteristic_Async(ServiceIndex serviceIndex, string serviceName, CharacteristicIndex characteristicIndex, string characteristicName)
        {
            if (Characteristics[(int)characteristicIndex] == null)
            {
                if (ble == null) return false;
                if (Services[(int)serviceIndex] == null)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(Service_Guids[(int)serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {serviceName}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {serviceName}", serviceStatus);
                        return false;
                    }
                    Services[(int)serviceIndex] = serviceStatus.Services[0];
                }
                var service = Services[(int)serviceIndex];
                var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(Characteristic_Guids[(int)characteristicIndex]);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"unable to get characteristic for {characteristicName}", characteristicsStatus);
                    return false;
                }
                if (characteristicsStatus.Characteristics.Count == 0)
                {
                    Status.ReportStatus($"unable to get any characteristics for {characteristicName}", characteristicsStatus);
                    return false;
                }
                else if (characteristicsStatus.Characteristics.Count != 1)
                {
                    Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {characteristicName}", characteristicsStatus);
                    return false;
                }
                Characteristics[(int)characteristicIndex] = characteristicsStatus.Characteristics[0];
            }
            return true;
        }


        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(GattCharacteristic ch, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await ch.ReadValueAsync(cacheMode);
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


        private async Task<bool> SetupNotifyAsync(string name, 
            ServiceIndex serviceIndex, string serviceName, CharacteristicIndex index, 
            Windows.Foundation.TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> callback,
            GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            await Ensure_Characteristic_Async(serviceIndex, serviceName, index, name);
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return false;
            }
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyCharacteristic_ValueChanged_set[(int)index])
                {
                    // Only set the event callback once
                    NotifyCharacteristic_ValueChanged_set[(int)index] = true;
                    ch.ValueChanged += callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"Notify{name}: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"Notify{name}: set notification", result);

            return true;
        }

        //
        //
        // Start of the service + characteristic
        //
        //


        //
        // All services / characteristics and data structures
        //


[[ServicesReadWriteNotify]]

// Long obsolete! [[zzMETHOD+LIST]]
    }
}
```

## Links Type=list Source=LINKS CodeListZero="        // No links for this device" Trim=endCR

```
        // Link: [[TEXT]]
```

## ServiceIndexList Type=list Source=Services Trim=endCR Code="            [[Name]]_index = [[Count.Child]]," 

## ServiceGuidsList Type=list Source=Services Trim=endCR  

```
            Guid.Parse("[[UUID]]"), // #[[Count.Child]] is [[Name]]
```
## SERVICES+NULL+LIST Type=list Source=Services Trim=true Code="null, " 

## CharacteristicIndexList Type=list Source=Services/Characteristics Trim=endCR

```
            [[ServiceName.dotNet]]_[[CharacteristicName.dotNet]]_index = [[COUNTALL]],     // GUID [[UUID]]
```

## CharacteristicGuidsList Type=list Source=Services/Characteristics Trim=endCR
```
            Guid.Parse("[[UUID]]"), // #[[COUNTALL]] is [[ServiceName]] [[Name]]
```

## CHARACTERISTICS+NULL+LIST Type=list Source=Services/Characteristics Trim=true Code="null, " 

## CHARACTERISTICS+FALSE+LIST Type=list Source=Services/Characteristics Trim=true Code="false, " 


## ServiceSummary Type=list Source=Services CodeListSubZero="" Trim=endCR

The ServiceSummary is a simple user-readable summary of the device services and characteristics.
It's placed into the protocol CS file so that there's a short summary of all services and
characteristics which is easily found in a GitHub search of the generated code.

```
        [[Name]] service Guid=[[UuidShort]]
[[DataGroupSummary]]
```

## DataGroupSummary Type=list ListOutput=parent Source=Services/DataGroups CodeListSubZero="" Trim=false

```
            [[DataGroupName]] (DataGroup record)
[[CharacteristicSummary]]
```
## CharacteristicSummary Type=list ListOutput=parent Source=Services/DataGroups/Characteristics CodeListSubZero="" Trim=false
```
                [[Name]] characteristic has [[PropertySummary]] Guid=[[UuidShort]]
```

## PropertySummary Type=list ListOutput=parent Trim=false Source=Services/DataGroups/Characteristics/Properties  CodeListSubZero="" Trim=true
```
[[DataName.dotNet]] ([[VariableType]]-->[[VariableTypeDS]]) 
```


## ZZCharacteristicSummary Type=list ListOutput=parent Source=Services/Characteristics CodeListSubZero="" Trim=false
```
            [[Name]] characteristic has [[PropertySummary]] Guid=[[UuidShort]] Data=[[DataGroupName.dotNet]]
```

## ZZPropertySummary Type=list ListOutput=parent Trim=false Source=Services/Characteristics/Properties  CodeListSubZero="" Trim=true
```
[[DataName.dotNet]] ([[VariableType]]-->[[VariableTypeDS]]) 
```
## ServicesReadWriteNotify Type=list Source=Services CodeListSubZero="" Trim=false

This is the primary section of the code. 

```
        #region Service_[[ServiceName.dotNet]]
        [[ServiceDataGroups]]

        #endregion
//
```

## ServiceDataGroups Type=list ListOutput=parent Source=Services/DataGroups CodeListSubZero="" Trim=endCR
```
        // Service [[Name]] 
        /// <summary>
        /// Data from all of the characteristics in the [[ServiceName]] Service
        /// </summary>
        public class [[DataGroupName.dotNet]]
        {
[[CharacteristicDataFields]]
        }
        public [[DataGroupName.dotNet]] Curr[[DataGroupName.dotNet]] { get; set; } = new [[DataGroupName.dotNet]]();

[[CHARACTERISTIC+METHOD+NOTIFY]]
[[CHARACTERISTIC+METHOD+READ]]

```


## CharacteristicDataFields Type=list ListOutput=parent Trim=endCR Source=Services/DataGroups/Characteristics CodeListSubZero=""
```
[[CharacteristicPropertyDataFields]]
```


## CharacteristicPropertyDataFields Type=list ListOutput=parent Trim=endCR Source=Services/DataGroups/Characteristics/Properties CodeListSubZero=""

```
            public [[VARIABLETYPE+DS]] [[DataName.dotNet]]; // From [[ServiceName]] and [[CharacteristicName]]
```


## CHARACTERISTIC+METHOD+NOTIFY Type=list ListOutput=parent Trim=endCR Source=Services/DataGroups/Characteristics CodeListSubZero=""

```
        // Per-characteristics methods for [[ServiceName.dotNet]] [[CharacteristicName.dotNet]]
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> Notify[[CharacteristicName.dotNet]]Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("[[CharacteristicName.dotNet]]", ServiceIndex.[[ServiceName.dotNet]]_index, "[[ServiceName]]", CharacteristicIndex.[[ServiceName.dotNet]]_[[CharacteristicName.dotNet]]_index, Notify[[CharacteristicName.dotNet]]Callback, notifyType);
            return retval;
        }

        private void Notify[[CharacteristicName.dotNet]]Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.[[ServiceName.dotNet]]_[[CharacteristicName.dotNet]]_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("[[Type]]"); // TODO: should be done ahead of time!
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
[[CHARACTERISTIC+PROPERTY+READ+FIELD]]
            OnPropertyChanged("[[CharacteristicName.dotNet]]");
        }

```

## CHARACTERISTIC+METHOD+READ Type=list ListOutput=parent Trim=endCR Source=Services/DataGroups/Characteristics CodeListSubZero=""

The read method for each characteristic.

```
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>[[DataGroupName.dotNet]] of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<[[DataGroupName.dotNet]]> Read[[CharacteristicName.dotNet]](BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.[[ServiceName.dotNet]]_[[CharacteristicName.dotNet]]_index;
            await Ensure_Characteristic_Async(ServiceIndex.[[ServiceName.dotNet]]_index, "[[ServiceName]]", index, "[[CharacteristicName]]");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "[[CharacteristicName]]", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("[[Type]]"); // TODO: should be done ahead of time!
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
[[CHARACTERISTIC+PROPERTY+READ+FIELD]]
            OnPropertyChanged("[[CharacteristicName.dotNet]]");
            return Curr[[DataGroupName.dotNet]];
        }
```


## CHARACTERISTIC+PROPERTY+READ+FIELD Type=list ListOutput=parent Trim=endCR Source=Services/DataGroups/Characteristics/Properties

TODO: the numeric values are all handled correctly (turned into bytes). But what about 

```
            Curr[[DataGroupName.dotNet]].[[DataName.dotNet]] = vr.GetNextDouble();
```

## METHOD+LIST Type=list Source=Services/Characteristics CodeListSubZero="// No methods for [[Name]]"

In my **TODO:** list
- Each characteristic might have read, write and notify; these have to be conditionally applied
- How should I handle the argument lists?

```
        // method.list for [[ServiceName.dotNet]] [[CharacteristicName.dotNet]]
[[METHOD+PROPERTY]]
[[METHOD+READ]]
[[METHOD+NOTIFY]]
[[METHOD+WRITE]]
[[METHOD+COMMANDS]]

```


//
//
//
//
//
//
//
//

REST OF FILE IS THE ORIGINAL

//
//
//
//
//
//
//
//
//

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
zzz## CHARACTERISTIC+GUID+LIST Type=list Source=Services/Characteristics Code="            Guid.Parse(\"[[UUID]]\"), // #[[Count.Child]] is [[Name]]" CodeListSubZero="            // No characteristics for [[Name]]"
## CHARACTERISTIC+NAME+LIST Type=list Source=Services/Characteristics Code="            \"[[Name]]\", // #[[Count.Child]] is [[UUID]]" CodeListSubZero="            // No characteristics for [[Name]]"
## CHARACTERISTIC+LIST Type=list Source=Services/Characteristics Code="            null," CodeListSubZero="            // No characteristics for [[Name]]"
## HASH+LIST Type=list Source=Services/Characteristics Code="[[COUNT]], " Trim=true CodeWrap="            new HashSet<int>(){ [[TEXT]] }," CodeListSubZero="            // No characteristics for [[Name]]"
## HASH+LIST+REVERSE Type=list Source=Services/Characteristics Code="            [[../COUNT]], // Characteristic [[COUNT]]" CodeListSubZero="            // No characteristics for [[Name]]"


## SET+PROPERTY+VALUES Type=list Source=Services/Characteristics/ReadProperties ListOutput=parent CodeListZero="            // No properties for this characteristic" CodeListSubZero="            // No properties for this characteristic"

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
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return false;
            }

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
            var ch = Characteristics[(int)CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum];
            if (ch == null) return;
            Notify[[Name.dotNet]]_ValueChanged_Set = false;
            ch.ValueChanged -= Notify[[Name.dotNet]]Callback;
        }


```

## METHOD+PROPERTY Type=list If="[[Verbs]] contains :RdInNo:" Type=list ListOutput=parent Source=Services/Characteristics/ReadProperties CodeListSubZero=""

```
        // METHOD+PROPERTY for ListOutput=parent Source=Services/Characteristics/ReadProperties
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
        // 
        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> Read[[Name.dotNet]](BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return null;
            }
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

## WRITE+PARAMS If="[[Verbs]] contains :WrWw:" Type=list ListOutput=parent Source=Services/Characteristics/WriteProperties CodeListSubZero="" CodeListSeparator=", " Trim=true 
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
        public async Task<GattCommunicationStatus> Write[[Name.dotNet]]([[WRITE+PARAMS]])
        {
            var ensureResult = await EnsureCharacteristicAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum);
            if (ensureResult != GattCommunicationStatus.Success) 
            {
                return ensureResult;
            }

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
[[DATAWRITER]]
            var command = dw.DetachBuffer().ToArray();
            [[XORFIXUP]]
            var retval = await WriteCommandAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum, "[[Name.dotNet]]", command, [[WRITEMODE]]);

            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession.maxpdusize?view=winrt-26100
            // You can send large amounts of data, and it will be fragmented automatically by the 
            // OS using the MTU. Your application is not limited by the MTU size as to the data transfer of each packet.

            // Old code, not needed. After checking the file history; this code has always been this way, so it's not
            // clear that it was ever needed.
            //const int MAXBYTES = 20;
            //if (command.Length <= MAXBYTES) //TODO: make sure this works
            //{
            //    await WriteCommandAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum, "[[Name.dotNet]]", command, [[WRITEMODE]]);
            //}
            //else for (int i=0; i<command.Length; i+= MAXBYTES)
            //{
            //    // So many calculations and copying just to get a slice
            //    var maxCount = Math.Min(MAXBYTES, command.Length - i);
            //    var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
            //    await WriteCommandAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum, "[[Name.dotNet]]", subcommand, [[WRITEMODE]]);
            //}
            return retval;
        }
```

## DATAWRITER If="[[Verbs]] contains :WrWw:" Type=list ListOutput=parent Source=Services/Characteristics/WriteProperties CodeListSubZero="" Trim=false
```
            [[ARGDWCALL]]([[ARGDWCALLCAST]][[DATANAME]]);
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
## zzzMETHOD+LIST Type=list Source=Services/Characteristics CodeListSubZero="// No methods for [[Name]]"

In my **TODO:** list
- Each characteristic might have read, write and notify; these have to be conditionally applied
- How should I handle the argument lists?

```
// method.list for [[Name.dotNet]]
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


