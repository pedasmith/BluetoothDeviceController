# Ruuvi RuuviTag

![RuuviTag](../DevicePictures/Ruuvi_RuuviTag-175.png)

The RuuviTag is a small round environmental sensor that transmits data using the EddyStone protocol. It can also
be programmed using the Espruino system. The sensor reports temperature, pressure, and relative humidity values.

The Kickstarter version of the device is supported by this program.

# Example

![Environmental Data](../ScreenShots/Device_Ruuvi_RuuviTag.png)

# Connecting

Unlike most other Bluetooth device, all EddyStone devices constantly broadcast a URL. To see these values, go 
to Settings and unders Show Which Devices?, pick a Beacon setting. The RuuviTag will be displayed as one
of the devices found. Note that it may be delayed in showing up; the beacon is only sent periodically, and until
the beacon data is received, it can't be shown.

# Useful Links

* [Ruuvi](https://ruuvi.com/ruuvitag-specs/)
* [Github](https://github.com/ruuvi)
* [URL specification](https://github.com/ruuvi/ruuvi-sensor-protocols)
* [Kickstarter](https://www.kickstarter.com/projects/463050344/ruuvitag-open-source-bluetooth-sensor-beacon)

