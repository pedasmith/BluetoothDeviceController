# Registration: supported and known devices

A *supported* device is a device that you can buy that's supported by this program. 

A *known* device is a device that you have already used with this program.

An example of the difference: if you've never used a Thingy device with this program, it's a supported device but not a known device. If you buy two thingy device and use them both with this program, then they are both known devices.

The list of supported devices is maintained in the source code of this program. They are identified by their Bluetooth device name as it's sent via advertisements.

The list of known devices is saved in a (JSON?) list and is found by Bluetooth address. Each known device also stores some history and the user-preferred name.