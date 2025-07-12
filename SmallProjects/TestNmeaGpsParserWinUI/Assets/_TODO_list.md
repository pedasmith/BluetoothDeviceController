
# TODO list for the TestNmeaGpsParserWinUI3 app

## Big TODO areas

Connect the AdaFruit GPS via serial port (and write blog)
Connect the AdaFruit GPS via Bluetooth (and write blog)


## Map V2: Use https://leafletjs.com/

https://leafletjs.com/ is a JavaScript library for interactive maps. It can be used with WinUI3 via WebView2, which allows you to embed web content in your app.



## Map V1: Just plain lines, but with highlights, selection, and info.


DONE 2025-07-11: Work item: highlight point that's grouped only shows one item. Update: 2025-07-11 all works but need better keyboard manipulation. Should be able to just press an "arrow" to go left, left, left, left either by grouped or ungrouped items. 
DONE 2025-07-10: Work item: add colors to app settings
DONE 2025-07-10: 2025-06-28 Status: trivial map with lines is being prototyped. But the line-drawing code in XAML is weak: too often, a long line will be truncated. Status 2025-07-10: this is done enough; on to more map types
DONE 2025-07-11: Work item: ctrl-scroll should zoom
DONE 2025-08-06: Code cleanup: removed dead code, consistant logging, 
DONE 2025-07-06: Work item: when adding a bunch of points, at some point I need to transition to having start/end points via a redraw? Turns out that 5k lines is fine, 10k is a bit laggy, and it just gets worse from there.
DONE 2025-07-05: Work item: after scaling, the highlight circle is in the wrong position
DONE 2025-07-06: Work item: add fake, clear, add fake: resulting map's point are all sized at the minimum size, not the correct size.
DONE 2025-07-05: Work item: move the Focus code to a seperate file so I can write about it.
DONE 2025-07-05: Work item: clear doesn't remove the point information or this highlight circle
DONE 2025-07-04: Work item: focus is still wonky. On start, press arrows (works). click on map, then try arrows and they fail.
DONE 2025-07-04: Select a point to "highlight" and show info about it. Arrow keys / space+Shift+space to go forward and back
DONE 2025-07-03: Work item: add fake, pan, move to another tab, and then come back. The OnLoaded event triggers again!
DONE 2025-07-03: Work item: Centering works, but the keyboard is weird because the focus doesn't stay put
DONE 2025-07-02: Work item: clear doesn't clear the points or the cursors. User should be able to add fake, pan+zoom, clear, add fake and get the original back
DONE 2025-07-02: Work item: add fake, zoom should result in the starting cursor being centered.
DONE 2025-07-02: Work item: bug: do the fake, then zoom out. The start cursor vanishes!
DONE 2025-07-02: Work item: always center relative to the center of the screen, not the touch point?
DONE 2025-07-02: Work item: doesn't set the starting point reliably (it assumes the first point is the reference point, but it's not.)
DONE 2025-07-02: Work item: center the first point exactly on screen (set top + left of the canvas)
DONE 2025-07-02: Work item: move the constants etc to the to of the class
DONE 2025-07-02: Work item : always draw the first 1000? segements and the last 2000? segments (but be careful of the overlap). 
DONE 2025-07-01 Work item: Colors seem off for start / end
DONE 2025-06-29 Work item : clump points that are close together. Update the circle so that a "clumpier" area is bigger, but with a max size.
DONE convert lat / long to Degrees Decimal (DD). 


## Smaller to-do items

2025-06-17 ABANDONED Support the Dual switch set either way. Result: it's a weird binary that I haven't cracked 

### Completed

Completed 2025-06-17
2025-06-17 Update GPGSV to well-handle satellite data 
2025-06-17 Update all parsers, not just GPGLL, for the fancy new header/detail/explanation system
2025-06-17 Make a single (static) method for the explanation
2025-06-17 The output has a BindingExpression failure at startup (trivial; just set the DataContext to null)
2025-06-17 In the history tab, automatically update the detail view when the selected value changes

Completed 2025-06-18
2025-06-17 Make nice versions of all enum for detail display
2025-06-18 Duplicating the header-type info is pointless. And the raw NMEA should be seperate.
2025-06-18 Hide the detail box when there's nothing selected
2025-06-17 The messages dont' really start with $; that's just message framing
2025-06-17 Check for checksums     


## Copy

Be able to copy data out from all tabs, including tabular data


## Later Skater: Support input messages?

There's a bunch of input messages for a bunch of things. Should they be supported?
Maybe have a user-enterable string and then I add the asterisk + checksum?


## Complete: 2025-06-18 Annnunciator isn't complete

Although much of the annunciator is done, and all the foundation is complete, there's too much slop. Update: all done!

## Complete: 2025-06-18 Checksum

From the *SiRF* NMEA PDF, page 2.1 in one of the footnotes.

CKSUM is a two-hex character checksum as defined in the NMEA specification, NMEA-0183 Standard For Interfacing 
Marine Electronic Devices. Checksum consists of a binary exclusive OR the lower 7 bits of each character after the “$” 
and before the “*” symbols. The resulting 7-bit binary number is displayed as the ASCII equivalent of two hexadecimal 
characters representing the contents of the checksum. Use of checksums is required on all input messages.
