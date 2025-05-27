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

    public class Nmea_Gps_Parser
    { 
        // Useful links:
        // https://learn.sparkfun.com/tutorials/gps-basics/all
        // https://cdn.sparkfun.com/assets/a/3/2/f/a/NMEA_Reference_Manual-Rev2.1-Dec07.pdf
        // https://www.cypress.bc.ca/documents/Report_Messages/CTM200/msg_82_GPRMC.html

        public enum ParseResult {  Ok, NotEnoughFields, 
            OpcodeUnknown, OpcodeIncorrect,
            TimeStringWrongLength, TimeStringInvalid,
            ValidityInvalid,
            LatitudeStringWrongLength, LatitudeStringInvalid, LatitudeNorthSouthInvalid,
            LongitudeStringWrongLength, LongitudeStringInvalid, LongitudeEastWestInvalid,
            VelocityKnotsInvalid,
            HeadingDegreesTrueInvalid,
            DateStringWrongLength, DateStringInvalid,

            // Additional items for GPGGA
            PositionFixIndicatorInvalid,
            SatellitesUsedInvalid,
            HdopInvalid,
            MlsAltitudeInvalid, MlsAltitudeUnitsInvalid,
            GeoidSeparationInvalid, GeoidSeparationUnitsInvalid,

            // GPGSA additional errors
            Mode1Invalid, Mode2Invalid,
            PdopInvalid, VdopInvalid,

            // GPRMC additional errors
            MagneticVariationInvalid,
            EastWestIndicatorInvalid,

            // GPVTG additional errors
            CourseTrueInvalid, CourseTrueUnitsInvalid,
            CourseMagneticInvalid, CourseMagneticUnitsInvalid,
            VelocityKnotsUnitsInvalid,
            VelocityKphInvalid, VelocityKphUnitsInvalid,

            // GPZDA
            LocalZoneHourInvalid, LocalZoneMinutesInvalid,

            // GPGLL
            ModeInvalid,

            // GPPWR
            OpcodeIsNotUnderstoodByAnyoneOnTheInternet,

            OtherError
        }
        public ParseResult Parse(string Nmea)
        {
            ParseResult retval = ParseResult.OtherError; // always an error unless we know it's OK

            if (Nmea.StartsWith("$GPGGA,"))
            {
                var data = new GPGGA_Data(Nmea);
                OnNmeaAll?.Invoke(this, data);
                if (data.ParseStatus == ParseResult.Ok) OnGpggaOk?.Invoke(this, data);
                else OnGpggaParseError?.Invoke(this, data);
                return data.ParseStatus;
            }
            else if (Nmea.StartsWith("$GPGLL,"))
            {
                var data = new GPGLL_Data(Nmea);
                OnNmeaAll?.Invoke(this, data);
                if (data.ParseStatus == ParseResult.Ok) OnGpgllOk?.Invoke(this, data);
                else OnGpgllParseError?.Invoke(this, data);
                return data.ParseStatus;
            }
            else if (Nmea.StartsWith("$GPGSA,"))
            {
                var data = new GPGSA_Data(Nmea);
                OnNmeaAll?.Invoke(this, data);
                if (data.ParseStatus == ParseResult.Ok) OnGpgsaOk?.Invoke(this, data);
                else OnGpgsaParseError?.Invoke(this, data);
                return data.ParseStatus;
            }
            else if (Nmea.StartsWith("$GPPWR,"))
            {
                var data = new GPPWR_Data(Nmea);
                OnNmeaAll?.Invoke(this, data);
                if (data.ParseStatus == ParseResult.Ok) OnGppwrOk?.Invoke(this, data);
                else OnGppwrParseError?.Invoke(this, data);
                return data.ParseStatus;
            }
            else if (Nmea.StartsWith("$GPRMC,"))
            {
                var data = new GPRMC_Data(Nmea);
                OnNmeaAll?.Invoke(this, data);
                if (data.ParseStatus == ParseResult.Ok) OnGprmcOk?.Invoke(this, data);
                else OnGprmcParseError?.Invoke(this, data);
                return data.ParseStatus;
            }
            else if (Nmea.StartsWith("$GPVTG,"))
            {
                var data = new GPVTG_Data(Nmea);
                OnNmeaAll?.Invoke(this, data);
                if (data.ParseStatus == ParseResult.Ok) OnGpvtgOk?.Invoke(this, data);
                else OnGpvtgParseError?.Invoke(this, data);
                return data.ParseStatus;
            }
            else if (Nmea.StartsWith("$GPZDA,"))
            {
                var data = new GPZDA_Data(Nmea);
                OnNmeaAll?.Invoke(this, data);
                if (data.ParseStatus == ParseResult.Ok) OnGpzdaOk?.Invoke(this, data);
                else OnGpzdaParseError?.Invoke(this, data);
                return data.ParseStatus;
            }
            else
            {
                var data = new Nmea_Data(Nmea);
                OnNmeaUnknown?.Invoke(this, data);
            }

            return retval;
        }

        public event EventHandler<GPGGA_Data> OnGpggaOk;
        public event EventHandler<GPGGA_Data> OnGpggaParseError;

        public event EventHandler<GPGLL_Data> OnGpgllOk;
        public event EventHandler<GPGLL_Data> OnGpgllParseError;

        public event EventHandler<GPGSA_Data> OnGpgsaOk;
        public event EventHandler<GPGSA_Data> OnGpgsaParseError;

        public event EventHandler<GPPWR_Data> OnGppwrOk;
        public event EventHandler<GPPWR_Data> OnGppwrParseError;

        public event EventHandler<GPRMC_Data> OnGprmcOk;
        public event EventHandler<GPRMC_Data> OnGprmcParseError;

        public event EventHandler<GPVTG_Data> OnGpvtgOk;
        public event EventHandler<GPVTG_Data> OnGpvtgParseError;

        public event EventHandler<GPZDA_Data> OnGpzdaOk;
        public event EventHandler<GPZDA_Data> OnGpzdaParseError;

        public event EventHandler<Nmea_Data> OnNmeaAll;
        public event EventHandler<Nmea_Data> OnNmeaUnknown;



        public static string Example_01 = @"$GPRMC,235316.000,A,4003.9040,N,10512.5792,W,0.09,144.75,141112,,*19
$GPGGA,235317.000,4003.9039,N,10512.5793,W,1,08,1.6,1577.9,M,-20.7,M,,0000*5F
$GPGSA,A,3,22,18,21,06,03,09,24,15,,,,,2.5,1.6,1.9*3E";


        public static string Example_02 = @"$GPRMC,172113.000,V,4739.6693,N,12207.8055,W,000.0,000.0,070425,,,N*6A
$GPVTG,000.0,T,,M,000.0,N,000.0,K,N*02
$GPZDA,172113.000,07,04,2025,00,00*57
$GPGGA,172114.000,4739.6693,N,12207.8055,W,0,00,0.0,79.1,M,0.0,M,,0000*4D
$GPGLL,4739.6693,N,12207.8055,W,172114.000,V,N*5E
$GPGSA,A,1,,,,,,,,,,,,,0.0,0.0,0.0*30
$GPPWR,0289,0,1,1,0,00,0,S,56*0D
$GPRMC,172114.000,V,4739.6693,N,12207.8055,W,000.0,000.0,070425,,,N*6D
$GPVTG,000.0,T,,M,000.0,N,000.0,K,N*02
$GPZDA,172114.000,07,04,2025,00,00*50
$GPGGA,172115.000,4739.6693,N,12207.8055,W,0,00,0.0,79.1,M,0.0,M,,0000*4C
$GPGLL,4739.6693,N,12207.8055,W,172115.000,V,N*5F
$GPGSA,A,1,,,,,,,,,,,,,0.0,0.0,0.0*30
$GPPWR,0289,0,1,1,0,00,0,S,56*0D
$GPRMC,172115.000,V,4739.6693,N,12207.8055,W,000.0,000.0,070425,,,N*6C
$GPVTG,000.0,T,,M,000.0,N,000.0,K,N*02
$GPZDA,172115.000,07,04,2025,00,00*51
$GPGGA,172116.000,4739.6693,N,12207.8055,W,0,00,0.0,79.1,M,0.0,M,,0000*4F
$GPGLL,4739.6693,N,12207.8055,W,172116.000,V,N*5C
$GPGSA,A,1,,,,,,,,,,,,,0.0,0.0,0.0*30
$GPPWR,0289,0,1,1,0,00,0,S,55*0E
$GPGSV,2,1,08,19,80,077,,24,54,280,,22,47,090,,06,36,150,*72
$GPGSV,2,2,08,14,27,099,,01,08,032,,13,05,204,,03,05,056,*7D
$GPRMC,172116.000,V,4739.6693,N,12207.8055,W,000.0,000.0,070425,,,N*6F
$GPVTG,000.0,T,,M,000.0,N,000.0,K,N*02
$GPZDA,172116.000,07,04,2025,00,00*52
$GPGGA,172117.000,4739.6693,N,12207.8055,W,0,00,0.0,79.1,M,0.0,M,,0000*4E
$GPGLL,4739.6693,N,12207.8055,W,172117.000,V,N*5D
$GPGSA,A,1,,,,,,,,,,,,,0.0,0.0,0.0*30
$GPPWR,0289,0,1,1,0,00,0,S,55*0E
$GPRMC,172117.000,V,4739.6693,N,12207.8055,W,000.0,000.0,070425,,,N*6E
";
    }
}
