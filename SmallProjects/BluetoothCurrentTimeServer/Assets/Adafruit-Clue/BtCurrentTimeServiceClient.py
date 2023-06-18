import time

from adafruit_ble.advertising.standard import ProvideServicesAdvertisement
# from adafruit_ble.characteristics import Characteristic
from adafruit_ble.characteristics import StructCharacteristic
from adafruit_ble.services import Service
from adafruit_ble.uuid import StandardUUID

# Sometimes needed, but not here.
# from adafruit_ble.advertising import Advertisement
# from adafruit_ble.uuid import UUID

class BtCurrentTimeServiceClientRunner:
    def Scan(self, ble):
        """Returns the time from the scanner"""
        print("NOTE: BT scan started for CurrentTimeService")
        retval = None

        found = set()
        timeServiceConnection = None
        GOAL_TIME = 15
        maxTime = GOAL_TIME
        startTime = time.monotonic()

        while (timeServiceConnection is None) and (maxTime > 0):
            timeUsed = time.monotonic() - startTime
            maxTime = GOAL_TIME - timeUsed
            timeServiceConnection = self.ScanOnce(ble, found, maxTime)
        if timeServiceConnection is not None:
            service = self.ConnectToCurrentTimeService(ble, timeServiceConnection)
            retval = service.data
            timeServiceConnection.disconnect()
            timeServiceConnection = None
        print("DBG: return from Scan: retval=", retval)
        return retval

    def ScanOnce(self, ble, found, maxTime):
        """Do one Bluetooth scan and return a connected CurrentTimeService connection"""
        if (maxTime <= 0):
            return None

        timeServiceConnection = None
        list = ble.start_scan(ProvideServicesAdvertisement, timeout=maxTime)
        # We get the list right away. It will actually be filled in over time
        # and will stop only after the timeout.
        total_advert = 0
        for advert in list:
            total_advert = total_advert + 1
            addr = advert.address
            if advert.scan_response or addr in found:
                # A response is an advertisement response. We're just
                # looking for a plain advert, so skip those.
                # We also don't need to revisit device we're already
                # looked at.
                continue
            found.add(addr)
            if type(advert) is ProvideServicesAdvertisement:
                svs = advert.services
                if BtCurrentTimeServiceClient.uuid in svs:
                    timeServiceConnection = ble.connect(addr)
                    break
        # Should match the start_scan with stop_scan.
        ble.stop_scan()
        return timeServiceConnection

    def ConnectToCurrentTimeService(self, ble, connection):
        service = None
        if BtCurrentTimeServiceClient.uuid not in connection:
            print("Error: in ConnectToService: expected service UUID in connection")
            return

        if (connection.connected is False):
            print("Error: in ConnectToService: expected a connected connection")
            return

        try:
            service = connection[BtCurrentTimeServiceClient]
            if (service is not None):
                # print("DBG: Type(class)", type(BtCurrentTimeServiceClient.data))
                # print("DBG: class prop", BtCurrentTimeServiceClient.data.properties)
                # print("DBG: Type(obj)", type(service.data))
                ts = service.GetTimeString()
                print("Got time=", ts)
                # time.sleep(4)
                # print("after 4 seconds: ", service.GetTimeString())

        except Exception as ex:
            print("ERROR: in ConnectToService: exception when getting data", ex)
        print("DBG: return from ConnectToCurrentTimeService service=", service)
        return service

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





