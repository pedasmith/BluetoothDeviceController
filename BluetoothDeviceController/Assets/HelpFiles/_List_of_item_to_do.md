# List of work items

## Oscilloscope

- Poke constantly (e.g., 10 times or for 5 seconds)

## The MultiMeter control / Pokit Pro!

Big thing: get the lifetime all sorted out. 

- the page (other than the MM) should not be trying to connect.
- set up a temp "connect" button
- make a proper annunciator with symbols. 

- For e.g. private async Task DoWriteDSO_Settings(string text, System.Globalization.NumberStyles dec_or_hex), it really needs to take an array of a new string + numberstyles instead of the singletons.


**Lifetime** states are:
Unconnected. Press connect to 
TryingToConnect. Will get the device name (because ???). GOTO Failed or Setup
Failed to connect
Setup: connect up the notify for MM and do a setting for resistance (?). GOTO WorkingProbably or Failed
WorkingProbably: move to Working when we get data
Working: got data!

Starting with nothing selected and click on an item (like Resistance)

Idle will turn everything off.
For the moment, make an all-on button



### Annunciator settings



### Things to think about

The pokit pro does have a way to tell if you can to the Volts or not based on the switch. So it's possible to tell if the switch is in the right position and give guidance to the user.



