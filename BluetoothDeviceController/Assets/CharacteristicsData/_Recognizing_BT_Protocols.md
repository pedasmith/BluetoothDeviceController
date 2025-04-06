# Recognizing Bluetooth LE (BT) protocols

## Modbus: A<address> <opcode> <len> <params> <CRC16>

Example: Daybetter_LedLight.json


```
              "UUID": "0000a031-0000-1000-8000-00805f9b34fb",
              "Name": "ModbusSend",
              "Type": "U8|HEX|Address||A0 U8|HEX|Function||15 BYTES|HEX|Command||06_FF_00_00 OEB U16|HEX|CRC||UpdateModbusCrc16AtEnd",
              "ExampleData": [
                "A0 13 04 0C D124 set brightness. A0 is address. 13 is set brightness; 04 seems to be the length including the length byte and CRC. 0C is the value D124 is the CRC",
                "A0 15 06 00FFFF 1440 set to cyan (G+B). A0 is address, 15 is set RGB, 06 is the weird length 00FFFF is the color 1440 is the CRC"
              ]

```