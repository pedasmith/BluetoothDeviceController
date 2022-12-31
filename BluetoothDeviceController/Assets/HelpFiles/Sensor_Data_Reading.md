# Collecting and examining sensor data

The Bluetooth Device Controller lets you collect, visualize, and save data from your sensors.

![Nordic Thingy](../ScreenShots/Device_Nordic_Thingy_Data.png)

Most Bluetooth data sensors are set up so that you have to ask for *Notifications* before you can get data.
Click the "Notify" button next to the data you want to see. In the example above for the Nordic Thingy device,
the Environment service includes data for temperature, pressure, humidity and more. These are all seperate 
readings, and you have to click notify for each one that you're interested in.

Underneath the current data boxes is an expander that will show the data data and a data graph. 

There are two options for how much data will be collected. 

![Nordic Thingy](../ScreenShots/Device_Nordic_Thingy_Settings.png)

1. You can choose how much data is saved in one run. You can save more data or less data. 
2. You can also choose what happens when there's too much data. The default is to keep a statistically *random sample* of the data; this is often useful when you're collecting environment data for an extended period of time. You can also choose to keep the *latest data*. This will throw out the oldest data to make room for the newer data.

You can also copy the data as it is onto the clipboard; you can then paste it into Excel or other analysis programs.

# See Also

[Importing into Excel](Sensor_Data_Excel.md)