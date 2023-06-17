# The Adafruit Clue-Clock

## Script

*video of clock running* I made a clock! And it has a special feature. *reset* when it resets, it doesn't know the time. But wait a second, and it's automatically reset itself! It's all thanks to Bluetooth and service 0x1805, the Current Time Service.

You can get an app called "Simple Bluetooth Time Server" on the Windows store; that's how I set the time.



## Links to technical docs

* [Current Time Service](https://www.bluetooth.com/specifications/specs/current-time-service-1-1/); you will want the first doc ("Current Time Service 1.1"). But the service and characteristic values are described in the [Assigned Numbers](https://www.bluetooth.com/specifications/specs/assigned-numbers/) doc (look for "Current Time" to find that the service is 0x1805 and the corresponding characteristic is 0x2A2B. The [Specifications Supplement](https://www.bluetooth.com/specifications/specs/gatt-specification-supplement/) then says what the contents of an 'Exact Time 256' are.
* [Adafruit]() has complete information on their Clue device