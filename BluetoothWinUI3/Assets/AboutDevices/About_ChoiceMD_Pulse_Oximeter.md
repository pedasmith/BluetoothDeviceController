# Pulse Oximeters

The app supports several pulse oximeters.

* 2024 **Viatom** PC60FW
* 2026 **Innovo** iP900P-B (ChoiceMD)
* 2026 **Zac VRate** 500E-B (ChoiceMD)

# Program updates

The IOT Number formats now includes EQ and NE specifically to handle these ChoiceMD devices. These device send out either a type 3E or 01 data. The Type 3E is the pulse data; the 01 is single byte pulse ('pleth') data.

The devices have the unique and bad property that merely connecting to the Nordic service 6e400001-b5a3-f393-e0a9-e50e24dcca9e and putting a Notify on the Transmit characteristic FFF1 isn't enough to start data flowing. Instead you also have to do an "indicate" on FFF0


# Raw Data

## Advertisement data for Innovo iP900P-B

```
Event time: 2026-08-16 09:32:01.503
Address: F6:50:E5:F4:2B:00
Address type: Random
Advertisement type: ConnectableUndirected
Flags: Connectable
Signal strength (dBm): -61
Transmit power (dBm): 
Timestamp: 2026-08-16 09:32:01.503
Section: Flags: LE General Discoverable Mode+BR/EDR Not Supported
Section: Service UUIDs (complete): 6E400001-B5A3-F393-E0A9-E50E24DCCA9E
Section: Name: iP900BPB


Event time: 2026-08-16 09:32:01.504
Address: F6:50:E5:F4:2B:00
Address type: Random
Advertisement type: ScanResponse
Flags: Connectable,ScanResponse
Signal strength (dBm): -61
Transmit power (dBm): 
Timestamp: 2026-08-16 09:32:01.504
Section: Flags: LE General Discoverable Mode+BR/EDR Not Supported
Section: Service UUIDs (complete): 6E400001-B5A3-F393-E0A9-E50E24DCCA9E
Section: Name: iP900BPB
Section: section RandomTargetAddress data=F6 50 E5 F4 2B 00
```


## Service Characteristics


```
Services for F6:50:E5:F4:2B:00 iP900BPB

Service GAP Uuid=1800  handle=1
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-f6:50:e5:f4:2b:00#GATT:00000001:{00001800-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=Device Name Uuid=2A00 handle=2
        Properties: Read
        Protection Level: Plain
        Read: iP900BPB
    Characteristic name=Appearance Uuid=2A01 handle=4
        Properties: Read
        Protection Level: Plain
        Read: 00
    Characteristic name=Peripheral Privacy Flag Uuid=2A02 handle=6
        Properties: Read
        Protection Level: Plain
        Read: 00
    Characteristic name=Reconnection Address Uuid=2A03 handle=8
        Properties: Write
        Protection Level: Plain
    Characteristic name=Peripheral Preferred Connection Parameters Uuid=2A04 handle=10
        Properties: Read
        Protection Level: Plain
        Read: 18 00 24 00 00 00 C8 00

Service Device Information Uuid=180A  handle=12
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-f6:50:e5:f4:2b:00#GATT:0000000c:{0000180a-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=Model Number String Uuid=2A24 handle=13
        Properties: Read
        Protection Level: Plain
        Read: C228_D4
    Characteristic name=Serial Number String Uuid=2A25 handle=15
        Properties: Read
        Protection Level: Plain
        Read: 433232385F4434
    Characteristic name=Software Revision String Uuid=2A28 handle=17
        Properties: Read
        Protection Level: Plain
        Read: 1.0.0
    Characteristic name=Manufacturer Name String Uuid=2A29 handle=19
        Properties: Read
        Protection Level: Plain
        Read: ChoiceMMed

Service Battery Uuid=180F  handle=21
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-f6:50:e5:f4:2b:00#GATT:00000015:{0000180f-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=Battery Level Uuid=2A19 handle=22
        Properties: Read
        Protection Level: Plain
        Read: 43

Service Uuid=6e400001-b5a3-f393-e0a9-e50e24dcca9e  handle=26
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-f6:50:e5:f4:2b:00#GATT:0000001a:{6e400001-b5a3-f393-e0a9-e50e24dcca9e}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=FFF0 Uuid=FFF0 handle=27
        Properties: Indicate
        Protection Level: Plain
    Characteristic name=FFF1 Uuid=FFF1 handle=30
        Properties: Notify
        Protection Level: Plain
    Characteristic name=FFF2 Uuid=FFF2 handle=33
        Properties: Read
        Protection Level: Plain
        Read: 43 00 20 00 24 08 00

Service Uuid=00000001-0000-6465-6d6d-65636c6f6843  handle=35
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-f6:50:e5:f4:2b:00#GATT:00000023:{00000001-0000-6465-6d6d-65636c6f6843}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic Uuid=00000002-0000-6465-6d6d-65636c6f6843 handle=36
        Properties: Read
        Protection Level: Plain
        Read: F6 50 E5 F4 2B 00
    Characteristic Uuid=00000003-0000-6465-6d6d-65636c6f6843 handle=39
        Properties: Notify
        Protection Level: Plain
    Characteristic Uuid=00000004-0000-6465-6d6d-65636c6f6843 handle=42
        Properties: Write
        Protection Level: Plain
    Characteristic Uuid=00000005-0000-6465-6d6d-65636c6f6843 handle=44
        Properties: Read
        Protection Level: Plain
        Read: 00

Service  Uuid=FF00  handle=46
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-f6:50:e5:f4:2b:00#GATT:0000002e:{0000ff00-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=FF01 Uuid=FF01 handle=47
        Properties: Read, WriteWithoutResponse, Write, Notify
        Protection Level: Plain
        Read: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
    Characteristic name=FF02 Uuid=FF02 handle=50
        Properties: Read, WriteWithoutResponse, Write
        Protection Level: Plain
        Read: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
    Characteristic name=FF03 Uuid=FF03 handle=52
        Properties: Read, WriteWithoutResponse, Write
        Protection Level: Plain
        Read: 00
```

```JSON

{
  "AllDevices": [
    {
      "Name": "iP900BPB",
      "CompletionStatus": 0,
      "Services": [
        {
          "UUID": "00001800-0000-1000-8000-00805f9b34fb",
          "Name": "GAP",
          "Characteristics": [
            {
              "UUID": "00002a00-0000-1000-8000-00805f9b34fb",
              "Name": "Device Name",
              "Type": "STRING|ASCII|Device_Name",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "iP900BPB"
              ]
            },
            {
              "UUID": "00002a01-0000-1000-8000-00805f9b34fb",
              "Name": "Appearance",
              "Type": "U16|Speciality^Appearance|Appearance",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "00"
              ]
            },
            {
              "UUID": "00002a02-0000-1000-8000-00805f9b34fb",
              "Name": "Peripheral Privacy Flag",
              "Type": "U8|DEC|Flag",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "00"
              ]
            },
            {
              "UUID": "00002a03-0000-1000-8000-00805f9b34fb",
              "Name": "Reconnection Address",
              "Type": "BYTES|HEX|ReconnectAddress",
              "DataGroupName": "GAP_Data",
              "IsWrite": true,
              "Verbs": ":Write:WrWw:"
            },
            {
              "UUID": "00002a04-0000-1000-8000-00805f9b34fb",
              "Name": "Peripheral Preferred Connection Parameters",
              "Type": "U16^1.25_*|DEC|Interval_Min|ms U16^1.15_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "18 00 24 00 00 00 C8 00"
              ]
            }
          ]
        },
        {
          "UUID": "0000180a-0000-1000-8000-00805f9b34fb",
          "Name": "Device Information",
          "Characteristics": [
            {
              "UUID": "00002a24-0000-1000-8000-00805f9b34fb",
              "Name": "Model Number String",
              "Type": "STRING|ASCII|ModelNumber",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "C228_D4"
              ]
            },
            {
              "UUID": "00002a25-0000-1000-8000-00805f9b34fb",
              "Name": "Serial Number String",
              "Type": "STRING|ASCII|SerialNumber",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "433232385F4434"
              ]
            },
            {
              "UUID": "00002a28-0000-1000-8000-00805f9b34fb",
              "Name": "Software Revision String",
              "Type": "STRING|ASCII|SoftwareRevision",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "1.0.0"
              ]
            },
            {
              "UUID": "00002a29-0000-1000-8000-00805f9b34fb",
              "Name": "Manufacturer Name String",
              "Type": "STRING|ASCII|ManufacturerName",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "ChoiceMMed"
              ]
            }
          ]
        },
        {
          "UUID": "0000180f-0000-1000-8000-00805f9b34fb",
          "Name": "Battery",
          "Characteristics": [
            {
              "UUID": "00002a19-0000-1000-8000-00805f9b34fb",
              "Name": "Battery Level",
              "Type": "I8|DEC|BatteryLevel|%",
              "DataGroupName": "Battery_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "43"
              ]
            }
          ]
        },
        {
          "UUID": "6e400001-b5a3-f393-e0a9-e50e24dcca9e",
          "Name": "Transmit",
          "Characteristics": [
            {
              "UUID": "0000fff0-0000-1000-8000-00805f9b34fb",
              "Name": "FFF0",
              "Type": "BYTES|HEX|Unknown0",
              "DataGroupName": "Transmit_Data",
              "IsIndicate": true,
              "Verbs": ":Indicate:RdInNo:InNo:"
            },
            {
              "UUID": "0000fff1-0000-1000-8000-00805f9b34fb",
              "Name": "FFF1",
              "Type": "BYTES|HEX|Unknown1",
              "DataGroupName": "Transmit_Data",
              "IsNotify": true,
              "Verbs": ":Notify:RdInNo:InNo:"
            },
            {
              "UUID": "0000fff2-0000-1000-8000-00805f9b34fb",
              "Name": "FFF2",
              "Type": "BYTES|HEX|Unknown2",
              "DataGroupName": "Transmit_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "43 00 20 00 24 08 00"
              ]
            }
          ]
        },
        {
          "UUID": "00000001-0000-6465-6d6d-65636c6f6843",
          "Name": "Unknown4",
          "Characteristics": [
            {
              "UUID": "00000002-0000-6465-6d6d-65636c6f6843",
              "Name": "Unknown0",
              "Type": "BYTES|HEX|Unknown0",
              "DataGroupName": "Unknown4_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "F6 50 E5 F4 2B 00"
              ]
            },
            {
              "UUID": "00000003-0000-6465-6d6d-65636c6f6843",
              "Name": "Unknown1",
              "Type": "BYTES|HEX|Unknown1",
              "DataGroupName": "Unknown4_Data",
              "IsNotify": true,
              "Verbs": ":Notify:RdInNo:InNo:"
            },
            {
              "UUID": "00000004-0000-6465-6d6d-65636c6f6843",
              "Name": "Unknown2",
              "Type": "BYTES|HEX|Unknown2",
              "DataGroupName": "Unknown4_Data",
              "IsWrite": true,
              "Verbs": ":Write:WrWw:"
            },
            {
              "UUID": "00000005-0000-6465-6d6d-65636c6f6843",
              "Name": "Unknown3",
              "Type": "BYTES|HEX|Unknown3",
              "DataGroupName": "Unknown4_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "00"
              ]
            }
          ]
        },
        {
          "UUID": "0000ff00-0000-1000-8000-00805f9b34fb",
          "Characteristics": [
            {
              "UUID": "0000ff01-0000-1000-8000-00805f9b34fb",
              "Name": "FF01",
              "Type": "BYTES|HEX|Unknown0",
              "DataGroupName": "_Data",
              "IsRead": true,
              "IsWrite": true,
              "IsWriteWithoutResponse": true,
              "IsNotify": true,
              "Verbs": ":Read:Write:WriteWithoutResponse:Notify:RdInNo:InNo:WrWw:",
              "ExampleData": [
                "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00"
              ]
            },
            {
              "UUID": "0000ff02-0000-1000-8000-00805f9b34fb",
              "Name": "FF02",
              "Type": "BYTES|HEX|Unknown1",
              "DataGroupName": "_Data",
              "IsRead": true,
              "IsWrite": true,
              "IsWriteWithoutResponse": true,
              "Verbs": ":Read:Write:WriteWithoutResponse:RdInNo:WrWw:",
              "ExampleData": [
                "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00"
              ]
            },
            {
              "UUID": "0000ff03-0000-1000-8000-00805f9b34fb",
              "Name": "FF03",
              "Type": "BYTES|HEX|Unknown2",
              "DataGroupName": "_Data",
              "IsRead": true,
              "IsWrite": true,
              "IsWriteWithoutResponse": true,
              "Verbs": ":Read:Write:WriteWithoutResponse:RdInNo:WrWw:",
              "ExampleData": [
                "00"
              ]
            }
          ]
        }
      ],
      "Details": "TODO: line 190"
    }
  ]
}
```

## Advertisement data for Zac VRate 

```Event time: 2026-08-16 09:39:54.380
Address: C2:25:F8:1D:1F:56
Address type: Random
Advertisement type: ScanResponse
Flags: Connectable,ScanResponse
Signal strength (dBm): -60
Transmit power (dBm): 
Timestamp: 2026-08-16 09:39:54.380
Section: Flags: LE General Discoverable Mode+BR/EDR Not Supported
Section: Service UUIDs (complete): 6E400001-B5A3-F393-E0A9-E50E24DCCA9E
Section: Name: 500E-B
Section: section RandomTargetAddress data=C2 25 F8 1D 1F 56
```

## Service and Characteristic data for Zac VRate

```
Services for C2:25:F8:1D:1F:56 500E-B

Service GAP Uuid=1800  handle=1
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-c2:25:f8:1d:1f:56#GATT:00000001:{00001800-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=Device Name Uuid=2A00 handle=2
        Properties: Read
        Protection Level: Plain
        Read: 500E-B
    Characteristic name=Appearance Uuid=2A01 handle=4
        Properties: Read
        Protection Level: Plain
        Read: 00
    Characteristic name=Peripheral Privacy Flag Uuid=2A02 handle=6
        Properties: Read
        Protection Level: Plain
        Read: 00
    Characteristic name=Reconnection Address Uuid=2A03 handle=8
        Properties: Write
        Protection Level: Plain
    Characteristic name=Peripheral Preferred Connection Parameters Uuid=2A04 handle=10
        Properties: Read
        Protection Level: Plain
        Read: 18 00 24 00 00 00 C8 00

Service Device Information Uuid=180A  handle=12
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-c2:25:f8:1d:1f:56#GATT:0000000c:{0000180a-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=Model Number String Uuid=2A24 handle=13
        Properties: Read
        Protection Level: Plain
        Read: C228_D4
    Characteristic name=Serial Number String Uuid=2A25 handle=15
        Properties: Read
        Protection Level: Plain
        Read: 433232385F4434
    Characteristic name=Software Revision String Uuid=2A28 handle=17
        Properties: Read
        Protection Level: Plain
        Read: 1.0.0
    Characteristic name=Manufacturer Name String Uuid=2A29 handle=19
        Properties: Read
        Protection Level: Plain
        Read: ChoiceMMed

Service Battery Uuid=180F  handle=21
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-c2:25:f8:1d:1f:56#GATT:00000015:{0000180f-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=Battery Level Uuid=2A19 handle=22
        Properties: Read
        Protection Level: Plain
        Read: 42

Service Uuid=6e400001-b5a3-f393-e0a9-e50e24dcca9e  handle=26
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-c2:25:f8:1d:1f:56#GATT:0000001a:{6e400001-b5a3-f393-e0a9-e50e24dcca9e}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=FFF0 Uuid=FFF0 handle=27
        Properties: Indicate
        Protection Level: Plain
    Characteristic name=FFF1 Uuid=FFF1 handle=30
        Properties: Notify
        Protection Level: Plain
    Characteristic name=FFF2 Uuid=FFF2 handle=33
        Properties: Read
        Protection Level: Plain
        Read: 43 11 20 00 24 08 00

Service Uuid=00000001-0000-6465-6d6d-65636c6f6843  handle=35
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-c2:25:f8:1d:1f:56#GATT:00000023:{00000001-0000-6465-6d6d-65636c6f6843}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic Uuid=00000002-0000-6465-6d6d-65636c6f6843 handle=36
        Properties: Read
        Protection Level: Plain
        Read: C2 25 F8 1D 1F 56
    Characteristic Uuid=00000003-0000-6465-6d6d-65636c6f6843 handle=39
        Properties: Notify
        Protection Level: Plain
    Characteristic Uuid=00000004-0000-6465-6d6d-65636c6f6843 handle=42
        Properties: Write
        Protection Level: Plain
    Characteristic Uuid=00000005-0000-6465-6d6d-65636c6f6843 handle=44
        Properties: Read
        Protection Level: Plain
        Read: 00

Service  Uuid=FF00  handle=46
    AccessInformation: status=Allowed prompt=False
    DeviceId=BluetoothLE#BluetoothLE2c:0d:a7:c8:53:33-c2:25:f8:1d:1f:56#GATT:0000002e:{0000ff00-0000-1000-8000-00805f9b34fb}
    Session: Status=Active MaxPduSize (MTU)=23
    Session: CanMaintainConnection=False MaintainConnection=False
    Characteristic name=FF01 Uuid=FF01 handle=47
        Properties: Read, WriteWithoutResponse, Write, Notify
        Protection Level: Plain
        Read: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
    Characteristic name=FF02 Uuid=FF02 handle=50
        Properties: Read, WriteWithoutResponse, Write
        Protection Level: Plain
        Read: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
    Characteristic name=FF03 Uuid=FF03 handle=52
        Properties: Read, WriteWithoutResponse, Write
        Protection Level: Plain
        Read: 00
```


```JSON
{
  "AllDevices": [
    {
      "Name": "500E-B",
      "CompletionStatus": 0,
      "Services": [
        {
          "UUID": "00001800-0000-1000-8000-00805f9b34fb",
          "Name": "GAP",
          "Characteristics": [
            {
              "UUID": "00002a00-0000-1000-8000-00805f9b34fb",
              "Name": "Device Name",
              "Type": "STRING|ASCII|Device_Name",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "500E-B"
              ]
            },
            {
              "UUID": "00002a01-0000-1000-8000-00805f9b34fb",
              "Name": "Appearance",
              "Type": "U16|Speciality^Appearance|Appearance",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "00"
              ]
            },
            {
              "UUID": "00002a02-0000-1000-8000-00805f9b34fb",
              "Name": "Peripheral Privacy Flag",
              "Type": "U8|DEC|Flag",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "00"
              ]
            },
            {
              "UUID": "00002a03-0000-1000-8000-00805f9b34fb",
              "Name": "Reconnection Address",
              "Type": "BYTES|HEX|ReconnectAddress",
              "DataGroupName": "GAP_Data",
              "IsWrite": true,
              "Verbs": ":Write:WrWw:"
            },
            {
              "UUID": "00002a04-0000-1000-8000-00805f9b34fb",
              "Name": "Peripheral Preferred Connection Parameters",
              "Type": "U16^1.25_*|DEC|Interval_Min|ms U16^1.15_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms",
              "DataGroupName": "GAP_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "18 00 24 00 00 00 C8 00"
              ]
            }
          ]
        },
        {
          "UUID": "0000180a-0000-1000-8000-00805f9b34fb",
          "Name": "Device Information",
          "Characteristics": [
            {
              "UUID": "00002a24-0000-1000-8000-00805f9b34fb",
              "Name": "Model Number String",
              "Type": "STRING|ASCII|ModelNumber",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "C228_D4"
              ]
            },
            {
              "UUID": "00002a25-0000-1000-8000-00805f9b34fb",
              "Name": "Serial Number String",
              "Type": "STRING|ASCII|SerialNumber",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "433232385F4434"
              ]
            },
            {
              "UUID": "00002a28-0000-1000-8000-00805f9b34fb",
              "Name": "Software Revision String",
              "Type": "STRING|ASCII|SoftwareRevision",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "1.0.0"
              ]
            },
            {
              "UUID": "00002a29-0000-1000-8000-00805f9b34fb",
              "Name": "Manufacturer Name String",
              "Type": "STRING|ASCII|ManufacturerName",
              "DataGroupName": "Device Information_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "ChoiceMMed"
              ]
            }
          ]
        },
        {
          "UUID": "0000180f-0000-1000-8000-00805f9b34fb",
          "Name": "Battery",
          "Characteristics": [
            {
              "UUID": "00002a19-0000-1000-8000-00805f9b34fb",
              "Name": "Battery Level",
              "Type": "I8|DEC|BatteryLevel|%",
              "DataGroupName": "Battery_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "42"
              ]
            }
          ]
        },
        {
          "UUID": "6e400001-b5a3-f393-e0a9-e50e24dcca9e",
          "Name": "TransmitNordic",
          "Priority": 10,
          "Characteristics": [
            {
              "UUID": "0000fff0-0000-1000-8000-00805f9b34fb",
              "Name": "FFF0",
              "Type": "BYTES|HEX|TransmitData",
              "DataGroupName": "TransmitNordic_Data",
              "IsIndicate": true,
              "Verbs": ":Indicate:RdInNo:InNo:"
            },
            {
              "UUID": "0000fff1-0000-1000-8000-00805f9b34fb",
              "Name": "FFF1",
              "Type": "U8|HEX|Opcode OSKIP^9^$Opcode_GN_62_EQ_NT U16|DEC|OxygenSaturationInPercent U16|DEC|PulseRate|bpm U8|DEC|RespirationRate U8|DEC|Unknown10 U8|DEC|Unknown11 U8|DEC|Unknown12 U8|DEC|Unknown13 U8|DEC|Unknown14 U8^10_/|FIXED|PerfusionIndexInPercent OSKIP^1^$Opcode_GN_01_EQ_NT U8|DEC|PulseData OOPT BYTES|HEX|RestOfData",
              "DataGroupName": "TransmitNordic_Data",
              "IsNotify": true,
              "Verbs": ":Notify:RdInNo:InNo:"
            },
            {
              "UUID": "0000fff2-0000-1000-8000-00805f9b34fb",
              "Name": "FFF2",
              "Type": "BYTES|HEX|ReceiveDataRead",
              "DataGroupName": "TransmitNordic_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "43 11 20 00 24 08 00"
              ]
            }
          ]
        },
        {
          "UUID": "00000001-0000-6465-6d6d-65636c6f6843",
          "Name": "ServiceControl0001",
          "Characteristics": [
            {
              "UUID": "00000002-0000-6465-6d6d-65636c6f6843",
              "Name": "ReadC0002",
              "Type": "BYTES|HEX|ReadC0002",
              "DataGroupName": "ServiceControl0001_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "C2 25 F8 1D 1F 56"
              ]
            },
            {
              "UUID": "00000003-0000-6465-6d6d-65636c6f6843",
              "Name": "NotifyC0003",
              "Type": "BYTES|HEX|NotifyC0003",
              "DataGroupName": "ServiceControl0001_Data",
              "IsNotify": true,
              "Verbs": ":Notify:RdInNo:InNo:"
            },
            {
              "UUID": "00000004-0000-6465-6d6d-65636c6f6843",
              "Name": "WriteC0004",
              "Type": "BYTES|HEX|WriteC0004",
              "DataGroupName": "ServiceControl0001_Data",
              "IsWrite": true,
              "Verbs": ":Write:WrWw:"
            },
            {
              "UUID": "00000005-0000-6465-6d6d-65636c6f6843",
              "Name": "ReadC0005",
              "Type": "BYTES|HEX|ReadC0005",
              "DataGroupName": "ServiceControl0001_Data",
              "IsRead": true,
              "Verbs": ":Read:RdInNo:",
              "ExampleData": [
                "00"
              ]
            }
          ]
        },
        {
          "UUID": "0000ff00-0000-1000-8000-00805f9b34fb",
          "Characteristics": [
            {
              "UUID": "0000ff01-0000-1000-8000-00805f9b34fb",
              "Name": "FF01",
              "Type": "BYTES|HEX|FF01",
              "DataGroupName": "_Data",
              "IsRead": true,
              "IsWrite": true,
              "IsWriteWithoutResponse": true,
              "IsNotify": true,
              "Verbs": ":Read:Write:WriteWithoutResponse:Notify:RdInNo:InNo:WrWw:",
              "ExampleData": [
                "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00"
              ]
            },
            {
              "UUID": "0000ff02-0000-1000-8000-00805f9b34fb",
              "Name": "FF02",
              "Type": "BYTES|HEX|FF02",
              "DataGroupName": "_Data",
              "IsRead": true,
              "IsWrite": true,
              "IsWriteWithoutResponse": true,
              "Verbs": ":Read:Write:WriteWithoutResponse:RdInNo:WrWw:",
              "ExampleData": [
                "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00"
              ]
            },
            {
              "UUID": "0000ff03-0000-1000-8000-00805f9b34fb",
              "Name": "FF03",
              "Type": "BYTES|HEX|FF03",
              "DataGroupName": "_Data",
              "IsRead": true,
              "IsWrite": true,
              "IsWriteWithoutResponse": true,
              "Verbs": ":Read:Write:WriteWithoutResponse:RdInNo:WrWw:",
              "ExampleData": [
                "00"
              ]
            }
          ]
        }
      ],
      "Details": "TODO: line 190"
    }
  ]
}```