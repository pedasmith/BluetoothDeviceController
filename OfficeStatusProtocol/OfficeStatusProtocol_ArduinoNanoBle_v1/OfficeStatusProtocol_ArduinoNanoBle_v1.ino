// Steps 
// 1. Can advertise
// 2. Will advertise based on potentiometer

#include "ArduinoBLE.h"
// OFficial library to control Bluetooth LE AKA BLE
// https://www.arduino.cc/reference/en/libraries/arduinoble/
// https://www.arduino.cc/en/Reference/ArduinoBLEBLEServiceBLEService


#include <NanoBLEFlashPrefs.h>
// Nano doesn't have EEPROM like other Arduino; replacement is 
// Library: https://www.arduino.cc/reference/en/libraries/nanobleflashprefs/
// Github: https://github.com/Dirk-/NanoBLEFlashPrefs
// Uses the FDS service which is ... somewhere?

#define NSWITCHES 20
byte CurrSwitchesValues[NSWITCHES];
byte LastSwitchesValues[NSWITCHES];
#define NCONTROLS 10

struct  {
  char MemoString[20] = "(no message)";
  byte CurrControlsValues[NCONTROLS] = { 0,0,0,0,0,0,0,0,0,0 };
} FlashData;
NanoBLEFlashPrefs myFlashPrefs;  
    // I'm not keen on the name 'myFlashPref'. It's not a good name because it's 
    // not my references; it's the manager for the long-term storage on the chip.
    // FlashDataManager would be a better name.
    // Not changed because to maintain compatability with the NanoBLEFlashPrefs code.



byte Advertisement[20] = { 0,0,0,0,0,0,0,0,0,0,  0,0,0,0,0,0,0,0,0,0 };
int AdvertisementLength = 0;
enum BRIGHTNESS { Off=0, Dim=1, Normal=2, Bright=3};

void InitAdvertisement(byte msg, BRIGHTNESS bright, byte blink, char* text) {
  int index = 0;
  Advertisement[index++] = 0xFF; // Manufacturer ID;
  Advertisement[index++] = 0x10;

  Advertisement[index++] = 0xBA; // Protocol and sub-protocol
  Advertisement[index++] = 0x59;

  Advertisement[index++] = msg;
  Advertisement[index++] = bright;
  Advertisement[index++] = blink;
  // Can't use strcpy because I don't want a NUL at the end.
  for (char* p=text; *p; p++) {
    Advertisement[index++] = (byte)*p;
  }
  AdvertisementLength = index;
}

void setup() {

  // Set up the pins
    pinMode(A0, INPUT);


  // Set up the advertisements
  BLE.begin();
  BLE.setLocalName("OfficeStatusLN");
  BLE.setDeviceName("OfficeStatusDN");
  //BLE.setAdvertisingInterval(120); // 75ms / 0.625 = 120; units are 0.625 ms
  InitAdvertisement(0xFF, Off, 0, "Hello Office");
  BLE.setManufacturerData(Advertisement, AdvertisementLength);
  BLE.advertise();



}


void loop() {
  int value = analogRead(A0);
  int index = 3;
  if (value < 25) index = 0;
  else if (value < 65) index = 1;
  else if (value < 470) index = 2;
  else index = 3;

  InitAdvertisement(index, Normal, 0, "ABCDEF");
  BLE.setManufacturerData(Advertisement, AdvertisementLength);

  Serial.print ("Loop: index="); Serial.println(index);

  delay(1000);
}