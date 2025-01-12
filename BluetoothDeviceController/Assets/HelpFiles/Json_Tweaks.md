# JSON tweaks for a better UI

The JSON file that can be created from the the captured Bluetooth signals is often not ideal. 
You can tweak the resulting JSON file to make it exactly match your needs.

## Device name, class name, aliases

The default C# class name that is generated is derived from the Name of the device. This is 
not always what you want. For example, I like to have each Protocol class be given a name that
includes the maker name and the device (for example, TI_SensorTag_1352). Do this by providing 
a class name:

    "ClassName":  "Triones_LedLight",

Sometimes the JSON from one device is useable directly with other devices. When you add an aliases list,
you allow an instant reuse of the JSON. A great example of this is the Triones LED light protocol
which is used by lots of different lamps

    "Aliases": [ "LEDBlue" ],

## Naming services and characteristics

This program does its best to figure out a good name for each service and characteristic. Often
there is no useful information, and so each service and characteristic is simply given a name
like "Unknown2". You should replace all these names with more suitable names.

You can also add a Description to each service and characteristic.

## Suppress, Priority

Not all services are equally useful in the automatically-generated UI. There are two settings
to help create a better UI

Set Suppress to true to prevent a service from being placed in the UI at all. This is commonly
done for services used for services that have no good support in this program (like over-the-air updates),
services that are not understood, and services that don't work. Don't laugh; a surprising number
of devices have boilerplate services that aren't actually hooked up. 

          "Suppress":  true,

Set the priority of a service to place it higher in the list of services in the UI. Services with
higher priorities are displayed first. If the priority is >= 10, the service expander is open by default.

          "Priority":  5,

# Characteristics

Characteristics can also have names and descriptions

## Supress, SupressRead and SupressWrite

Individual characteristics can suppress the read and write properties. This is needed because some characteristics are incorrectly marked as "writable" or "readable" when in reality they are not. 

You can suppress the UI for a characteristic by setting Supress to true

          "Suppress":  true,

In the case where a characteristic includes sub-items, individual items whose name ends with Unused or Internal will not be shown in the UI.

## Automatic notification

You can have a characteristic's notify be automatically called by setting AutoNotify to true

        "AutoNotify": true,

## Adding tables and graph

See [Graphs and JSON](Json_Graphs.md) for details on creating tables and graphs

## Supporting LED lamps

See the Elk protocol for an example.

If your device is an LED lamp, you can automatically add a control for it in the resulting XAML file. To do this, for the characteristic that controls the LED, add a JSON line: 

    "ExtraUI":  "LampControl",

This will add in a <lamps:LampControl> near the top of the page. However, when you do this you must also make a Custom protocol class that extents the LampControl class. You will need to implement several methods for this to work smoothly. 