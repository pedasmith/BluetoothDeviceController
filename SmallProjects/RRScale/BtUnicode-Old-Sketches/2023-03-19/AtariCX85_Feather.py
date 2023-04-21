"""CircuitPython program to control an Atari CX85 keyboard'"""

#
# To use this file, put these lines into your code.py file

# import AtariCX85_Feather as AtariCX85
# AtariCX85.setup()
# AtariCX85.loop()
#
# By doing that, the code.py can be small and all of the specific functions
# can be in a file with a proper name.

import time
import board
import digitalio
import neopixel

#
#  AtartiCx85 Section
#
CurrKey = True

AtariCx85TriggerPin = digitalio.DigitalInOut(board.D13)
AtariCx85Fwd = digitalio.DigitalInOut(board.D6)
AtariCx85Back = digitalio.DigitalInOut(board.D9)
AtariCx85Left = digitalio.DigitalInOut(board.D10)
AtariCx85Right = digitalio.DigitalInOut(board.D11)
AtariCx85BPot = digitalio.DigitalInOut(board.D12)
# // AtariCx85APot isn't used by the Cx85

#
# Bluetooth LE Section
#

#
# LED + LOG Section
#
Red = 0
Yellow = 1
Green = 2
Cyan = 3
Blue = 4
Magenta = 5
White = 6
Off = 7
CurrLed = Off
pixels = neopixel.NeoPixel(board.NEOPIXEL, 30)
pixels[0] = (30, 100, 30)

led = digitalio.DigitalInOut(board.LED)
led.direction = digitalio.Direction.OUTPUT

nrawprint = 0
nrawprintline = 0

#
# LED + LOG Functions
#
def LedNext(color):
    next = color + 1
    if (next > Magenta):
        next = Red  # do not include white or off in the rotation
    return next

def SetLed(color):
    hi = 50
    if (color == Off):
        pixels[0] = (0, 0, 0)
    elif (color == Red):
        pixels[0] = (hi, 0, 0)
    elif (color == Yellow):
        pixels[0] = (hi, hi, 0)
    elif (color == Green):
        pixels[0] = (0, hi, 0)
    elif (color == Cyan):
        pixels[0] = (0, hi, hi)
    elif (color == Blue):
        pixels[0] = (0, 0, hi)
    elif (color == Magenta):
        pixels[0] = (hi, 0, hi)
    elif (color == White):
        pixels[0] = (hi, hi, hi)
    else:
        pixels[0] = (hi, hi, hi)


# AtariCX85 Functions

# Reads in raw electrical signals from an ATARI CX85 keypad and
# converts them into Microsoft-style virtual key codes.
#
# Link: Virtual key codes:
# https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
def ReadAtariCx85Pins():

    fwd = AtariCx85Fwd.value
    back = AtariCx85Back.value
    left = AtariCx85Left.value
    right = AtariCx85Right.value
    bpot = AtariCx85BPot.value

    raw = 0
    if (fwd is True):
        raw += 1
    if (back is True):
        raw += 2
    if (left is True):
        raw += 4
    if (right is True):
        raw += 8

    # print ("raw=", hex(raw), end=' ')
    # print ("fwd=", fwd, end=' ')
    # print ("back=", back, end=' ')
    # print ("left=", left, end=' ')
    # print ("right=", right, end=' ')
    # print ("bpot=", bpot, end=' ')

    keycode = 0
    scancode = 0
    # https://gist.github.com/MightyPork/6da26e382a7ad91b5496ee55fdc73db2/5c4e72a23e7e70ec29982c34a27de076d7903bc0
    stringcode = ""
    if (bpot is True):  # most keys
        if (raw == 0x0C):   # 0
            keycode = 0x60
            scancode = 0x62
            stringcode = "0"
        elif (raw == 0x09):   # 1
            keycode = 0x61
            scancode = 0x59
            stringcode = "1"
        elif (raw == 0x0A):   # 2
            keycode = 0x62
            scancode = 0x5a
            stringcode = "2"
        elif (raw == 0x0B):  # 3
            keycode = 0x63
            scancode = 0x5b
            stringcode = "3"
        elif (raw == 0x01):  # 4
            keycode = 0x64
            scancode = 0x5c
            stringcode = "4"
        elif (raw == 0x02):  # 5
            keycode = 0x65
            scancode = 0x5d
            stringcode = "5"
        elif (raw == 0x03):  # 6
            keycode = 0x66
            scancode = 0x5e
            stringcode = "6"
        elif (raw == 0x05):  # 7
            keycode = 0x67
            scancode = 0x5f
            stringcode = "7"
        elif (raw == 0x06):  # 8
            keycode = 0x68
            scancode = 0x60
            stringcode = "8"
        elif (raw == 0x07):  # 9
            keycode = 0x69
            scancode = 0x61
            stringcode = "9"
        elif (raw == 0x0D):  # .
            keycode = 0x6E  # vk_decimal
            scancode = 0x63
            stringcode = "."
        elif (raw == 0x0F):  # -
            keycode = 0x6D  # vk_subtract minus
            scancode = 0x56
            stringcode = "😋👩‍👩‍👧‍👧" # Try that Unicode! "-"
        elif (raw == 0x0E):
            keycode = 0x0D  # vk_enter
            scancode = 0x58
            stringcode = "\n"
        elif (raw == 0x04):
            keycode = 0x21  # F2=no=vk_prior=page up
            scancode = 0x4b
            stringcode = "pgup"
        elif (raw == 0x00):
            keycode = 0x2E  # F3=vk_delete
            scancode = 0x2a
            stringcode = "del"
        elif (raw == 0x08):
            keycode = 0x22  # F4=yes=vk_next=page down
            scancode = 0x4e
            stringcode = "pgdo"
    else:  # only LOW key is F1=ESCAPE
        keycode = 0x1B  # F1=vk_escape
        scancode = 0x29
        stringcode = "esc"
    return (keycode, scancode, stringcode)

# main function to read in the trigger pin and from there
# call the specialized functions to handle the actual value.
# Sets the various globals
# returns False when we should break out of the loop
# ble = adafruit_ble.BLERadio()
def readPin(ble, btUnicodeKeyboard):
    global CurrKey, CurrLed, nrawprint, nrawprintline
    retval = True

    pin = AtariCx85TriggerPin.value
    if (pin == CurrKey):
        return retval

    CurrKey = pin
    if (pin is True):
        # LED + LOG
        SetLed(Off)

        # BLE
        btUnicodeKeyboard.press = 0
        return retval

    time.sleep(.050)  # pause for values to settle
    # AtariCx85
    values = ReadAtariCx85Pins()
    key = values[0]
    scancode = values[1]
    stringcode = values[2]

    # LED + LOG
    print(stringcode, "=", hex(key), end='')
    nrawprint += 1
    if (nrawprint < 8):
        print(" ", end='')
    else:
        print("\n", end='')
        nrawprint = 0
        nrawprintline += 1
    CurrLed = LedNext(CurrLed)
    SetLed(CurrLed)

    # BLE
    #  remove for now: k.send(scancode)
    btUnicodeKeyboard.virtualKey = key
    btUnicodeKeyboard.scanCode = scancode
    btUnicodeKeyboard.utf8 = stringcode
    btUnicodeKeyboard.pressCount += 1
    btUnicodeKeyboard.press = 1

    # Handy debug code when "esc" is pressed
    if (stringcode == "esc"):
        retval = False
        print("Special debugging for escape")

    return retval

def SetupAtariCx85():

    # AtariCx85
    AtariCx85TriggerPin.switch_to_input(pull=digitalio.Pull.DOWN)
    AtariCx85Fwd.switch_to_input(pull=digitalio.Pull.DOWN)
    AtariCx85Back.switch_to_input(pull=digitalio.Pull.DOWN)
    AtariCx85Left.switch_to_input(pull=digitalio.Pull.DOWN)
    AtariCx85Right.switch_to_input(pull=digitalio.Pull.DOWN)
    AtariCx85BPot.switch_to_input(pull=digitalio.Pull.DOWN)

#
# Main Program Section
#
def setup():
    SetupAtariCx85()
    led.value = False

# ble = adafruit_ble.BLERadio()
def loop(ble, btUnicodeKeyboard):

    keep_going = True
    while keep_going:
        # led.value= AtariCx85TriggerPin.value
        time.sleep(0.05)
        keep_going = readPin(ble, btUnicodeKeyboard)


# end of file

