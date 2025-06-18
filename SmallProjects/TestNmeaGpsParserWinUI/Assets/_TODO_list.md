
# TODO list for the TestNmeaGpsParserWinUI3 app

## Annnunciator isn't complete

Although much of the annunciator is done, and all the foundation is complete, there's too much slop

## Checksum

From the *SiRF* NMEA PDF, page 2.1 in one of the footnotes.

CKSUM is a two-hex character checksum as defined in the NMEA specification, NMEA-0183 Standard For Interfacing 
Marine Electronic Devices. Checksum consists of a binary exclusive OR the lower 7 bits of each character after the “$” 
and before the “*” symbols. The resulting 7-bit binary number is displayed as the ASCII equivalent of two hexadecimal 
characters representing the contents of the checksum. Use of checksums is required on all input messages.

## Support input messages?

There's a bunch of input messages for a bunch of things. Should they be supported?

Maybe have a user-enterable string and then I add the asterisk + checksum?

## Smaller to-do items

2025-06-17 DONE Update GPGSV to well-handle satellite data 
2025-06-17 DONE Update all parsers, not just GPGLL, for the fancy new header/detail/explanation system
2025-06-17 DONE Make a single (static) method for the explanation
2025-06-17 DONE The output has a BindingExpression failure at startup (trivial; just set the DataContext to null)

2025-06-17      In the history tab, automatically update the detail view when the selected value changes
2025-06-17      Support the Dual switch set either way
2025-06-17      The messages dont' really start with $; that's just message framing

