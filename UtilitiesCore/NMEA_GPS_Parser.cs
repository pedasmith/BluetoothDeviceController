using System;
using System.Collections.Generic;
using System.Text;

// FindBitPattern tries to be more nullable enabled.
// Need this super weird set of ifs because:
// 1. many of my projects are "old" and give errors when #nullable disable is present
// 2. I can't use the ? for the events because old projects don't do that
// 3. But for FindBitPattern I need to suppress the warning just for this file.

#if NET8_0_OR_GREATER
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
            PdopInvalid, VdopInvalid, /* there's already an Hdop originally added for GPGGA */

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
            VoltageInvalid,
            ChargingStatusInvalid,
            OpcodeIsNotUnderstoodByAnyoneOnTheInternet,

            // GPGSV
            NMessageInvalid, MessageIndexInvalid, NSatelliteInViewInvalid,
            ElevationInvalid, AzimuthInvalid, SignalToNoiseRationInvalid,

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

    }
}
