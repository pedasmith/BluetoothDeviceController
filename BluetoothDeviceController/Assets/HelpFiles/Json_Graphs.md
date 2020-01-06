# How to set up a graph in the JSON files

Graphs and tables are set up per characteristic. Each characteristic can have one table and one graph
(which may be awkward: some characteristics include multiple values which you might want to handle individually,
but you will not be able to handle them individually).


# Sample
This JSON sample is from the Nordic Thingy temperature characteristic. In the characteristic there is a 
**UI** element; that element determines the table and chart (graph) types.

          "Characteristics": [
            {
              "UUID": "EF680201-9B35-4933-9B10-52FFA9740042",
              "Name": "Temperature (c)",
              "Type": "/I8/P8|FIXED|Temperature|C",
              "UI": {
                "tabletype": "standard",
                "chartType": "ytime",
                "chartDefaultMinY": 0,
                "chartDefaultMaxY": 100,
                "chartCommand": "AddYTime<[[CHARACTERISTICNAME]]Record>(addResult, [[CHARACTERISTICNAME]]RecordData)",
                "chartLineDefaults": {
                  "Temperature": {
                    "stroke": "DarkGreen"
                  }
				}
			  }
            },

## To make a table

To make a table, add a UI : { } JSON element and in that element add add a **"tableType": "standard"**. This will automatically generate
the charting UI code in the XAML and and code-behind. The chart UI will include date, time, all of the data in the Type field, 
and a Notes column (adding notes to the data turns out to be super useful)

## To make a chart

The simplest chart is like the table: dd in the UI : { } JSON element and add add a **"chartType": "standard"** and also 
a **"chartCommand": "AddYTime<[[CHARACTERISTICNAME]]Record>(addResult, [[CHARACTERISTICNAME]]RecordData)"**. You will need both commands;
without the first the chart won't be made, and without the second the chart won't be added to. The resulting chart UI 
in the XAML and code-behind will be a time-oriented chart with one line per data value in the Type data.

You can customize the chart with these values
- **chartMinY** **chartMaxY** will set the min and max Y value range. This is useful when a sensor provides data in a range (like an accelerometer)
- **chartDefaultMinY** **chartDefaultMaxY** are like the min and max Y values but they are suggestions only. This is useful for things like thermometers where the common range is "room temperature" but you want the actual data to override the default.
- **chartLineDefaults** is a dictionary of settings per line. When your graph has multiple data lines, the color for each line will be automatically picked. You might prefer to explicitly set the color of each line (for example, the Nordic Thingy light sensor has four outputs for the strength of the ambient red, green, blue and combined values. These are displayed in the appropriate colors, so that the Red sensor has a red line)