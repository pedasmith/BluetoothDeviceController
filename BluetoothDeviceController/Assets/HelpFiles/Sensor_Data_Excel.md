# Analyzing sensor data in Excel

![Data Table](../ScreenShots/Device_Nordic_Thingy_Data.png)

In the screen shot, a tabular data is being shown. The particular device is the Nordic Thingy, 
and the data is from the color RGB+Clear light sensor, but the same kind of data output is generated
by most of the data devices. There is both a *chart* and a *table*. 

## Copy to Excel
To copy data to insert into Excel, click the *Copy* button. This puts your data into the clipboard.
To paste it into Excel

1. Open Excel, click Paste and select Use Text Import Wizard...
2. In the wizard, select My data has headers and click Next
3. On the second page, select the Comma delimiter and not the Space and click NExt
4. On the third page, click Finish

The resulting table looks like this

![Excel](../ScreenShots/Data_Excel_Table_10.png)

## Format the EventTime column
The EventTime column will show time as minutes, second and milliseconds. To switch to
time-of-day including the hour

1. Click the "B" row header to select the entire column
2. In Cells, select Format and Format Cells
3. In the resulting Format Cells dialog, select the Time category and your preferred time style

## Graph the data
To make a graph of the data,
1. Select the "B" row header to select the event times
2. Control-click one of the data headers (like the "F" column) to also select that data column
3. In the ribbon, click Insert and Recommended Charts. 
4. Pick your preferred chart and click OK

The resulting chart looks like this

![Excel](../ScreenShots/Data_Excel_Table_20.png)

## Data format
You can save to a program other than Excel by simply pasting the data. The data is in a text (comma-seperated-value) 
form which many programs can import.
