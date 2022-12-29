# Govee H5074 Smart Thermo-Hygrometer

![H5074](../DevicePictures/Govee_H5074-175.png)

The Govee H5074 thermostat + humidity sensor is a small, coin-cell battery (CR2477) powered smart sensor. It sends data over advertisements and does not need to be connected.

The Govee website no longer mentions the H5074


# Example

![Environmental Data](../ScreenShots/Device_Govee_H5074.png)

# Connecting

Unlike most other Bluetooth device, you must scan for the Govee device using the "BLE Advertisements and beacons" settings. In the app, click Scan if needed to stop the scan. Then select the Settings and set the "Show which device" to "BLE Advertisements and beacons" and click OK. Now click Scan to start scanning and wait for the Govee device to show up (might take up to a minute).

![Select BLE Advertisements](../ScreenShots/Settings_Advertisements.png)

Click on the device to see the display. The app doesn't scan indefinitely for data; you will need to click 'Scan' every so often to keep the data moving into the app.


# Useful Links

* [Github](https://github.com/wcbonner/GoveeBTTempLogger) or [Github ble_monitor](https://github.com/custom-components/ble_monitor)
* [Govee](https://us.govee.com/products/govee-bluetooth-indoor-thermo-hygrometer) compares their current sensors
* [Amazon](https://www.amazon.com/Govee-Thermometer-Hygrometer-Bluetooth-Temperature/dp/B07R586J37)