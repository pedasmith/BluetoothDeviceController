set JSON="C:\bin\My Small Utils\Shipwreck\BluetoothDeviceController\BluetoothDeviceController\Assets\CharacteristicsData\TI_SensorTag_1350.json"
set BIN= ".\bin\Debug\net6.0-windows10.0.19041.0\BluetoothCodeGeneratordotNetCore"
%BIN% -inputJsonFile %JSON% -inputTemplates Templates -output output
