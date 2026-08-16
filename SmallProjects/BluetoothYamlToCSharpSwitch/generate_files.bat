REM Generate C# switch statements

rmdir output /S /Q
mkdir output
mkdir outputnames
set CSHARPSRC=..\..\BluetoothConversions
set CSHARPSRC2=..\..\BluetoothProtocolsNames
set BINSRC=.\bin\Debug\net10.0
set CVT=%BINSRC%\BluetoothYamlToCSharpSwitch

"%CVT%" --type updatefile --updatewith file --outputdir outputnames --file "%CSHARPSRC2%\BluetoothServiceRegistration.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothBodySensorLocation.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothUnit.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothCharacteristic.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothCompanyIdentifier.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothServiceUuid16Bit.cs"