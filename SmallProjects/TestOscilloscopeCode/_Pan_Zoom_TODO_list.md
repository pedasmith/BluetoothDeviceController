# Pan, Zoom, Reticule TODO list

This is the list of all the work needed to make the pan, zoom, reticule look good.

## In progress right now


## UX in general


### Fix up the Zoom panel
Right now the zoom and pan panel is intended to be used for debugging zoom + pan; it's not actually what should be shown to the user. Take a look at existing oscilloscopes (including software ones) for what it should have.

## Reticule

### Color and size
Reticule color and size should be updated to look classier. While I'm at it, why not the moving cursor, too? And have these all be user-settable.

Colors are for:
- background
- waveforms (1..N)
- Cursor
- text background for cursor + reticule scale
- text foregrond for cursor + reticule scale
- reticule major and minor + width + 

## Panning + Zooming

### Keyboard UX
Must ensure a keyboard UX. Should be usable both from a laptop keyboard and from just a keypad.


## Status 2024-07-05

### Zoom out works weird
Zoom way in, and then start to zoom out. It's freaking slow!


### Min/Max Zoom level
There should be a min and max zoom level.


### Min/Max Panning
You should not be able to pan beyond the waveform. Note that a little bit of padding is OK


### Click on bottom half should not show the cursor
The cursor shows up and flickers when the user clicks the bottom of the screen. It really shouldn't.


### Reticule zoom
Should be based on screen (e.g., always have 10 bars on the screen). Right now when you zoom in, the result is that you only see a couple of reticule lines


### Minor reticule lines
Having just the major lines is silly. There should be minor lines (10 per major line)


### Scale marker
There's no scale marker, so you can't tell how big a division is


### Zooming + Panning display clip
The waveform should be clipped to the viewport


## Status 2024-07-05 Pand and Zoom OK

Panning and zoom are basically OK
