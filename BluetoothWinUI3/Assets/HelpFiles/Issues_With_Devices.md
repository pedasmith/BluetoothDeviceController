## ThermPro 357

The ThermPro 357 is a small, AAA powered device with an LCD screen. When put into an oven, the case will melt.

### Problems with the device

The Bluetooth advertisements don't meet the SIG requirements (or the Bluetooth Accessory Guidelines, either). In the Manufacturer's specific data area, they don't include a manufacturer ID; they just use those two bytes for their own protocol.

(In the example data, the column marked ?? changes (02 to 00), but the cause is unknown.)


### Example data
```
-- TL TB HH ?? -- 
C2 C7 00 2F 02 2C
C2 CD 00 2E 02 2C # 20.5 47%
C2 D3 00 2D 02 2C # 21.1 45%
C2 D2 00 2D 00 2C # 
C2 D2 00 2D 00 2C # 21.4C 46% low battery
C2 C2 00 1E 00 2C # 19.4C 30% low battery (ice)
C2 AE 00 32 00 2C # 30.2C 41%
C2 88 01 19 00 2C # 39.2 25%
```