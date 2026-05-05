# Issues with H.OxyPlot

I won't complain that the original OxyPlot code doesn't work with WinUI3. It's a valid complaint, but they already know about it :-)

## No support for ObservableCollection

It's super common to use the ObservableCollection and the coresponding ICollectionChanged (sp?) interface. These collections, when updated, will trigger an event. That event, in turn, could be used for automatically updated the plots.

Except it's not supported. When I update a collection, I have to manually update the plot.

## Bug: Axes visibility

My app starts off with a number of small graphs. Those graphs, naturally, should have invisible axes. But the graphs can also be zoomed-in, at which point having visible axes is useful. My original concept (start off invisible and then turn invisibility on) doesn't work.

Instead, I have to start with the axes being visible, and then when I have two data points, set the axes visibility off. The toggle between invisible (when small) and visible (when large) then works perfectly.


## Awkward: colors

The OxyPlot code has its own set of color definitions. AFAICT, this is required by C#, and all of the different systems (Silverlight, WPF, etc) that it supports. But it would be super helpful to have a seperate set of code that handles the conversions. In my own app, I created a tiny "UtilitiesOxyColor.cs" class with trivial conversions.

