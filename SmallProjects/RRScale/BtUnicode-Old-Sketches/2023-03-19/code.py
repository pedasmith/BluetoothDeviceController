"""CircuitPython program to control an Atari CX85 keyboard'"""

# This file must be named "code.py" to run on the
# AdaFruit nRF52832 Feather Express board
#
#

import time
import AtariCX85_Feather as AtariCX85
import BtUnicodeKeyboardService as BtUnicodeKeyboardService

import adafruit_ble
# from adafruit_ble.advertising import Advertisement
from adafruit_ble.advertising.standard import ProvideServicesAdvertisement
# from adafruit_ble.services.standard.hid import HIDService
from adafruit_ble.services.standard.device_info import DeviceInfoService
# from adafruit_hid.keyboard import Keyboard
# from adafruit_hid.keyboard_layout_us import KeyboardLayoutUS
# not used from adafruit_hid.keycode import Keycode

# set up the device information
device_info = DeviceInfoService(
    software_revision=adafruit_ble.__version__,
    manufacturer="ShipwreckSoftware",
    model_number="BTUnicode Keyboard DI1100",
    )

# now set up the adverts.
# hid = HIDService()  # present but not added to advertisement
btUnicodeKeyboard = BtUnicodeKeyboardService.BtUnicodeKeyboardService()
advertisement = ProvideServicesAdvertisement(btUnicodeKeyboard, device_info)
advertisement.appearance = 0x3c1  # was 961 is Keyboard
advertisement.complete_name = "BTUnicode Keyboard AD1100"

ble = adafruit_ble.BLERadio()
ble.name = "BTUnicode Keyboard BLE1100"


# If I'm not mistaken, this logic makes no sense. It's
# only called at startup. But at startup, will there be
# any connections already?
if not ble.connected:
    print("Radio startup status: not connected; start advertising")
    ble.start_advertising(advertisement, None, interval=0.021)
else:
    print("Radio startup status: already connected. Connection list:")
    print(ble.connections)

# k = Keyboard(hid.devices)
# kl = KeyboardLayoutUS(k)

AtariCX85.setup()
while True:
    AtariCX85.loop(ble, btUnicodeKeyboard)
    print("NOTE: address=", ble.address_bytes)
    print("NOTE: connected=", ble.connected)
    if (ble.connected):
        for connection in ble.connections:
            print("    connection connected=", connection.connected,
                  " paired=", connection.paired)
            connection.disconnect()
    ble.stop_advertising()
    #  nope, not allowed: ble.address_bytes[0] += 1  # change the address...
    time.sleep(0.2)
    ble.start_advertising(advertisement, None, interval=0.021)
    print("Started advertising")
