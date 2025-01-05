@echo off

REM Copy all of the generated files (from, e.g., the "run.bat" file)

echo Current directory=%CD%
REM set REPODIR=%UserProfile%\source\repos

set REPODIR=%CD%\..
set JSONDIR=%REPODIR%\BluetoothDeviceController\Assets\CharacteristicsData
set BIN= ".\bin\Debug\net6.0-windows10.0.19041.0\BluetoothCodeGeneratordotNetCore"
if not exist output mkdir output

REM
REM Only the proven files are copied. Files that are still in progress are
REM not copied.
REM

if "%~1"=="-all" goto :All
goto :Help


:All
copy output\BluetoothProtocols\*.* %REPODIR%\BluetoothProtocolsDevices
copy output\SpecialtyPages\*.* %REPODIR%\BluetoothDeviceController\SpecialtyPages
ECHO You will need to copy the help files only once
ECHO copy output\Help\*.* %REPODIR%\BluetoothDeviceController\Assets\HelpFiles
goto :EOF


:Help
@echo RUN.BAT from the BluetoothCodeGeneratordotNetCore directory
@echo This runs the Bluetooth code generator. It takes in the JSON BT description files
@echo and a series of Template files and makes all of the BT device-specific outputs.
@echo.
@echo run -all to do all conversions
@echo run -file <file> to do a single conversion. File is in the normal %JSONDIR% directory.
@echo run -debug to just convert a few active items


