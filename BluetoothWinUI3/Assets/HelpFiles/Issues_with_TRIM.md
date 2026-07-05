# Issues with ducking TRIM

One of the most irritating problems with the TRIM option in .NET is that it randomly works or doesn't work depending on whether you are in release mode or debug mode, and based on the SDK version. This binary calvinball is simply a waste of time; the project as of 2026-05-10 turns TRIM off entirely.

## Fixes Attempted

### Baseline and test plan (Doesn't work) Size=36,868 KB

At the start, in release mode, the graph does not work. Things to test include:

1. Does the graph work
2. Does the table automatically update
3. Will the JSON files get saved?


**Test procedure**

1. Find the Preferences folder. This is normally your OneDrive/Documents/BluetoothDevices folder. There are normally two files here: AllDeviceData.devices and UserPreferences.preferences. Make a backup of these files, if desired.
2. Run the app. Be sure it's in release mode! Connect to a bluetooth device.
3. Verify that the graph shows up
4. Verify that the table updates
5. Set some setting in the app (e.g., units preferences)
6. Go to the preferences folder and verify that the two files are present.


### 1. Turn off all trimming (works) Size=76,063 KB

Changed this line in the .csproj file. It used to say that PublishTrimmed was ```True```; I changed it to false and the app compiled with no warnings and worked correctly. 

Result: no warnings (yay!) and the OxyPlot worked again.
```
    <PublishTrimmed Condition="'$(Configuration)' != 'Debug'">False</PublishTrimmed>

```

### 2. Turn off trim for H.OxyPlot.WinUI (works) Size=37271 KB

Later update: this only work in SDK 8. With SDK 10, this fails.

Find the ```<PackageReference Include="H.OxyPlot.WinUI" Version="0.9.30" />``` line in the .csproj file

```
	<PackageReference Include="H.OxyPlot.WinUI" Version="0.9.30" />
	<TrimmerRootAssembly Include="H.OxyPlot.WinUI" />
	<TrimmerRootAssembly Include="OxyPlot" />

```

## The error messages

The original error messages are below. They have been updated to show the actual lines of the XamlTypeInfo.g.cs file that showed up.

```


2>%PROJECT%\BluetoothWinUI3\obj\ARM64\Release\net8.0-windows10.0.22000.0\win-arm64\XamlTypeInfo.g.cs(554,57): 
Trim analysis warning IL2059: BluetoothWinUI3.BluetoothWinUI3_XamlTypeInfo.XamlTypeInfoProvider.StaticInitializer_39_Nullable(): 
Unrecognized value passed to the parameter 'type' of method 'System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(RuntimeTypeHandle)'. It's not possible to guarantee the availability of the target static constructor.
        private void StaticInitializer_39_Nullable() => global::System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(global::System.Nullable<global::WinUI.TableView.TableViewCellSlot>).TypeHandle);


2>%PROJECT%\BluetoothWinUI3\obj\ARM64\Release\net8.0-windows10.0.22000.0\win-arm64\XamlTypeInfo.g.cs(563,57): 
Trim analysis warning IL2059: BluetoothWinUI3.BluetoothWinUI3_XamlTypeInfo.XamlTypeInfoProvider.StaticInitializer_50_Nullable(): 
Unrecognized value passed to the parameter 'type' of method 'System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(RuntimeTypeHandle)'. It's not possible to guarantee the availability of the target static constructor.
        private void StaticInitializer_50_Nullable() => global::System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(global::System.Nullable<global::System.Double>).TypeHandle);


2>%PROJECT%\BluetoothWinUI3\obj\ARM64\Release\net8.0-windows10.0.22000.0\win-arm64\XamlTypeInfo.g.cs(565,57): 
Trim analysis warning IL2059: BluetoothWinUI3.BluetoothWinUI3_XamlTypeInfo.XamlTypeInfoProvider.StaticInitializer_55_Nullable(): 
Unrecognized value passed to the parameter 'type' of method 'System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(RuntimeTypeHandle)'. It's not possible to guarantee the availability of the target static constructor.
        private void StaticInitializer_55_Nullable() => global::System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(global::System.Nullable<global::System.Int32>).TypeHandle);


2>%PROJECT%\BluetoothWinUI3\obj\ARM64\Release\net8.0-windows10.0.22000.0\win-arm64\XamlTypeInfo.g.cs(566,57): 
Trim analysis warning IL2059: BluetoothWinUI3.BluetoothWinUI3_XamlTypeInfo.XamlTypeInfoProvider.StaticInitializer_57_Nullable(): 
Unrecognized value passed to the parameter 'type' of method 'System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(RuntimeTypeHandle)'. It's not possible to guarantee the availability of the target static constructor.
        private void StaticInitializer_57_Nullable() => global::System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(global::System.Nullable<global::WinUI.TableView.SortDirection>).TypeHandle);


2>%USER%\.nuget\packages\h.oxyplot.winui\0.9.30\lib\net6.0-windows10.0.18362\H.OxyPlot.WinUI.dll : 
warning IL2104: Assembly 'H.OxyPlot.WinUI' produced trim warnings. For more information see https://aka.ms/dotnet-illink/libraries

2>%USER%\.nuget\packages\oxyplot.core\2.1.0\lib\netstandard2.0\OxyPlot.dll : 
warning IL2104: Assembly 'OxyPlot' produced trim warnings. For more information see https://aka.ms/dotnet-illink/libraries

2>%USER%\.nuget\packages\winui.tableview\1.4.1\lib\net8.0-windows10.0.19041\WinUI.TableView.dll : w
arning IL2104: Assembly 'WinUI.TableView' produced trim warnings. For more information see https://aka.ms/dotnet-illink/libraries

========== Rebuild All: 2 succeeded, 0 failed, 0 skipped ==========
========== Rebuild completed at 10:34 AM and took 01:18.245 minutes ==========


```


## Helpful Links

* [Microsoft Learn: painful ways to handle TRIM](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/fixing-warnings)
* [Microsoft Learn: TRIM is horrible](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/incompatibilities)