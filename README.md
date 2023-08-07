# Bluetooth Device Controller
Need a way to control your Bluetooth devices, but there isn't a Windows app for it? Try the [Bluetooth Device Controller](https://www.microsoft.com/en-us/p/bluetooth-device-controller/9pp2jw8c2nrt?activetab=pivot:overviewtab)  app on the Windows Store! 

If you're a hardware developer, you'll find here JSON files that describe many different Bluetooth devices including
* several generations of TI SensorTag hardware
* Multiple kinds of lights
* Various other devices

The JSON says which services and characteristics different devices support plus the math needed to convert the raw data into something usable. 

The app also serves as a simple way to explore your Bluetooth devices; it will list devices that are found and what their services and characteristics are. There's code to create the JSON files from on-the-air Bluetooth, and code to convert the JSON files into C# protocol code and C#/XAML UI code. It's not awesome code, but it is auto-generated, which is super useful.


# Other projects

## SmallProjects\BluetoothCurrentTimeServer and Adafruit-Clue and Clue-Clock

The BluetoothTimeServer is a tiny C# app ((Store link)[https://apps.microsoft.com/store/detail/simple-bluetooth-current-time-service/9NJQ3TD3K06F]) that acts as a Bluetooth SIG "Current Time Service" source.

The Adafruit Clue-clock is a couple of Python files that 
- have Python code to read from the Current Time Service
- act as a clock that can reset itself.

That directory also blog posts, etc, about the project.