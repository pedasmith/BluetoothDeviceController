using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Bluetooth.Advertisement;
using static BluetoothProtocols.BDM_Aneng_Multimeter;
using static BluetoothProtocols.BTStandard_CyclingSpeedCadence;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocolsDevicesCoreExtensions;

/// <summary>
/// Just plain data for a multimeters. Generally created by the BdmMultimeter.Parse method.
/// </summary>
public class BdmMultimeterDisplayFacade : BTCommonMetaData<BdmMultimeterDisplayFacade>
{
    public string DisplayText = "";
    public double DisplayNumber = double.NaN;
    public bool Overload = false;
    public string DisplayUnits = "";
    public string DisplayOther = "";
    public static string HoldStr = "Hold ";
    public static string AutoStr = "AUTO ";
    public enum DmmMode {  Other, Volts, DutyCycle, Hertz, Temperature, Resistance, Diode, Continuity, Capacitance, Amps }
    // DmmMode {  Other, XXVolts, XXDutyCycle, XXHertz, XXTemperature, XXResistance, XXDiode, Continuity, XXCapacitance, XXAmps }
    // Missing: continuity? Which is resistance with beeps!

    public DmmMode Mode { get; set; } = DmmMode.Other;
    public string ModeStr
    {
        get
        {
            string retval = Mode switch
            {
                DmmMode.Volts => "Volts",
                DmmMode.DutyCycle => "Duty cycle",
                DmmMode.Hertz => "Frequency",
                DmmMode.Temperature => "Temperature",
                DmmMode.Resistance => "Resistance",
                DmmMode.Diode => "Diode",
                DmmMode.Continuity => "Continuity",
                DmmMode.Capacitance => "Capacitance",
                DmmMode.Amps => "Current",
                _ => ""
            };
            return retval;
        }
    }
    public double Multiplier = 1.0;
    public double MultimeterValue { get { return DisplayNumber * Multiplier; } }


    public override BdmMultimeterDisplayFacade Clone(string name = null)
    {
        var retval = this.MemberwiseClone() as BdmMultimeterDisplayFacade;
        if (name != null)
        {
            retval.Name = name;
        }
        return retval;
    }

    public override void CopyFrom(BdmMultimeterDisplayFacade source)
    {
        var dest = this;

        // All of the facade-specific values.
        dest.Mode = source.Mode;
        dest.DisplayText = source.DisplayText;
        dest.DisplayNumber = source.DisplayNumber;
        dest.Overload = source.Overload;
        dest.DisplayUnits = source.DisplayUnits;
        dest.DisplayOther = source.DisplayOther;
        dest.Multiplier = source.Multiplier;

        // From the original class
        dest.TimestampMostRecent = source.TimestampMostRecent;
        dest.Name = source.Name;

    }

    // Like CopyFrom, but convert the doubles as appropriate + sets name
    public static BdmMultimeterDisplayFacade CopyToWithConvertAndCreate(Multimeter_Data source, BdmMultimeterDisplayFacade dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
    {
        if (dest == null)
        {
            dest = new();
        }

        // From the original class
        dest.TimestampMostRecent = source.TimestampMostRecent;
        dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;

        var newMM = BdmMultimeterProtocol.Parse(source.Data);
        // The Parse won't set these two values :-(
        newMM.TimestampMostRecent = dest.TimestampMostRecent;
        newMM.Name = dest.Name;
        dest.CopyFrom(newMM); // copy all the fields

        return dest;
    }


    public override string[] ExportGetHeaders(IExportData _)
    {
        return ["Value", "DisplayText", "Units", "Mode", "Overload", "DisplayOther"];
    }

    public override void ExportRow(IExportData exporter)
    {
        // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
        // RowEnd and add in the timestamps
        exporter.CellSet(MultimeterValue);
        exporter.CellSet(DisplayText);
        exporter.CellSet(DisplayUnits);
        exporter.CellSet(Overload.ToString());
        exporter.CellSet(ModeStr);
        exporter.CellSet(DisplayOther);
    }
    public override string ToString()
    {
        return $"{DisplayText}{DisplayUnits} measure={ModeStr} overload={Overload} value={MultimeterValue} other={DisplayOther}";
    }
}

public static class BdmMultimeterProtocol
{
    // https://github.com/libreble/multimeter/blob/main/docs/protocols/bdm.md
    // https://github.com/libreble/multimeter/blob/main/packages/protocol/src/drivers/bdm.ts#L136

    /// <summary>
    /// Parses a BDM Multimeter protocol string into a BdmMultimeterDisplay object.
    /// </summary>

    public static BdmMultimeterDisplayFacade Parse(byte[] data)
    {
        BdmMultimeterDisplayFacade retval = new();
        if (data == null) return retval;

        // From the decodeAb300
        // Characteristic Changed: FFF4: 1B 84 70 51 48 9A DC 7E 66 AC 3A
        byte[] decode = new byte[11];
        string bits = "";
        for (int i=0; i<data.Length; i++)
        {
            var b = (data[i] ^ DATASHIFT[i]);
            bits += Convert.ToString(b, 2).PadLeft(8, '0');
        }
        string[] prePoints = new[] { "-", ".", ".", "." };

        Console.WriteLine($"Bits: {bits}");
        DecodeDisplayUnits(retval, bits);
        Console.WriteLine($"Display Units: {retval.DisplayUnits}");

        for (int n=0; n<4; n++)
        {
            var fi = (n + 3) * 8; // AKA: n*8 + 24
            var first = bits.Substring(fi, 3);

            var si = (n + 4) * 8 + 4; // AKA: n*8 + 36 or fi+12
            var second = bits.Substring(si, 4);
            var prefix = on(bits, (n + 3) * 8 + 3) ? prePoints[n] : ""; // AKA n*8 + 27 or fi+3
            var text = prefix + GetSeg(first + second);
            retval.DisplayText += text;
        }
        retval.Overload = retval.DisplayText.Contains("L");
        double value = double.NaN;
        bool isNumber = double.TryParse(retval.DisplayText, out value);
        retval.DisplayNumber = value;

        Console.WriteLine($"Parsed value: {retval}");
        return retval;
    }



    private static void DecodeDisplayUnits(BdmMultimeterDisplayFacade retval, string bits)
    {
        const double MEGA = 1000.0;
        const double KILO = 1000.0;
        const double MILLI = 0.001;
        const double MICRO = 0.000_0001;
        const double NANO = 0.000_000_001;

        if (on(bits, 56))
        {
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Diode;
            retval.DisplayOther += "Diode ";
        }

        // Mentioned in the manual (page 3) HOLD MAX MIN MANUAL AUTI TRUE RMS LowBattery AC DC Negative Real Bluetooth
        if (on(bits, 57))
        {
            retval.DisplayUnits += "°C"; // testing temperature
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Temperature;
        }
        if (on(bits, 58))
        {
            retval.DisplayUnits += "°F"; // testing temperature
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Temperature;
        }

        if (on(bits, 59)) retval.DisplayOther += BdmMultimeterDisplayFacade.HoldStr; // "Hold ";
        // These are used by the 'si' part of digit n=3 (last digit)
        //if (on(bits, 60)) retval.DisplayOther += "60 ";
        //if (on(bits, 61)) retval.DisplayOther += "61 ";
        //if (on(bits, 62)) retval.DisplayOther += "62 ";
        //if (on(bits, 63)) retval.DisplayOther += "63 ";

        if (on(bits, 64))
        {
            retval.DisplayUnits += "n";
            retval.Multiplier = NANO;
        }
        if (on(bits, 65))
        {
            retval.DisplayUnits += "m";
            retval.Multiplier = MILLI;
        }
        if (on(bits, 66))
        {
            retval.DisplayUnits += "µ";
            retval.Multiplier = MICRO;
        }
        if (on(bits, 67))
        {
            retval.DisplayUnits += "F"; // Testing capacitance
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Capacitance;
        }

        if (on(bits, 68)) retval.DisplayOther += "AC ";

        // Duty Cycle
        if (on(bits, 69))
        {
            retval.DisplayUnits += "%"; // Testing duty cycle
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.DutyCycle;
        }

        if (on(bits, 70)) retval.DisplayOther += "MIN "; // not in original code
        if (on(bits, 71)) retval.DisplayOther += "MAX "; // not in original code

        // Current is bit 72 but is sorted to 85 and up

        if (on(bits, 73)) retval.DisplayOther += "DC ";

        // Voltage
        if (on(bits, 74))
        {
            retval.DisplayUnits += "m"; // milli
            retval.Multiplier = MILLI;
        }
        if (on(bits, 75))
        {
            retval.DisplayUnits += "V"; // testing volts
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Volts;
        }


        // Resistance
        if (on(bits, 76))
        {
            retval.DisplayUnits += "M";
            retval.Multiplier = MEGA;
        }
        if (on(bits, 77))
        {
            retval.DisplayUnits += "k";
            retval.Multiplier = KILO;
        }
        if (on(bits, 78))
        {
            retval.DisplayUnits += "Ω"; // testing ohms
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Resistance;
        }

        // Frequency
        if (on(bits, 79))
        {
            retval.DisplayUnits += "Hz"; // testing frequency
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Hertz;
        }
        if (on(bits, 80)) retval.DisplayOther += "80 ";
        if (on(bits, 81)) retval.DisplayOther += "81 ";
        if (on(bits, 82)) retval.DisplayOther += "82 ";
        if (on(bits, 83)) retval.DisplayOther += "83 ";

        // Current (see also bit 72!)
        if (on(bits, 84))
        {
            retval.DisplayUnits += "m";
            retval.Multiplier = MICRO;
        }
        if (on(bits, 85))
        {
            retval.DisplayUnits += "µ";
            retval.Multiplier = MICRO;
        }
        if (on(bits, 72))
        {
            retval.DisplayUnits += "A"; // testing current
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Amps;
        }


        if (on(bits, 86)) retval.DisplayOther += "86 ";
        if (on(bits, 87)) retval.DisplayOther += BdmMultimeterDisplayFacade.AutoStr; // "AUTO ";

        for (int i=88; i<bits.Length; i++) // Length should be 88; this is for extra
        {
            if (on(bits, i)) retval.DisplayOther += i.ToString() + " ";

        }
    }
    private static bool on(string bits, int bitnumber)
    {
        var retval = bits[bitnumber] == '1';
        return retval;
    }
    private static string GetSeg(string lookup, string defaultValue="?")
    {
        if (SEG.ContainsKey(lookup)) return SEG[lookup];
        return defaultValue;
    }
    //static byte[] DATASHIFT = { 65, 33, 115, 85, 162, 193, 50, 113, 102, 170, 59 };
    // Every now any then by Aneg produces a 20-byte notification. From the 'all-Variants' doc, 
    // the original decode was 20 bytes which the DMM for Windows cut to 11 because all the 
    // messages they see are that length.
    // See: https://github.com/ludwich66/Bluetooth-DMM/wiki/Protocol-all-Variants

    static byte[] DATASHIFT = { 65, 33, 115, 85, 162, 193, 50, 113, 102, 170, 59, 0xD0, 0xE2, 0xA8, 0x33, 0x14, 0x20, 0x21, 0xAA, 0xBB };
    static Dictionary<string, string> SEG = new() {
        { "0000000", " " },
        { "1111110", "A" },
        { "0010011", "U" },
        { "0110101", "T" },
        { "0010111", "O" },
        { "1110101", "E" },
        { "1110100", "F" },
        { "0110001", "L" },
        { "0000100", "-" },
        { "1111011", "0" },
        { "0001010", "1" },
        { "1011101", "2" },
        { "1001111", "3" },
        { "0101110", "4" },
        { "1100111", "5" },
        { "1110111", "6" },
        { "1001010", "7" },
        { "1111111", "8" },
        { "1101111", "9" },
    };

    private static byte[] StringToByte(string str)
    {
        var data = str.Split(" ");
        var retval = new byte[data.Length];
        for (int i=0; i<data.Length; i++)
        {
            var item = data[i];
            byte b;
            bool ok = Byte.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out b);
            retval[i] = b;
        }
        return retval;
    }

    public static int Test()
    {
        int nerror = 0;

        var b = StringToByte("1B 84 70 51 48 9A DC 7E 66 AC 3A");
        BdmMultimeterProtocol.Parse(b);
        return nerror;
    }
}

