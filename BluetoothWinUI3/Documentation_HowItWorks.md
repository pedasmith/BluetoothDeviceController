# How the BluetoothWinUI3 project works

## Connecting to Bluetooth devices

* The devices are found by the bluetooth advertisement watcher. The MainWindow AdvertisementWatcher_WatcherEvent will be called with the watcher event.
* If the advertisement is a supported device (e.g., "A thingy 52") then a new UserControl of the right type is made. The control is added to the list of known devices
* The Control's DataContext is set to the KnownDevice. That data include the original advertisement and therefore the Bluetooth address.





## Programming Notes

### Menus and keyboard shortcuts

[Picking the right menu type](https://learn.microsoft.com/en-us/windows/apps/design/basics/commanding-basics)

Shortcut keys are via the AccessKey option on each menu item.

