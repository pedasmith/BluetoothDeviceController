# Licenses for Leaflet and OpenStreetMap

Link to [OpenStreetMap](https://www.openstreetmap.org/) and their [license](https://www.openstreetmap.org/copyright).

In particular:

### [Copyright](https://www.openstreetmap.org/copyright). Analysis: 

### [Acceptable Use](https://wiki.openstreetmap.org/wiki/Acceptable_Use_Policy)

And see also [this link](https://operations.osmfoundation.org/policies/) and critically [tile server usage](https://operations.osmfoundation.org/policies/tiles/)

**Key requirements** for my app:
- Verify I'm using the right URL
- Display the license attribution (check)

### [Privacy Policy](https://osmfoundation.org/wiki/Privacy_Policy)

**Summary**: needs an addition to my regular privacy policy and link to theirs.

**Key requirements** must add user control to block all third-party services! I have a generally usable mapping page.

for my app: because this app is now sending user data up to a 3rd party, my privacy policy needs something like:

```
ShipwreckSoftware apps that display maps to the user use third-party map services. For more information, please see the privacy policy of the map service provider. 

OpenStreetMap [Privacy Policy](https://osmfoundation.org/wiki/Privacy_Policy)

```

### [Trademark](https://osmfoundation.org/wiki/Trademark_Policy). 

**Summary**: I need to provide an attribution notice.

Per 1.3.1 and 1.3.2, this app is an "unrelated organization or individual" (at least, as used by the Simple GPS App). I don't have a trademark license, and therefore have to be careful about using the OpenStreamMap logo (etc.) in the app. 

Per 3.3, I can refer to OSM in a referential way. Perhaps a small blurb at the bottom of the Leaflet map is best. And per 3.3.5, I can use OSM for complying with the attribution requirements.


Per 2.2, here's the right verbiage: 
```
[Wordmark / name of logo] is a trademark of the OpenStreetMap Foundation, and is used with their permission. [We / this product / this project] are not endorsed by or affiliated with the OpenStreetMap Foundation.
```


