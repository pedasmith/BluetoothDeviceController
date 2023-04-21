//#include "ArduinoBle.h"
//#include "Nano33BleHID.h"

//Nano33BleKeyboard keyboard("BTUnicode-Keyboard");
//HIDKeyboardService* hid;
enum LedColor { Red, Yellow, Green, Cyan, Blue, Magenta, White, Off };
LedColor CurrLed = Off;
void SetLedColor(PinStatus red, PinStatus green, PinStatus blue);

PinStatus CurrKey = HIGH;

void setup() {
    pinMode(LEDR, OUTPUT);
    pinMode(LEDG, OUTPUT);
    pinMode(LEDB, OUTPUT);
    pinMode(LED_BUILTIN, OUTPUT);

    pinMode(D8, INPUT_PULLUP);

    //keyboard.initialize();

    // Launch the eventqueue thread.
    //MbedBleHID_RunEventThread();
    //hid = keyboard.hid();
}


void loop() {
  PinStatus pin = digitalRead(D8);
  if (pin != CurrKey)
  {
    CurrKey = pin;
    switch (pin)
    {
      case HIGH: SetLed(Off); break;
      case LOW: CurrLed = LedNext(CurrLed); SetLed(CurrLed); break;
    }
  }
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
