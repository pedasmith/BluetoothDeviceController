# display imports
from adafruit_clue import clue

# rtc imports
import rtc
import time

# BT Current Time Service imports
import adafruit_ble
import BtCurrentTimeServiceClient


# led = digitalio.DigitalInOut(board.LED)
# led.direction = digitalio.Direction.OUTPUT

def DisplayTime(clue_display, dt):
    timestr = "{:02d}:{:02d}:{:02d}".format(dt[3], dt[4], dt[5])
    clue_display[0].text = " " + timestr
    datestr = "{:04d}-{:02d}-{:02d}".format(dt[0], dt[1], dt[2])
    clue_display[1].text = datestr
    daw = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
    dawstr = daw[dt[6]].center(10)
    clue_display[2].text = dawstr
    clue_display.show()

def DisplayBTStart(clue_display):
    clue_display[2].text = "BT:Scan"
    clue_display.show()

def DisplayBTEnd(clue_display, result):
    clue_display[2].text = "BT: " + result
    clue_display.show()

# Step 1: write to the display
colors = ((0xff, 0xff, 0xff),(0xff, 0xff, 0xff),(0xa0, 0xa0, 0xff),)
display = clue.simple_text_display(title="Clock",
                                   title_scale=2, text_scale=4,
                                   title_color=(0xa0, 0xa0, 0xa0),
                                   colors=colors
                                   )

# Step 2: set up the real-time clock

clock = rtc.RTC()
DisplayTime(display, clock.datetime) # display the current time


# Step 3: set up the BT Timer Service
DisplayBTStart(display)
timeservice = BtCurrentTimeServiceClient.BtCurrentTimeServiceClientRunner()
ble = adafruit_ble.BLERadio()
nowBT = timeservice.Scan(ble)
if (nowBT is None):
    DisplayBTEnd(display, "No BT clck")
    time.sleep(5)
else:
    # (y, m, d, hh, mm, ss, j1, j2, j3) = timeresult.data
    # returns e.g., (year, mon, day, hour, minute, second, 6, 0, 1)
    now = (nowBT[0], nowBT[1], nowBT[2], nowBT[3], nowBT[4], nowBT[5], nowBT[6], 0, 0)
    clock.datetime = now
    DisplayBTEnd(display, "OK")


# Do a loop to run the clock. This is slightly off
# because it will likely occasionally skip a second.
while True:
    DisplayTime(display, clock.datetime)
    time.sleep(1)



