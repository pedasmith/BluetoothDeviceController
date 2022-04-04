# More fun with the RRScale BT protocol

You've got a Bluetooth-enabled Lionel train. But how can you upgrade your scenery and track switches to be Bluetooth, too? Why can't they also be controlled from a Blueteooth app? One solution is the RRScale system: it's easy to get started with just a few updated, and fun to expand.

This page will demonstrate the power of the RRScale Bluetooth railroad protocol. Just like the LionChief BT protocol defines how Bluetooth can control your train, the RRSCale protocol defines how to communicate with your scenery and your Bluetooth track.

## First, some electronics

You'll need an [Arduino](http://arduino.cc) chip. These example all use the Arduino 33 IOT or [Arduino Nano 33 BT](https://docs.arduino.cc/hardware/nano-33-ble) chips; these are available for less that $30 from the [Arduino store](https://store.arduino.cc/collections/boards/products/arduino-nano-33-ble). The Arduino has long been a mainstay of the electronic hobbyist community; it's widely available from many supplies, there are many tutorials, and there's plenty of examples to show you what it can do.

You'll need to program the Arduino chip. I've already got a sample code that is a great place to start: the sample will let you get up to **TODO: 4** inputs and control **TODO: 4** LEDS and servos. 

You'll need a program on your PC or Smartphone that can handle the RRScale protocol. I'm a Windows kind of person, so I've created a Windows 10 program that understands both the Lionel LionChief and this new RRScale protocol. It can also control some set of Bluetooth-enabled lights. 

You'll need to power your Arduino. The easiest way to start is to power it with a USB charger, but you can also power it from your tracks (with a little care).

## Step by Step Arduino Sketch

I'm starting out with the **Arduino Nano 33 BLE** -- this is a modern, advanced board that's ideal for our purposes.

### Very First Arduino Sketch

1. Plug the Arduino board into your Windows PC. I'm using a laptop-style PC with Bluetooth and running Windows 10. After a minute or so you'll get a notification that  the device is set up and ready to be used. By default, the Arduino doesn't do any Bluetooth broadcasts (at least, not that I an find)

2. Prepare your Arduino IDE (Integrated Development Environment). Go to the official [Arduino Site](https://www.arduino.cc/) and get the Arduino IDE 2.0. As of February 2022, it's a "release candidate" -- the quality is generally quite good. The entire download is a bit under 250 megabytes. There will be several security popups from Windows Defender; you'll have to agree to them. 

3. Run the Arduino IDE for the first time! A bunch of stuff will automatically install; let it.

4. Select "Arduino Nano 33 BLE at COM ___". This will pop up a message asking about more stuff to install (e.g., "Aduino Mbed OS Nano Board [v 2.7.2]"). Allow all of this to install. This will include more security prompts.

5. Follow along the instructions to run "verify" on the template (mostly empty) sketch. You should eventually be told that the sketch uses some number of bytes in the Arduino.

6. And now verify that the "upload" works. You'll need to figure out which serial port the Arduino is on; I've used Device Manager to figure it out. You want the serial port connected via USB, not one of the Bluetooth serial ports.


### Arduino Sketch with Bluetooth LE

We're going to set up a mini RRScale service with a single characteristic that reports back on the setting of a potentiometer or switch.

1. Add ```#include <ArduinoBLE.h>``` to your sketch and also install the ArduinoBLE library (Sketch-->Include Libraries-->Manage Lbiraries... and then search for ArduinoBLE). Go ahead and do a compile (**^R**) to make sure it's all installed correctly.

## Building the RRScale Arduino Sketch

Remember that there are two programs: a 'sketch' that is  embedded in the Arduino the does the "arduino" side of the RRScale protocol, and the Windows app that you interact with on your laptop.

# Connecting your layout to the controller

The RRScale controller has *switches* (inputs) which include analog (multiple value) and digital (on/off) inputs. It also has *controls* which are outputs. They are set up as follows

Pin|Switch|Type|Notes
-----|-----|-----|-----
A0|[0]|Analog input
A1|[1]|Analog input
A2|[2]|Analog input
A3|[3]|Analog input
A6|[4]|Analog input
A7|[5]|Analog input
D2|[6]|Digital input
D3|[7]|Digital input
D4|[8]|Digital input
D5|[9]|Digital input
D6|[10]|Digital input

There are a total of 11 inputs. Most on/off type inputs (like from a Track Contactor) can work with either an analog or digital value.

Pin|Control|Type|Notes
-----|-----|-----|-----
D9|[0]|Digital output
D10|[1]|Digital output
D11|[2]|Digital output
D12|[2]|Digital output


# RRScale Protocol

The RRScale protocol is a proper Bluetooth LE protocol. There's a single RRScale service with a set of characteristics that let you read input from switches on your track and output characteristics that let you control parts of your track.

One of the special features of th RRScale protocol is that every RRScale GUID will start with the letters "09a9e" -- it's like a mutated HEX version of the word O-Gauge, heavily changed to fit with the HEX alphabet.

Each characteristic is marked for whether it can be written (W) or read (R). Each characterisitc that can be written can also be read (although this may not be a primary use case). Each characteristic that is read-only (not written) can be read can also be notified (N)

## Service RRScale 09a9e5eb-b5de-4c23-9f51-b08c87fd6650

### Memo Characteristic 09a9e440-0d08-4c2d-abe3-f9ab5e2544ed (W/R/N)

The Memo characteristic is a short text value that you can set. It will be maintained even after the power is cut off. It's designed so that you can program each device with a unique tag line so you can more easily track which device is which.

### MOTD (Message of the Day) Characteristic 09a9eeca-5a4f-4dc2-9a18-74080c704e21 (R/N)

The MOTD characteristic is a short text string with no particular message. It's used to add "chatter" to a track layout; for the primary Arduino Nano 33 this is a single 

### Switches Characteristic (byte array) 09a9e524-277d-405e-bcc4-a40bed62f2be (R/N)

The Switches characteristic tells you the current state of the input pins on the microcontroller. An input is data from the track layout going into the microcontroller. For example, you might hook up a Track Contactor (type 1045C or similar) as an input; that way you'll know when a train goes over a bit of track.

Switches contains an array of up to 20 bytes; each byte is the "analog" value of a switch. For digital switches (like bumper and track contacters), this will be either 00 or FF. For dial-type switches, this will be a value from 00 to FF.

Switches is read-only and also supports notifications

### SwitchCount Characteristic (uint16) 09a9e940-bbf2-4e2f-9b05-9107cbe4d5cf (R/N)

The SwitchCount is the actual number of switches that a particular controller supports. By having this value available, the app can be smarter abouit showing you what can actually be controlled. The number is the number of possible switches; you usually won't have hooked up all your switches.

This is mostly used by "smart" controller programs to adjust their user interface.

### Controls Characteristic (byte array) 09a9e678-afea-419b-a117-923619763bb7 (W/R)

The Control characteristic lets you control parts of your layout. Each byte of the output is a value from 00 to FF. 

The values can be saved from across power-outages; use the SaveControls command in the Command characteristic. You'll want to save a good power-up default for your layout: you might want certain lights to start up as "on" or "off", or set switches to one value or another.


### ControlCount Characteristic (uint16) 09a9e406-a94c-4111-a5ed-d2d7673cff72

Like the SwitchCount characteristic, this says how many devices your controller supports. 

You will probably not have them all hooked up at once. Controls that aren't hooked up can be left to their initial values.

### Command Characteristic (byte) 09a9ef97-5e6d-498f-bdf9-7c0ed1f41333 (W/R)

The Command characteristic lets you control certain aspects of the RRScale controller.

Possible Commands

Command|Value|Explanation
-----|-----|-----
SaveToFlash | 0x01 | Saves the current Controls values so they persist across reboots
ReadFlash | 0x02 | Causes the FLash memory to be read. This is an uncommon command to use; the RRScale controller will automatically read from flash when it boots 

### CommandResult Characteristic (4 bytes) 09a9e19f-e915-4a3b-9f0c-66ec48b10f81 (R/N)

Use the CommandResult to make sure that your commands have been properly handled. 

Data format

Field|Size|Meaning
-----|-----|-----
Count|int16|Count of the number of reply messages sent. Without this, sending the same command (like 'SaveToFlash') would often not change this value
Cmd|uint8|Original command sent OR an error opcode like 0xFF for UnknownCommand
Result|uint8|Result from the command. Will be 0 for success and non-zero for failure. Every command opcode has its own set of error codes 

### TODO: Text0 to Text9 Characteristics

The Text0 to Test9 characteristics let you set up signage in your setup. Each sign can have up to 20 characters. 

### TODO: TextCount Characteristic

Like the SwitchCount characteristic, this say how many text outputs your controller can handle. The RRScale-Basic-Arduino program can handle no text output devices (sorry!)

## Electrical Hookups

### Hooking up Track Contactor or Bumper

### Hooking up an incadescent light (18V)

### Hooking up a small LED

### Hooking up a small servo



# Some FYI about adding characteristics to a BLE device

## How much memory per characteristic?

You should feel confident about adding additional characteristics to a service. Each characteristic takes less than 200 bytes of memory in your controller; my Arduino Nano 33 BLE controler has over 600,000 bytes of memory free -- that's enough for over 3,000 characteristics!

Item | Memory in bytes |Delta | Global Variables in bytes | Notes
---|---|---|---|---|
Starting | 317512| * | 69432| Values at the start of the test
Add char. | 317688 | 176 |69456 | After adding a 2-byte characteristic with descriptor
Add 2d char | 317840 | 328 | 69480 | After adding a second 2-byte characteristic with descriptor



### GUIDS
```
Been used
09a9e406-a94c-4111-a5ed-d2d7673cff72
09a9e678-afea-419b-a117-923619763bb7
09a9e940-bbf2-4e2f-9b05-9107cbe4d5cf
09a9e524-277d-405e-bcc4-a40bed62f2be
09a9e440-0d08-4c2d-abe3-f9ab5e2544ed
09a9eeca-5a4f-4dc2-9a18-74080c704e21
09a9e5eb-b5de-4c23-9f51-b08c87fd6650
09a9ef97-5e6d-498f-bdf9-7c0ed1f41333
09a9e19f-e915-4a3b-9f0c-66ec48b10f81

Ready to use

09a9e347-09ec-4d08-b6df-63373ab30d3d
```