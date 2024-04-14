# Version information for the Simple Bluetooth Current Time Server


## Version 2.0 2024-04-08

Version two includes new features
1. The advertisement is time-limited: it advertises for 60 seconds and then stops
2. The user unit preferences is now a seperate card and always available
3. The user unit preferences are saved from run to run so the user doesn't need to keep entering them
4. The user user changes are propagated more correctly to Bluetooth listeners.

## Version 1.0 2023-06-23

Version 1.0 was the first version on the store. It supports the Bluetooth Current Time Server and also includes an "advanced" feature where the user can specify the user's preferred time units (AM/PM or 24-hour and temperature).