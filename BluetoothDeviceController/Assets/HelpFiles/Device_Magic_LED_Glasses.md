{
  "AllDevices": [
    {
      "Name": "GLASSES-01ED4F",
      "Services": [
        {
          "UUID": "00001800-0000-1000-8000-00805f9b34fb",
          "Name": "Common Configuration",
          "Characteristics": [
            {
              "UUID": "00002a00-0000-1000-8000-00805f9b34fb",
              "Name": "Device Name",
              "Type": "STRING|ASCII|Device_Name",
              "IsRead": true,
              "IsWrite": true,
              "ExampleData": [
                "Name: GLASSES-01ED4F",
                "GLASSES-01ED4F"
              ]
            },
            {
              "UUID": "00002a01-0000-1000-8000-00805f9b34fb",
              "Name": "Appearance",
              "Type": "U16|Speciality^Appearance|Appearance",
              "IsRead": true,
              "IsWrite": true,
              "ExampleData": [
                "Appearance:  00 00",
                "Unknown"
              ]
            },
            {
              "UUID": "00002a04-0000-1000-8000-00805f9b34fb",
              "Name": "Connection Parameter",
              "Type": "BYTES|HEX|ConnectionParameter",
              "IsRead": true,
              "ExampleData": [
                "Value:  2C 01 40 01 04 00 58 02"
              ]
            },
            {
              "UUID": "00002aa6-0000-1000-8000-00805f9b34fb",
              "Name": "Central Address Resolution",
              "Type": "U8|DEC|AddressResolutionSupported",
              "IsRead": true,
              "ExampleData": [
                "Value:  00",
                "0"
              ]
            }
          ]
        },
        {
          "UUID": "00001801-0000-1000-8000-00805f9b34fb",
          "Name": "Generic Service",
          "Characteristics": [
            {
              "UUID": "00002a05-0000-1000-8000-00805f9b34fb",
              "Name": "Service Changes",
              "Type": "U16|DEC|StartRange U16|DEC|EndRange",
              "IsRead": true,
              "IsIndicate": true,
              "ExampleData": [
                "Value:  01 00 FF FF",
                "1 65535"
              ]
            }
          ]
        },
        {
          "UUID": "0000fff0-0000-1000-8000-00805f9b34fb",
          "Name": "Unknown2",
          "Characteristics": [
            {
              "UUID": "d44bc439-abfd-45a2-b575-925416129600",
              "Name": "Unknown0",
              "Type": "BYTES|HEX|Unknown0",
              "IsWrite": true,
              "IsWriteWithoutResponse": true
            },
            {
              "UUID": "d44bc439-abfd-45a2-b575-92541612960a",
              "Name": "Unknown1",
              "Type": "BYTES|HEX|Unknown1",
              "IsWrite": true,
              "IsWriteWithoutResponse": true
            },
            {
              "UUID": "d44bc439-abfd-45a2-b575-92541612960b",
              "Name": "Unknown2",
              "Type": "BYTES|HEX|Unknown2",
              "IsWrite": true,
              "IsWriteWithoutResponse": true
            },
            {
              "UUID": "d44bc439-abfd-45a2-b575-925416129601",
              "Name": "Unknown3",
              "Type": "BYTES|HEX|Unknown3",
              "IsNotify": true
            }
          ]
        },
        {
          "UUID": "0000fd00-0000-1000-8000-00805f9b34fb",
          "Name": "Unknown3",
          "Characteristics": [
            {
              "UUID": "0000fd01-0000-1000-8000-00805f9b34fb",
              "Name": "Unknown0",
              "Type": "BYTES|HEX|Unknown0",
              "IsWriteWithoutResponse": true
            },
            {
              "UUID": "0000fd02-0000-1000-8000-00805f9b34fb",
              "Name": "Unknown1",
              "Type": "BYTES|HEX|Unknown1",
              "IsWrite": true,
              "IsNotify": true
            }
          ]
        }
      ],
      "Details": "Id:BluetoothLE#BluetoothLEbc:83:85:22:5a:70-00:3a:00:01:ed:4f\nCanPair:True IsPaired:False"
    }
  ]
}