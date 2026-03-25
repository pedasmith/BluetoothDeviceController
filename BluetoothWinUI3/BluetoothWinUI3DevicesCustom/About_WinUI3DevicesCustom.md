# Custom devices use hand-crafted code

Previous versions of this program (BluetoothDeviceController) tried to automatically generate the Bluetooth code and UX to control a device. That UX was always awful: the grpahs looked terrible and they weren't flexible.

Note that the intent is to still use the automatically generated protocol code

Each of these represents a single *supported* device.

AND: they aren't in this directory because Visual Studio 2026 is dumb as box of rocks. If you put a WinUI3 control into a folder, it will compile but won't run! Because Jeez!

[blog post](https://sunriseprogrammer.blogspot.com/2026/03/fixing-dreaded-xaml-parsing-failed.html)