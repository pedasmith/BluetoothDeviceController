REM Generate all of the protocol, etc., files from the JSON device
echo %CD%
set PROJDIR=C:\Users\toomr\source\repos\BluetoothDeviceController
set PROJDIR=%CD%\..
set JSONDIR=%PROJDIR%\BluetoothDeviceController\Assets\CharacteristicsData
set JSON=%JSONDIR%\TI_SensorTag_1350.json
set JSON=%JSONDIR%\Elegoo_Mini_Car.json
set BIN= ".\bin\Debug\net6.0-windows10.0.19041.0\BluetoothCodeGeneratordotNetCore"
if not exist output mkdir output
REM goto :Debug
%BIN% -inputJsonDirectory "%JSONDIR%" -inputTemplates Templates -output output
goto :EOF

REM Or generate just the one
:Debug
%BIN% -inputJsonFile "%JSONDIR%"\Elegoo_Mini_Car.json -inputTemplates Templates -output output


