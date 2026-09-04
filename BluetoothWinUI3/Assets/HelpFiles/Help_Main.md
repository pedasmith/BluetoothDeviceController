# Simple Bluetooth Device Controller [[version]]

The Simple Bluetooth Device Controller app automatically connects to and display information from a variety of common Bluetooth devices. Most devices in range will be automtically detected and displayed without you having to do anything.



## Supported devices

Supported devices include

|Type|Supported|
|----|----|
|Bike sensors|Bluetooth standard bike cadence and speed sensors
|Environment Sensors|[Nordic Thingy:52](./Device_Nordic_Thingy.md) Govee H5074 H5075 H5103 H5106 H5171 H5179 <br>Ruuvi Air and other sensors<br>SensorPro T201<br>ThermPro TP351 TP357 PT359 |
|Heart Rate bands|Bluetooth standard heart rate bands
|Pulse Oximeters|Viatom compatible PC-60F<br>Vibeat S5W S6W<br>Wellvue KS-60FW



## Menus

### **File** menu

* **File** > **Keep Screen On** . When checked, the screen will stay on. This is useful when you want the app to always be visible.
* **File** > **Copy Graph as PNG** copies the current graph to the clipboard as a PNG image. To save the graph, paste it into Paint or other image editor.
* **File** > **Copy Data for Excel** copies the current data to the clipboard in a format that's easily pastable into Excel.
* **File** > **Copy Data as CSV** copies the current data to the clipboard in CSV (Comma Separated Values) format. This is a common format to import into analysis programs
* **File** > **Copy Details** copies the details of the selected device to the clipboard.
* **File** > **Copy Details All** copies the details of all devices to the clipboard.
* **File** > **Clear Data** clears the data for the selected device.
* **File** > **Exit** . Exits the app


### **View** menu

* **View** > **Highlight** lets you highlight a specific line on a graph. This won't be selectable if there's no graph in the selected device.
* **View** > **Set Large**, when checked, makes the selected device larger until it's almost full screen.
* **View** > **Show Table**, when checked, displays the data for the selected device in a table format instead of a graph format. This won't be selectable if there's no graph in the selected device.

### **Bluetooth Device** menu

These are all settings which are set seperately for each individual device. For example, if you own two Nordic Thingy:52 devices, you can set their names and background and text colors seperately.

* **Bluetooth Device** > **Rename** . The default name for each device is the device it's advertised as. For example, a Nordic Thingy:52 will be advertised as a "Thingy". But often a specific device should be named . You might have a sensor in both your kitchen and your living room, and might want to given them easily-remembered named.
* **Bluetooth Device** > **Background Color** and > **Text Color** . Sets your preferred background color and text color for the device.
* **Bluetooth Device** > **Graph Colors** leads to a sub-menu that lets you pick your preferred color for each line of the graph.

### **Preferences** menu

The preferences menu lets you set your preferences that will be used for all devices.

* **Preferences** > **Temperature Units** lets you pick your preferred temperature unit. This is commonly either Celcius or Fahrenheit, but other choices are availabl.

* **Preferences** > **Pressure Units** lets you pick your preferred pressure unit. Most sensors will return their data in milliBars (otherwise called hPA, or hecto-Pascals), but weather data is often displayed in other units.

* **Preferences** > **Distance Units** leads to a sub-menu that lets you pick your preferred distance units including Kilometers and miles.


### **Help** menu

Provides information about the app

* **Help** > **View Help** displays this help text.

* **Help** > **About** shows a dialog box with version and contact information




## Helpful Links

* [Release Notes](./Release_Notes.md)
* [Nordic Semiconductor Thingy:52](https://www.nordicsemi.com/Products/Development-hardware/Nordic-Thingy-52)


