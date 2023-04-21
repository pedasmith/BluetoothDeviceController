//#include "ArduinoBle.h"
//#include "Nano33BleHID.h"

//Nano33BleKeyboard keyboard("BTUnicode-Keyboard");
//HIDKeyboardService* hid;
enum LedColor { Red, Yellow, Green, Cyan, Blue, Magenta, White, Off };
LedColor CurrLed = Off;
void SetLedColor(PinStatus red, PinStatus green, PinStatus blue);
void Log (char* text);


PinStatus CurrKey = HIGH;

auto AtariCx85TriggerPin = D7;
auto AtariCx85Fwd = D2;
auto AtariCx85Back = D3;
auto AtariCx85Left = D4;
auto AtariCx85Right = D5;
auto AtariCx85BPot = D6;
// AtariCx85APot isn't used by the Cx85

void setup() {
    pinMode(LEDR, OUTPUT);
    pinMode(LEDG, OUTPUT);
    pinMode(LEDB, OUTPUT);
    pinMode(LED_BUILTIN, OUTPUT);

    pinMode(AtariCx85TriggerPin, INPUT_PULLUP);
    pinMode(AtariCx85Fwd, INPUT_PULLUP);
    pinMode(AtariCx85Back, INPUT_PULLUP);
    pinMode(AtariCx85Left, INPUT_PULLUP);
    pinMode(AtariCx85Right, INPUT_PULLUP);
    pinMode(AtariCx85BPot, INPUT_PULLUP);

    //keyboard.initialize();

    // Launch the eventqueue thread.
    //MbedBleHID_RunEventThread();
    //hid = keyboard.hid();
}

int nrawprint = 0;

void loop() {
  PinStatus pin = digitalRead(AtariCx85TriggerPin);
  if (pin != CurrKey)
  {
    CurrKey = pin;
    switch (pin)
    {
      case HIGH: SetLed(Off); break;

      // AtariCx85 trigger is active LOW. When the trigger goes
      // low, check for the state of the AtariCx85 pins
      case LOW: 
        delay(50);
        auto key = ReadAtariCx85Pins();
        Serial.print (key, HEX);
        nrawprint++;
        if (nrawprint < 32) Serial.print (" "); else { Serial.println(""); nrawprint=0; }

        CurrLed = LedNext(CurrLed); 
        SetLed(CurrLed); 
        break;
    }
  }
}

// Reads in "raw" electrical signals from an ATARI CX85 keypad and
// converts them into Microsoft-style virtual key codes.
//
// Link: Virtual key codes: https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
int ReadAtariCx85Pins()
{
  PinStatus fwd = digitalRead(AtariCx85Fwd);
  PinStatus back = digitalRead(AtariCx85Back);
  PinStatus left = digitalRead(AtariCx85Left);
  PinStatus right = digitalRead(AtariCx85Right);
  PinStatus bpot = digitalRead(AtariCx85BPot);

  int raw = 0;
  if (fwd == HIGH) raw += 1;
  if (back == HIGH) raw += 2;
  if (left == HIGH) raw += 4;
  if (right == HIGH) raw += 8;

   int retval = 0;
  if (bpot == HIGH) // most keys
  {
    switch (raw)
    {
      case 0x0C: retval=0x60; break;
      case 0x09: retval=0x61; break;
      case 0x0A: retval=0x62; break;
      case 0x0B: retval=0x63; break;
      case 0x01: retval=0x64; break;
      case 0x02: retval=0x65; break;
      case 0x03: retval=0x66; break;
      case 0x05: retval=0x67; break;
      case 0x06: retval=0x68; break;
      case 0x07: retval=0x69; break;
      case 0x0D: retval=0x6E; break; // vk_decimal
      case 0x0F: retval=0x6D; break; // vk_subtract minus
      case 0x0E: retval=0x0D; break; // vk_enter
      case 0x04: retval=0x21; break; // F2=no=vk_prior=page up
      case 0x00: retval=0x2E; break; // F3=vk_delete
      case 0x08: retval=0x22; break; // F4=yes=vk_next=page down
    }
  }
  else // only LOW key is F1=ESCAPE
  {
    retval = 0x1B; // F1=vk_escape
  }
  return retval;
}

LedColor LedNext(LedColor color)
{
    int next = (int)color + 1;
    if (next > (int)Magenta) next = 0; // do not include white or off in the rotation
    return (LedColor)next;
}
void SetLed(LedColor color)
{
  switch (color)
  {
    case Off:
      digitalWrite(LED_BUILTIN, LOW);
      SetLedColor(HIGH, HIGH, HIGH);
      break;
    default:
    case White:
      digitalWrite(LED_BUILTIN, HIGH);
      SetLedColor(LOW, LOW, LOW);
      break;
    case Red    : SetLedColor(LOW, HIGH, HIGH); break;
    case Yellow : SetLedColor(LOW, LOW, HIGH); break;
    case Green  : SetLedColor(HIGH, LOW, HIGH); break;
    case Cyan   : SetLedColor(HIGH, LOW, LOW); break;
    case Blue   : SetLedColor(HIGH, HIGH, LOW); break;
    case Magenta: SetLedColor(LOW, HIGH, LOW); break;
  }
}

// Function to set the color of the RGB LED on the 
// Nano 33 BLE. The LED is controlled by three pins
// LEDR, LEDG, LEDR for Red, Green, and Blue; each 
// can be ON or OFF.
// Unusually, the pins are "Active Low", meaning that
// to turn the color ON your need to set the pin LOW.
void SetLedColor(PinStatus red, PinStatus green, PinStatus blue)
{
      digitalWrite(LEDR, red);
      digitalWrite(LEDG, green);
      digitalWrite(LEDB, blue);
}

void Log (char* text)
{
  Serial.print (text);
}

