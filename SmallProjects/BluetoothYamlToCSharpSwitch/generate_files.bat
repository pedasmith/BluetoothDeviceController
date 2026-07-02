REM Generate C# switch statements

rmdir output /S /Q
mkdir output
set CSHARPSRC=..\..\BluetoothConversions
set BINSRC=.\bin\Debug\net10.0
set CVT=%BINSRC%\BluetoothYamlToCSharpSwitch

"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothBodySensorLocation.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothBodySensorLocation.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothUnit.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothCharacteristic.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothCompanyIdentifier.cs"
"%CVT%" --type updatefile --updatewith file --outputdir output --file "%CSHARPSRC%\BluetoothServiceUuid16Bit.cs"