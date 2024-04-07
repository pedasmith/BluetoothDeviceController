# Youtube Description

Clocks that set themselves!

Isn't resetting all your clocks after a power outage a bit of a waste of time? Did you know that for the IOT devices you make, you can use the Bluetooth Current Time Service to set the clock automatically? In this video I demonstrates an automatically-resetting clock using the Adafruit Clue device and CircuitPython. The video dives through the code, showing how the display works, how to use the real-time clock chip on the Clue device, and a full walkthrough of the CircuitPython Bluetooth code that uses the adafruit_ble library to create a Bluetooth client. 


## Most popular links

The Python source code is on (Github)[https://github.com/pedasmith/BluetoothDeviceController/tree/main/SmallProjects/BluetoothCurrentTimeServer/Assets/Adafruit-Clue]. A simple Windows app that broadcasts the Bluetooth Current Time Service can be downloaded from the (Microsoft App Store)[https://apps.microsoft.com/store/detail/simple-bluetooth-current-time-service/9NJQ3TD3K06F]. And I've got handy links for the [Adafruit Clue](https://www.adafruit.com/product/4500) and Bluetooth [Current Time Service](https://www.bluetooth.com/specifications/specs/current-time-service-1-1/)


# SunriseProgrammer blog: The Adafruit Clue-Clock

I'm always frustrated when I have to reset a bunch of clocks after a power outage. Did you know that setting the time on an IOT device can be easy when you add support for the Bluetooth [Current Time Service](https://www.bluetooth.com/specifications/specs/current-time-service-1-1/)? (On the SIG site you will want the first doc, "Current Time Service 1.1"). In this blog post I show some of the code I wrote for the [Adafruit Clue](https://www.adafruit.com/product/4500) using CircuitPython and the adafruit_ble library. I've even got a [Youtube video!](https://www.youtube.com/watch?v=_OHZJ7dY5v0) to show the device and step through the code. 

There's also a handy Windows app that is the server side of the time setting; it will broadcast out the current time. Download it "[Simple Bluetooth Time Service](https://apps.microsoft.com/store/detail/simple-bluetooth-current-time-service/9NJQ3TD3K06F)" on the Windows store; that's how I set the time. The complete source code for it is (on Github)[https://github.com/pedasmith/BluetoothDeviceController/tree/main/SmallProjects/BluetoothCurrentTimeServer]

In the video I step through three interesting features of the clock.

### Feature 1: **Display** stuff to the screen

We'll want to print text to the screen; this is done with the ```clue.simple_text_display()``` object. The clock uses the *simple_text_display*, so we can display several lines of text, but nothing fancier. 

A key point (that took me far to long to figure out!) is that after you update one or more a lines of text, you must call the *.show()* method -- otherwise, nothing gets displayed!

*Sample Code*
```
from adafruit_clue import clue
colors = ((0xff, 0xff, 0xff),)
display = clue.simple_text_display(title="Clock", 
                                   title_scale=2, text_scale=4,
                                   title_color=(0xa0, 0xa0, 0xa0),
                                   colors=colors
                                   )
str = "{:02d}:{:02d}:{:02d}".format(currHour, currMinute, currSecond)
clue_display[0].text = str
clue_display.show()

```
The [simple_text_display](https://docs.circuitpython.org/projects/clue/en/latest/api.html#adafruit_clue.Clue.simple_text_display) is documented on the circuitpython site.

The text_scale value of 4 fits a time display with a format of HH:MM:SS (8 characters long) with room for two more characters (eg, enough room for an AM/PM indicator, if desired)

The title_scale is relative to the text_scale.

The colors set the colors of each line. I set it so tht the time and date are white, and the day (and the bluetooth scan results) are blue.

### Feature 2: Use the **Real Time Clock**

The chip used to track time accurately is the "real time clock" -- without it, the code would slowly drift. Fun fact: the first IBM PC did not include a battery-backed real-time clock chip. Every time you turned on the computer, you had to enter the date and time.

The real-time clock uses [struct_time](https://docs.python.org/3/library/time.html#time.struct_time) for many operations; it's just a tuple where you can grab values by index. the current hour, for example, is index 3.

The real-time clock needs power to work; if it loses power, it will stop stracking an accurate date and time (it says "real time clock", but it does dates, too). How we set it is the topic of the next section.

The CircuitPython rtc module is a delight to use: there's just a simple way to set the initial value and a simple way to pull out the current time.


### Feature 3: Connect to **Bluetooth Current Time Service**

Now we get the hard stuff: reading data from an external Bluetooth "Current Time Service" source. The idea is that a nearby PC will broadcast out a "current time" (there's a standard for this); the clock will pick it up and use it to set its time.

To make it work, you should have already added the **adafruit_ble** to your **lib** directory. It's not on the list of libraries in the Adafruit Clue documentation.

The bulk of the Bluetooth code is in BtCurrentTimeServiceClient.py. There's two critical classes in that file: the *BtCurrentTimeServiceClient* class which matches the Bluetooth Special Interest Group (SIG) standard and which is compatible with the Adafruit CircuitPython Bluetooth setup, plus a helpful wrapper class *BtCurrentTimeServiceClientRunner* class which listens for Bluetooth advertisements and connects to the time service.

#### BtCurrentTimeServiceClient

The BtCurrentTimeServiceClient class is less than 20 lines of code. The Adafruit CircuitPython Bluetooth system isn't too hard to use, but there isn't a very good tutorial on it. Hopefully this explanation will help!

The BtCurrentTimeServiceClient class exists for only one reason: it's the "glue" between the Bluetooth system and your code. When you get an advertisement for a Bluetooth device you want to connect to, you'll provide this class (the *class* and not an *object*) and will get back an object that's mostly this class (it will have been updated)

The object you get back will only be valid until the connection is broken. In the code, the connection is broken almost as soon as the data is read.

```
class BtCurrentTimeServiceClient(Service):
    uuid = StandardUUID(0x1805)
    data = StructCharacteristic(
        uuid=StandardUUID(0x2A2B),
        # Don't need to provide these; they should be discovered
        # by the Bluetooth system.
        # properties=Characteristic.READ | Characteristic.NOTIFY,
        struct_format="<HBBBBBBBB"
    )

    def GetTimeString(self):
        (y, m, d, hh, mm, ss, j1, j2, j3) = self.data
        retval = "{0}-{1}-{2} {3}:{4}:{5}".format(y, m, d, hh, mm, ss)
        return retval
```

The **data** value is set to be a *StructCharacteristic*. But when you examine the data later on (like after it's been updated by the remote side!), it will instead be a tuple of the data, parsed by the struct_format string. You just have to know from other sources what the data values actually mean.

#### BtCurrentTimeServiceClientRunner

The BtCurrentTimeServiceClientRunner is the class you'll actually call to get the Bluetooth current time data. Just call Scan, passing in a bluetooth "*ble* object; it's the Bluetooth from the clue device (```ble = adafruit_ble.BLERadio()```). There aren't any other methods in the class that should be called.

The runner **Scan** method will scan for Bluetooth advertisements for a set amount of time (15 seconds in this case); the scans can complete in less time, so I loop around as needed. The inner loop of Scan calls **ScanOnce** to do a single advertisement scan, returning a connected bluetooth device. Once this method has a connected bluetooth device, we hook up the service connection with the **ConnectToCurrentTimeService** method (yeah, I'm using the word "connected" here in kind of two different ways). Once we have a service connection, we can pull out the time data directly.

The **ScanOnce** returns a connected connection to the remote device (or None, of course). It does a single advertisement scan, up to a maximum amount of time, looking for an advertisment that says it supports the current time service. When one of those is found, we connect to that device. In my case, the device will just be my laptop when it's running the *Simple Bluetooth Current Time Service* app. 

The **ConnectToCurrentTimeService** creates a 'live' (connected) service object given a connection to a device. 

To convert a connection to a bluetooth device into a useable per-service object, you need to provide a class with a **uuid** that matches the service you need to use, plus a **data** object which needs to be one of the **Characteristic** types (for example, **StructCharacteristic**). When you "get" an object from the connection, the smart connection "array" will create a brand-new object for you, of the class you specify, that's hooked to (connected to) the live Bluetooth object. As part of this, the "data" value in the class, which had been, e.g., a StructCharacteristic, will now just be a tuple of data. Reading that tuple will get you the latest data.

To recap: the **Scan** method will scan advertisements for an appropriate BT device, will connect to it, will make a service connection, read the characteristic data, put that data into a tuple, and return the tuple. In case of errors, it will just return None.

Once the tuple of date is read, we just set up the real-time clock at about line 74 of the code.py file. Once this happens, the clock will be updated!

You can make this work for your device, too -- just pop in the BtCurrentTimeServiceClient.py, and call the Scan() method with a BT radio. Just don't forget to include the adafruit_ble library on your device!

Good luck!


### Feature #4: Multiple displays

(See hiker companion


)

### Feature #5: Power reduction!

The Clock, by itself, uses too much power for the battery. I've wanted to learn more about low-power Clue usage, and Feature #4 is where I'm doing my experiments.

#### Step 1: How much power? 
Measuring the power used by the Clue is done by simply placing a milli-Amp meter (I've just got a cheap Radio Shack multimeter) in series with the battery. I killed two birds with one stone, and soldered a SPST switch in the red (power) line from the battery. When it's ON, the Clue is powered normally. By clipping the multimeter leads to the two pins of the switch, and turning it off, power flows through the multimeter, and gives me a reading: **5.5 mA**

Since the LiPo battery is a 350 mAH battery, it should be able to power the clock for 63 hours (not bad!). Except that it's not; the power lasts for much less time.

AS it turns out, the loud "pop" I got when I hooked up the multimeter incorrectly back in the 80s turns out to have fried some of the current meter circuitry. Using a more modern meter shows a drain of 50 mA, which roughly matches the observed drain.

|Current|Amount|Notes
|-----|-----|-----
|Correct starting value| 50 mA


#### Lower power by cutting off the display

https://learn.adafruit.com/deep-sleep-with-circuitpython/overview

0. Use a different meter to measure power -- maybe the Radio Shack is wrong? answer: yes, it's wrong. Actual value from the more modern meter says 50 mA.
1. Do a longer (light) sleep and wake on battery press. Turn off the display after 10 seconds (ish). Add in the "state" python file.
2. Try a deep sleep (will need to stop listening to BT :-) )
3. Try the alarm.sleep_memory?

Doing all of those reduces the power consumption from about 50 mA to about 45 mA -- which is hardly worth it.

Here's the sensors we can control:

* Adafruit [LSM6DS accel/gyro](https://github.com/adafruit/Adafruit_CircuitPython_LSM6DS)
* Adafruit [LIS3MDL magnetometer](https://github.com/adafruit/Adafruit_CircuitPython_LIS3MDL)
* Adafruit [APDS9960 proximity and light](https://github.com/adafruit/Adafruit_CircuitPython_APDS9960) has an "enable"
* Adafruit [BMP280 temp/pressure](https://github.com/adafruit/Adafruit_CircuitPython_BMP280)
* Adafruit [SHT31D temp/humidity](https://github.com/adafruit/Adafruit_CircuitPython_SHT31D)
* Adafruit [NeoPixel](https://github.com/adafruit/Adafruit_CircuitPython_NeoPixel)
* Adafruit [Display Text](https://github.com/adafruit/Adafruit_CircuitPython_Display_Text)


## Links to technical docs

* [Current Time Service](https://www.bluetooth.com/specifications/specs/current-time-service-1-1/); you will want the first doc ("Current Time Service 1.1"). But the service and characteristic values are described in the [Assigned Numbers](https://www.bluetooth.com/specifications/specs/assigned-numbers/) doc (look for "Current Time" to find that the service is 0x1805 and the corresponding characteristic is 0x2A2B. The [Specifications Supplement](https://www.bluetooth.com/specifications/specs/gatt-specification-supplement/) then says what the contents of an 'Exact Time 256' are.
* [Simple Bluetooth Current Time Service](https://apps.microsoft.com/store/detail/simple-bluetooth-current-time-service/9NJQ3TD3K06F) app on the Microsoft app store
* [Source Code](https://github.com/pedasmith/BluetoothDeviceController); look in [SmallProjects / BluetoothCurrentTimeServer](https://github.com/pedasmith/BluetoothDeviceController/tree/main/SmallProjects/BluetoothCurrentTimeServer) for the code just for the Windows Current Time Service plus the Adafruit CircuitPython
* [Adafruit](https://www.adafruit.com/product/4500) has complete information on their Clue device
* [CircuitPython for CLUE](https://circuitpython.org/board/clue_nrf52840_express/) for the "UF2" file that will install CircuitPython. Follow the instructions!
* [Clue Libraries](https://circuitpython.org/libraries) -- I got the version 8.x ones marked as being from 2023-06-17, otherwise known as "the most recent ones"
* [Adafruit CircuitPython BLE](https://learn.adafruit.com/circuitpython-ble-libraries-on-any-computer)
* [Real Time Clock](https://docs.circuitpython.org/en/latest/shared-bindings/rtc/index.html)
* [struct_time](https://docs.python.org/3/library/time.html#time.struct_time)
* [The Clue device](https://docs.circuitpython.org/projects/clue/en/latest/api.html) show the APIs in the "clue" device in Python

## Links to Videos and Blogs

* [Youtube](https://www.youtube.com/watch?v=_OHZJ7dY5v0)
* [This post](https://sunriseprogrammer.blogspot.com/2023/06/clocks-that-set-themselves.html)
* [Earlier post](https://sunriseprogrammer.blogspot.com/2023/04/hints-on-using-circuitpythons.html)

### **Initial steps: install CircuitPython** and start **mu**
1. Download CircuitPython .UF2 file. I'm using version **8.1**
2. Plug in Clue with USB cable (not a power-only one!)
3. Double-client reset button
4. Drag and drop the .UF2 file
5. You will know it worked because the drive is now **CIRCUITPY**
6. Download the libraries that are mentioned in the documentation. Open the zip file, Install the 6 mpy files and 6 directories in the [list](https://learn.adafruit.com/adafruit-clue/clue-circuitpython-libraries)

adafruit_bmp280.mpy, adafruit_clue.mpy, adafruit_lis3mdl.mpy, adafruit_sht31d.mpy, adafruit_slideshow.mpy, neopixel.mpy

adafruit_apds9960
adafruit_bus_device
adafruit_display_shapes
adafruit_display_text
adafruit_lsm6ds
adafruit_register

7. Also install the **adafruit_ble** library. This is essential for setting the clock
7. Start mu, the editor