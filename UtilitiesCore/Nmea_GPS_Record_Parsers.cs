/// This file is classes that can parse an NMEA format string into classes. The classes must be given
/// the correct kind of string (e.g., the GPGGA_Data class must be given an NMEA line that starts with $GPGGA).
/// The overall parser is in NMEA_GPS_Parser.cs

using System;
using System.Collections.Generic;


// Why is the weird #if here? Because FindBitPattern tries to be more nullable enabled.
// Need this super weird set of ifs because:
// 1. many of my projects are "old" and give errors when #nullable disable is present
// 2. I can't use the ? for the events because old projects don't do that
// 3. But for FindBitPattern I need to suppress the warning just for this file.

#if NET8_0_OR_GREATER
#nullable disable
#endif


// Parser information
// SiRF NMEA Reference Manual, January 2005 revision 1.3 www.SIRF.com
namespace Parsers.Nmea
{

    /// <summary>
    /// Advanced position; includes lat/long/time/altitude/satellites and more
    /// </summary>
    public class GPGGA_Data : Nmea_Data
    {
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
                return;
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

        public enum PositionFixIndicatorType { Invalid = 0, GpsSpsMode_Valid = 1, GpsDifferentialMode_Valid = 2, DeadReckoningMode_Valid = 6, }
        public static string EnumToString(PositionFixIndicatorType value)
        {
            switch (value)
            {
                case PositionFixIndicatorType.Invalid: return "No fix";
                case PositionFixIndicatorType.GpsSpsMode_Valid: return "GPS mode valid";
                case PositionFixIndicatorType.GpsDifferentialMode_Valid: return "Differential mode valid";
                case PositionFixIndicatorType.DeadReckoningMode_Valid: return "Dead reckoning mode valid";
            }
            return value.ToString();
        }
        public string PositionFixIndicatorString { get { return GetPart(6); } }
        public PositionFixIndicatorType PositionFixIndicator;

        public string SatellitesUsedString { get { return GetPart(7); } }
        public int SatellitesUsed;
        public string HdopString { get { return GetPart(8); } }
        public double Hdop;

        public string MlsAltitudeString { get { return GetPart(9); } }
        public double MlsAltitude;
        public enum AltitudeUnitsType { Meter };
        public string MlsAltitudeUnitsString { get { return GetPart(10); } }
        public AltitudeUnitsType MlsAltitudeUnits;
        public string GeoidSeparationString { get { return GetPart(11); } }
        public double GeoidSeparation;
        public string GeoidSeparationUnitsString { get { return GetPart(12); } }
        public AltitudeUnitsType GeoidSeparationUnits;
        public string AgeOfDifferentialCorrectionString { get { return GetPart(13); } }
        public string DifferentialReferenceStationsIdString { get { return LastElement; } } // is the 0000 in the 0000*5E checksum

        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"{Latitude} {Longitude} {Time} {EnumToString(PositionFixIndicator)}";
                return retval;
            }
        }
        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                return $"{Latitude} {Longitude}\nTime: {Time}\nFix type: {EnumToString(PositionFixIndicator)}\nN. Satellites: {SatellitesUsed}\nHDOP: {Hdop}\nMLS Altitude: {MlsAltitude} {MlsAltitudeUnits}\nGeoid Separation: {GeoidSeparation} {GeoidSeparationUnits}\nAge differential corretion: {AgeOfDifferentialCorrectionString}\nDifferential Reference Station: {DifferentialReferenceStationsIdString}";
            }
        }

        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} {Time} {Latitude} {Longitude} fix indicator={PositionFixIndicator} nsatellites={SatellitesUsed} altitude={MlsAltitude} {MlsAltitudeUnits} separation={GeoidSeparation} {GeoidSeparationUnits} station={DifferentialReferenceStationsIdString}";
            }
            return $"{OpcodeString} {Time} {Latitude} {Longitude} fix indicator={PositionFixIndicator} nsatellites={SatellitesUsed} altitude={MlsAltitude} {MlsAltitudeUnits} separation={GeoidSeparation} {GeoidSeparationUnits} station={DifferentialReferenceStationsIdString}";
        }
    }


    /// <summary>
    /// Simple position: include lat/long/time
    /// </summary>
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

            ParseStatus = Mode.Parse(ModeString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

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

        public enum ValidityType { ValidCurrentData, ValidStoredData, InvalidCurrentData, InvalidStoredData };
        public static string EnumToString(ValidityType value)
        {
            switch (value)
            {
                case ValidityType.ValidCurrentData: return "Valid (current data)";
                case ValidityType.ValidStoredData: return "Valid (stored data)";
                case ValidityType.InvalidCurrentData: return "Invalid (current data)";
                case ValidityType.InvalidStoredData: return "Invalid (stored data)";
            }
            return value.ToString();
        }
        public string ValidityString { get { return GetPart(6); } } // A=valid current data B=valid stored data V=invalid current data W=invalid stored data
        public ValidityType Validity;

        /// <summary>
        /// Mode is the type of fix e.g. dead reckoning and more
        /// </summary>
        public string ModeString {  get { return LastElement; } }
        public Nmea_Mode_Field Mode = new Nmea_Mode_Field();


        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"{Latitude} {Longitude} {Time} {Mode}";
                retval += $" {EnumToString(Validity)}";
                return retval;
            }
        }
        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                return $"{Latitude} {Longitude}\nTime: {Time}\nStatus={EnumToString(Validity)}\nMode={Mode}";
            }
        }

        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} {Latitude} {Longitude} {Time} {Mode}";
            }
            return $"{OpcodeString} {Latitude} {Longitude} {Time} {Mode}";
        }
    }

    /// <summary>
    /// Detailed satellite information
    /// </summary>
    public class GPGSA_Data : Nmea_Data
    {
        public GPGSA_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 17)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPGSA")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
                return;
            }

            switch (Mode1String)
            {
                case "A": Mode1 = Mode1Type.Automatic; break;
                case "M": Mode1 = Mode1Type.Manual; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.Mode1Invalid;
                    return;
            }

            bool parseOk = true;
            int intval = 0;
            parseOk = parseOk && Int32.TryParse(Mode2String, out intval);
            if (!parseOk || intval < 1 || intval >3)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.Mode2Invalid;
                return;
            }
            Mode2 = (Mode2Type)intval;

            parseOk = parseOk && Double.TryParse(PdopString, out Pdop);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.PdopInvalid;
                return;
            }

            parseOk = parseOk && Double.TryParse(HdopString, out Hdop);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.HdopInvalid;
                return;
            }

            parseOk = parseOk && Double.TryParse(VdopString, out Vdop);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.VdopInvalid;
                return;
            }

            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;

        }

        public string OpcodeString { get { return GetPart(0); } }

        public enum Mode1Type {  Manual, Automatic};
        public string Mode1String { get { return GetPart(1); } }
        public Mode1Type Mode1;
        public enum Mode2Type {  FixNotAvailable=1, Is2D=2, Is3D=3}
        public static string EnumToString(Mode2Type value)
        {
            switch (value)
            {
                case Mode2Type.FixNotAvailable: return "Fix not available";
                case Mode2Type.Is2D: return "2D";
                case Mode2Type.Is3D: return "3D";
            }
            return value.ToString();
        }
        public string Mode2String { get { return GetPart(2); } }
        public Mode2Type Mode2;

        public string SatelliteUsed01 { get { return GetPart(3); } }
        public string SatelliteUsed02 { get { return GetPart(4); } }
        public string SatelliteUsed03 { get { return GetPart(5); } }
        public string SatelliteUsed04 { get { return GetPart(6); } }
        public string SatelliteUsed05 { get { return GetPart(7); } }
        public string SatelliteUsed06 { get { return GetPart(8); } }
        public string SatelliteUsed07 { get { return GetPart(9); } }
        public string SatelliteUsed08 { get { return GetPart(10); } }
        public string SatelliteUsed09 { get { return GetPart(11); } }
        public string SatelliteUsed10 { get { return GetPart(12); } }
        public string SatelliteUsed11 { get { return GetPart(13); } }
        public string SatelliteUsed12 { get { return GetPart(14); } }

        public int NSatelliteUsed
        {
            get
            {
                int retval = 0;
                retval += string.IsNullOrEmpty(SatelliteUsed01) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed02) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed03) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed04) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed05) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed06) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed07) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed08) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed09) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed10) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed11) ? 0 : 1;
                retval += string.IsNullOrEmpty(SatelliteUsed12) ? 0 : 1;
                return retval;
            }
        }
       
        public string PdopString { get { return GetPart(15); } }
        /// <summary>
        /// Position Dilution of Precision
        /// </summary>
        public double Pdop;
        public string HdopString { get { return GetPart(16); } }
        /// <summary>
        /// Horizontal Dilution of Precision
        /// </summary>
        public double Hdop;
        public string VdopString {  get { return LastElement; } }
        /// <summary>
        /// Vertical Dilution of Precision
        /// </summary>
        public double Vdop;

        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"N. Satellites: {NSatelliteUsed} {Mode1} {EnumToString(Mode2)}";
                return retval;
            }
        }
        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = "";
                retval += string.IsNullOrEmpty(SatelliteUsed01) ? "" : $"\nSatellite 01: {SatelliteUsed01}";
                retval += string.IsNullOrEmpty(SatelliteUsed02) ? "" : $"\nSatellite 02: {SatelliteUsed02}";
                retval += string.IsNullOrEmpty(SatelliteUsed03) ? "" : $"\nSatellite 03: {SatelliteUsed03}";
                retval += string.IsNullOrEmpty(SatelliteUsed04) ? "" : $"\nSatellite 04: {SatelliteUsed04}";
                retval += string.IsNullOrEmpty(SatelliteUsed05) ? "" : $"\nSatellite 05: {SatelliteUsed05}";
                retval += string.IsNullOrEmpty(SatelliteUsed06) ? "" : $"\nSatellite 06: {SatelliteUsed06}";
                retval += string.IsNullOrEmpty(SatelliteUsed07) ? "" : $"\nSatellite 07: {SatelliteUsed07}";
                retval += string.IsNullOrEmpty(SatelliteUsed08) ? "" : $"\nSatellite 08: {SatelliteUsed08}";
                retval += string.IsNullOrEmpty(SatelliteUsed09) ? "" : $"\nSatellite 09: {SatelliteUsed09}";
                retval += string.IsNullOrEmpty(SatelliteUsed10) ? "" : $"\nSatellite 10: {SatelliteUsed10}";
                retval += string.IsNullOrEmpty(SatelliteUsed11) ? "" : $"\nSatellite 11: {SatelliteUsed11}";
                retval += string.IsNullOrEmpty(SatelliteUsed12) ? "" : $"\nSatellite 12: {SatelliteUsed12}";
                retval += $"\nPosition DOP: {PdopString}\nHoriontal DOP: {HdopString}\nVertical DOP: {VdopString}";
                return retval;
            }
        }
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} {Mode1} {EnumToString(Mode2)} Satellites={SatelliteUsed01},{SatelliteUsed02},{SatelliteUsed03},{SatelliteUsed04},{SatelliteUsed05},{SatelliteUsed06},{SatelliteUsed07},{SatelliteUsed08},{SatelliteUsed09},{SatelliteUsed10},{SatelliteUsed11},{SatelliteUsed12}  PDOP={Pdop} HDOP={Hdop} VDOP={Vdop}";
            }
            return $"{OpcodeString} {Mode1} {EnumToString(Mode2)} Satellites={SatelliteUsed01},{SatelliteUsed02},{SatelliteUsed03},{SatelliteUsed04},{SatelliteUsed05},{SatelliteUsed06},{SatelliteUsed07},{SatelliteUsed08},{SatelliteUsed09},{SatelliteUsed10},{SatelliteUsed11},{SatelliteUsed12}  PDOP={Pdop} HDOP={Hdop} VDOP={Vdop}";
        }
    }

    public class GPGSV_Satellite_Data
    {
        public Nmea_Gps_Parser.ParseResult Init (Nmea_Data data, int starting_index)
        {
            Id = data.GetPart(starting_index + 0);
            ElevationString = data.GetPart(starting_index + 1);
            AzimuthString = data.GetPart(starting_index + 2);
            SignalToNoiseRatioString = data.GetPart(starting_index + 3);

            bool parseOk = true;
            int value = 0;

            parseOk = parseOk && Int32.TryParse(ElevationString, out value);
            if (!parseOk) return Nmea_Gps_Parser.ParseResult.ElevationInvalid;
            Elevation = value;

            parseOk = parseOk && Int32.TryParse(AzimuthString, out value);
            if (!parseOk) return Nmea_Gps_Parser.ParseResult.AzimuthInvalid;
            Azimuth = value;

            parseOk = parseOk && Int32.TryParse(SignalToNoiseRatioString, out value);
            if (!parseOk) return Nmea_Gps_Parser.ParseResult.SignalToNoiseRationInvalid;
            SignalToNoiseRatio = value;

            return Nmea_Gps_Parser.ParseResult.Ok;
        }
        public string Id { get; internal set; }
        public string ElevationString { get; internal set; }
        public int Elevation { get; internal set; }
        public string AzimuthString { get; internal set; }
        public int Azimuth {  get; internal set; }
        public string SignalToNoiseRatioString { get; internal set; }
        public int SignalToNoiseRatio { get; internal set; }

        public string SatelliteDetailString
        {
            get
            {
                return $"{Id:D3} {Elevation:D3} {Azimuth:D3} {SignalToNoiseRatio:D3}";
            }
        }

        public override string ToString()
        {
            return $"{Id:D3} {Elevation:D3} {Azimuth:D3} {SignalToNoiseRatio:D3}";
        }

    }
    /// <summary>
    /// In progress! TODO:
    /// </summary>
    public class GPGSV_Data : Nmea_Data
    {
        public GPGSV_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 3)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPGSV")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
                return;
            }

            bool parseOk = true;
            int value = 0;
            parseOk = parseOk && Int32.TryParse(NMessageString, out value);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NMessageInvalid;
                return;
            }
            NMessage = value;

            parseOk = parseOk && Int32.TryParse(MessageIndexString, out value);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.MessageIndexInvalid;
                return;
            }
            MessageIndex = value;

            parseOk = parseOk && Int32.TryParse(NSatelliteInViewString, out value);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NSatelliteInViewInvalid;
                return;
            }
            NSatelliteInView = value;

            Data.Clear();
            Data.Add(new GPGSV_Satellite_Data());
            Data.Add(new GPGSV_Satellite_Data());
            Data.Add(new GPGSV_Satellite_Data());
            Data.Add(new GPGSV_Satellite_Data());

            ParseStatus = Data[0].Init(this, 4);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Data[1].Init(this, 8);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Data[2].Init(this, 12);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Data[3].Init(this, 16);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;
        }



        public string OpcodeString { get { return GetPart(0); } }

        public string NMessageString { get { return GetPart(1); } }
        public int NMessage { get; internal set; }

        public string MessageIndexString {  get {  return GetPart(2); } }
        public int MessageIndex { get; internal set; }  
        public string NSatelliteInViewString {  get { return GetPart(3); } }
        public int NSatelliteInView { get; internal set; }

        public List<GPGSV_Satellite_Data> Data = new List<GPGSV_Satellite_Data>();

        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"Message {MessageIndex} of {NMessage} NSatelliteInView={NSatelliteInView} {Data[0].Id} is {Data[0].SatelliteDetailString}";
                return retval;
            }
        }
        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"NMessages: {NMessage}\nMessage Index: {MessageIndex}\nSatellites in view: {NSatelliteInView}\n";
                retval += Data[0].SatelliteDetailString + "\n";
                retval += Data[1].SatelliteDetailString + "\n";
                retval += Data[2].SatelliteDetailString + "\n";
                retval += Data[3].SatelliteDetailString + "\n";
                return retval;
            }
        }
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} Message {MessageIndex} of {NMessage} NSatelliteInView={NSatelliteInView}";
            }
            return $"{OpcodeString} Message {MessageIndex} of {NMessage} NSatelliteInView={NSatelliteInView}";
        }
    }

    /// <summary>
    /// Power info. Includes charging status and voltage. There are no definitive descriptions of GPGSV
    /// </summary>

    public class GPPWR_Data : Nmea_Data
    {
        // See: https://github.com/Knio/pynmea2/issues/56
        // It looks like PWR is a proprietary addition to the XGPS160 (and other models from Dual? Other brands? No idea.).
        // Their docs say:
        // The PPWR sentence contains device specific information and looks like this:
        // $GPPWR,0876,0,0,0,0,00,F,0,97,1,3,000,00190108EEEE,0017E9B92122*74
        // • Element #1, 0876, is the battery voltage. Battery voltage is not valid while the device is charging.
        // • Element #5 is the charging status: 1 = charging, 0 = not charging.

        //That comes from the SDK documentation which isn't on their site, we had to request it.

        public GPPWR_Data(string str)
            : base(str)
        {
            if (NmeaParts.Length < 3)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.NotEnoughFields;
                return;
            }
            if (OpcodeString != "$GPPWR")
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.OpcodeIncorrect;
                return;
            }

            bool parseOk = true;

            // See https://github.com/platypii/BASElineFlightComputer/blob/96471257f83c238af16681f118aa80cc4f053d71/common/src/main/java/com/platypii/baseline/location/NMEA.java#L16
            // See https://github.com/dualav/XGPS160-SDK-iOS/blob/f4786798000c6efae72638f4826de031d0ece0a4/XGPS160API.m#L4
            {
                int hexval = -1;
                parseOk = parseOk && Int32.TryParse(VoltageString, System.Globalization.NumberStyles.HexNumber, null, out hexval);
                if (parseOk)
                {
                    // AFAICT: these values are for one specific device where the person knows the
                    // upper and lower voltage bounds.
                    //const int MinVal = 1091;
                    //const int MaxVal = 1280;
                    //Voltage = 100.0 * (hexval - MinVal) / (MaxVal - MinVal); // TODO: Voltage is really BatteryChargePercent
                    Voltage = (double)hexval / 100.0; // just voltage?
                }
                else
                {
                    ParseStatus = Nmea_Gps_Parser.ParseResult.VoltageInvalid;
                }
            }

            //parseOk = parseOk && double.TryParse(VoltageString, out Voltage);
            //if (!parseOk)
            //{
            //    ParseStatus = Nmea_Gps_Parser.ParseResult.VoltageInvalid;
            //    return;
            //}
            //Voltage = Voltage / 100.0; // Seems logical?

            switch (ChargingStatusString)
            {
                case "0": ChargingStatus = ChargingStatusType.NotCharging; break;
                case "1": ChargingStatus = ChargingStatusType.Charging; break;
                default:
                    ParseStatus = Nmea_Gps_Parser.ParseResult.ChargingStatusInvalid;
                    return;
            }

            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;
        }

        public string OpcodeString { get { return GetPart(0); } }
        public string VoltageString {  get {  return GetPart(1); } }
        public double Voltage;
        public enum ChargingStatusType {  Charging, NotCharging };
        public static string EnumToString(ChargingStatusType value)
        {
            switch (value)
            {
                case ChargingStatusType.Charging: return "Charging";
                case ChargingStatusType.NotCharging: return "Not charging";
            }
            return value.ToString();
        }
        public string ChargingStatusString { get { return GetPart(5); } }
        public ChargingStatusType ChargingStatus;

        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"Battery {Voltage} volts {EnumToString(ChargingStatus)}";
                return retval;
            }
        }
        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"Voltage: {Voltage} volts\nCharging status: {EnumToString(ChargingStatus)}";
                return retval;
            }
        }
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} volts={Voltage} {ChargingStatus}";
            }
            return $"{OpcodeString} volts={Voltage} {ChargingStatus}";
        }
    }

    /// <summary>
    /// Position fix includes lat/long/time/date and velocity
    /// </summary>
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

            if (MagneticVariationString != "")
            {
                parseOk = parseOk && Double.TryParse(MagneticVariationString, out MagneticVariation);
                if (!parseOk)
                {
                    ParseStatus = Nmea_Gps_Parser.ParseResult.MagneticVariationInvalid;
                    return;
                }
            }

            if (EastWestIndicatorString != "")
            {
                switch (EastWestIndicatorString)
                {
                    case "E": EastWestIndicator = EastWestType.East; break;
                    case "W": EastWestIndicator = EastWestType.West; break;
                    default:
                        ParseStatus = Nmea_Gps_Parser.ParseResult.EastWestIndicatorInvalid;
                        return;
                }
            }

            ParseStatus = Mode.Parse(ModeString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;
        }

        public string OpcodeString { get { return GetPart(0); } }
        public string TimeString { get { return GetPart(1); } }
        public Nmea_Time_Fields Time = new Nmea_Time_Fields();
        public enum ValidityType { ValidCurrentData, ValidStoredData, InvalidCurrentData, InvalidStoredData };
        public static string EnumToString(ValidityType value)
        {
            switch (value)
            {
                case ValidityType.ValidCurrentData: return "Valid (current data)";
                case ValidityType.ValidStoredData: return "Valid (stored data)";
                case ValidityType.InvalidCurrentData: return "Invalid (current data)";
                case ValidityType.InvalidStoredData: return "Invalid (stored data)";
            }
            return value.ToString();
        }
        public string ValidityString { get { return GetPart(2); } } // A=valid current data B=valid stored data V=invalid current data W=invalid stored data
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

        public string MagneticVariationString {  get { return GetPart(10); } }
        public double MagneticVariation;

        public enum EastWestType {  NotSpecified, East, West };
        public static string EnumToString(EastWestType value)
        {
            switch (value)
            {
                case EastWestType.East: return "East";
                case EastWestType.West: return "West";
                case EastWestType.NotSpecified: return "";
            }
            return value.ToString();
        }
        public string EastWestIndicatorString { get { return GetPart(11); } }
        public EastWestType EastWestIndicator;

        public string ModeString { get { return LastElement; } }
        public Nmea_Mode_Field Mode = new Nmea_Mode_Field();

        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"{Latitude} {Longitude} At {Date} {Time} Validity={EnumToString(Validity)}";
                return retval;
            }
        }

        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                var retval = $"{Latitude} {Longitude}\nTime: {Time}\nDate: {Date.ToString()}\nValidity: {EnumToString(Validity)}\n";
                if (VelocityKnotsString != "") retval += $"Velocity: {VelocityKnots} knots\n";
                if (HeadingDegreesTrueString != "") retval += $"Heading: {HeadingDegreesTrue}° (true)\n";
                if (MagneticVariationString != "") retval += $"Magnetic variation: {MagneticVariation}° {EnumToString(EastWestIndicator)}\n";
                retval += $"Navigation mode: {Mode}";
                return retval;
            }
        }
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} {Time} {Latitude} {Longitude} {Date} validity={Validity} velocity={VelocityKnots} heading={HeadingDegreesTrue} variation={MagneticVariation} {EastWestIndicator} mode={Mode}";
            }
            return $"{OpcodeString} {Time} {Latitude} {Longitude} {Date} validity={Validity} velocity={VelocityKnots} heading={HeadingDegreesTrue} variation={MagneticVariation} {EastWestIndicator} mode={Mode}";
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

            ParseStatus = Mode.Parse(ModeString);
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return;

            ParseStatus = Nmea_Gps_Parser.ParseResult.Ok;
        }

        public string OpcodeString { get { return GetPart(0); } }
        public string CourseTrueString { get { return GetPart(1); } }
        public double CourseTrue = -1;
        public enum CourseReferenceType { True, Magnetic };
        public string CourseTrueReferenceUnitsString { get { return GetPart(2); } }
        public CourseReferenceType CourseTrueReferenceUnits;
        public string CourseMagneticString { get { return GetPart(3); } }
        public double CourseMagnetic = -1;
        public string CourseMagneticReferenceUnitsString { get { return GetPart(4); } }
        public CourseReferenceType CourseMagneticReferenceUnits;
        public string SpeedKnotsString { get { return GetPart(5); } }
        public double SpeedKnots;
        public enum SpeedUnitsType {  Knots, Kph };
        public string SpeedKnotsUnitsString { get { return GetPart(6); } }
        public SpeedUnitsType SpeedKnotsUnits;
        public string SpeedKphString { get { return GetPart(7); } }
        public double SpeedKph;
        public string SpeedKphUnitsString { get { return GetPart(8); } }
        public SpeedUnitsType SpeedKphUnits;

        public string ModeString { get { return LastElement; } } 
        public Nmea_Mode_Field Mode = new Nmea_Mode_Field();

        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                string retval = "";
                if (CourseTrueString != "") retval += $"Course={CourseTrue} (true) ";
                if (CourseMagneticString != "") retval += $"Course={CourseMagnetic} (magnetic) ";
                if (SpeedKnotsString != "") retval += $"Speed={SpeedKnots} knots ";
                if (SpeedKphString != "") retval += $"Speed={SpeedKph} kph ";
                retval += $"Mode={Mode}";
                return retval;
            }
        }
        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                string retval = "";
                if (CourseTrueString != "") retval += $"Course: {CourseTrue} (true)\n";
                if (CourseMagneticString != "") retval += $"Course: {CourseMagnetic} (magnetic)\n";
                if (SpeedKnotsString != "") retval += $"Speed: {SpeedKnots} knots\n";
                if (SpeedKphString != "") retval += $"Speed: {SpeedKph} kph\n";
                retval += $"Mode: {Mode}";
                return retval;
            }
        }
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} Course={CourseTrue} ({CourseTrueReferenceUnits}) AKA {CourseMagnetic} ({CourseMagneticReferenceUnits}) velocity={SpeedKnots} {SpeedKnotsUnits} AKA {SpeedKph} {SpeedKphUnits} mode={Mode}";
            }
            return $"{OpcodeString} Course={CourseTrue} ({CourseTrueReferenceUnits}) AKA {CourseMagnetic} ({CourseMagneticReferenceUnits}) velocity={SpeedKnots} {SpeedKnotsUnits} AKA {SpeedKph} {SpeedKphUnits} mode={Mode}";
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

            parseOk = parseOk && Int32.TryParse(LocalZoneHoursString, out LocalZoneHours);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.LocalZoneHourInvalid;
                return;
            }

            parseOk = parseOk && Int32.TryParse(LocalZoneMinutesString, out LocalZoneMinutes);
            if (!parseOk)
            {
                ParseStatus = Nmea_Gps_Parser.ParseResult.LocalZoneMinutesInvalid;
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

        public string LocalZoneHoursString {  get { return GetPart(5); } }
        public int LocalZoneHours;

        public string LocalZoneMinutesString {  get { return LastElement; } }
        public int LocalZoneMinutes;

        public override string SummaryString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                string retval = $"{Date} {Time} Time zone={LocalZoneHours:D2}:{LocalZoneMinutes:D2}";
                return retval;
            }
        }
        public override string DetailString
        {
            get
            {
                if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok) return ParseErrorString;
                string retval = $"{Date} {Time}\nTime zone: {LocalZoneHours:D2}:{LocalZoneMinutes:D2}";
                return retval;
            }
        }
        public override string ToString()
        {
            if (ParseStatus != Nmea_Gps_Parser.ParseResult.Ok)
            {
                return $"{OpcodeString} {ParseStatus} {OriginalNmeaString} {Time} {Date} Time zone={LocalZoneHours}:{LocalZoneMinutes:D2}";
            }
            return $"{OpcodeString} {Time} {Date} Time zone={LocalZoneHours:D2}:{LocalZoneMinutes:D2}";
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
            return $"{LatitudeDegrees}° {LatitudeMinutes:F4} {LatitudeNorthSouth}";
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
            return $"{LongitudeDegrees}° {LongitudeMinutes:F4} {LongitudeEastWest}";
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

    /// <summary>
    /// Type of fix; include manual/simulator/estimated dead reckoning and more
    /// </summary>
    public class Nmea_Mode_Field
    {
        public enum ModeType { NotSpecified, Autonomous, DifferentialDGP, EstimatedDeadReckoning, Manual, Simulator, NotValid }
        public static string EnumToString(ModeType value)
        {
            switch (value)
            {
                case ModeType.NotSpecified: return "Not specified";
                case ModeType.Autonomous: return "Autonomous";
                case ModeType.DifferentialDGP: return "Differential DGP";
                case ModeType.EstimatedDeadReckoning: return "Estimated (dead reckoning)";
                case ModeType.Manual: return "Manual";
                case ModeType.Simulator: return "Simulator";
                case ModeType.NotValid: return "Not valid";
            }
            return value.ToString();
        }
        public ModeType Mode;

        public Nmea_Gps_Parser.ParseResult Parse(string ModeString)
        {
            // From https://www.cypress.bc.ca/documents/Report_Messages/CTM200/msg_82_GPRMC.html
            // e is the positioning system mode indicator(NMEA 0183 v3.0 only): A = autonomous mode, D = Differential mode, E = Estimated(dead reckoning) mode, M = Manual input mode, S = Simulator mode, N = Data not valid. Note: The CTM internal GPS module supports autonomous mode only.
            switch (ModeString)
            {
                case "": Mode = ModeType.NotSpecified; break;
                case "A": Mode = ModeType.Autonomous; break;
                case "D": Mode = ModeType.DifferentialDGP; break;
                case "E": Mode = ModeType.EstimatedDeadReckoning; break;
                case "M": Mode = ModeType.Manual; break;
                case "N": Mode = ModeType.NotValid; break;
                case "S": Mode = ModeType.Simulator; break;
                default:
                    return Nmea_Gps_Parser.ParseResult.ModeInvalid;
            }
            return Nmea_Gps_Parser.ParseResult.Ok;
        }
        public override string ToString()
        {
            return EnumToString(Mode);
        }
    }

    public class Nmea_Time_Fields
    {
        public int TimeHours, TimeMinutes, SecondsInteger, SecondsDecimal;
        public double TimeSeconds { get { return SecondsInteger + (double)SecondsDecimal / 1000.0; } }

        public override string ToString()
        {
            return $"{TimeHours:D2}:{TimeMinutes:D2}:{SecondsInteger:D2}.{SecondsDecimal:D2}";
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
}
