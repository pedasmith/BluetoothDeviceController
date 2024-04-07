# The Hiker's Companion

The Hiker's Companion is an Adafuit Clue with hiker-friendly features. If you make one, you should pick a large battery -- the typical 370 mAH battery will only run the device for 6 to 8 hours, which isn't enough for safety. A Hiker Companian really needs to be good enough. Note that the device uses about 50 mA of power.

The device can use other microcontrollers and displays.

The display can show: 
1. Large title + 3 lines of 10 chars
2. Small title + 4 lines of 10 chars
3. Large value + 7 lines of small text


1. Pressure including barometer + height from a starting point
2. Compass
3. Humidity
4. Automatically set expected sunset times
5. Add a "turn around" time. After turning around, will the hiker to estimate time remaining


## Menu

There's only two buttons, next and select
There's 3 displays

### Basic clock
Shows the time, date, and day of week
Next --> Hiker data
Select --> Flashlight

### Hiker data
Shows the compass, pressure, temp + humidity  
Next --> Setup
Select --> Flashlight

### Setup (small title)
Does common setups that the user will generally do once. These are stored in the permanent memory, so they will be saved from one use to the next.

Next --> goto Basic clock
temp --> C/F
pressure --> height/PSI/mbar
alarm --> off / beep twice / continuous beep
Hiker Settings --> 
Advanced settings -->

### Hiker Settings
The hiker settings are all the settings that a hiker might want to do at the start of a hike. Note that it's a little funny that these "once a hike" settings are done 

Next --> goes back to the setup screen
SLP --> Number setting for SLP
Altitude --> Sets the current altitude which will set the SLP.
Sunset --> 
Turnaround time --> 

Time to sunset: says how many hours until sunset; is handy for setting the turnaround time.


#### How Select Altitude works

There's a formula that, given the correct SLP (Sea Level Pressure) will calculate the current altitude. While hiking, of course, often you won't know the SLP, but you will know your exact altitude at an area. Although the formula can be reverse engineered, it's much easier to do a "binary search" for the value. It's also more correct: although there is a formula for altitude, it's also the case that the Adafruit has this built in as a black box: we don't know for sure exactly how it works.

Instead, we will pick the potential value (PV) with a reasonable starting value ( 1000) plus a  delta (256). We also of course have a target value. 

1. **START** Set the SLP to the PV
2. if delta < tiny-value goto DONE // this is an error
2. If altitude == target, goto DONE
3. If altitude > target, PV += delta; delta = delta / 2; goto START
4. else PV -= delta; delta = delta / 2; goto START
5. **DONE**

Note that this is guaranteed to find the right value in about 8 steps, so it's pretty fast. If does assume that the altitude and pv doesn't ever reverse direction (e.g., the derivative is always the same).



### Number setting

The user needs to be able to set the Sea Level Pressure (SLP), possibly from the weird SLP format (where "100" means "1100" and "600" means "9600")

#### Current version 2023-08-03

With the number setting, the display will look like this:

Title is the number to set (e.g., "1154")
1. What's being set ("SLP" or "Sea Level Pressure")(light gray)
2. Next
3. Clear
4. Digits which wraps between 1000, 100, 10, 1, .1. The values are settable
5. +digit
6. -digit

The left button selects between 2..6, wrapping around. The right button will select the action. 


#### Original concept
With the number setting, the display will look like this:

Title is the number to set (e.g., "1154") displayed as \*1\*154
1. What's being set ("SLP" or "Sea Level Pressure")(light gray)
2. Higher
3. Lower
4. Next digit
5. Previous digit
6. OK

The left button selects between 2..6, wrapping around. The right button will select the action. 

Example: the current SLP is 1234 and the user wants it to be 1241. The user will:
1. LEFT LEFT LEFT (Next Digit) Right (1\*2\*34) Right (12\*3\*4). 
2. LEFT LEFT LEFT (Higher) Right (12\*4\*4)
3. LEFT LEFT (Next Digit) Right (124\*4\*)
4. LEFT LEFT LEFT LEFT (Lower) Right Right Right (124\*1\*)
5. LEFT LEFT LEFT (OK) Right



### Clock settings

The user also needs to set time values like the sunset time. This is similar to the number setting

Title is the time to set
1. What's being set ("sunset time")
2. Hours +
3. Hours -
5. 10 minutes +
6. Minutes +
7. Minutes -
7. OK

Example: the time is 7:12 and the user wants it to be 9:59
1. LEFT (+) right right (9:12)
2. LEFT LEFT (10 minutes) right right right right (9:52)
3. LEFT LEFT (Minutes -) right right right (9:59)


## Picking a new device

When the project was mostly done, my Clue stopped working. Now I need to pick a new controller (sad face). What I want is something with Bluetooth (**BLE**), a RTC, pressure/humidity (**PTH**), altitude (**alt**), compass (**mag**)

Choices are

| device | BT | charging | RTC |PTH|alt|compass|
|---|---|---|---|---|---|---|
|[**nrf52840** feather](https://www.adafruit.com/product/4062)|?|BT|no|no|no|no
|[**nrf52832** feather](https://www.adafruit.com/product/3406)|charge|BT|no|no|no|no
|[**nrf52840** itsy bitsy](https://www.adafruit.com/product/4481)|?|BT

| Syntax      | Description |
| ----------- | ----------- |
| Header      | Title       |
| Paragraph   | Text        |



## Details

Magnetometer is the LIS3MDL
