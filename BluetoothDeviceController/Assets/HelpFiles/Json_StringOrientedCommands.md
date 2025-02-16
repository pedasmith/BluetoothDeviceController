# String-oriented commands

Some Bluetooth devices (like the Elegoo mini-car) are powered by string-oriented commands. Unlike proper Bluetooth BLE commands, a single characteristic will be used to control a variety of functionality on a device.

For example, the single **ffe28** characteristic on the Elegoo mini-car supports these 

```
{BEEPS[Tone][Duration]}
{CLEAR[Mode]}
```

The curly-braces {} are part of the commands that have to be sent to the car. The CLEAR mode parameter can be 0, 1, 2, or 3; 1 will turn off the lights, 2 will stop the car, 3 will turn off the buzzer (but won't interupt the BEEP mode) and 0 will do all three types of clear at once.


## Commands and UIList

Each command (like BEEPS) is described in a single entry in the Commands list. The command entry includes a list of Parameters (like the Tone and Duration) and a Compute value. The Compute value is string with a set of commands; they are just like the commands described in [Interpreting Data](https://shipwrecksoftware.wordpress.com/2019/10/13/modern-iot-number-formats/). 

The command string is a set of sub-commands separated with spaces. In the BEEPS example, the goal is to make a string like

```
{BEEPS[100][0.2]}
```

This is done by pasting together 5 different sub-strings. Some are constant; some are variables (parameters), and others require some calculations. The full command is below
```
"Compute": "${BEEPS[ $Tone_GN $][ $Duration_GN_1000_/ $]}"
```

A **$** will indicate a string. The **${BEEPS[** command is simply the string **{BEEPS[**
Sub-commands are indicated with an **_** underscore. Each subcommand is handled in turn. The **GN** command will get a numerical value (parameter) by name. The **$Tone_GN** part is two sub-commands: **Tone** is just the string "Duration" and then GN will get the value of Tone from the parameter list. 
Sub-commands form a complete stack-based RPN type language. For example, **$Duration_GN_1000_/** is four sub-commands: the string Duration, the command **GN** to Get Numerical value of whatever string is on the stack (in this case, the string Duration), and then the number 1000 and a divide request. The end result is that the Duration number is returned, divided by 1000.
The **GS** command will also get a parameter by name, but will get the string value, not the numeric value.

## Command Parameters
The named parameter for a command are provided by the Parameters dictionary; the parameters for the BEEPs command is below

```
    "Parameters": {
    "Tone": {
        "Label": "Beep Tone",
        "Min": 1,
        "Max": 255,
        "Init": 100,
    },
    "Duration": {
        "Label": "Duration in milliseconds",
        "Min": 0,
        "Max": 65535,
        "Init": 250,
    }
    },
```

There are two parameters, one called Tone and the other called Duration. Each has a minimum, maximum and initial value, plus a label. The label will be used by default in the UI, and is a handy way to remember what each parameter does.

A language purist might wish to call these "variables" and not "parameters". They should feel free to write their own mini-language :-)

## Enum-like parameters

Some parameter have named values; some languages call them enums. I just call them ValueNames, and you can add them to individual parameters. There isn't a way to re-use them (at least, not yet)

Example of a named value (enum) from the Move2 command paramters

```
    "RightDirection": {
        "Label": "Right Direction",
        "ValueNames": {
        "Stop": 0,
        "Forward": 1,
        "Reverse": 2,
        "No Execution": 3
        },
        "Init": 1
    },
```

In the example, there's a parameter (variable) called RightDirection. It includes 4 possible values, each with a name. The name is more like a label; it will be used by the UI elements.


## UI Elements

The UI elements are what's actually presented to the user. Often there's a direct relationship between the UI and the commands. However, not all commands need to be present, and some commands might be present multiple times

For the Elegoo mini-car, for example, there are two different BEEP commands (BEEP and BEEPS). But the first of these isn't very useful, and so in the UI it's not present.


```
    { "UIType": "RowStart" },
    {
        "UIType": "SliderFor",
        "Target": "Beep2 Tone"
    },
    {
        "UIType": "SliderFor",
        "Target": "Beep2 Duration"
    },
    {
        "UIType": "ButtonFor",
        "Target": "Beep2",
        "Label": "Beep"
    },
    { "UIType": "RowEnd" },

```

## UI Types

### Blank

### RowStart and RowEnd
 
RowStart can include an "N" value which is the number of columns. The default is that N=4. Despite the name, this is really a horizontal wrapping grid; the UI elements will be added one after the other on the same line up to a maximum amount (N), and then they spill into the next line.

### The ...For values

These all have a **Target** that's either a full command (like "ObstacleAvoidance") or a specific paramter (variable) like "LineTrack Sensor".

They all can have a Label; if not present then the target's label is used.

### ButtonFor

When clicked, the command is run. Has an optional Set, a list of target settings. Example **"Sport Direction Forward"** which will will set the Sport command's Direction variable to the value of the named value Forward. The command is often the same command as the button's target, but it doesn't have to be.

### ComboBoxFor and RadioFor

Example target: **"Linetrack Sensor"**

### SliderFor

When slide the command's variable is updated.

Example target: **"Sport Speed"**

