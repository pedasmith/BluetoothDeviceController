@echo off

REM Generate all of the protocol, etc., files from the JSON device
echo Current directory=%CD%
REM set REPODIR=%UserProfile%\source\repos

set REPODIR=%CD%\..
set JSONDIR=%REPODIR%\BluetoothDeviceController\Assets\CharacteristicsData
set BIN= ".\bin\Debug\net6.0-windows10.0.19041.0\BluetoothCodeGeneratordotNetCore"
if not exist output mkdir output

REM
REM Normally we want to generate everything. But when we are debugging just something new,
REM it's nicer to have just the single file handled.
REM
if "%~1"=="-debug" goto :Debug
if "%~1"=="-file" goto :SingleFile
if "%~1"=="-all" goto :All
goto :Help


:All
%BIN% -verbose -inputJsonDirectory "%JSONDIR%" -inputTemplates Templates -output output
goto :EOF

REM Or generate just the one, which is a little easier to handle when debugging.
:Debug
REM %BIN% -inputJsonFile "%JSONDIR%"\Govee_H6005.json -inputTemplates Templates -output output
REM %BIN% -inputJsonFile "%JSONDIR%"\Bluetooth_CurrentTimeService.json -inputTemplates Templates -output output
REM %BIN% -inputJsonFile "%JSONDIR%"\BtUnicodeKeyboard.json -inputTemplates Templates -output output
REM %BIN% -inputJsonFile "%JSONDIR%"\Nordic_Thingy.json -inputTemplates Templates -output output
%BIN% -inputJsonFile "%JSONDIR%"\PokitPro_Meter.json -inputTemplates Templates -output output
goto :EOF

:SingleFile
ECHO -inputJsonFile "%JSONDIR%\%~2" -inputTemplates Templates -output output
%BIN% -inputJsonFile "%JSONDIR%\%~2" -inputTemplates Templates -output output
goto :EOF

:Help
@echo RUN.BAT from the BluetoothCodeGeneratordotNetCore directory
@echo This runs the Bluetooth code generator. It takes in the JSON BT description files
@echo and a series of Template files and makes all of the BT device-specific outputs.
@echo.
@echo run -all to do all conversions
@echo run -file <file> to do a single conversion. File is in the normal %JSONDIR% directory.
@echo run -debug to just convert a few active items


