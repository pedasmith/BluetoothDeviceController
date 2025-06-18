using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Parsers.Nmea
{
    public class Nmea_Data
    {
        /// <summary>
        /// Splits an NMEA string into its parts
        /// </summary>
        /// <param name="str">Example: $GPPWR,0876,0,0,0,0,00,F,0,97,1,3,000,00190108EEEE,0017E9B92122*74</param>

        public Nmea_Data(string str)
        {
            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok; // Starts Ok then gets flipped to an error as appropriate.
            OriginalNmeaString = str;

            var starpos = str.IndexOf("*");
            if (starpos < 1)
            {
                Checksum = "";
            }
            else
            {
                Checksum = str.Substring(starpos);
                str = str.Substring(0, starpos);

                if (Checksum.Length < 3) // includes the *
                {
                    ParseStatus = Nmea_Gps_Parser.ParseResult.ChecksumLengthTooShort;
                }
                else if (Checksum.Length > 3) // includes the *
                {
                    ParseStatus = Nmea_Gps_Parser.ParseResult.ChecksumLengthTooLong;
                }
                else
                {
                    int value = -1;
                    var ok = Int32.TryParse(Checksum.Substring(1), NumberStyles.AllowHexSpecifier, null, out value);
                    ChecksumValue = value;
                    if (!ok)
                    {
                        ParseStatus = Nmea_Gps_Parser.ParseResult.ChecksumNotHex;
                    }
                    else
                    {
                        if (value >= 128 || value < 0)
                        {
                            ParseStatus = Nmea_Gps_Parser.ParseResult.ChecksumValueImpossible;
                        }
                        else
                        {
                            // Actually calulate the checksum!
                            var checkstr = str.Substring(1); // Don't include $
                            int checkvalue = 0;
                            for (int i=0; i<checkstr.Length; i++)
                            {
                                int b = (checkstr[i]) & 0x7F; // Remove all but the bottom bits
                                checkvalue = checkvalue ^ b; // XOR
                            }
                            if (checkvalue != ChecksumValue)
                            {
                                ParseStatus = Nmea_Gps_Parser.ParseResult.ChecksumDoesntMatch;
                            }
                        }
                    }
                }
            }

            if (str.Length < 1)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.ZeroLength;
            }
            else
            {
                if (!str.StartsWith("$"))
                {
                    ParseStatus = Nmea_Gps_Parser.ParseResult.NoStartingDollarSign;
                }
                else
                {
                    str = str.Substring(1); // Remove the $
                }
            }

            NmeaParts = str.Split(',');
        }
        static public int Test()
        {
            int nerror = 0;
            var v1 = new Nmea_Data("NAME,1,2,3*99");
            if (v1.GetPart(3) != "3")
            {
                nerror++;
                Console.WriteLine($"Error: expected 3 not {v1.GetPart(3)} in {v1}");
            }
            if (v1.Checksum != "*99")
            {
                nerror++;
                Console.WriteLine($"Error: expexted *99 not {v1.Checksum} in {v1}");
            }
            if (v1.GetPart(0) != "NAME")
            {
                nerror++;
                Console.WriteLine($"Error: expexted index 0=NAME not {v1.GetPart(0)} in {v1}");
            }
            if (v1.GetPart(1) != "1")
            {
                nerror++;
                Console.WriteLine($"Error: expexted index 1=1 not {v1.GetPart(1)} in {v1}");
            }
            if (v1.GetPart(3) != "3")
            {
                nerror++;
                Console.WriteLine($"Error: expexted index 3=3 not {v1.GetPart(3)} in {v1}");
            }

            v1 = new Nmea_Data("NAME,1,2,*98");
            if (v1.GetPart(3) != "")
            {
                nerror++;
                Console.WriteLine($"Error: expected blank not {v1.GetPart(3)} in {v1}");
            }
            if (v1.Checksum != "*98")
            {
                nerror++;
                Console.WriteLine($"Error: expexted *98 not {v1.Checksum} in {v1}");
            }

            v1 = new Nmea_Data("NAME,1,2,97");
            if (v1.GetPart(4) != "")
            {
                nerror++;
                Console.WriteLine($"Error: expected blank not {v1.GetPart(4)} in {v1}");
            }
            if (v1.Checksum != "")
            {
                nerror++;
                Console.WriteLine($"Error: expexted blank not {v1.Checksum} in {v1}");
            }

            return nerror;
        }

        public override string ToString()
        {
            return $"{OriginalNmeaString}  parse={ParseStatus}";
        }

        public virtual string SummaryString { get { return ToString(); } }
        public virtual string DetailString { get { return ToString(); } }

        public string ExplanationString { get { return OpcodeExplanation(GetPart(0)); } }

        public virtual string ParseErrorString { get { return $"{GetPart(0)} {ParseStatus} {OriginalNmeaString}"; } }
        public Nmea_Gps_Parser.ParseResult ParseStatus { get; set; } = Nmea_Gps_Parser.ParseResult.OtherError; // default to error.

        public string[] NmeaParts { get; internal set; }

        public string Checksum { get; internal set; }
        public int ChecksumValue { get; internal set; }
        public string GetPart(int index)
        {
            if (NmeaParts.Length == 0) return "";
            if (index > NmeaParts.Length - 1) return ""; 
            return NmeaParts[index];
        }
        public string GetFirstPart()
        {
            return GetPart(0);
        }
        public static string OpcodeExplanation(string name)
        {
            switch (name)
            {
                case "GPGGA": return "Position and time plus fix type";
                case "GPGLL": return "Latitude and longitude";
                case "GPGSA": return "Satellite data plus operating mode and DOP values";
                case "GPGSV": return "Satellite ID and position";
                case "GPPWR": return "Power data";
                case "GPRMC": return "Position, time, course and speed";
                case "GPVTG": return "Course and speed relative to ground";
                case "GPZDA": return "Time and date";
                case "$": return "";
            }
            return name;
        }
        public string OriginalNmeaString { get; set; } = "";
    }
}
