
#if NET8_0_OR_GREATER
#nullable disable
#endif


using System.Collections.Generic;

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// the ValueParserResult used to be in the middle of the BleEditor classes.
    /// In the new system, it's in IotNumberFormats. This creates a class
    /// which is compatible with the old systme.
    /// </summary>
    public class ValueParserResult : IotNumberFormats.ValueParserResult
    {

    }


    public static class ValueParserHelpers
    {
        public static ValueParserResult ConvertToBuffer(string value, string type)
        {
            var Retval = new ValueParserResult();
            Retval.ByteResult = new List<byte>() { Capacity = (value.Length + 1) / 3 }; // most likely size
            type = type.ToUpper();
            switch (type)
            {
                case "ASCII":
                    foreach (var ch in value)
                    {
                        Retval.ByteResult.Add((byte)(ch & 0xFF)); //just whack them.
                    }
                    break;
                case "DEC":
                case "HEX":
                    var items = value.Split(" ");
                    foreach (var item in items)
                    {
                        byte b = 0;
                        bool converted;

                        var specifier = System.Globalization.NumberStyles.None;
                        if (type == "HEX")
                        {
                            specifier = System.Globalization.NumberStyles.AllowHexSpecifier;
                        }
                        converted = byte.TryParse(item, specifier, null, out b);
                        if (converted)
                        {
                            Retval.ByteResult.Add(b);
                        }
                        else
                        {
                            Retval.Result = ValueParserResult.ResultValues.Error;
                            Retval.ErrorString += $"Item {item} could not be converted as {type}\n";
                        }
                    }
                    break;
            }
            return Retval;
        }
    }

}
