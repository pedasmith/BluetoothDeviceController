# How the BluetoothWinUI3 project works

## Connecting to Bluetooth devices

* The devices are found by the bluetooth advertisement watcher. The MainWindow AdvertisementWatcher_WatcherEvent will be called with the watcher event.
* If the advertisement is a supported device (e.g., "A thingy 52") then a new UserControl of the right type is made. The control is added to the list of known devices
* The Control's DataContext is set to the KnownDevice. That data include the original advertisement and therefore the Bluetooth address.