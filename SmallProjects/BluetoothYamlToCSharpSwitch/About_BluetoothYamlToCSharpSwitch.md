# About the BluetoothYamlToCSharpSwitch program

The Bluetooth program needs to be able to convert Bluetooth codes and uuids into user-understandable text. This conversion is performed by methods like the static class BluetoothUnit's "Decode" method. The problem is that the Bluetooth consortium is constantly adding new codes and uuids, and the BluetoothUnit class needs to be updated. 

In the past, the updates were done manually. But now the Bluetooth consortium has published a yaml file with all the codes and uuids. This program can take in a specially crafted template C Sharp program file and the YAML file and will update the case statements in the program file.

# History

|Date|Description |
|-----|---|
|2026-03-17|Initial creation of the program. |

# Usage: updating the YAML files

The ```download_yaml.bat``` file can be used to download the latest yaml files from the Bluetooth consortium. It will download the files to the same directory. 

# Usage: updating the C Sharp program file

The ```generate_csharp.bat``` file can be used to update the C Sharp program file. The resulting updated files are dumped into the output directory; you will need to copy them into the BluetoothConversions directory yourself.