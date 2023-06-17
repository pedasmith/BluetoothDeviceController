# Creating a special Bluetooth keyboard with Adafruit

I've made a Bluetooth keyboard that's customized for my morning Web browsing. There's buttons for page up, page down, and the always-awesome middle-mouse click. The keyboard is from a vintage Atari CX85 keypad, controlled from a handy Adafruit Feather microcontoller, all going to a custom Windows app to handle the keyboard injection.

Bluetooth LE is a delightful protocol that lets you build smart devices (we will make a smart keyboard in this article) and control it. However, actually using the Adafruit Bluetooth LE libraries is a little painful. It's not always obvious how the library works, or what you need to provide in order add your own services and characteristics. In this series I'll step through what I learned about programming with the Adafruit CircuitPython.

The final project is all available on [github](https://github.com/pedasmith/BluetoothDeviceController); look in the [Small Projects/BTUniversalKeyboard](https://github.com/pedasmith/BluetoothDeviceController/tree/main/SmallProjects/BTUniversalKeyboard) project for the Windows app and CircuitPython code.

## IP: Potential topics

- Reading the Atari CX85 pins
- about BT services/characteristics
- why BT keyboards need a custom protocol
- Driving the display 
- Power and Battery
- Being a client
- Being a server
- Windows BT programming
- GUIDs
- Pain points
- DONE: Time server


## Getting started: what are all these things?

Stuff I used for this project:

The vintage CX85 keyboard is described on the [AtariWiki.org](https://atariwiki.org/wiki/Wiki.jsp?page=AtariCX85) site; a vintage [review](https://www.atarimagazines.com/compute/issue36/043_1_REVIEWS_Atari_CX85_Numerical_Keypad.php) is a good introduction to its abilities. Note in particular the technical manuals -- they clearly demonstrate the keyboard protocol.

I picked the [Adafruit Feather](https://learn.adafruit.com/introducing-the-adafruit-nrf52840-feather) system for my microcontroller. This was a good choice; it's not expensive, and has pretty good documentation. To help debug, I attached a [FeatherWing OLED](https://www.adafruit.com/product/4650) display. The display worked like champ (although I had to redo my wiring so the keyboard connection didn't interfere with the display pins).

To use the Feather, I needed to learn a little bit of [CircuitPython](https://www.adafruit.com/circuitpython). CircuitPython looks a lot like regular Python, which I last used about 10 years ago. I'll recommend that you also download a copy of Python for Windows; it was much faster for some of my Python code to try it out first on Windows. I used the [mu](https://codewith.mu/) editor; use the [Adafruit link](https://learn.adafruit.com/welcome-to-circuitpython/installing-mu-editor) to get the version that works with their boards and CircuitPython. I found mu to be usable: their point -of-view is to make a simple, uncluttered editor with just enough feature to make a small project like this.

AdaFruit CircuitPython [adafruit_ble](https://docs.circuitpython.org/projects/ble/en/latest/api.html) is the Bluetooth library I'm using. It's a wrapper over the [bleio](https://docs.circuitpython.org/en/latest/shared-bindings/_bleio/index.html#bleio.Connection) low-level library; I essentially found that it was never useful to look at that library. I found it essential to read the [Adafruit Github library](https://github.com/adafruit/Adafruit_CircuitPython_BLE) CircuitPython source code -- hopefully after you read my article here you won't need to! 

The final project is 9 CircuitPython files which are about 1,000 lines of CircuitPython code.

## IP: Reading the Pins is Step One

Step one (after getting all the parts and software) is to figure out reading in the keypad. All the code for this is in AtariCX85_Feather.py file but the actual keyboard table (to convert the Atari codes into something a bit more standard) are in KeyboardTables.py.

I tried to divide the code into sections: **LED + LOG** is for logging data and lighting the color LED, **Features** is for my features flag (it controls whether the keyboard will do deep sleeps), the **AtariCx85** sections controls the keboard through the 9-pin "Joystick" interface, **Bluetooth LE** is for Bluetooth, **Power and battery** is for power (rather messy), and **Timer** for the timers. 

There's a timer state machine in TimCode.py. You provide it with information about what's happending (e.g., "the user did something" or "a timer went off") and it will tell you what needs to happen ("should blank the screen").

### The AtariCx85 sections

The key pin on the Atari CX85 is the trigger pin. The trigger is normally at about 3 volts and will dip to 0 volts when any key is pressed. The 0 volts will read as "False"



## Diversion: getting the time from a Bluetooth Current Time Service

OK, so funny story. In the middle of this project, I got diverted by a comment from an old coworker: is it possible to make a small device with a button and a dim LED. If the button is pressed between, say, midnight and 7:30 in the morning, the light won't turn on. But after 7:30, it will. They wanted to have a sort of reverse alarm clock: if it's "too early" in the morning they wanted to go back to sleep with a minimum of fuss.

This got me to thinking about Bluetooth Time Servers, and from there to the thickets of the Bluetooth Special Interest Group (SIG) documentation, and more. Along the way, I learned how to make a server with advertisements in Windows (weirdly, I never did the server side of Bluetooth prorgramming in Windows) and built a little Time Server there. I also learned how to listen for advertisements on the AdaFruit Feather, plus connect to the device, and how to built a correct Time Service reader in CircuitPython, plus how to find and read the newest Bluetooth SIG documentation (after many years, they completely re-organized the docs, and many common things are much easier to find.)

I learned ever so much from diving into the Current Time Service.

### There's a standard [Current Time Service](https://www.bluetooth.com/specifications/specs/current-time-service-1-1/)
The Current Time Service is a simple time beacon. The service is at 0x1805 and has one widely-used characteristic, the Current Time Characteristic at 0x2A2B. There's also a Local Time Characteristic at 0x2A0F and Reference Time Information Characteristic that say what the current time "means" (like, "Pacific Time") and where the reference time comes from.

The time is well described in [Nordic](https://infocenter.nordicsemi.com/index.jsp?topic=%2Fcom.nordic.infocenter.sdk5.v12.2.0%2Fstructexact__time__256__t.html) as being u16_year, u8_month, u8_day, u8_hours, u8_minutes, u8_seconds, and more.

Walking through the entire list of structures, sub-structures and more eventually gets to this simple table of data:

|BLE Type|BLE Name|Parent Field Type|.. Name|GParent Field Type|..Name|GGParent Field Type|
|----|----|----|----|----|----|----|
|u16|year|ble_date_time_t|date_time|day_date_time_t|day_date_time|exact_time_256_t
|u8|month|ble_date_time_t|date_time | | | 
|u8|day|ble_date_time_t|date_time | | | |
|u8|hours|ble_date_time_t|date_time
|u8|minutes|ble_date_time_t|date_time
|u8|seconds|ble_date_time_t|date_time
|u8|day_of_week| ||day_date_time_t|day_date_time|
|u8|fractions256|||||exact_time_256_t|

The structures match what's in Bluetooth Sig guides. Take a look at the SIG [CurrentTimeService_CTS_SPEC_V1.1.0.pdf](https://www.bluetooth.com/specifications/specs/current-time-service-1-1/) file. It says that the Current Time is defined by the "Exact Time 256". This is defined in [GATT_Specification_Suppliment_v8.pdf](https://www.bluetooth.com/specifications/specs/) (you need to scroll *way* down to get to it). That spec lists all of these data types. You can find Exact Time 256 there as a Day Date Time plus a Fransion256. Just keep pawing through at you'll find it all.

### CircuitPython: Looking for Current Time Service advertisements

I set up the Windows Current Time Service to advertise that it has the Current Time Service. In CircuitPython, start at the BTCurrentTimeServiceClient.py file. The BtCurrentTimeServiceClientRunner will start scanning for advertisements (actual scan in ScanOnce). The advertisements have to include the "ProvideServicesAdvertisement" so we only try to connect to device that say they provide the Current Time Service. For each advertisement found (and there may be a lot!), we look for one that has the "BtCurrentTimeServiceClient.uuid" service; that UUID is 0x1805 and is a Bluetooth SIG standard.

Once we find a device advertisement, we can connect to it (ble.connect) and return.

When we have a connection to a device that says it supports the Current Time Service, the code calls **ConnectToCurrentTimeService**. This uses the connection, which if you haven't use the CircuitPython adafruit_ble library, will need some explanation.

The "connection" represents the connection. And when connected, it also can be indexed like an array or list with the **[]** syntax. The contents are the set of services that the device supports. But we warned! In the CircuitPython code you can see that that I test to see if the connection has my service using a GUID: ```if UUID not in connection```. But you can't **extract** the service that way!

To use a service from the ```connection``` object, you need define a class that matches what the service will need. Yes, this is confusing. In my code, the class is the ```BtCurrentTimeServiceClient``` class; it's derived from the ```Service``` class. The derivation is essential: what happens is that the adaFruit bluetooth_ble library code and the bleio code will see the class (BtCurrentTimeServiceClient) and combine it with the actual data coming from Bluetooth. When you connect to a Bluetooth LE device, you can pull in a list of all of the service and characteristics and some of the "metadata" about the services and characteristics (like names and UUIDs and more). The AdaFruit and bleio code will create a new object from the class you provide and will *modify* it to match what it discovered from the Bluetooth connection. This is a dynamic process.

What you need to provide in your is a UUID (I used a "StandardUUID" which takes in a short hex value like 0x1805; there's also a VendorUUID that takes the full 128-bit UUID) plus all of the characteristics. You provide the characteristics in the class area of the class. In my case I used a ```StructCharacteristics``` that is initialized with the characteristics UUID (0x2A2B) plus the ```struct_format``` which says how to unpack the data. To get the data, you'll just use the ```data``` field. In the class this will be a StructCharacteristic, but once you get an instance of the object, the ```data``` value will be a tuple.

Yes, for this non-dynamic programmer, this entire thing is just a little wierd. The lack of tutorial documentation and the small number of hits in Github looking for sample code didn't help.

My CurrentTimeService client code, by the way, eventually gets the current time in **ConnectToCurrentTimeService**; all I do is pull the connection out and then print the formatted time value with ```service.GetTimeString()```. What I should do is use the value to initialize the real-time clock on the Feather. 

## IP: Measuring the battery power

Link: [https://learn.adafruit.com/introducing-the-adafruit-nrf52840-feather/power-management-2](https://learn.adafruit.com/introducing-the-adafruit-nrf52840-feather/power-management-2)

There's a 1/2 of battery voltage available on analog A6. That pin is not exposed on the breakout (it just goes A0 to A5), so that value is available for use. They call it A6, but you have to refer to it as ```board.VOLTAGE_MONITOR``` (for CircuitPython). Arduino people can call it A6 but it's commonly converted to ```#define VBATPIN A6```.

The voltage is measure with this function:
```
    #
    # Power and battery section
    #
    batteryPin = AnalogIn(board.VOLTAGE_MONITOR)

    def get_battery_voltage(self):
        return 2.0 * (self.batteryPin.value * 3.3) / 65536.0    
```

The code is eventually called from the main pin-reading loop and logs the battery voltage about every 60 seconds. Note that logging data doesn't turn on the display. The values I see start out at about 4.2 volts and slowly decrease to 3.something.

## Picking GUIDs for Bluetooth

Bluetooth services and characteristics are all specified by GUID. Many devices -- even from well-known companies -- have a habbit of using impropertly-created GUIDs. This is wrong; a proper GUID can only be generated by a "real" GUID program. You should never just type in random letters and numbers to create one, you should never reuse one, and you should never make one by incrementing an existing GUID.

GUIDs (also known as UUID) are carefully designed (see [RFC 4122](https://www.rfc-editor.org/rfc/rfc4122)) so that any number of people can each make GUIDs without having them overlap. If you create a GUID by hand (by just typing in hex letters and numbers), you run the risk of re-using a GUID. This is not good.

People create their GUID by hand because they like to have a "set" of GUIDs so that all of their services and characteristics all match in some way. Among other things, it makes debugging your devices a little easier. Luckily, although we can't make just any old GUID, we **can** generate as many GUIDs as we like and filter out the ones we want to keep.

For this project, I decided that every GUID would start with "b7b0a" -- so it looks a little bit like "BT Board" as in BT Keyboard. Using a 7 for the 'T' is normal "leetspeak".

Because I use Windows, I generate the GUIDs with PowerShell. I'm not in any way a PowerShell expert. 

The simplest way to make a GUID in PowerShell is the **new-guid** command. I created a tiny little script to make Guids in a loop and only print the ones that match my "lookfor" value.

```powershell
$lookfor = "b7b0a"
while ($true) {
    $guid = new-guid
    if ($guid.ToString().StartsWith($lookfor)) {
        Write-Output $guid
    }
}
```

I've found that on a modern low-end laptop that I can generate about 2 GUIDs that match 5 letters per minute. I just left the script running during lunch to generate a long list, and those are the GUIDs used in my project.


# Writer's Notes

*Delete all these*

