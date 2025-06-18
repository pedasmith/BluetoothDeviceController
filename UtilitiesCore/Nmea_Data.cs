using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parsers.Nmea
{
    public class Nmea_Data
    {
        /// <summary>
        /// Splits an NMEA string into its parts
        /// </summary>
        /// <param name="str"></param>
        public Nmea_Data(string str)
        {
            ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeUnknown; // nice default :-)
            OriginalNmeaString = str;
            NmeaParts = str.Split(',');
            // Might be old style ,*VV
            // Might be new style ,value*VV where the value is the last value
            var chk = ChecksumStringRaw;
            var starpos = chk.IndexOf("*");
            if (starpos < 1)
            {
                LastElement = "";
                Checksum = chk;
            }
            else
            {
                LastElement = chk.Substring(0, starpos);
                Checksum = chk.Substring(starpos); // include the "*"
            }
        }

        static public int Test()
        {
            int nerror = 0;
            var v1 = new Nmea_Data("NAME,1,2,3*99");
            if (v1.LastElement != "3")
            {
                nerror++;
                Console.WriteLine($"Error: expected 3 not {v1.LastElement} in {v1}");
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
            if (v1.LastElement != "")
            {
                nerror++;
                Console.WriteLine($"Error: expected blank not {v1.LastElement} in {v1}");
            }
            if (v1.Checksum != "*98")
            {
                nerror++;
                Console.WriteLine($"Error: expexted *98 not {v1.Checksum} in {v1}");
            }

            v1 = new Nmea_Data("NAME,1,2,97");
            if (v1.LastElement != "")
            {
                nerror++;
                Console.WriteLine($"Error: expected blank not {v1.LastElement} in {v1}");
            }
            if (v1.Checksum != "97")
            {
                nerror++;
                Console.WriteLine($"Error: expexted 97 not {v1.Checksum} in {v1}");
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

        public string ChecksumStringRaw { get { return GetPart(-1); } }
        public string[] NmeaParts { get; internal set; }

        public string Checksum { get; internal set; }
        public string LastElement { get; internal set; }
        public string GetPart(int index)
        {
            if (NmeaParts.Length == 0) return "";
            if (index == -1) return NmeaParts[NmeaParts.Length - 1];
            if (index == NmeaParts.Length - 1) return LastElement; // The only way to get the checksum (last value) is to ask for -1.
            if (index > NmeaParts.Length - 1) return ""; // The only way to get the checksum (last value) is to ask for -1.
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
                case "$GPGGA": return "Time and position plus fix type";
                case "$GPGLL": return "Latitude and longitude";
                case "$GPGSA": return "Satellite data plus operating mode and DOP values";
                case "$GPGSV": return "Satellite ID and position";
                case "$GPPWR": return "Power data";
                case "$GPRMC": return "Time, position, course, speed";
                case "$GPVTG": return "Course and speed relative to ground";
                case "$GPZDA": return "Time and date";
                case "$": return "";
            }
            return name;
        }
        public string OriginalNmeaString { get; set; } = "";
    }
}
