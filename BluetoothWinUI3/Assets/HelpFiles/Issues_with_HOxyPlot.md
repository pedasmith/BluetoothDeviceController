# Issues with H.OxyPlot

I won't complain that the original OxyPlot code doesn't work with WinUI3. It's a valid complaint, but they already know about it :-)

The [H.OxyPlot](https://github.com/HavenDV/H.OxyPlot) library is a combination of the old UWP OxyPlot with the latest WPF OxyPlot, and set up to work with WinUI3 (and some others)

## No support for ObservableCollection

It's super common to use the ObservableCollection and the coresponding ICollectionChanged (sp?) interface. These collections, when updated, will trigger an event. That event, in turn, could be used for automatically updated the plots.

Except it's not supported. When I update a collection, I have to manually update the plot.

## Bug: Axes visibility

My app starts off with a number of small graphs. Those graphs, naturally, should have invisible axes. But the graphs can also be zoomed-in, at which point having visible axes is useful. My original concept (start off invisible and then turn invisibility on) doesn't work.

Instead, I have to start with the axes being visible, and then when I have two data points, set the axes visibility off. The toggle between invisible (when small) and visible (when large) then works perfectly.


## Awkward: colors

The OxyPlot code has its own set of color definitions. AFAICT, this is required by C#, and all of the different systems (Silverlight, WPF, etc) that it supports. But it would be super helpful to have a seperate set of code that handles the conversions. In my own app, I created a tiny "UtilitiesOxyColor.cs" class with trivial conversions.

## Mouse control of cursors is awkward

I'm making an oscilloscope graph. Modern oscilloscope have little measuring cursors: the user can position the cursors and some measurements (like the distance between the cursors) is displayed automatically. But there's no obvious way to let the user move the annotations around.

## Events are all obsolete?

The events (like the axis translations changing) are all marked as obsolete. I rely on these events to make the cursor controls work.

