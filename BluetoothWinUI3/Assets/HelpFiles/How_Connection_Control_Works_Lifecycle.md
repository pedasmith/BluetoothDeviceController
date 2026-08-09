# Connection control and device lifetime

## TODO list for the connection control

- The connection control is ugly
- Add a connect ring
- DONE On status failures, disconnect
- DONE On device disconnect, set to disconnected
- On timer, reconnect

## Code that sets status

|Bluetooth call|State|Error|New State|Notes
|----|----|----|
|BluetoothLEDevice.FromBluetoothAddressAsync|Control Initializing|NoBLE| ?? |Unlikely Total failure
|ble.SetNotify*|Control Initializing|NoBLE| ?? |Unlikely total failure
|ble.Read*|Control Initializing|NoBLE| ?? |Unlikely total failure

## Sequence Diagram: connect and status flows

In the sequence diagram

* **sensor** is the Windows [BluetoothLEDevice](https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.bluetoothledevice?view=winrt-28000) device
* **protocolCS** is the protocol file that's generated from the protocol JSON file
* **controlCS** is the UserControl-derived object for the device. This is the large file with the OxyPlot, table, etc. code for UX
* **connection** is the BTConnectionControl. Most of the controlCS files include a BTConnectionControl for convenience.
* **mainwindow** is the MainWindow. It has the AdvertisementWatcher that triggers the creation of a Known device when the right advertisements are seen
* **user** is the user. They click a button sometimes :-)

```mermaid
sequenceDiagram
participant sensor
participant protocolCS
participant controlCS
participant connection
participant mainwindow
participant user

mainwindow ->> controlCS: DataContextChanged()
controlCS ->> protocolCS: new device()
controlCS ->> sensor: device.ble = FromBluetoothAddress()
controlCS ->> protocolCS: SetNotify() etc
sensor ->> protocolCS: BT Notify
protocolCS ->> controlCS: INCP
protocolCS -->> controlCS: call Status_OnBluetoothStatus()
sensor -->> controlCS: cll Ble_ConnectionStatusChanged()
controlCS -->> connection: call Device_Disconnected()
user -->> connection: User clicks connect
connection -->> controlCS: DoReconnect()
```

## Flowchart for Bluetooth connection and data

```mermaid
---
config:
  look: handDrawn
  theme: neutral
---
flowchart TD
subgraph main["`**MainWindow.cs**`"]
	A[Advertisement] --> CheckKnown{KnownDevices.Get}
	CheckKnown --> |No| CheckSupported{Is supported?}
	CheckKnown --> |Yes| Call_HandleMyAdvertisement1
	CheckSupported -->|Yes| MakeControlAndAdd
	CheckSupported -->|No| NotSupported[Log & Ignore]
	MakeControlAndAdd --> Call_Set_DataContext
	Call_Set_DataContext --> Call_HandleMyAdvertisement2
	end

	subgraph IDeviceControlBasic[Control with IDeviceControlBasic]
	Call_HandleMyAdvertisement1[Call HandleMyAdvertisement if control is IHandleMyBTAdvertisements] 
		--> HandleMyAdvertisement
	Call_HandleMyAdvertisement2[Call HandleMyAdvertisement if control is IHandleMyBTAdvertisements] 
		--> HandleMyAdvertisement
	Call_Set_DeviceContext[Set DeviceContext] --> Control_DataContextChanged 
	Control_DataContextChanged{{Control_DataContextChanged}} --> InitializeUX
	InitializeUX --> new_DeviceSpecificType
	new_DeviceSpecificType 
		--> ble_BluetoothLEDevice.FromBluetoothAddressAsync["`set **ble** with BluetoothLEDevice. FromBluetoothAddressAsync `"]
	ble_BluetoothLEDevice.FromBluetoothAddressAsync 
		--> ble_OnBluetoothStatus
	ble_OnBluetoothStatus[Set up ble.OnBluetoothStatus]
		--> ble_SetupConnectionStatusChanged
	ble_SetupConnectionStatusChanged[Set up ble BluetoothLEDevice .ConnectionStatusChanged]
		--> ble_SetNotify
	ble_SetNotify[call ble.SetNotify*]
		--> ble_Read
	ble_Read[call ble.Read*]
		--> SetupCompleted
	SetupCompleted[DataContextChanged Complete]

	ble_ConnectionStatusChanged{{BluetoothLEDevice ConnectionStatusChanged callback}}
		--> UpdateConnectivity
	control_StatusEvent{{control OnBluetoothStatus callback}}
		--> UpdateConnectivity

	HandleMyAdvertisement{{HandeMyAdvertisement}} --> UpdateData

	Device_PropertyChanged --> UpdateData
end

subgraph ConnectionControl
	Reconnect[Reconnect Button]
		-->UpdateControlUX

	UpdateConnectivity-->cc_DeviceDisconnected
	cc_DeviceDisconnected[DeviceDisconnected]
		-->UpdateControlUX

	UpdateControlUX
		-->Control_DataContextChanged
end


subgraph ble[ble protocol file]
	BluetoothStatusEvent --> control_StatusEvent
	DevicePropertyChanged --> Device_PropertyChanged
end

subgraph BluetoothLEDevice[BluetoothLEDevice]
	ConnectionStatusChangedEvent[ConnectionStatusChanged event] 
		-->ble_ConnectionStatusChanged
	GotNotify[Got Notify from device]
		--> DevicePropertyChanged
end


classDef CanTriggerError stroke:#D44
class ble_BluetoothLEDevice.FromBluetoothAddressAsync CanTriggerErrocontrol
class ble_SetNotify CanTriggerError
class ble_Read CanTriggerError
class ConnectionStatusChangedEvent CanTriggerError

classDef NewCode20260808 stroke:#4D4,fill:#aFa
class control_StatusEvent NewCode20260808
class ble_ConnectionStatusChanged NewCode20260808
class ble_SetupConnectionStatusChanged NewCode20260808
class ble_OnBluetoothStatus NewCode20260808
class UpdateConnectivity NewCode20260808
class cc_DeviceDisconnected NewCode20260808
class UpdateControlUX NewCode20260808



```