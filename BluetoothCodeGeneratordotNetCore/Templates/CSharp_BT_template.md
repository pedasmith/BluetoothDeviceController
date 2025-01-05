# Protocol FileName=[[CLASSNAME]].cs DirName=BluetoothProtocols SuppressFile=:SuppressCSharpProtocol:

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

using Utilities;

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
            var ch = Characteristics[(int)CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum];
            if (ch == null) return;
            Notify[[Name.dotNet]]_ValueChanged_Set = false;
            ch.ValueChanged -= Notify[[Name.dotNet]]Callback;
        }


```

## METHOD+PROPERTY Type=list If="[[Verbs]] contains :RdInNo:" Type=list ListOutput=parent Source=Services/Characteristics/ReadProperties CodeListSubZero=""

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
        public async Task Write[[Name.dotNet]]([[WRITE+PARAMS]])
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bluetooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
[[DATAWRITER]]
            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(CharacteristicsEnum.[[Name.dotNet]]_[[../Name.dotNet]]_enum, "[[Name.dotNet]]", command, [[WRITEMODE]]);

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
        }
```

## DATAWRITER If="[[Verbs]] contains :WrWw:" Type=list ListOutput=parent Source=Services/Characteristics/WriteProperties CodeListSubZero="" Trim=false
```
            [[ARGDWCALL]]( [[ARGDWCALLEXTRA]][[ARGDWCALLCAST]] [[DATANAME]]);
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


