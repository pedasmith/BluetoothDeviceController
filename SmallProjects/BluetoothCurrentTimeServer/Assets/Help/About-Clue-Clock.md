# The Adafruit Clue-Clock

## Script

*video of clock running* I made a clock! And it has a special feature. *reset* when it resets, it doesn't know the time. But wait a second, and it's automatically reset itself! It's all thanks to Bluetooth and service 0x1805, the Current Time Service. The device here is an Adafruit "Clue" microcontroller. 

The visisble 

You can get an app called "[Simple Bluetooth Time Service](https://apps.microsoft.com/store/detail/simple-bluetooth-current-time-service/9NJQ3TD3K06F)" on the Windows store; that's how I set the time. The complete source code for it is at 


## Steps to making it work

### **Initial steps: install CircuitPython** and start **mu**
1. Download CircuitPython .UF2 file. I'm using version **8.1**
2. Plug in Clue with USB cable (not a power-only one!)
3. Double-client reset button
4. Drag and drop
5. You will know it worked because the drive is now **CIRCUITPY**
6. Download the libraries, open the zip file, Install the 6 mpy files and 6 directories in the list
7. Also install the **adafruit_ble** library. This is essential for setting the clock
7. Start mu, the editor


### **Display** stuff to the screen

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

### Use the **Real Time Clock**

The chip used to track time accurately is the "real time clock" -- without it, the code would slowly drift. Fun fact: the first IBM PC did not include a battery-backed real-time clock chip. Every time you turned on the computer, you had to enter the date and time.

The real-time clock uses [struct_time](https://docs.python.org/3/library/time.html#time.struct_time) for many operations; it's just a tuple where you can grab values by index. the current hour, for example, is index 3.

The real-time clock needs power to work; if it loses power, it will stop stracking an accurate date and time (it says "real time clock", but it does dates, too). How we set it is the topic of the next section.

The CircuitPython rtc module is a delight to use: there's just a simple way to set the initial value and a simple way to pull out the current time.


### Connect to Bluetooth Current Time Service

Now we get the hard stuff: reading data from an external Bluetooth "Current Time Service" source. 

You should have already added the **adafruit_ble** to your **lib** directory. It's not on the list of libraries in the Adafruit Clue documentation.



## Links to technical docs

* [Current Time Service](https://www.bluetooth.com/specifications/specs/current-time-service-1-1/); you will want the first doc ("Current Time Service 1.1"). But the service and characteristic values are described in the [Assigned Numbers](https://www.bluetooth.com/specifications/specs/assigned-numbers/) doc (look for "Current Time" to find that the service is 0x1805 and the corresponding characteristic is 0x2A2B. The [Specifications Supplement](https://www.bluetooth.com/specifications/specs/gatt-specification-supplement/) then says what the contents of an 'Exact Time 256' are.
* [Simple Bluetooth Current Time Service](https://apps.microsoft.com/store/detail/simple-bluetooth-current-time-service/9NJQ3TD3K06F) app on the Microsoft app store
* [Source Code](https://github.com/pedasmith/BluetoothDeviceController); look in [SmallProjects / BluetoothCurrentTimeServer](https://github.com/pedasmith/BluetoothDeviceController/tree/main/SmallProjects/BluetoothCurrentTimeServer) for the code just for the Windows Current Time Service plus the Adafruit CircuitPython
* [Adafruit](https://www.adafruit.com/product/4500) has complete information on their Clue device
* [CircuitPython for CLUE](https://circuitpython.org/board/clue_nrf52840_express/) for the "UF2" file that will install CircuitPython. Follow the instructions!
* [Clue Libraries](https://circuitpython.org/libraries) -- I got the version 8.x ones marked as being from 2023-06-17, otherwise known as "the most recent ones"
* [Real Time Clock](https://docs.circuitpython.org/en/latest/shared-bindings/rtc/index.html)
* [struct_time](https://docs.python.org/3/library/time.html#time.struct_time)
* [The Clue device](https://docs.circuitpython.org/projects/clue/en/latest/api.html) show the APIs in the "clue" device in Python