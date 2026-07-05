# Missing large pieces 2026-06-19


## Smart Export is started but missing all UX pieces

Finish Smart Export

## Automatically make Appearance from YAML data

This is complex because there is a value + sub-value, so it's a new type for me.

## Create Bluetooth device checker

And add extra checks: maker shouldn't be "20" (heart monitor)
Firmware and software should generally be different (or only one included)

## Multi-hour running issues

* The get all detail data for adverts becomes huge and barely works with notepad
* Take a long time to catch up the adverts
* Thingy graph is out of chronological order

# Devices

## Heart Rate improvements

Make the 

## The cycle speed and cadence should show normal bike data instead of weird data. 

Make seperate Crank and Wheel? 

Actually: make a Crank, Wheel, and pulse/O2 health

## Air Lab has a protocol! And copilot knows about it

Sensything! S1 Pro v1.0
- https://github.com/sensy-one/S1-Pro-Multi-Sense
- https://sensy-one.com/products/s1-pro-multi-sense-1

Flashed to version 1.2.21 2026-06-20; starts up in Wi-Fi provisioning mode improv-wifi.com

## Improv Wi-Fi provisioning!

https://www.improv-wifi.com/ble/
Protocol docs at https://www.improv-wifi.com/ble/


# Completed items

## Complete: Lifecycle information 2026-06-22
The known device data is supposed to include lifecycle information: last time a beacon was seen, etc. This is entirely missing. In particular: C:\Users\toomr\OneDrive\Documents\BluetoothDevices AllDeviceData.json, for each known device, there's a history section that's never filled in. It should be filled in and then written out (every hour?)


Status 2026-06-22: Advertisement-based known devices show up now
Status 2026-06-21: Completed and verified.
Status 2026-06-20: known device data yes, but doesn't write out automatically. Save every minute for 10 minutes, then every ten minutes 5 times, then every hour?
