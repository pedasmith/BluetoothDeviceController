# Simple Bluetooth Device Controller [[version]]

The Simple Bluetooth Device Controller app automatically connects to and display information from a variety of common Bluetooth devices. Most devices in range will be automtically detected and displayed without you having to do anything.



## Supported devices

Supported devices include

The [Nordic Thingy:52](./Device_Nordic_Thingy.md)


## Menus

### **File** menu

* **File** > **Keep Screen On** . When checked, the screen will stay on. This is useful when you want the app to always be visible.

* **File** > **Exit** . Exits the app

### **Bluetooth Device** menu

These are all settings which are set seperately for each individual device. For example, if you own two Nordic Thingy:52 devices, you can set their names and background and text colors seperately.

* **Bluetooth Device** > **Rename** . The default name for each device is the device it's advertised as. For example, a Nordic Thingy:52 will be advertised as a "Thingy". But often a specific device should be named . You might have a sensor in both your kitchen and your living room, and might want to given them easily-remembered named.
* **Bluetooth Device** > **Bsckground Color** and > **Text Color** . Sets your preferred background color and text color for the device.
* **Bluetooth Device** > **Graph Colors** leads to a sub-menu that lets you pick your preferred color for each line of the graph.

### **Preferences** menu

The preferences menu lets you set your preferences that will be used for all devices.

* **Preferences** > **Temperature Units** lets you pick your preferred temperature unit. This is commonly either Celcius or Fahrenheit, but other choices are availabl.

* **Preferences** > **Pressure Units** lets you pick your preferred pressure unit. Most sensors will return their data in milliBars (otherwise called hPA, or hecto-Pascals), but weather data is often displayed in other units.

### **Help** menu

Provides information about the app

* **Help** > **View Help** displays this help text.

* **Help** > **About** shows a dialog box with version and contact information




## Helpful Links

* [Release Notes](./Release_Notes.md)
* [Nordic Semiconductor Thingy:52](https://www.nordicsemi.com/Products/Development-hardware/Nordic-Thingy-52)


