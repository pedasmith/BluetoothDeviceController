# How to add a new Device Control

The device controls are the 400x400 (approx) square(ish) panels. There is one per each instance of a device (e.g., if you have two Nordic Thingy:52s, there will be two panes visible).

It's important to distinguish

- **Supported** devices are like a Nordic Thingy:53. This code knows about the Nordic Thingy.
- **Known** device are specific instances of a supported device. If you have several of them, you will have several instances. Each known device gets its own control in the  uiKnownDevices panel in MainWindow.


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

## Include the JSON file and Potocol files

The JSON file lives in another directory but should be included in the project. Add the JSON file to the project's *Assets\CharacteristicsData* **as a link**. Do not copy the file; make sure it's linked.

Update the file so it's Build Action is *Content* and the Copy to Output Directory is *Copy When Newer*.


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


