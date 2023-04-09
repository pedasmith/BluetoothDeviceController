# BtUnicode-Protocol -- a protocol for controlling BT Keyboards

## TODO list

- [] Switch to "struct"
- [] Create Windows app 

## Concepts for things the user can set

The keyboard can be as incredibly fancy as we want and the universal Keyboard app has to track it.

- [] Keyboard colors RGB on event
- [] Send keyboard description to Windows
- [] Set up keyboard mapping
- [] Specialty items like PAUSE
- [] Switch between mappings e.g., Lukshootseed/English
- [] RBG tracks mapping
- [] URL to define keyboard for QR Codes!
- [] File format to define keyboard

## About the BtUnicode Protocol

The BT Unicode keyboard and HID protocol always sets a bunch of data

1. The name of the device always start BtUnicode-
2. The rest of the name can be set ("UserSetName") and defaults to "Keyboard"
3. The Keyboard service is a6f04fc6-f78f-4fdd-b6ca-0c0c44d1eb57
4. It's call a keyboard service, but it includes mouse and gamepad, too
5. The keyboards service includes characteristics for
    1. Press (int32) is either 1 (press) or 0 (release) or 2... for repeats. This is always set after the rest of the values -- so that e.g. the user presses a key and we first get virtualkey, scancode, utf8 all set up, then a Press event and then the Release event.
    b7b0a009-b23d-428f-985c-f6f26a80bf1f
    2. VirtualKey (int32) code for the individual keys. This is like "VK_DELETE". In Windows terms, this is a translated version of the scan codes. This may be -1 when not set, and won't reset on key release.
    b7b0a035-852f-4a31-bae4-fcd4510c444d
    3. ScanCode (int32) is the Unicode consortium style scan code for a key. This is just the row and column for the key
    b7b0a047-d291-41a3-8c2c-2f4bfa46fef9
    4. Utf8Value is the Unicode UTF8 of the keypress. This allows
    b7b0a074-e122-4a2d-ae7e-3c596cfcae3b



## Steps to making the first version

1. Run the Arduino IDE. I'm using version 2.0.3.
    1. Install the needed Board support package. 
        1. From the IDE, select Tools→Board→Board Manager. 
        2. Click "Install" for the Arduino SAMD Boards (32-bits ARM Cortex-M0+) to support the Arduino Nano IOT. Say "yes: to installing the different device and driver software. Wait until the the little "Output" windows finally says that, for example, "Platform arduino"samd@1.8.3 installed"
        3. Also click "Install" for the "Arduino Mbded OS Nano Boards" package to support the Nano BT (which I might use later). 
    2. Decide to use the **Nano 33 BLE** because it supports the Mbed BLE HID library!
    2. **NOPE** Get the [Bluetooth Keyboard library](https://github.com/tcoppex/mbed-ble-hid). This will let you send out ordinary keystrokes (but not fancy Unicode; that comes later)
    2. **NOPE** Get the Mbed BLE HID by Thibault Coppex library.
        1. From the IDE, select Tools→Manage Libraries
        2. In the search box, type "mbed ble hid"
        3. Click "Install" on the Mbed BLE HID by Thibault Coppex library
        4. You might as well pick up that mouse library, too
        5. In a web browser, go to [https://www.arduino.cc/reference/en/libraries/mbed-ble-hid/](https://www.arduino.cc/reference/en/libraries/mbed-ble-hid/) to look at the documentation
    3. Plug your Arduino into the USB cable and select it from the IDE
        1. Plug in the Arduino board to a USB cable to your PC
        2. In the IDE, for "Select Board", Pick the Arduino Nano 33 BLE
    4. Create your first sketch!
    5. Compile the sketch with ^R or Sketch→Verify/Compile
    6. It's time for Sketch→Upload!
     
2. Arduino is an unusable choice. The device doesn't work at all with Windows. This is confirmed by checking online forums where it doesn't work for a lot of people. Switching to AdaFruit.


## Choice and data: which AdaFruit?

[Adafruit Feather nRF52840 Express](https://www.adafruit.com/product/4062)
Chip is both BT and the feather which they are pretty pleased with. Supports CircuitPython which might be good for programming.

[Adafruit Feather 32u4 Bluefruit](https://www.adafruit.com/product/3379)
[Adafruit Feather nRF52 Bluefruit LE](https://www.adafruit.com/product/3406)
Older product, so no, don't use this one.



## Choice and data: Which Arduino?

I have both the Arduino Nano 33 **BT** ($26.30) and **IOT** ($24). 

|Spec|Nano BT|Nano IOT|
|-----|-----|-----
|Summary|Bigger $26.30|Smaller $24
|Sensors|9-axis inertial|6-Axis IMU
|Bits|32|32
|ARM core|Cortex-M4|Cortex-M0
|Clock|64 MHz|48 MHz
|Flash|1MB|256 KB
|SRAM|256KB|32 KB
|Voltage|3.3V|3.3V
|BT|
|Wi-Fi|?no?|NINA-W10 2.4 GHz

First attempt will use the **Nano 33 IOT**. Which failed, because Arduino BT libraries aren't compatible with Windows. Next up is the Adafruit devices which hopefully will work better.

## What are some libraries to check out?

Larry Bank's code!

https://github.com/bitbank2/BLE_Keyboard/blob/master/BLE_Keyboard.ino



## Choice and data: should I use the Micro-Bit?

The advantages of the micro:bit it that it's widely available. The downside is that the only BT they really do is like a UART service that has very lmited customization. As regular readers of my BT writings know, I'm 100% not a fan of UART style BT LE; it's the opposite of what BT LE stands for.

## Choice and data: how about the pygo1 device?

The pygo1 is a fairly new device that was made by a now defunct company. References to the pygo on their website have been (mostly) removed, so the device is now dead as a doornail.

## Do any BT keyboards really support Unicode?

Why don't existing keyboard already support Unicode? Isn't there a big market for them? And the answer is: no one company solves enough of the problem to solve the problem. The original USB and BT keyboard specs really only support ancient "return a scan code" keyboards; they don't include the concept of sending the actual requested characters.

In order to support 

Short answer: no. See [this Github answer](https://github.com/kiibohd/KiiConf/issues/30) for details.

A person on the [Microsoft forums](https://social.msdn.microsoft.com/Forums/windowsdesktop/en-US/f3078fb5-354b-4357-bdde-19a57647475a/im-make-new-usb-keyboard-which-can-input-unicode-need-help-on-its-driver-design?forum=wdk) is trying to do this

### Microsoft  - Insert Unicode characters

Link to [Insert ASCII or Unicode latinbased symbols and characters](https://support.microsoft.com/en-us/office/insert-ascii-or-unicode-latin-based-symbols-and-characters-d13f58d3-7bcb-44a7-a4d5-972ee12e50e0) suggests that a person can type ALT-X: first type in the Unicode (BMP-only) value and then force an ALT-X. This will work only in Office and a few other programs.

This is different from the old trick of ALT-numberpad to type in a "latin" key.

Note that this is an overall horrible way to send data

### Some random example characters in a random text


Example: ◎ BULLSEYE U+25CE     U+25CE
Example: ◎ BULLSEYE U+25CE     U+25CE
Example: ◎ BULLSEYE U+25CE     U+25CE


Example: 😸is U+D83D U+DE38
Example: 😸is U+D83D U+DE38
Example: 😸is U+D83D U+DE38

# Sketches along the way

## Third Python: BT!

The first couple of python 

Link: Adafruit CircuitPython learning: [Bluetooth Low Energy Basics](https://learn.adafruit.com/circuitpython-nrf52840/bluetooth-basics)

From it: my device is the **peripheral**. 

Link: [CircuitPython](https://docs.circuitpython.org/projects/ble/en/latest/index.html)

## Third sketch: abject failure with BT

BT doesn't work with Arduino and Windows 11.

## Second working sketch: ATARI!

The Second sketch (BTUnicode-V2-Connect-to-AtariCx85) hooks up to the AtariCx85 keyboard and correctly translates all of the keys! 

A bit of measuring shows that the keyboard draws 5 to 15 mA of power depending on which keys have been pressed. Noting that an AdaFruit battery is often 350 mAH, the battery can drive the keyboard for 35 hours (not including the power needs of the Adafruit Feather :-) ). Perhaps the power can be lowered with a few carefully chosen resistors. After testing, it seems like each line when (high?) is a couple of mA but that value drops a lot when I put in a resistor (I picked a 9K reistor because it's what I had; the circuit seems to work exactly as well.)

## First working sketch: trigger LED

Trigger the LED when 'trigger' goes to ground. Took forever because there's not much doc about INPUT_PULLUP versus INPUT and the RGB LED sample code is wrong (says the LED is active HIGH when it's actually active LOW)


# List of b7boa GUIDs





b7b0a07e-a995-4eae-9315-856e31bd7334
b7b0a09b-1890-4538-ab0b-0ea2f16846d7
b7b0a0ad-0e99-45db-8426-e62f6a13109e
b7b0a0c3-7fda-428e-8254-a31a3d48d954
b7b0a0e4-6b1f-41ab-a3ab-08d2a9155b69
b7b0a0f1-6a98-45c5-b732-5ef993ce9b81
b7b0a0f3-35f9-4ef4-a3f2-49d0279642d5
b7b0a131-9195-4c68-98a3-d5c72b275733
b7b0a140-2159-46e1-86cf-01cdeeb29e14
b7b0a178-d745-4935-9c6c-7453838a73f8
b7b0a19c-70a4-4fc9-965e-d4a7fb17b289
b7b0a1b2-76f6-46f4-b8d5-abbec9563ce7
b7b0a1b3-bd3f-4577-9745-2c76f8504f19
b7b0a1da-0f90-4c19-ad91-a562d2342aa8
b7b0a200-eed7-43ce-a69c-c1fba674cbb9
b7b0a215-31a0-4daa-9304-e9fbbf7a2369
b7b0a21e-0617-4d0c-a220-a8ac724850ab
b7b0a237-79ea-491d-8832-4e8649010c65
b7b0a252-973a-4bf7-ade6-5c2aaefccceb
b7b0a25b-c484-4dd3-99bd-fb621c1809de



# Useful Links

## Arduino docs

[Nano 33 BLE](https://docs.arduino.cc/hardware/nano-33-ble)
[Nano 33 IOT](https://docs.arduino.cc/hardware/nano-33-iot)

[ArduinoBLE library](https://www.arduino.cc/reference/en/libraries/arduinoble/). The library can be used with both the Nano 33 BLE and the Nano 33 IOT.
[Walkthrough](https://docs.arduino.cc/tutorials/nano-33-ble-sense/ble-device-to-device)

## Atari docs

[AtariWiki](https://atariwiki.org/wiki/Wiki.jsp?page=AtariCX85)
[Archive.org](https://archive.org/details/AtariTheBookkeeperCX85Manual)

## Microsoft docs

[Virtual Keys](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes)