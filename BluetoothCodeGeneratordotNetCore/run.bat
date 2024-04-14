REM Generate all of the protocol, etc., files from the JSON device
echo %CD%
set REPODIR=%UserProfile%\source\repos
set REPODIR=%CD%\..
set JSONDIR=%REPODIR%\BluetoothDeviceController\Assets\CharacteristicsData
set JSON=%JSONDIR%\TI_SensorTag_1350.json
set JSON=%JSONDIR%\Elegoo_Mini_Car.json
set BIN= ".\bin\Debug\net6.0-windows10.0.19041.0\BluetoothCodeGeneratordotNetCore"
if not exist output mkdir output

REM
REM Normally we want to generate everything. But when we are debugging just something new,
REM it's nicer to have just the single file handled.
REM
if "%~1"=="-debug" goto :debug
if "%~1"=="-all" goto :All
goto :Help

goto :Debug
:All
%BIN% -verbose -inputJsonDirectory "%JSONDIR%" -inputTemplates Templates -output output
goto :EOF

REM Or generate just the one, which is a little easier to handle when debugging.
:Debug
REM %BIN% -inputJsonFile "%JSONDIR%"\Govee_H6005.json -inputTemplates Templates -output output
REM %BIN% -inputJsonFile "%JSONDIR%"\Bluetooth_CurrentTimeService.json -inputTemplates Templates -output output
REM %BIN% -inputJsonFile "%JSONDIR%"\BtUnicodeKeyboard.json -inputTemplates Templates -output output
%BIN% -inputJsonFile "%JSONDIR%"\Nordic_Thingy.json -inputTemplates Templates -output output
goto :EOF

:Help
@echo run -all to do all conversions
@echo run -debug to just convert a few active items


