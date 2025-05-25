using System;
using System.Collections.Generic;
using System.Text;

// FindBitPattern tries to be more nullable enabled.
// Need this super weird set of ifs because:
// 1. many of my projects are "old" and give errors when #nullable disable is present
// 2. I can't use the ? for the events because old projects don't do that
// 3. But for FindBitPattern I need to suppress the warning just for this file.

#if NET9_0_OR_GREATER
#nullable disable
#endif

namespace Parsers.Nmea
{

    public class GPRMC_Data : Nmea_Data
    {
        public GPRMC_Data(string str) 
            :base(str)
        {
            if (NmeaParts.Length < 12)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            bool parseOk = true;

            // UTC time
            ParseStatus = Time.Parse(TimeString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            switch (ValidityString)
            {
                case "A": Validity = ValidityType.ValidCurrentData; break;
                case "V": Validity = ValidityType.InvalidCurrentData; break;
                case "B": Validity = ValidityType.ValidStoredData; break;
                case "W": Validity = ValidityType.InvalidStoredData; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.ValidityInvalid;
                    return;
            }

            ParseStatus = Latitude.Parse(LatitudeString, LatitudeNorthSouthString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Longitude.Parse(LongitudeString, LongitudeEastWestString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            parseOk = parseOk && double.TryParse(VelocityKnotsString, out VelocityKnots);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.VelocityKnotsInvalid;
                return;
            }

            parseOk = parseOk && double.TryParse(HeadingDegreesTrueString, out HeadingDegreesTrue);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.HeadingDegreesTrueInvalid;
                return;
            }

            ParseStatus = Date.Parse(DateString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;
        }

        public string OpcodeString { get { return GetPart(0); } } // $GPRMC
        public string TimeString { get { return GetPart(1); } }
        public Nmea_Time_Fields Time = new Nmea_Time_Fields();
        public string ValidityString { get { return GetPart(2); } } // A=valid current data B=valid stored data V=invalid current data W=invalid stored data
        public enum ValidityType {  ValidCurrentData, ValidStoredData, InvalidCurrentData, InvalidStoredData };
        public ValidityType Validity;
        public string LatitudeString { get { return GetPart(3); } } // DDMM.dddd dd=decimal minutes
        public string LatitudeNorthSouthString { get { return GetPart(4); } } // N S
        public Nmea_Latitude_Fields Latitude = new Nmea_Latitude_Fields();

        public string LongitudeString { get { return GetPart(5); } } // Degrees.MMdd
        public string LongitudeEastWestString { get { return GetPart(6); } } // E W
        public Nmea_Longitude_Fields Longitude = new Nmea_Longitude_Fields();
        public string VelocityKnotsString { get { return GetPart(7); } }
        public double VelocityKnots;

        public string HeadingDegreesTrueString { get { return GetPart(8); } }
        public double HeadingDegreesTrue;
        public string DateString { get { return GetPart(9); } } // ddmmyy
        public Nmea_Date_Fields Date = new Nmea_Date_Fields();

        public override string ToString()
        {
            return $"{OpcodeString} {Time} {Latitude} {Longitude} {Date} validity={Validity} velocity={VelocityKnots} heading={HeadingDegreesTrue}";
        }



        // Whoops; part of checksum: public string PositioningString { get { return GetPart(10); } } // A=autonomous D-differential E=estimated (dead reckoning) M=manual S=simulator N=no values
    }

    public class Nmea_Date_Fields
    {

        public int DateDay, DateMonth, DateYear;

        public override string ToString()
        {
            return $"{DateYear}-{DateMonth}-{DateDay}";
        }


        public Nmea_Gps_Parser.ParseResult Parse(string DateString)
        {
            if (DateString.Length != 6)
            {
                return Nmea_Gps_Parser.ParseResult.DateStringWrongLength;
            }
            var daystr = DateString.Substring(0, 2);
            var monstr = DateString.Substring(2, 2);
            var yearstr = DateString.Substring(4, 2);
            var parseOk = true;
            parseOk = parseOk && Int32.TryParse(daystr, out DateDay);
            parseOk = parseOk && Int32.TryParse(monstr, out DateMonth);
            parseOk = parseOk && Int32.TryParse(yearstr, out DateYear);
            if (!parseOk)
            {
                return Nmea_Gps_Parser.ParseResult.DateStringInvalid;
            }
            return Nmea_Gps_Parser.ParseResult.Ok;
        }
    }
    public class Nmea_Latitude_Fields
    {
        int LatitudeDegrees, LatitudeMinutesInteger, LatitudeMinutesDecimal;
        public double LatitudeMinutes { get { return LatitudeMinutesInteger + (double)LatitudeMinutesDecimal / 10_000.0; } }
        public enum NorthSouthType { North, South };
        public NorthSouthType LatitudeNorthSouth;

        public override string ToString()
        {
            return $"{LatitudeDegrees}:{LatitudeMinutes} {LatitudeNorthSouth}";
        }

        public Nmea_Gps_Parser.ParseResult Parse(string LatitudeString, string LatitudeNorthSouthString)
        {
            bool parseOk = true;
            if (LatitudeString.Length != 9)
            {
                return Nmea_Gps_Parser.ParseResult.LatitudeStringWrongLength;
            }
            var latstr = LatitudeString.Substring(0, 2);
            var latminstr = LatitudeString.Substring(2, 2);
            var latdotstr = LatitudeString.Substring(4, 1);
            var latdecstr = LatitudeString.Substring(5, 4);
            parseOk = true;
            parseOk = parseOk && Int32.TryParse(latstr, out LatitudeDegrees);
            parseOk = parseOk && Int32.TryParse(latminstr, out LatitudeMinutesInteger);
            parseOk = parseOk && Int32.TryParse(latdecstr, out LatitudeMinutesDecimal);
            parseOk = parseOk && latdotstr == ".";
            if (!parseOk)
            {
                return Nmea_Gps_Parser.ParseResult.LatitudeStringInvalid;
            }

            switch (LatitudeNorthSouthString)
            {
                case "N": LatitudeNorthSouth = NorthSouthType.North; break;
                case "S": LatitudeNorthSouth = NorthSouthType.North; break;
                default:
                    return Nmea_Gps_Parser.ParseResult.LatitudeNorthSouthInvalid;
            }

            if (!parseOk)
            {
                return Nmea_Gps_Parser.ParseResult.LatitudeStringInvalid;
            }
            return Nmea_Gps_Parser.ParseResult.Ok;
        }
    }

    public class Nmea_Longitude_Fields
    {

        public int LongitudeDegrees, LongitudeMinutesInteger, LongitudeMinutesDecimal;
        public double LongitudeMinutes { get { return LongitudeMinutesInteger + (double)LongitudeMinutesDecimal / 10_000.0; } }

        public enum EastWestType { East, West };
        public EastWestType LongitudeEastWest;

        public override string ToString()
        {
            return $"{LongitudeDegrees}.{LongitudeMinutes} {LongitudeEastWest}";
        }

        public Nmea_Gps_Parser.ParseResult Parse(string LongitudeString, string LongitudeEastWestString)
        {
            if (LongitudeString.Length != 10)
            {
                return Nmea_Gps_Parser.ParseResult.LongitudeStringWrongLength;
            }
            var longstr = LongitudeString.Substring(0, 3);
            var longminstr = LongitudeString.Substring(3, 2);
            var longdotstr = LongitudeString.Substring(5, 1);
            var longdecstr = LongitudeString.Substring(6, 4);
            bool parseOk = true;
            parseOk = parseOk && Int32.TryParse(longstr, out LongitudeDegrees);
            parseOk = parseOk && Int32.TryParse(longminstr, out LongitudeMinutesInteger);
            parseOk = parseOk && Int32.TryParse(longdecstr, out LongitudeMinutesDecimal);
            parseOk = parseOk && longdotstr == ".";
            if (!parseOk)
            {
                return Nmea_Gps_Parser.ParseResult.LongitudeStringInvalid;
            }

            switch (LongitudeEastWestString)
            {
                case "E": LongitudeEastWest = EastWestType.East; break;
                case "W": LongitudeEastWest = EastWestType.West; break;
                default:
                    return Nmea_Gps_Parser.ParseResult.LongitudeEastWestInvalid;
            }
            if (!parseOk)
            {
                return Nmea_Gps_Parser.ParseResult.LatitudeStringInvalid;
            }
            return Nmea_Gps_Parser.ParseResult.Ok;

        }
    }

    public class Nmea_Time_Fields
    {
        public int TimeHours, TimeMinutes, SecondsInteger, SecondsDecimal;
        public double TimeSeconds { get { return SecondsInteger + (double)SecondsDecimal / 1000.0; } }

        public override string ToString()
        {
            return $"{TimeHours}:{TimeMinutes}:{TimeSeconds}";
        }

        public Nmea_Gps_Parser.ParseResult Parse(string TimeString)
        {
            bool parseOk;

            if (TimeString.Length != 10)
            {
                return Nmea_Gps_Parser.ParseResult.TimeStringWrongLength;
            }
            var hhstr = TimeString.Substring(0, 2);
            var mmstr = TimeString.Substring(2, 2);
            var ssstr = TimeString.Substring(4, 2);
            var dotstr = TimeString.Substring(6, 1);
            var sssstr = TimeString.Substring(7, 3);
            parseOk = true;
            parseOk = parseOk && Int32.TryParse(hhstr, out TimeHours);
            parseOk = parseOk && Int32.TryParse(mmstr, out TimeMinutes);
            parseOk = parseOk && Int32.TryParse(ssstr, out SecondsInteger);
            parseOk = parseOk && Int32.TryParse(sssstr, out SecondsDecimal);
            parseOk = parseOk && dotstr == ".";
            if (!parseOk)
            {
                return Nmea_Gps_Parser.ParseResult.TimeStringInvalid;
            }
            return Nmea_Gps_Parser.ParseResult.Ok;
        }

    }

    public class Nmea_Data
    {
        public Nmea_Data(string str)
        {
            OriginalNmeaString = str;
            NmeaParts = str.Split(',');
        }

        public override string ToString()
        {
            return $"{OriginalNmeaString}  parse={ParseStatus}";
        }
        public Nmea_Gps_Parser.ParseResult ParseStatus { get; set; } = Nmea_Gps_Parser.ParseResult.OtherError; // default to error.

        public string ChecksumString { get { return GetPart(-1); } }
        public string[] NmeaParts { get; internal set; }
        protected string GetPart(int index)
        {
            if (NmeaParts.Length == 0) return "";
            if (index == -1) return GetPart(NmeaParts.Length - 1);
            if (index >= NmeaParts.Length - 1) return ""; // The only way to get the checksum (last value) is to ask for -1.
            return NmeaParts[index];
        }

        public string OriginalNmeaString { get; set; } = "";
    }


    public class Nmea_Gps_Parser
    { 
        // Useful links:
        // https://learn.sparkfun.com/tutorials/gps-basics/all
        // https://cdn.sparkfun.com/assets/a/3/2/f/a/NMEA_Reference_Manual-Rev2.1-Dec07.pdf
        // https://www.cypress.bc.ca/documents/Report_Messages/CTM200/msg_82_GPRMC.html

        public enum ParseResult {  Ok, NotEnoughFields, 
            TimeStringWrongLength, TimeStringInvalid,
            ValidityInvalid,
            LatitudeStringWrongLength, LatitudeStringInvalid, LatitudeNorthSouthInvalid,
            LongitudeStringWrongLength, LongitudeStringInvalid, LongitudeEastWestInvalid,
            VelocityKnotsInvalid,
            HeadingDegreesTrueInvalid,
            DateStringWrongLength, DateStringInvalid,
            OtherError
        }
        public ParseResult Parse(string Nmea)
        {
            ParseResult retval = ParseResult.OtherError; // always an error unless we know it's OK

            if (Nmea.StartsWith("$GPRMC,"))
            {
                var data = new GPRMC_Data(Nmea);
                if (data.ParseStatus == ParseResult.Ok) OnGprmcOk?.Invoke(this, data);
                else OnGprmcParseError?.Invoke(this, data);
                return data.ParseStatus;
            }

            return retval;
        }

        public event EventHandler<GPRMC_Data> OnGprmcOk;
        public event EventHandler<GPRMC_Data> OnGprmcParseError;



        public static string Example_01 = @"$GPRMC,235316.000,A,4003.9040,N,10512.5792,W,0.09,144.75,141112,,*19
$GPGGA,235317.000,4003.9039,N,10512.5793,W,1,08,1.6,1577.9,M,-20.7,M,,0000*5F
$GPGSA,A,3,22,18,21,06,03,09,24,15,,,,,2.5,1.6,1.9*3E";
    }
}
