REM Generate C# switch statements

rmdir output /S /Q
mkdir output
set CSHARPSRC=..\..\BluetoothConversions
set BINSRC=.\bin\Debug\net10.0
set CVT=%BINSRC%\BluetoothYamlToCSharpSwitch

"%CVT%" --type updatefile --file "%CSHARPSRC%\BluetoothUnit.cs" --updatewith file > output\BluetoothUnit.cs
"%CVT%" --type updatefile --file "%CSHARPSRC%\BluetoothCharacteristic.cs" --updatewith file > output\BluetoothCharacteristic.cs
"%CVT%" --type updatefile --file "%CSHARPSRC%\BluetoothCompanyIdentifier.cs" --updatewith file > output\BluetoothCompanyIdentifier.cs
