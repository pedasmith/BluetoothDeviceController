# Skoobot from William Weiler Engineering

![Robot](../DevicePictures/WilliamWeilerEngineering_Skoobot-175.png)

The Skoobot is a CrowdSupply pre-assembled robot. It's about a 1-inch cube, which makes it
the smallest and cutest of the robots supported by this program. 

The Bluetooth connection is via BluetoothLE using mostly unmodified set of bluetooth
values: you write a single byte as the command bytes; there's a 1-byte return value which is
a distance value, and there's a two-byte value which is the ambient light (big-endian). There's
also a 20-byte return value which is microphone data, plus an entirely unused 4-byte value.

The specialization uses the command enum values to create a grid of buttons with 
the classic "robot control" look. The device is set up to automatically notify for
distance and ambient light values; when you ask for these values, the screen will be
updated automatically.

# Screen Shot
![Screen Shot](../ScreenShots/Device_WilliamWeilerEngineering_Skoobot.png)

# Links
Some useful links to learn more about the Skoobot robot:

* [Web site](https://www.william-weiler-engineering.com/)
* [Facebook](https://www.facebook.com/skoobot/)
* [Github code](https://github.com/bweiler/Skoobot-firmware)
* [Original Crowdsupply funding](https://www.crowdsupply.com/william-weiler-engineering/skoobot)
* [Hackster article](https://www.hackster.io/william-weiler/skoobot-27222a)

