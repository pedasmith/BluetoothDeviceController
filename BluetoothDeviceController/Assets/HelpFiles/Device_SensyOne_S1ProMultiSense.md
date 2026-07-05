# Instructions for this file

This is just a template help file; you will need to edit it to add additional information.

Instructions for this help file:

1. Copy the final version of this file to Assets/HelpFiles
2. Make sure to add it in Visual Studio, and set the "Copy to Output Directory" to "Copy if newer"
3. Add a link to the Help.md file ** [HLK-LD2450_](Device_SensyOne_S1ProMultiSense.md) **
4. Add a link to the Welcome.md file ** [![HLK-LD2450_](../DevicePictures/SensyOne_S1ProMultiSense-175.png)](Device_SensyOne_S1ProMultiSense.md) **
5. Create an Assets/DevicesPictures/SensyOne_S1ProMultiSense-175.png and Assets/DevicesPictures/SensyOne_S1ProMultiSense-350.png. The -175 needs to be 175x175 pixels and the -350 file needs to be 350x350 pixels.
6. Make sure to add it in Visual Studio, and set the "Copy to Output Directory" to "Copy if newer"
7. Create an Assets/ScreenShots/Device_SensyOne_S1ProMultiSense.PNG file
8. Make sure to add it in Visual Studio, and set the "Copy to Output Directory" to "Copy if newer"

Instrutions for adding new devices

1. Move the Protocol file to BluetoothProtocols/BluetoothProtocols
2. Move the XAML and XAML.CS files to BluetothDeviceController/SpecialtyPages
3. Add the specialization to  **mainpage.xaml.cs** about line 90-135 add a specialization line like ```            new Specialization (typeof(SpecialtyPages.SensyOne_S1ProMultiSensePage), new string[] { "HLK-LD2450_" }, , "", ""),```
4. Compile and run!
5. Update the help_version.md file in Assets/HelpFiles!



-------------- start of file -------------

# The HLK-LD2450_  from 

![Device](../DevicePictures/SensyOne_S1ProMultiSense-175.png)

The Sensy-One S1 Pro Multi Sense is an open-source presence and environmental sensor, built from the ground up with Home Assistant in mind

## Pairing and using the device



![Device](../ScreenShots/Device_SensyOne_S1ProMultiSense.png)

## Helpful Links

* [https://sensy-one.com/products/s1-pro-multi-sense](https://sensy-one.com/products/s1-pro-multi-sense)
