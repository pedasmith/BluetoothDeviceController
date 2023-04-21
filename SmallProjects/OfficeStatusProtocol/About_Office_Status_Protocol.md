# Office Status Protocol with Bluetooth Advertisements

## Coordinating our status in a shared office

My office mate and I have a variety of statuses:

1. On a critical phone call that must not be disturbed
2. Need to concentrate
3. Just working normally
4. Ready for socializing

But we don't want to keep on interupting each other with our updated. My solution: a pair of small Bluetooth boxes with status lights and a rotary switch. Each one reflects the position of the other boxes' switch, and lights up the corresponding LED.

For example, I can set my boxes switch to "Working normally". Within a few seconds, my office mate's box will update accordingly. And when my office mate sets the switch on their box, my boxes LED lights update as well.

## Technical details

The Office Status Protocol is sent between "boxes" -- small user agents, often implements with small microcontrollers. 

The messages are transmitted as small Bluetooth advertisements. The protocol consists of each box sending messages which will be picked up by other boxes (generally just one other box). The messages do not include a reply mechanism; there's no guarantee that it will be picked up.

Bytes|Value|Meaning
-----|-----|-----
0,1| XYZ |Manufacturer index. I picked '__' even though it's an invalid value
2| 0xBA| Protocol type; 0xBA is for Office Status Protocol 
3| 0x59 | Protocol sub-type, alwatys 0x59
4| *message index* | A value 0..15 or 0xFF for a locking message
5| *brightness* | 0=off 1=dim 2=normal 3=bright
6| *blink speed* | 0=not blinking 1=slow 2=normal 3=bright
7+| *text* | Text to display; is a UTF8 encoded string

The message index values have a semantic meaning seperate from this protocol. Messages might indicate, "Watson, come here, I want you", or "Do not disturb" or any other meaning. Boxes that implement this protocol often have the messages written out next to indicator lights.

The messages in addition include a text value; this enables boxes to print out the meanings. Many boxes 

### Locking two devices to each other

The Office Status Protocol device operate in  busy office. Each box can see potentially dozens or more of different boxes, each with their own status values. 

Two boxes can be locked to each other so that they only reflect each other's status. (The obvious alternative English words for this are "pairing" and "bonding", but those have technical meanings for Bluetooth which don't apply to "locking").

Locking is initiated by pressing a hardware button on a box. Both boxes must press have their locking buttons pressed for locking to be complete. 

A message is a *locking messge* when the index is 0xFF and protocol=0xBA and sub-protocol=0x59. 

The boxes are in one of three states: *lock button pressed*, *got lock message* or some other state (generally *normal*).

When the lock button is pressed, the box will switch to the lock button pressed and will start advertising a locking message. The box will stay in the lock button pressed state for 30 seconds. 

The box will state in got lock message state for 10 seconds.

Each box run the following algorithm when a locking message is received.

* If a box is in lock button pressed state it will save the source message's Bluetooth address, and will transition into 'got lock message' state. 


### Security requirements

Messages in the Office Status Protocol shall not carry information more secure than can be observed from watching people in an open-plan or shared-office space. In those environments, peoples current state can generally be observed as one of the common states: busy, concentrating, absent, and so on.

To maintain security, 
1. Do not place private information into any of the text fields. 2. Do not create message values which contain private information. 

### Common implementation details

The advertisements should be "general" advertisements

The boxes should not be pairable

### Common message index values

Index|Meaning
-----|-----
1|In meeting; do not disturb
2|Concentrating; do not disturb
3|Working
4|Ready for social activity

Status messages are typically sent at a normal brightness values and set to bright for critical messages. For example, a person in a normal meeting would use index 1. If it's especially important to not be distrubed -- for example, it's a meeting with an important external customer -- the message would be sent with a highlight.