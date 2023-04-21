# BtUnicodeKeyboard service

# import adafruit_ble
from adafruit_ble.services import Service
from adafruit_ble.characteristics import Characteristic
# from adafruit_ble.characteristics.json import JSONCharacteristic
from adafruit_ble.characteristics.int import Int32Characteristic
from adafruit_ble.characteristics.string import StringCharacteristic
from adafruit_ble.uuid import VendorUUID

class BtUnicodeKeyboardService(Service):
    # pylint: disable=too-few-public-methods

    uuid = VendorUUID("b7b0a005-d6a5-41ed-892b-4ce97f8c0397")

    press = Int32Characteristic(
        uuid=VendorUUID("b7b0a009-b23d-428f-985c-f6f26a80bf1f"),
        properties=Characteristic.READ | Characteristic.NOTIFY,
    )
    pressCount = Int32Characteristic(
        uuid=VendorUUID("b7b0a07e-a995-4eae-9315-856e31bd7334"),
        properties=Characteristic.READ | Characteristic.NOTIFY,
    )

    virtualKey = Int32Characteristic(
        uuid=VendorUUID("b7b0a035-852f-4a31-bae4-fcd4510c444d"),
        properties=Characteristic.READ | Characteristic.NOTIFY,
    )

    scanCode = Int32Characteristic(
        uuid=VendorUUID("b7b0a047-d291-41a3-8c2c-2f4bfa46fef9"),
        properties=Characteristic.READ | Characteristic.NOTIFY,
    )
    utf8 = StringCharacteristic(
        uuid=VendorUUID("b7b0a074-e122-4a2d-ae7e-3c596cfcae3b"),
        properties=Characteristic.READ | Characteristic.NOTIFY,
        initial_value="(start)",
    )

    def __init__(self, service=None):
        super().__init__(service=service)
        self.connectable = True
        print("BtUnicode: dir=", dir(self.__class__))

