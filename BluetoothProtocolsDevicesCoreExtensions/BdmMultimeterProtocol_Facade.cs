using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

// This file is also used in a test framework. When it is, STANDALONE is defined so the file
// can be used without the rest of the system
#if !STANDALONE
using BluetoothProtocols;
using Windows.Storage.Streams;
#endif

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocolsDevicesCoreExtensions;

public static class DoubleExtension
{
    /// <summary>
    /// Rounds to a certain number of significant figures
    /// </summary>
    public static double Significant(this double value, int digits = 3)
    {
        if (value == 0)
            return 0;

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
        var retval = scale * Math.Round(value / scale, digits);
        return retval;
    }

    public static string FixupSignificant(this string sig)
    {
        var sigindex = sig.IndexOf('.');
        if (sigindex >= 0)
        {
            // converted to e.g. 16.000000000000001
            var ndigit = sig.Length - sigindex - 1;
            if (ndigit > 3)
            {
                // nuke the '1' at the end then remove all the 0s
                // there will be a '.' as the sentinal
                sig = sig.Substring(0, sig.Length - 1);
                char lookfor = sig[sig.Length - 1]; // either 0 or 9!
                while (sig[sig.Length - 1] == lookfor)
                {
                    sig = sig.Substring(0, sig.Length - 1);
                }
                if (sig[sig.Length - 1] == '.')
                {
                    sig = sig.Substring(0, sig.Length - 1);
                }
            }
        }
        return sig;
    }
}
/// <summary>
/// Just plain data for a multimeters. Generally created by the BdmMultimeter.Parse method.
/// </summary>
public class BdmMultimeterDisplayFacade
#if !STANDALONE
    : BTCommonMetaData<BdmMultimeterDisplayFacade>
#endif
{
    public string DisplayText = "";
    public double DisplayNumber = double.NaN;
    public bool Overload = false;
    public string DisplayUnits = "";
    public string DisplayOther = "";
    public static string HoldStr = "Hold ";
    public static string AutoStr = "AUTO ";
    public enum DmmMode { Other, Volts, DutyCycle, Hertz, Temperature, Resistance, Diode, Continuity, Capacitance, Amps,
        Idle, // PokitPro
    }
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

#if !STANDALONE
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
    public static BdmMultimeterDisplayFacade CopyToWithConvertAndCreate(BDM_Aneng_Multimeter.Multimeter_Data source, IMultimeterProtocolParser parser, BdmMultimeterDisplayFacade dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
    {
        if (dest == null)
        {
            dest = new();
        }

        // From the original class
        dest.TimestampMostRecent = source.TimestampMostRecent;
        dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;

        var data = source.DataFFF1;
        if (data == null) data = source.DataFFF4;

        var newMM = parser.Parse(data);
        // The Parse won't set these two values :-(
        newMM.TimestampMostRecent = dest.TimestampMostRecent;
        newMM.Name = dest.Name;
        dest.CopyFrom(newMM); // copy all the fields

        return dest;
    }

    public static BdmMultimeterDisplayFacade CopyToWithConvertAndCreate(PokitProMeter.Multimeter_Data source, BdmMultimeterDisplayFacade dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
    {
        if (dest == null)
        {
            dest = new();
        }


        // All of the facade-specific values.
        dest.Mode = DmmModeFromPokitProMode(source.OperationMode);
        dest.Multiplier = 1.0; // all the PokitPro data is in normal units
        dest.DisplayNumber = source.Data;
        dest.DisplayUnits = "";
        AdjustDisplayNumberAndUnitsAndMultiplier(source.Data, dest);
        if (source.Data != 0)
        {
            dest.DisplayUnits += UnitsFromPokitProMode(source.OperationMode);
        }
        dest.DisplayOther = OtherFromPokitProMode(source.OperationMode); // e.g., "DC" and "diode"
        var sig = dest.DisplayNumber.Significant(3).ToString();
        sig = sig.FixupSignificant();
        dest.DisplayText = sig;

        // From the original class
        dest.TimestampMostRecent = source.TimestampMostRecent;
        dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;

        return dest;
    }

    /// <summary>
    /// mode 0=idle, 1=voltDC, 2=voltAC 3=ampDC,4=ampAC 5=ohm 6=diode 7=continuity 8=temp
    /// </summary>
    private static DmmMode DmmModeFromPokitProMode(double value)
    {
        DmmMode retval = ((int)value) switch
        {
            0 => DmmMode.Idle,
            1 => DmmMode.Volts, // DC
            2 => DmmMode.Volts, // AC
            3 => DmmMode.Amps, // DC
            4 => DmmMode.Amps, // AC
            5 => DmmMode.Resistance,
            6 => DmmMode.Diode,
            7 => DmmMode.Continuity,
            8 => DmmMode.Temperature,
            _ => DmmMode.Other,
        };
        return retval;
    }

    private static string UnitsFromPokitProMode(double value)
    {
        String retval = ((int)value) switch
        {
            0 => "",
            1 => "V", // DC
            2 => "V", // AC
            3 => "A", // DC
            4 => "A", // AC
            5 => "Ω",
            6 => "",
            7 => "",
            8 => "°C",
            _ => "",
        };
        return retval;

    }
    private static string OtherFromPokitProMode(double value)
    {
        String retval = ((int)value) switch
        {
            0 => "idle",
            1 => "DC", // DC
            2 => "AC", // AC
            3 => "DC", // DC
            4 => "AC", // AC
            5 => "",
            6 => "Diode ",
            7 => "",
            8 => "",
            _ => "other",
        };
        return retval;

    }

    private static void AdjustDisplayNumberAndUnitsAndMultiplier(double sourceData, BdmMultimeterDisplayFacade dest)
    {
        var sourceDataAbs = Math.Abs(sourceData);
        if (sourceDataAbs == 0)
        {
            dest.Multiplier = 1.0;
            dest.DisplayNumber = sourceDataAbs;
            //dest.DisplayUnits = ""; Don't change this -- e.g., reistance drops from 2 KOhm to 0 should show 0 KOhm
        }
        else if (sourceDataAbs < 0.000_000_001)
        {
            dest.Multiplier = 0.0000_000_000_0001;
            dest.DisplayNumber = sourceData * 1000.0 * 1000 * 1000 * 1000;
            dest.DisplayUnits = "p"; // pico
        }
        else if (sourceDataAbs < 0.000_000_001)
        {
            dest.Multiplier = 0.000_000_001;
            dest.DisplayNumber = sourceData * 1000.0 * 1000 * 1000;
            dest.DisplayUnits = "n"; // nano
        }
        else if (sourceDataAbs < 0.001)
        {
            dest.Multiplier = 0.000_001;
            dest.DisplayNumber = sourceData * 1000.0 * 1000;
            dest.DisplayUnits = "µ"; // micro
        }
        else if (sourceDataAbs < 1.0)
        {
            dest.Multiplier = 0.001;
            dest.DisplayNumber = sourceData * 1000.0;
            dest.DisplayUnits = "m"; // milli
        }
        else if (sourceDataAbs > 1000_000)
        {
            dest.Multiplier = 1000_000.0;
            dest.DisplayNumber = sourceData / (1000.0 * 1000);
            dest.DisplayUnits = "M";
        }
        else if (sourceDataAbs > 1000)
        {
            dest.Multiplier = 1000.0;
            dest.DisplayNumber = sourceData / 1000.0;
            dest.DisplayUnits = "k";
        }
        else
        {
            dest.Multiplier = 1.0;
            dest.DisplayNumber = sourceData;
            dest.DisplayUnits = "";
        }

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
        exporter.CellSet(ModeStr);
        exporter.CellSet(Overload ? "Overload" : "");
        exporter.CellSet(DisplayOther);
    }
#endif
    public override string ToString()
    {
        return $"{DisplayText}{DisplayUnits} measure={ModeStr} overload={Overload} value={MultimeterValue} other={DisplayOther}";
    }
}

public interface IMultimeterProtocolParser
{
    BdmMultimeterDisplayFacade Parse(byte[] data);
    bool MatchesThis(byte[] data);
}

public class BdmMultimeterProtocol : IMultimeterProtocolParser
{
    // https://github.com/libreble/multimeter/blob/main/docs/protocols/bdm.md
    // https://github.com/libreble/multimeter/blob/main/packages/protocol/src/drivers/bdm.ts#L136


    /// <summary>
    /// Very unsophisticated matching algorithm for Multimeter data.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool Matches(byte[] data)
    {
        var retval = data.Length == 11;
        return retval;
    }

    public bool MatchesThis(byte[] data)
    {
        return Matches(data);
    }


    /// <summary>
    /// Parses a BDM Multimeter protocol string into a BdmMultimeterDisplay object.
    /// </summary>

    public BdmMultimeterDisplayFacade Parse(byte[] data)
    {
        BdmMultimeterDisplayFacade retval = new();
        if (data == null) return retval;

        // From the decodeAb300
        // Characteristic Changed: FFF4: 1B 84 70 51 48 9A DC 7E 66 AC 3A
        byte[] decode = new byte[11];
        string bits = "";
        for (int i = 0; i < data.Length; i++)
        {
            var b = (data[i] ^ DATASHIFT[i]);
            bits += Convert.ToString(b, 2).PadLeft(8, '0');
        }
        string[] prePoints = new[] { "-", ".", ".", "." };

        DecodeDisplayUnits(retval, bits);

        for (int n = 0; n < 4; n++)
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

        Log($"Bdm: Parsed value: {retval}");
        return retval;
    }

    private static void Log(string str)
    {
        Console.WriteLine(str);
        System.Diagnostics.Debug.WriteLine(str);
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

        for (int i = 88; i < bits.Length; i++) // Length should be 88; this is for extra
        {
            if (on(bits, i)) retval.DisplayOther += i.ToString() + " ";

        }
    }
    private static bool on(string bits, int bitnumber)
    {
        var retval = bits[bitnumber] == '1';
        return retval;
    }
    private static string GetSeg(string lookup, string defaultValue = "?")
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
        for (int i = 0; i < data.Length; i++)
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
        var protocol = new BdmMultimeterProtocol();

        var b = StringToByte("1B 84 70 51 48 9A DC 7E 66 AC 3A");
        protocol.Parse(b);
        return nerror;
    }
}



public class FortuneFS9952MultimeterProtocol : IMultimeterProtocolParser
{
    // https://www.ic-fortune.com/upload/Download/FS9952_LP2-DS-12_EN.pdf


    /// <summary>
    /// Very unsophisticated matching algorithm for Multimeter data.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool Matches(byte[] data)
    {
        var retval = data.Length == 15;
        retval = retval && data[0xC] == 0xDD && data[0xD] == 0xEE && data[0xE] == 0xFF;
        return retval;
    }
    public bool MatchesThis(byte[] data)
    {
        return Matches(data);
    }

    /// <summary>
    /// Parses a Fortune FS9952 Multimeter protocol string into a BdmMultimeterDisplay object.
    /// </summary>

    public BdmMultimeterDisplayFacade Parse(byte[] data)
    {
        BdmMultimeterDisplayFacade retval = new();
        if (data == null) return retval;
        if (data.Length != 15) return retval; // wrong length.

        byte[] decode = new byte[11];
        string bits = "";
        // The top four bits of each byte is just 1, 2, 3, 4... A, B, C, D, E, F
        for (int i = 0; i < data.Length; i++)
        {
            var b = (data[i] & 0x0F);
            bits += Convert.ToString(b, 2).PadLeft(4, '0');
        }
        string[] prePoints = new[] { "-", ".", ".", "." };

        DecodeDisplayUnits(retval, bits);

        var digitseg = GetDigitSegments(bits); // sign/decimal point is before ABCDEFG segments
        for (int n = 0; n < 4; n++)
        {
            var first = digitseg.Substring(n * 8 + 1, 7);
            var prefix = on(digitseg, n * 8) ? prePoints[n] : ""; // AKA n*8 + 27 or fi+3
            var text = prefix + GetSeg(first);
            retval.DisplayText += text;
        }
        retval.Overload = retval.DisplayText.Contains("L");
        double value = double.NaN;
        bool isNumber = double.TryParse(retval.DisplayText, out value);
        retval.DisplayNumber = value;

        Console.WriteLine($"Parsed value: {retval}");
        return retval;
    }

    /*
    Byte 0: seg1 seg2: B1C1 sign ac dc P3 E3 F3 auto 
    Byte 1: seg3 seg4: D3 C3 G3 A3 P2 E2 B3 diode
    Byte 2: seg5 seg 6: D2 G2 F2 A2 P1 C2 B2 beep
    Byte 3: seg7 seg 8: D1 E1 F1 p.h milli C1 G1 A1
    Byte 4: seg9 seg10: micro c B1 m.h v F k delta 
    Byte 5: seg11 seg 12: amp (blank) W hFE (blank) (blank) ohm battery
    */
    enum BitLocations
    {
        B1C1, sign, ac, dc, P3, E3, F3, auto,
        D3, C3, G3, A3, P2, E2, B3, diode,
        D2, G2, F2, A2, P1, C2, B2, beep,
        D1, E1, F1, ph, milli, C1, G1, A1,
        micro, celcius, B1, mh, volts, fahrenheit, kilo, delta,
        amp, blank1, mega, hFE, blank2, blank3, ohm, battery,
    };


    private static string GetDigitSegments(string bits)
    {
        StringBuilder sb = new();
        sb.Append(on(bits, BitLocations.sign) ? "1" : "0"); // Sign
        sb.Append(on(bits, BitLocations.B1C1) ? "0110000" : "0000000"); // It's a 3 1/2 display, so the first digit is either 1 or blank

        sb.Append(on(bits, BitLocations.P3) ? "1" : "0");
        sb.Append(on(bits, BitLocations.A3) ? "1" : "0");
        sb.Append(on(bits, BitLocations.B3) ? "1" : "0");
        sb.Append(on(bits, BitLocations.C3) ? "1" : "0");
        sb.Append(on(bits, BitLocations.D3) ? "1" : "0");
        sb.Append(on(bits, BitLocations.E3) ? "1" : "0");
        sb.Append(on(bits, BitLocations.F3) ? "1" : "0");
        sb.Append(on(bits, BitLocations.G3) ? "1" : "0");

        sb.Append(on(bits, BitLocations.P2) ? "1" : "0");
        sb.Append(on(bits, BitLocations.A2) ? "1" : "0");
        sb.Append(on(bits, BitLocations.B2) ? "1" : "0");
        sb.Append(on(bits, BitLocations.C2) ? "1" : "0");
        sb.Append(on(bits, BitLocations.D2) ? "1" : "0");
        sb.Append(on(bits, BitLocations.E2) ? "1" : "0");
        sb.Append(on(bits, BitLocations.F2) ? "1" : "0");
        sb.Append(on(bits, BitLocations.G2) ? "1" : "0");

        sb.Append(on(bits, BitLocations.P1) ? "1" : "0");
        sb.Append(on(bits, BitLocations.A1) ? "1" : "0");
        sb.Append(on(bits, BitLocations.B1) ? "1" : "0");
        sb.Append(on(bits, BitLocations.C1) ? "1" : "0");
        sb.Append(on(bits, BitLocations.D1) ? "1" : "0");
        sb.Append(on(bits, BitLocations.E1) ? "1" : "0");
        sb.Append(on(bits, BitLocations.F1) ? "1" : "0");
        sb.Append(on(bits, BitLocations.G1) ? "1" : "0");

        return sb.ToString();
    }

    private static void DecodeDisplayUnits(BdmMultimeterDisplayFacade retval, string bits)
    {
        const double MEGA = 1000.0;
        const double KILO = 1000.0;
        const double MILLI = 0.001;
        const double MICRO = 0.000_0001;
        //const double NANO = 0.000_000_001;

        if (on(bits, BitLocations.diode))
        {
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Diode;
            retval.DisplayOther += "Diode ";
        }

        // Mentioned in the manual (page 3) HOLD MAX MIN MANUAL AUTI TRUE RMS LowBattery AC DC Negative Real Bluetooth
        if (on(bits, BitLocations.celcius))
        {
            retval.DisplayUnits += "°C"; // testing temperature
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Temperature;
        }
        if (on(bits, BitLocations.fahrenheit))
        {
            retval.DisplayUnits += "°F"; // testing temperature
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Temperature;
        }

        if (on(bits, BitLocations.ac)) retval.DisplayOther += "AC ";

        if (on(bits, BitLocations.dc)) retval.DisplayOther += "DC ";

        // Voltage
        if (on(bits, BitLocations.milli))
        {
            retval.DisplayUnits += "m"; // milli
            retval.Multiplier = MILLI;
        }
        if (on(bits, BitLocations.micro))
        {
            retval.DisplayUnits += "µ";
            retval.Multiplier = MICRO;
        }
        if (on(bits, BitLocations.volts))
        {
            retval.DisplayUnits += "V"; // testing volts
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Volts;
        }


        // Resistance
        if (on(bits, BitLocations.mega))
        {
            retval.DisplayUnits += "M";
            retval.Multiplier = MEGA;
        }
        if (on(bits, BitLocations.kilo))
        {
            retval.DisplayUnits += "k";
            retval.Multiplier = KILO;
        }
        if (on(bits, BitLocations.ohm)) // actually 47?
        {
            retval.DisplayUnits += "Ω"; // testing ohms
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Resistance;
        }

        // Current (see also bit 72!)
        // uses the milli and micro from voltage
        if (on(bits, BitLocations.amp))
        {
            retval.DisplayUnits += "A"; // testing current
            retval.Mode = BdmMultimeterDisplayFacade.DmmMode.Amps;
        }

        if (on(bits, BitLocations.auto)) retval.DisplayOther += BdmMultimeterDisplayFacade.AutoStr; // "AUTO ";


        if (on(bits, BitLocations.beep)) retval.DisplayOther += "beep ";
        if (on(bits, BitLocations.ph)) retval.DisplayOther += "ph "; // Peak hold
        if (on(bits, BitLocations.mh)) retval.DisplayOther += "mh "; // Manual hold
        if (on(bits, BitLocations.delta)) retval.DisplayOther += "delta ";
        if (on(bits, BitLocations.blank1)) retval.DisplayOther += "blank1 ";
        if (on(bits, BitLocations.hFE)) retval.DisplayOther += "hFE "; // Transistor gain
        if (on(bits, BitLocations.blank2)) retval.DisplayOther += "blank2 ";
        if (on(bits, BitLocations.blank3)) retval.DisplayOther += "blank3 ";
        if (on(bits, BitLocations.battery)) retval.DisplayOther += "battery ";


        // continuity mode: -o- high-resistance shows resistance other=beep+hFE low-resistance shows 0.L
    }
    private static bool on(string bits, int bitnumber)
    {
        var retval = bits[bitnumber] == '1';
        return retval;
    }
    private static bool on(string bits, BitLocations bitnumber)
    {
        var retval = bits[(int)bitnumber] == '1';
        return retval;
    }
    private static string GetSeg(string lookup, string defaultValue = "?")
    {
        if (SEG.ContainsKey(lookup)) return SEG[lookup];
        return defaultValue;
    }
    //static byte[] DATASHIFT = { 65, 33, 115, 85, 162, 193, 50, 113, 102, 170, 59 };
    // Every now any then by Aneg produces a 20-byte notification. From the 'all-Variants' doc, 
    // the original decode was 20 bytes which the DMM for Windows cut to 11 because all the 
    // messages they see are that length.
    // See: https://github.com/ludwich66/Bluetooth-DMM/wiki/Protocol-all-Variants

    // Fortune doesn't do this: static byte[] DATASHIFT = { 65, 33, 115, 85, 162, 193, 50, 113, 102, 170, 59, 0xD0, 0xE2, 0xA8, 0x33, 0x14, 0x20, 0x21, 0xAA, 0xBB };
    static Dictionary<string, string> SEG = new() {
        { "0000000", " " },
        { "1110110", "A" },
        { "0111110", "U" },
        { "1000110", "T" },
        { "0011101", "O" },
        { "1001111", "E" },
        { "1000111", "F" },
        { "0001110", "L" },
        { "0000001", "-" },
        { "1111110", "0" },
        { "0110000", "1" },
        { "1101101", "2" },
        { "1111001", "3" },
        { "0110011", "4" },
        { "1011011", "5" },
        { "1011111", "6" },
        { "1110000", "7" },
        { "1111111", "8" },
        { "1111011", "9" },
    };

    private static byte[] StringToByte(string str)
    {
        var data = str.Split(" ");
        var retval = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            var item = data[i];
            byte b;
            bool ok = Byte.TryParse(item, System.Globalization.NumberStyles.HexNumber, null, out b);
            retval[i] = b;
        }
        return retval;
    }

    private static void Log(string str)
    {
        Console.WriteLine(str);
        System.Diagnostics.Debug.WriteLine(str);
    }

    private int TestOne(string bytestr, string expected)
    {
        int nerror = 0;

        var b = StringToByte(bytestr); // "18 27 3D 4E 5B 66 72 86 92 A2 B1 CA DD EE FF"
        var result = Parse(b);
        var actual = $"{result.DisplayText} {result.DisplayUnits} {result.DisplayOther}";
        if (actual != expected)
        {
            nerror++;
            Log($"Foture:DMM: Error: given {bytestr} expected={expected} actual={actual}");
        }
        return nerror;

    }

    public static int Test()
    {
        int nerror = 0;
        var protocol = new FortuneFS9952MultimeterProtocol();
        nerror += protocol.TestOne("18 27 3D 4E 5B 66 72 86 92 A2 B1 CA DD EE FF", "10.04 kΩ AUTO ");
        nerror += protocol.TestOne("10 27 3D 4E 5A 60 70 80 90 A0 B3 C2 DD EE FF", " 0.L  MΩ AUTO ");
        return nerror;
    }
}


public static class PokitPro_Meter_Multimeter_Extensions
{
    public enum MM_ModeType {  Idle=0, VoltDC=1, VoltAC=2, AmpDC=3, AmpAC=4, Resistance=5, Diode=6, Continuity=7, Temperature=8 }
    public static byte[] GetMM_Setting_Bytes(this PokitProMeter.Multimeter_Data data)
    {
        var dw = new DataWriter();
        dw.ByteOrder = ByteOrder.LittleEndian; // Bluetooth is little endian by default
        dw.WriteByte((byte)data.ModeSetting);
        dw.WriteByte((byte)data.RangeSetting);
        dw.WriteUInt32((UInt32)data.IntervalSetting);
        //await dw.StoreAsync(); will throw, which is silly
        var buffer = dw.DetachBuffer();
        return buffer.ToArray();
    }
}
