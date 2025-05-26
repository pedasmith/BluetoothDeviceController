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

    public class GPGGA_Data : Nmea_Data
    {
        // Potential checks: are all the lengths accurate?

        public GPGGA_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 15)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPGGA")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
            }

            ParseStatus = Time.Parse(TimeString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Latitude.Parse(LatitudeString, LatitudeNorthSouthString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Longitude.Parse(LongitudeString, LongitudeEastWestString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            bool parseOk = true;
            int intval = 0;
            parseOk = parseOk && Int32.TryParse(PositionFixIndicatorString, out intval);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.PositionFixIndicatorInvalid;
                return;
            }
            PositionFixIndicator = (PositionFixIndicatorType)intval;

            parseOk = parseOk && Int32.TryParse(SatellitesUsedString, out SatellitesUsed);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.SatellitesUsedInvalid;
                return;
            }

            parseOk = parseOk && Double.TryParse(HdopString, out Hdop);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.HdopInvalid;
                return;
            }

            parseOk = parseOk && Double.TryParse(MlsAltitudeString, out MlsAltitude);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.MlsAltitudeInvalid;
                return;
            }
            switch (MlsAltitudeUnitsString)
            {
                case "M": MlsAltitudeUnits = AltitudeUnitsType.Meter; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.MlsAltitudeUnitsInvalid;
                    return;
            }

            parseOk = parseOk && Double.TryParse(GeoidSeparationString, out GeoidSeparation);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.GeoidSeparationInvalid;
                return;
            }
            switch (GeoidSeparationUnitsString)
            {
                case "M": GeoidSeparationUnits = AltitudeUnitsType.Meter; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.GeoidSeparationUnitsInvalid;
                    return;
            }
        }

        public string OpcodeString { get { return GetPart(0); } }
        public string TimeString { get { return GetPart(1); } }
        public Nmea_Time_Fields Time = new Nmea_Time_Fields();
        public string LatitudeString { get { return GetPart(2); } } 
        public string LatitudeNorthSouthString { get { return GetPart(3); } } // N S
        public Nmea_Latitude_Fields Latitude = new Nmea_Latitude_Fields();

        public string LongitudeString { get { return GetPart(4); } } // Degrees.MMdd
        public string LongitudeEastWestString { get { return GetPart(5); } } // E W
        public Nmea_Longitude_Fields Longitude = new Nmea_Longitude_Fields();

        public string PositionFixIndicatorString { get { return GetPart(6); } }
        public enum PositionFixIndicatorType { Invalid = 0, GpsSpsMode_Valid = 1, GpsDifferentialMode_Valid = 2, DeadReckoningMode_Valid = 6, }
        public PositionFixIndicatorType PositionFixIndicator;

        public string SatellitesUsedString { get { return GetPart(7); } }
        public int SatellitesUsed;
        public string HdopString { get { return GetPart(8); } }
        public double Hdop;

        public string MlsAltitudeString { get { return GetPart(9); } }
        public double MlsAltitude;
        public string MlsAltitudeUnitsString { get { return GetPart(10); } }
        public enum AltitudeUnitsType { Meter };
        public AltitudeUnitsType MlsAltitudeUnits;
        public string GeoidSeparationString { get { return GetPart(11); } }
        public double GeoidSeparation;
        public string GeoidSeparationUnitsString { get { return GetPart(12); } }
        public AltitudeUnitsType GeoidSeparationUnits;
        public string AgeOfDifferentialCorrection { get { return GetPart(13); } }
        // Also a Differential Reference Station ID that's added to the checksum

        public override string ToString()
        {
            return $"{OpcodeString} {Time} {Latitude} {Longitude} fix indicator={PositionFixIndicator} nsatellites={SatellitesUsed} altitude={MlsAltitude} {MlsAltitudeUnits} separation={GeoidSeparation} {GeoidSeparationUnits}";
        }
    }

    public class GPGLL_Data : Nmea_Data
    {
        public GPGLL_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 6)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPGLL")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
                return;
            }

            ParseStatus = Latitude.Parse(LatitudeString, LatitudeNorthSouthString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Longitude.Parse(LongitudeString, LongitudeEastWestString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Time.Parse(TimeString, Nmea_Time_Fields.ParseOptionsType.hhmmss_sss);
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

            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;
        }

        public string OpcodeString { get { return GetPart(0); } }

        public string LatitudeString { get { return GetPart(1); } } 
        public string LatitudeNorthSouthString { get { return GetPart(2); } } // N S
        public Nmea_Latitude_Fields Latitude = new Nmea_Latitude_Fields();

        public string LongitudeString { get { return GetPart(3); } } // Degrees.MMdd
        public string LongitudeEastWestString { get { return GetPart(4); } } // E W
        public Nmea_Longitude_Fields Longitude = new Nmea_Longitude_Fields();

        public string TimeString { get { return GetPart(5); } }
        public Nmea_Time_Fields Time = new Nmea_Time_Fields();

        public string ValidityString { get { return GetPart(6); } } // A=valid current data B=valid stored data V=invalid current data W=invalid stored data
        public enum ValidityType { ValidCurrentData, ValidStoredData, InvalidCurrentData, InvalidStoredData };
        public ValidityType Validity;
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} {Latitude} {Longitude} {Time}";

            return $"{OpcodeString} {Latitude} {Longitude} {Time}";
        }
    }

    public class GPGSA_Data : Nmea_Data
    {
        // Potential checks: are all the lengths accurate?

        public GPGSA_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 15)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPGSA")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
            }

            ParseStatus = Time.Parse(TimeString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Latitude.Parse(LatitudeString, LatitudeNorthSouthString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Longitude.Parse(LongitudeString, LongitudeEastWestString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            bool parseOk = true;
            int intval = 0;
            parseOk = parseOk && Int32.TryParse(PositionFixIndicatorString, out intval);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.PositionFixIndicatorInvalid;
                return;
            }
            PositionFixIndicator = (PositionFixIndicatorType)intval;

            parseOk = parseOk && Int32.TryParse(SatellitesUsedString, out SatellitesUsed);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.SatellitesUsedInvalid;
                return;
            }

            parseOk = parseOk && Double.TryParse(HdopString, out Hdop);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.HdopInvalid;
                return;
            }

            parseOk = parseOk && Double.TryParse(MlsAltitudeString, out MlsAltitude);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.MlsAltitudeInvalid;
                return;
            }
            switch (MlsAltitudeUnitsString)
            {
                case "M": MlsAltitudeUnits = AltitudeUnitsType.Meter; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.MlsAltitudeUnitsInvalid;
                    return;
            }

            parseOk = parseOk && Double.TryParse(GeoidSeparationString, out GeoidSeparation);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.GeoidSeparationInvalid;
                return;
            }
            switch (GeoidSeparationUnitsString)
            {
                case "M": GeoidSeparationUnits = AltitudeUnitsType.Meter; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.GeoidSeparationUnitsInvalid;
                    return;
            }
        }

        public string OpcodeString { get { return GetPart(0); } }
        public string TimeString { get { return GetPart(1); } }
        public Nmea_Time_Fields Time = new Nmea_Time_Fields();
        public string LatitudeString { get { return GetPart(2); } } 
        public string LatitudeNorthSouthString { get { return GetPart(3); } } // N S
        public Nmea_Latitude_Fields Latitude = new Nmea_Latitude_Fields();

        public string LongitudeString { get { return GetPart(4); } } // Degrees.MMdd
        public string LongitudeEastWestString { get { return GetPart(5); } } // E W
        public Nmea_Longitude_Fields Longitude = new Nmea_Longitude_Fields();

        public string PositionFixIndicatorString { get { return GetPart(6); } }
        public enum PositionFixIndicatorType { Invalid = 0, GpsSpsMode_Valid = 1, GpsDifferentialMode_Valid = 2, DeadReckoningMode_Valid = 6, }
        public PositionFixIndicatorType PositionFixIndicator;

        public string SatellitesUsedString { get { return GetPart(7); } }
        public int SatellitesUsed;
        public string HdopString { get { return GetPart(8); } }
        public double Hdop;

        public string MlsAltitudeString { get { return GetPart(9); } }
        public double MlsAltitude;
        public string MlsAltitudeUnitsString { get { return GetPart(10); } }
        public enum AltitudeUnitsType { Meter };
        public AltitudeUnitsType MlsAltitudeUnits;
        public string GeoidSeparationString { get { return GetPart(11); } }
        public double GeoidSeparation;
        public string GeoidSeparationUnitsString { get { return GetPart(12); } }
        public AltitudeUnitsType GeoidSeparationUnits;
        public string AgeOfDifferentialCorrection { get { return GetPart(13); } }
        // Also a Differential Reference Station ID that's added to the checksum

        public override string ToString()
        {
            return $"{OpcodeString} {Time} {Latitude} {Longitude} fix indicator={PositionFixIndicator} nsatellites={SatellitesUsed} altitude={MlsAltitude} {MlsAltitudeUnits} separation={GeoidSeparation} {GeoidSeparationUnits}";
        }


    }

    public class GPRMC_Data : Nmea_Data
    {
        public GPRMC_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 12) // Really 12?
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPRMC")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
                return;
            }
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

            bool parseOk = true;

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

        public string OpcodeString { get { return GetPart(0); } }
        public string TimeString { get { return GetPart(1); } }
        public Nmea_Time_Fields Time = new Nmea_Time_Fields();
        public string ValidityString { get { return GetPart(2); } } // A=valid current data B=valid stored data V=invalid current data W=invalid stored data
        public enum ValidityType { ValidCurrentData, ValidStoredData, InvalidCurrentData, InvalidStoredData };
        public ValidityType Validity;
        public string LatitudeString { get { return GetPart(3); } } 
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
    }

    public class GPVTG_Data : Nmea_Data
    {
        public GPVTG_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 10)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPVTG")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
                return;
            }

            bool parseOk = true;

            if (CourseTrueString != "")
            {
                parseOk = parseOk && double.TryParse(CourseTrueString, out CourseTrue);
                if (!parseOk)
                {
                    ParseStatus = Nmea_Gps_Parser.ParseResult.CourseTrueInvalid;
                    return;
                }
            }
            switch (CourseTrueReferenceUnitsString)
            {
                case "T": CourseTrueReferenceUnits = CourseReferenceType.True; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.CourseTrueUnitsInvalid;
                    return;
            }

            if (CourseMagneticString != "")
            {
                parseOk = parseOk && double.TryParse(CourseMagneticString, out CourseMagnetic);
                if (!parseOk)
                {
                    ParseStatus = Nmea_Gps_Parser.ParseResult.CourseMagneticInvalid;
                    return;
                }
            }
            switch (CourseMagneticReferenceUnitsString)
            {
                case "M": CourseMagneticReferenceUnits = CourseReferenceType.Magnetic; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.CourseMagneticUnitsInvalid;
                    return;
            }

            parseOk = parseOk && double.TryParse(SpeedKnotsString, out SpeedKnots);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.VelocityKnotsInvalid;
                return;
            }
            switch (SpeedKnotsUnitsString)
            {
                case "N": SpeedKnotsUnits = SpeedUnitsType.Knots; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.CourseTrueUnitsInvalid;
                    return;
            }


            parseOk = parseOk && double.TryParse(SpeedKphString, out SpeedKph);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.VelocityKphInvalid;
                return;
            }
            switch (SpeedKphUnitsString)
            {
                case "K": SpeedKphUnits = SpeedUnitsType.Kph; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.VelocityKphUnitsInvalid;
                    return;
            }

            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;
        }

        public string OpcodeString { get { return GetPart(0); } }
        public string CourseTrueString { get { return GetPart(1); } }
        public double CourseTrue = -1;
        public string CourseTrueReferenceUnitsString { get { return GetPart(2); } }
        public enum CourseReferenceType { True, Magnetic };
        public CourseReferenceType CourseTrueReferenceUnits;
        public string CourseMagneticString { get { return GetPart(3); } }
        public double CourseMagnetic = -1;
        public string CourseMagneticReferenceUnitsString { get { return GetPart(4); } }
        public CourseReferenceType CourseMagneticReferenceUnits;
        public string SpeedKnotsString { get { return GetPart(5); } }
        double SpeedKnots;
        public string SpeedKnotsUnitsString { get { return GetPart(6); } }
        public enum SpeedUnitsType {  Knots, Kph };
        public SpeedUnitsType SpeedKnotsUnits;
        public string SpeedKphString { get { return GetPart(7); } }
        public double SpeedKph;
        public string SpeedKphUnitsString { get { return GetPart(8); } }
        public SpeedUnitsType SpeedKphUnits;


        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} Course={CourseTrue} ({CourseTrueReferenceUnits}) AKA {CourseMagnetic} ({CourseMagneticReferenceUnits}) velocity={SpeedKnots} {SpeedKnotsUnits} AKA {SpeedKph} {SpeedKphUnits}";

            return $"{OpcodeString} Course={CourseTrue} ({CourseTrueReferenceUnits}) AKA {CourseMagnetic} ({CourseMagneticReferenceUnits}) velocity={SpeedKnots} {SpeedKnotsUnits} AKA {SpeedKph} {SpeedKphUnits}";
        }
    }

    public class GPZDA_Data : Nmea_Data
    {
        public GPZDA_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 6)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPZDA")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
                return;
            }

            ParseStatus = Time.Parse(TimeString, Nmea_Time_Fields.ParseOptionsType.hhmmss_sss);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Date.Parse(DayString, MonthString, YearString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            bool parseOk = true;

            parseOk = parseOk && Int32.TryParse(LocalZoneHourString, out LocalZoneHour);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.SatellitesUsedInvalid;
                return;
            }


            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;
        }

        public string OpcodeString { get { return GetPart(0); } }
        public string TimeString { get { return GetPart(1); } }
        public Nmea_Time_Fields Time = new Nmea_Time_Fields();

        public string DayString { get { return GetPart(2); } }
        public string MonthString { get { return GetPart(3); } }
        public string YearString { get { return GetPart(4); } }
        Nmea_Date_Fields Date = new Nmea_Date_Fields();

        public string LocalZoneHourString {  get { return GetPart(5); } }
        public int LocalZoneHour;
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} {Time} {Date} Time zone={LocalZoneHour}";

            return $"{OpcodeString} {Time} {Date} Time zone={LocalZoneHour}";
        }
    }


    public class Nmea_Date_Fields
    {

        public int DateDay, DateMonth, DateYear;

        public override string ToString()
        {
            return $"{DateYear}-{DateMonth:D2}-{DateDay:D2}";
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

        public Nmea_Gps_Parser.ParseResult Parse(string DayString, string MonthString, String YearString)
        {
            if (DayString.Length != 2 || MonthString.Length != 2 || YearString.Length != 4)
            {
                return Nmea_Gps_Parser.ParseResult.DateStringWrongLength;
            }
            var parseOk = true;
            parseOk = parseOk && Int32.TryParse(DayString, out DateDay);
            parseOk = parseOk && Int32.TryParse(MonthString, out DateMonth);
            parseOk = parseOk && Int32.TryParse(YearString, out DateYear);
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
            return $"{LatitudeDegrees}° {LatitudeMinutes} {LatitudeNorthSouth}";
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
            return $"{LongitudeDegrees}° {LongitudeMinutes} {LongitudeEastWest}";
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
            return $"{TimeHours:D2}:{TimeMinutes:D2}:{TimeSeconds}";
        }
        public enum ParseOptionsType {  hhmmss_sss, hhmmss }
        public Nmea_Gps_Parser.ParseResult Parse(string TimeString, ParseOptionsType options=ParseOptionsType.hhmmss_sss)
        {
            bool parseOk;

            if (TimeString.Length != 10)
            {
                return Nmea_Gps_Parser.ParseResult.TimeStringWrongLength;
            }
            var hhstr = TimeString.Substring(0, 2);
            var mmstr = TimeString.Substring(2, 2);
            var ssstr = TimeString.Substring(4, 2);
            string dotstr="";
            string sssstr="";
            if (options.HasFlag(ParseOptionsType.hhmmss_sss))
            {
                dotstr = TimeString.Substring(6, 1);
                sssstr = TimeString.Substring(7, 3);
            }
            parseOk = true;
            parseOk = parseOk && Int32.TryParse(hhstr, out TimeHours);
            parseOk = parseOk && Int32.TryParse(mmstr, out TimeMinutes);
            parseOk = parseOk && Int32.TryParse(ssstr, out SecondsInteger);
            if (options.HasFlag(ParseOptionsType.hhmmss_sss))
            {
                parseOk = parseOk && Int32.TryParse(sssstr, out SecondsDecimal);
                parseOk = parseOk && dotstr == ".";
            }
            else
            {
                SecondsDecimal = 0; // is e.g. hhmmss and there's no fractional seconds.
            }
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
            ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeUnknown; // nice default :-)
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


}
