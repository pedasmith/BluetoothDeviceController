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

            var starpos = str.IndexOf("*");
            if (starpos < 1)
            {
                Checksum = "";
            }
            else
            {
                Checksum = str.Substring(starpos);
                str = str.Substring(0, starpos);
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
                case "$GPGGA": return "Position and time plus fix type";
                case "$GPGLL": return "Latitude and longitude";
                case "$GPGSA": return "Satellite data plus operating mode and DOP values";
                case "$GPGSV": return "Satellite ID and position";
                case "$GPPWR": return "Power data";
                case "$GPRMC": return "Position, time, course and speed";
                case "$GPVTG": return "Course and speed relative to ground";
                case "$GPZDA": return "Time and date";
                case "$": return "";
            }
            return name;
        }
        public string OriginalNmeaString { get; set; } = "";
    }
}
