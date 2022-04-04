#include "ArduinoBLE.h"
// OFficial library to control Bluetooth LE AKA BLE
// https://www.arduino.cc/en/Reference/ArduinoBLEBLEServiceBLEService


#include <NanoBLEFlashPrefs.h>
// Nano doesn't have EEPROM like other Arduino; replacement is 
// Library: https://www.arduino.cc/reference/en/libraries/nanobleflashprefs/
// Github: https://github.com/Dirk-/NanoBLEFlashPrefs
// Uses the FDS service which is ... somewhere?

#include <Servo.h>

//TODO: I'm just getting started with Servo support
// For now, just make  single servo.
Servo PinD9;

#define NSWITCHES 20
byte CurrSwitchesValues[NSWITCHES];
byte LastSwitchesValues[NSWITCHES];

#define NCONTROLS 10
byte LastControlsValues[NCONTROLS];
struct  {
  char MemoString[20] = "(RRScale memo)";
  byte CurrControlsValues[NCONTROLS] = { 0,0,0,0,0,0,0,0,0,0 };
} FlashData;
NanoBLEFlashPrefs myFlashPrefs;  
    // I'm not keen on the name 'myFlashPref'. It's not a good name because it's 
    // not my references; it's the manager for the long-term storage on the chip.
    // FlashDataManager would be a better name.
    // Not changed because to maintain compatability with the NanoBLEFlashPrefs code.





// BLE nomenclature can be confusing
// The Arduino is the **peripheral** and acts as a **server**
// The PC that reads in the Arduino data and sends commands is the **central** and is the **client**
// As a GATT server, the arduino peripheral provides a set of **services**, each of which has a set of **characteristics**
// The actual data is sent as part of a characteristic.

BLEService RRScaleService ("09a9e5eb-b5de-4c23-9f51-b08c87fd6650");

BLECharacteristic MOTD ("09a9eeca-5a4f-4dc2-9a18-74080c704e21", BLERead | BLENotify, "Safe travels today!");
BLEDescriptor MOTDName("2901", "MOTD");

BLECharacteristic Memo ("09a9e440-0d08-4c2d-abe3-f9ab5e2544ed",  BLEWrite| BLERead | BLENotify, FlashData.MemoString);
BLEDescriptor MemoName("2901", "Memo");

BLECharacteristic Switches ("09a9e524-277d-405e-bcc4-a40bed62f2be", BLERead | BLENotify, 20);
BLEDescriptor SwitchesName("2901", "Switches");

BLECharacteristic SwitchCount ("09a9e940-bbf2-4e2f-9b05-9107cbe4d5cf", BLERead | BLENotify, 2);
BLEDescriptor SwitchCountName("2901", "SwitchCount");

BLECharacteristic Controls ("09a9e678-afea-419b-a117-923619763bb7", BLEWrite | BLERead , 20);
BLEDescriptor ControlsName("2901", "Controls");

BLECharacteristic ControlCount ("09a9e406-a94c-4111-a5ed-d2d7673cff72", BLERead | BLENotify, 2);
BLEDescriptor ControlCountName("2901", "ControlCount");

BLECharacteristic Command ("09a9ef97-5e6d-498f-bdf9-7c0ed1f41333", BLEWrite | BLERead | BLENotify, 1);
BLEDescriptor CommandName("2901", "Command");

BLECharacteristic CommandResult ("09a9e19f-e915-4a3b-9f0c-66ec48b10f81", BLERead | BLENotify, 4);
BLEDescriptor CommandResultName("2901", "CommandResult");


void setup() {

  PinD9.attach(9); //, 1000, 2000); // TODO: does 9 mean pin D9? Answer: seems like it!
  PinD9.write (45);

  Serial.begin(9600); // Long obsolete; USB serial doesn't use a baud rate (and is always enabled).

  ReadFlash(); // Reads from flash but won't apply it to BLE pins


  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);

  if (!BLE.begin())
  {
    return;
  }

  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(A2, INPUT);
  pinMode(A3, INPUT);
  pinMode(A6, INPUT);
  pinMode(A7, INPUT);

  pinMode(D2, INPUT_PULLUP);
  pinMode(D3, INPUT_PULLUP);
  pinMode(D4, INPUT_PULLUP);
  pinMode(D5, INPUT_PULLUP);
  pinMode(D6, INPUT_PULLUP);

  //pinMode(D9, OUTPUT); //TODO: don't set this here; it's already set
  pinMode(D10, OUTPUT);
  pinMode(D11, OUTPUT);
  pinMode(D12, OUTPUT);


//DBG: what shows up in Windows? In BLE tools?
  BLE.setLocalName("RRScaleLN"); // LocalName is what show up in Bluetooth Data Explorer
  BLE.setDeviceName("RRScaleDN"); // Sometime I get the DN name
  BLE.setAdvertisedService(RRScaleService);

  Memo.addDescriptor(MemoName);
  RRScaleService.addCharacteristic(Memo);

  MOTD.addDescriptor(MOTDName);
  RRScaleService.addCharacteristic(MOTD);

  Switches.addDescriptor(SwitchesName);
  RRScaleService.addCharacteristic(Switches);

  SwitchCount.addDescriptor(SwitchCountName);
  RRScaleService.addCharacteristic(SwitchCount);

  Controls.addDescriptor(ControlsName);
  RRScaleService.addCharacteristic(Controls);

  ControlCount.addDescriptor(ControlCountName);
  RRScaleService.addCharacteristic(ControlCount);

  Command.addDescriptor(CommandName);
  RRScaleService.addCharacteristic(Command);

  CommandResult.addDescriptor(CommandResultName);
  RRScaleService.addCharacteristic(CommandResult);


  BLE.addService(RRScaleService);
  BLE.advertise();

  // Initial BLE values
  Command.setEventHandler(BLEWritten, OnCommandChanged);
  Controls.setEventHandler(BLEWritten, OnControlsChanged);

  Memo.writeValue(FlashData.MemoString, strlen (FlashData.MemoString));
  Memo.setEventHandler(BLEWritten, OnMemoChanged);

  SwitchCount.writeValue((uint16_t)NSWITCHES);
  ControlCount.writeValue((uint16_t)NCONTROLS);

  byte cmd[4] = { 0x0, 0x0, 0, 0 };
  CommandResult.writeValue(cmd, sizeof(cmd));

  // Update the digital output from the Flash
  // Could also do a WriteAllFromFlash() which would do that plus the memo.
  WriteControlsFromFlashCurrControlsValues();  

  Serial.println("RRScale: setup done");
}

bool isFirst = true;

void loop() {

  BLEDevice central = BLE.central();
  if (central && central.connected()) {
    int nwrite = 0;
    UpdateInputs();

    // DBG: sweep the servo for debugging
    FlashData.CurrControlsValues[0] += 0x10;

    nwrite += WriteSwitchesIfNeeded(isFirst);
    nwrite += WriteControlsIfNeeded(isFirst);
    isFirst = false;
    Morse (nwrite > 0 ? 'D' : 'U');  // d=dash-dot-dot u=dot-dot-dash
  } else { 
    Morse ('R'); // R for Ready dot-dash-dot
  }
  delay (200);
}


#define DOT BlinkLed(150, 200);
#define DASH BlinkLed(450, 200);
void Morse(char c) {
  switch (c) {
    case 'A': DOT DASH break;
    case 'B': DASH DOT DOT DOT break;
    case 'C': DASH DOT DASH DOT break;
    case 'D': DASH DOT DOT break; // DATA
    case 'E': DOT break;
    case 'F': DOT DOT DASH DOT break;
    case 'G': DASH DASH DOT break;
    case 'H': DOT DOT DOT DOT break;
    case 'I': DOT DOT break;
    case 'J': DOT DASH DASH DASH break;
    case 'K': DASH DOT DASH break;
    case 'L': DOT DASH DOT DOT break;
    case 'M': DASH DASH break;
    case 'N': DASH DOT break; // NO DATA
    case 'O': DASH DASH DASH break;
    case 'P': DOT DASH DASH DOT break;
    case 'Q': DASH DASH DOT DASH break;
    case 'R': DOT DASH DOT break; // READY
    case 'S': DOT DOT DOT break;
    case 'T': DASH break;
    case 'U': DOT DOT DASH break;
    case 'V': DOT DOT DOT DASH break;
    case 'W': DASH DOT DOT DASH break;
    case 'X': DASH DOT DOT DASH break;
    case 'Y': DASH DOT DASH DASH break;
    case 'Z': DASH DASH DOT DOT break;
 }
}

void BlinkLed(int onTimeMs, int offTimeMs) {
    digitalWrite(LED_BUILTIN, HIGH);
    delay(onTimeMs);
    digitalWrite(LED_BUILTIN, LOW);
    delay(offTimeMs);
}

enum CMD { CmdSaveToFlash=1, CmdReadFlash=2, CmdUnknownCommand=0xFF };
int NCommand = 0;
void OnCommandChanged(BLEDevice central, BLECharacteristic controls)
{
  int len = controls.valueLength();
  const int MaxLen = 1;
  if (len > MaxLen) len = MaxLen;
  byte newdata[len];
  controls.readValue(newdata, len);

  if (len >= 1)
  {
    byte cmd = newdata[0];
    byte reply[4];
    NCommand++; // zero value, properly speaking, is the initial value
    reply[0] = (NCommand) & 0xFF;
    reply[1] = (NCommand >> 8) & 0xFF;
    reply[2] = cmd;

    switch (cmd)
    {
      case CmdSaveToFlash:
        reply[3] = SaveToFlash();

        break;
      case CmdReadFlash:
        reply[3] = ReadFlash();
        WriteAllFromFlash();
        break;

      default:
        reply[2] = CmdUnknownCommand;
        reply[3] = cmd;
        break;
    }
    CommandResult.writeValue(reply, 4);
  }
}

// Controls are what the user wants to set
void OnControlsChanged(BLEDevice central, BLECharacteristic controls)
{
  const int MaxLen = NCONTROLS;
  int len = controls.valueLength();
  Serial.println (len);
  if (len > MaxLen) len = MaxLen;
  byte* newdata = FlashData.CurrControlsValues;
  controls.readValue(newdata, len);
  WriteControlsFromFlashCurrControlsValues();
}

void WriteAllFromFlash()
{
  WriteControlsFromFlashCurrControlsValues();
  WriteMemoFromFlash();
}

void WriteControlsFromFlashCurrControlsValues()
{
  byte* newdata = FlashData.CurrControlsValues;
  int len = NCONTROLS;

//TODO: how to pick between analog and digital? Should be configured somehow?
//TODO: note that pin 9 is PWM for Servo

  // This chip has only 4 digital outputs
  int index = 0;
  if (index < len) PinD9.write (newdata[index++]);
  if (index < len) analogWrite (D10, newdata[index++]);
  if (index < len) digitalWrite (D11, newdata[index++]);
  if (index < len) digitalWrite (D12, newdata[index++]);  
}

void WriteMemoFromFlash()
{
  Memo.writeValue(FlashData.MemoString, strlen (FlashData.MemoString));
}

byte ReadFlash()
{
  int rc = myFlashPrefs.readPrefs(&FlashData, sizeof(FlashData));
  if (rc == FDS_SUCCESS) {
    Serial.println ("Read FLASH memory OK");
    Serial.println(FlashData.MemoString);
    Memo.writeValue(FlashData.MemoString, strlen (FlashData.MemoString));

    return 0;

  } else {
    Serial.print("No FLASH preferences found. Return code: ");
    Serial.print(rc);
    Serial.print(", ");
    Serial.println(myFlashPrefs.errorString(rc));
    return rc;
  }  
}
//Return error code is an FDC erorr code. 
byte SaveToFlash()
{
  int rc = myFlashPrefs.writePrefs(&FlashData, sizeof(FlashData));
  if (rc == FDS_SUCCESS) {
    Serial.println ("Write FLASH memory OK");
    Serial.println (FlashData.MemoString);
    return 0;
  } else {
        Serial.print("Unable to write FLASH preferences found. Return code: ");
        Serial.print(rc);
        Serial.print(", ");
        Serial.println(myFlashPrefs.errorString(rc));  
        return rc;  
  }  
}

void OnMemoChanged(BLEDevice central, BLECharacteristic controls)
{
  int len = controls.valueLength();
  len = min (len, sizeof(FlashData.MemoString)-1);
  controls.readValue(FlashData.MemoString, len);
  FlashData.MemoString[len+1] = '\0'; // always NUL terminate
  SaveToFlash();
}


// Updates the CurrSwitchesValues with the current state of the analog
// and digital inputs.
// Every Arduino will have a different mapping from input pins to the switches values.
void UpdateInputs() {
  int index = 0;
  CurrSwitchesValues[index++] = (analogRead(A0) >> 2);
  CurrSwitchesValues[index++] = (analogRead(A1) >> 2);
  CurrSwitchesValues[index++] = (analogRead(A2) >> 2);
  CurrSwitchesValues[index++] = (analogRead(A3) >> 2);
  CurrSwitchesValues[index++] = (analogRead(A6) >> 2);
  CurrSwitchesValues[index++] = (analogRead(A7) >> 2);

  CurrSwitchesValues[index++] = digitalRead(D2) ? 0 : 0xFF; // index==6
  CurrSwitchesValues[index++] = digitalRead(D3) ? 0 : 0xFF;
  CurrSwitchesValues[index++] = digitalRead(D4) ? 0 : 0xFF;
  CurrSwitchesValues[index++] = digitalRead(D5) ? 0 : 0xFF;
  CurrSwitchesValues[index++] = digitalRead(D6) ? 0 : 0xFF;
}

// Writes the BLE for CurrControlsValues IIF it's changed from the last values.
int WriteControlsIfNeeded(bool forceWrite) {
  bool hasChange = memcmp (FlashData.CurrControlsValues, LastControlsValues, NCONTROLS) != 0; // Controls is bytes
  if (hasChange || forceWrite) {
    Controls.writeValue(FlashData.CurrControlsValues, NCONTROLS);
    memcpy (LastControlsValues, FlashData.CurrControlsValues, NCONTROLS);
    return 1;
  }
  return 0;
}

// Writes the BLE for CurrSwitchesValues IIF it's changed from the last values.
int WriteSwitchesIfNeeded(bool forceWrite) {
  bool hasChange = memcmp (CurrSwitchesValues, LastSwitchesValues, NSWITCHES) != 0; // switches is bytes
  if (hasChange || forceWrite) {
    Switches.writeValue(CurrSwitchesValues, NSWITCHES);
    memcpy (LastSwitchesValues, CurrSwitchesValues, NSWITCHES);
    return 1;
  }
  return 0;
}
