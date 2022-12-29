# MDHelpFile FileName=Device_[[CLASSNAME]].md DirName=Help

```
# Instructions for this file

This is just a template help file; you will need to edit it to add additional information.

Instructions for this help file:

1. Copy the final version of this file to Assets/HelpFiles
2. Make sure to add it in Visual Studio, and set the "Copy to Output Directory" to "Copy if newer"
3. Add a link to the Help.md file ** [[[UserName]]](Device_[[CLASSNAME]].md) **
4. Add a link to the Welcome.md file ** [![[[UserName]]](../DevicePictures/[[CLASSNAME]]-175.png)](Device_[[CLASSNAME]].md) **
5. Create an Assets/DevicesPictures/[[CLASSNAME]]-175.png and Assets/DevicesPictures/[[CLASSNAME]]-350.png. The -175 needs to be 175x175 pixels and the -350 file needs to be 350x350 pixels.
6. Make sure to add it in Visual Studio, and set the "Copy to Output Directory" to "Copy if newer"
7. Create an Assets/ScreenShots/Device_[[CLASSNAME]].PNG file
8. Make sure to add it in Visual Studio, and set the "Copy to Output Directory" to "Copy if newer"

Instrutions for adding new devices

1. Move the Protocol file to BluetoothProtocols/BluetoothProtocols
2. Move the XAML and XAML.CS files to BluetothDeviceController/SpecialtyPages
3. Add the specialization to  **mainpage.xaml.cs** about line 90-135 add a specialization line like ```            new Specialization (typeof(SpecialtyPages.[[CLASSNAME]]Page), new string[] { "[[DeviceName]]" }, [[DeviceType]], "[[Maker]]", "[[ShortDescription]]"),```
4. Compile and run!



-------------- start of file -------------

# The [[UserName]] [[ShortDescription]] from [[Maker]]

![Device](../DevicePictures/[[CLASSNAME]]-175.png)

[[Description]]

## Pairing and using the device

[[UsingTheDevice]]

![Device](../ScreenShots/Device_[[CLASSNAME]].png)

## Helpful Links

[[DeviceLinks]]

```


## DeviceLinks Type=list Source=LINKS CodeListZero="No links for this device"

```
* [[[TEXT]]]([[TEXT]])
```
