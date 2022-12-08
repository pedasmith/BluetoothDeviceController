REM Generate all of the protocol, etc., files from the JSON device
echo %CD%
set PROJDIR=C:\Users\toomr\source\repos\BluetoothDeviceController
set PROJDIR=%CD%\..
set JSONDIR=%PROJDIR%\BluetoothDeviceController\Assets\CharacteristicsData
set JSON=%JSONDIR%\TI_SensorTag_1350.json
set JSON=%JSONDIR%\Elegoo_Mini_Car.json
set BIN= ".\bin\Debug\net6.0-windows10.0.19041.0\BluetoothCodeGeneratordotNetCore"
if not exist output mkdir output

REM
REM Normally we want to generate everything. But when we are debugging just something new,
REM it's nicer to have just the single file handled.
REM
goto :Debug
%BIN% -inputJsonDirectory "%JSONDIR%" -inputTemplates Templates -output output
goto :EOF

REM Or generate just the one, which is a little easier to handle when debugging.
:Debug
%BIN% -inputJsonFile "%JSONDIR%"\Nordic_Thingy.json -inputTemplates Templates -output output
%BIN% -inputJsonFile "%JSONDIR%"\Pyle_PHBPBW40_Samico_BG512_Blood_Pressure.json -inputTemplates Templates -output output
%BIN% -inputJsonFile "%JSONDIR%"\TI_SensorTag_1352.json -inputTemplates Templates -output output


