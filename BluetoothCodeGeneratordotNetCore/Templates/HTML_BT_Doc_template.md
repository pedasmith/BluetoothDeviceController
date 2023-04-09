# HtmlDoc FileName=Device_[[CLASSNAME]].html DirName=Html SuppressFile=:SuppressHtml:

```
<!DOCTYPE HTML>
<html>
<head>
<title>[[UserName]]</title>
<style>
.Characteristic {
    margin-left: 50px;
    font-size: 14px;
}
h1 {
    font-size: 15px;
}
h2 {
    font-size: 14px;
}
h3 {
    font-size: 14px;
}
</style>
</head>
<body>
<main>
<h1>[[UserName]] device from [[Maker]]</h1>
[[Description]]

[[UsingTheDevice]]

<h2>Useful Links</h2>
<ul>
[[DeviceLinks]]
</ul>
<h2>All Services</h2>
[[Services]]
</main>
</body>
</html>
```


## DeviceLinks Type=list Source=LINKS CodeListZero="No links for this device"

```
<li><a href=[[TEXT]]>[[TEXT]]</a>
```




## Services Type=list Source=Services ListOutput=global

```
<div class="Service">
<h2>[[Name]] UUID=[[UuidShort]]</h2>
[[ServiceDescription]]
<br/>
Full UUID = [[UUID]]
[[Characteristics]]
</div>
```

## Characteristics Type=list Source=Services/Characteristics ListOutput=parent

```

<div class="Characteristic">
<h2>[[CharacteristicName]] UUID=[[UuidShort]]</h3>
[[CharacteristicDescription]]
<br/>
Byte description: [[Type]] <br/>
Full UUID = [[UUID]]
</div>
```
