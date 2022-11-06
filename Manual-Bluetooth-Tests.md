# Manual Bluetooth tests for Bluetooth Device Controller

When you do major changes to the Bluetooth Device Explorer, you will need

## Govee E0:17:54:D0:74:C5 + Smoke Test

Tests **advertisements** with scan responses for the **Sensor** display

* No need to pair.
* Start the app. Click 'scan' to stop the automatic scanning
* Click on Settings and switch to "BLE Advertisements and beacons" with full details and signal strength at about -80. Click OK.
* Click Scan to start scanning
* The Bluetooth Advertisement display should appear on a right with a bunch of found devices. Wait until the Govee shows up.
* *check* that the Govee shows data like below. It will also have other data like the advert start and an Apple-style advert.

```
Govee Data:: Temp=20.03℃ (68.054℉) Hum=46.08% Bat=100% (junk=0) 
```

* *check* that the Govee has a little graph icon
* click on the Govee. The display should shift to show a graph. Click 'Scan' to stop scanning if needed and then click 'Scan' again to start scanning.
* *check* that there's data coming in.
* Wait two minutes for data to show up. While waiting, put the sensor into a warm or cool place to get a variety of data (e.g., near a tea cup with hot tea)
* *check* that only Temeperature and Humidity are showing
* *check* that graph shows a variety of data
* click on "copy". *check* that the clipboard now has a CSV of the data.
* click on "copy for excel". Run excel, open a blank worksheet, and press ^V to paste. *check* that the data looks OK.
* click 'Scan' to stop the scan if needed. Then click 'Scan' to start the scan and wait 1 minute. *check* that there's more data and *check* that the old data was not discarded.

## Nordic Thingy

* Make sure the device is charged and the on/off switch is set to on (|). You may have to push the rubber cover aside.
* No need to pair. 
* Run the app. Click 'Scan' to stop the scan, then click 'settings'. Show BLE Devices with Names, Preferred Display=Specialized Display and make sure that When Editing, automatically read from device is checked. Then press OK
* Click 'scan' to start the scan.
* When the "Thingy" shows up *check* that the icon is a graph. Click 'Thingy' and *check* that the specialty display shows up.
* Click 'Notify' for temperature, pressure, humidity, and Air Quality. *check* that the values for temperature pressure and humidity show up quickly (within 30 seconds). The air quality takes longer and will start off as '0' values.