set JSONDIR=C:\bin\My Small Utils\Shipwreck\BluetoothDeviceController\BluetoothDeviceController\Assets\CharacteristicsData
set JSON=%JSONDIR%\TI_SensorTag_1350.json
set JSON=%JSONDIR%\Elegoo_Mini_Car.json
set BIN= ".\bin\Debug\net6.0-windows10.0.19041.0\BluetoothCodeGeneratordotNetCore"
%BIN% -inputJsonDirectory "%JSONDIR%" -inputTemplates Templates -output output
