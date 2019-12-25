# BBC micro:bit 

![micro:bit](../DevicePictures/Bbc_MicroBit-175.png)

The BBC micro:bit is a small (half the size of a credit card) robust computer that's designed
to teach teen-agers how to program controllers. It's got multiple sensors built in and includes
an array (5x5) of red LEDs that can be controlled. You can also control a set of pins at the edge
of the board.

# Pairing and usage details
The device is designed to have a variety of different HEX files (program files) loaded; each HEX file has different 
characteristics including whether or not the device needs to be paired, access to the LED screen, and Bleutooth sensors.

**Important note about pairing**
For most of the HEX files, you must first pair the micro:bit with your Windows device. To do this, 

1. Press down the A and B keys and press the RESET button. The 5x5 LED screen will start to fill with dots
2. Wait until the screen is all the way filled
3. Release A and B keys. The screen will show a Bluetooth icon and then a little "bar" diagram. 
4. Now pair the device with Windows. No pairing number is needed.

Once the device is paired, it can be used directly from the program without any more pairing.

**You will also want to download a new HEX file** to use the full power of the device.

## Magnetometer notes
The Magnetometer Bearing value is often unreadable. To make it readable, write a 01 to the Magentometer Calibration characteristic; 
the user will then need to move the device around until the magnetometer is fully calibrated. Once this happens, you'll get 
bearing data. This is also when the Magnetometer Calibration value (which is notify-only) will be updated with a 02
to indicate that it's calibrated. 

Other values are 0=unknown calibration 1=calibration requested 2=calibration OK 3=calibration is complete with error

## Using the IO Pins
The BBN micro:bit IO Pins work differently than pins on many other Bluetooth data sensors. 

1. You must ask for data for each pin. The PinInput field is a bitwise field that says which pins are input pins and should be read. For example, if you want to read in pin 0, you have to set the PinInput to 2**0 which is 1. If you want to read in pin 1, you have to set PinInput to 2**1 which is 2.
2. You also have to set the PinAnalog in the same way. Every bit which is 1 will return an analog value.
3. The data returned is a list of tuples; each tuple is a pin number and a pin values. Pins that haven't changed will not be returned. This means that the actual output needs more handling than some other devices

The device also reads analog data much faster than many devices -- fast enough that the "keep 1000 items" choice may run slowly on your computer.

## What the different HEX files actually do, bluetooth-wise. 
The BBC micro:bit, out of the box, doesn't do any Bluetooth; you have to reprogram it with a HEX file
to provide any Bluetooth services. There are HEX files hosted by 
[Lancaster University](https://lancaster-university.github.io/microbit-docs/ble/profile/#all-services-enabled-hex-file)
that include all of the available micro:bit services. The most useful of the HEX files they host is the
[BLE All services DAL No DFU](https://lancaster-university.github.io/microbit-docs/resources/BLE_All_Services_DAL_2-1-1-No-DFU.hex.zip)
(Scrolls  BLE ABEFILMT/P/0 on the screen). Contains services for all of the sensors and the event system.
 
Some useful HEX files are at [Bitty Software](http://www.bittysoftware.com/downloads.html)

[Bitty Blue](https://drive.google.com/uc?id=0B2Ud_NaMFsQSWmJhSDBfZGdwd0k&export=download)
Doesn't seem to work that well. Asks the user to tilt the screen.

[Microbit blue](https://drive.google.com/uc?id=0B2Ud_NaMFsQSdm1BMVMtN3F4a3c&export=download) -- this is the download marked Main BluetoothServices. (Scrolls BLUE - NP on the screen) Doesn't include the magnetometer or IO pins.

