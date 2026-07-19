# How to add a new Device Control

The device controls are the 400x400 (approx) square(ish) panels. There is one per each instance of a device (e.g., if you have two Nordic Thingy:52s, there will be two panes visible).

### Data type: Supported versus Known devices

The code is littered with "Known" and "Supported" devices. These are different (but related) concepts, and you need to keep them clear.

The code for SupportedDevice.cs and KnownDevice.cs is in directory *BluetoothWinUI3Registration*

- **Supported** devices are like a Nordic Thingy:53. This code knows about the Nordic Thingy.
- **Known** device are specific instances of a supported device. If you have several of them, you will have several instances. Each known device gets its own control in the  uiKnownDevices panel in MainWindow.

Every Known device contains four important fields:

- The corresponding Supported device
- The Control (the 400x400 UserControl) 
- The ZoomableDeviceContainer the control is placed in
- The WatcherData (Advertisement) that triggered the creation of the control

# The easy path: start with BTStandard_Demo

A cut-down sample is part of the source code! The BTStandard_Demo is a fully commented XAML and XAML.CS file that provides a complete example of connecting a device to this app. 

## Make the Bluetooth device protocol parser from JSON

You will need a ```Manufacturer_Device.cs``` protocol parser file and the corresponding ```Manufacturer_DeviceCollection.cs``` files. This are normally created from a JSON file ```Manufacturer_Device.json``` that includes a complete description of the services and characteristics that you want to support. 

The Manufacturer_Device.cs file has two key jobs. One is to connect to the device, read data, and handle notifications. The other is to parse data from the device into device-specific classes.

In the example, the device has two services (the *Common Configuration* service and the *Battery* service). These services are read into two data classes  *Common_Configuration_Data* and *Battery_Data*. The classes support the INotifyPropertyChanged so they work nicely with XAML.

|File|Location|
|----|----|
|JSON file|BluetoothDeviceController\BluetoothDeviceController\Assets\CharacteristicsData|
|Protocol file|BluetoothDeviceController\BluetoothProtocolsDevicesCore|
|Collection file|BluetoothDeviceController\BluetoothProtocolsDevicesCore|

The JSON file is converted to the Protocol and collection file using the tool in BluetoothDeviceController\BluetoothCodeGeneratordotNetCore. 

## Include the JSON file and Protocol files in the project (as linked)

The JSON file lives in another directory but should be included in the project. Add the JSON file to the project's *Assets\CharacteristicsData* **as a link**. Do not copy the file; make sure it's linked.

Update the Project file so the JSON file Build Action is *Content* and the Copy to Output Directory is *Copy When Newer*.


## Make the User Control

Make a blank User Control, then copy all the items from BTStandard_Demo XAML and XAML.CS files, changing the class name. The file name should be ```Manufacturer_DeviceControl```.

It's **critical** that the file is placed in the top-level directory. You **cannot** put a WinUI3 control in a sub-directory.

Right-click BluetoothWinUI3 and select Add > New Item in the popup
In the Add New Item dialog, select C# Items > WinUI and from the list select User Control
In the Name, type Manufacturer_DeviceControl.xaml. Replace "Manufacturer" and "Device" with the actual manufacturer and device name, of course.

## Connect the Supported Device to the control

Bluetooth devices which are supported are listed in the *BluetoothWinUI3Registration\SupportedDevices.cs* file. You will need to add your device to that file.

# Critical quality items

## Outermost panel is alway a Border named rootPanel

```
<Border Style="{StaticResource sDeviceBorder}" x:Name="rootPanel">
```


## Size is the CurrWindowSize. And there's a standard InternalDeviceType string for logging purposes.

```
    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>
    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400

    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "Nordic_Thingy";

```

```C#
    public void UpdateUX(MainWindow.WindowSize windowSize, Windows.Foundation.Size largeActualSize)
    {
        CurrWindowSize = windowSize;
        switch (CurrWindowSize)
        {
            default:
            case MainWindow.WindowSize.Normal:
                rootPanel.Width = 380;
                rootPanel.Height = 380;
                SetAxesVisibility(false);
                break;
            case MainWindow.WindowSize.Large:
                rootPanel.Width = largeActualSize.Width;
                rootPanel.Height = largeActualSize.Height;
                SetAxesVisibility(true);
                break;
        }
    }
```

# Facades and Extensions for the JSON data

Sometimes the JSON description-generated data classes aren't very convenient. When this happens, you might want to make either an extension or a facade class.

Quick reminder: The JSON device description is converted into a BT parser and a handful of data classes (plus some lifetime BT code and more). The parser will stuff data into the correct data class. The data class directly corresponds to the different fields in the specific BT protocol.

Quick reminder #2: the data from the device is (almost) always in a specific units (e.g., "Degrees Celcius"). But the user often has a preference for a different unit

The problem is that sometimes the specific BT protocol isn't very convenient. For example, the Heart_Rate_Data from the HeartRate service (0x180D) includes a one-byte and a two-byte Heart Rate value. That's not very convenient; it would be better to have a single property. A *facade* is used make a more convenient data type.

The Nordic Thingy has a different problem: the environment data is from different sensors that don't start up at the same time. That doesn't work well with the environment graph. An *extension* is used to include a property that says if the data has all of the environment data.

## Why pick an extension versus a facade?

If you just need to add an additional quick method (like the Nordic Thingy), use a C# Extension class. Put it into the *BluetoothProtocolsDevicesCoreExtensions* directory in its own file called *Manufacturer*_*Device*_Extension.cs.

If you need to override some properties like the Heart Rate does, make a Facade. Put it into the *BluetoothProtocolsDevicesCoreExtensions* directory in its own file called *Manufacturer*_*Device*_Facade.cs.

## Common "Gotchas" for facades

1. You have to make a new class. You cannot subclass the facade class on top of the original data class. This is because the data classes use the "Curiously Recursing Template Pattern" (CRTP), and one of the limitations of that is you can't make a subclass.
2. Your control will have to accept a generated data class as the data and then copy all of the data into the facade. 
3. You will have to carefully duplicate the CopyFrom and CopyToWithConvertAndCreate
4. Calculated property fields interact poorly with the user preferred units. For example, if you have a distance and a time data property already, and you want a speed property, you cannot just have a calculated property because then the user can't pick the units (usually MPH or KPH). Instead you have to do the calculation "by hand" in the CopyFrom and CopyToWithConvertAndCreate.

