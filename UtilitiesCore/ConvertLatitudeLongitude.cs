using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utilities
{
    public static class ConvertLatitudeLongitude
    {
        private static int RuntimeConversionErrors = 0;
        /// <summary>
        /// Returns a value -90 to 90; "S" is negative
        /// </summary>
        public static double ConvertToDecimal(Nmea_Latitude_Fields field)
        {
            double retval = ((double)field.LatitudeDegrees) + field.LatitudeMinutes / 60.0;
            if (field.LatitudeNorthSouth == Nmea_Latitude_Fields.NorthSouthType.South)
            {
                retval = -retval;
            }
            if (retval < -90 || retval > 90)
            {
                RuntimeConversionErrors++;
                Log($"Error: convert lat returns {retval} which is out-of-range. Input {field}");
            }
            return retval;
        }

        /// <summary>
        /// Longitude: Returns a value -180 to 180; "W" is negative
        /// </summary>
        public static double ConvertToDecimal(Nmea_Longitude_Fields field)
        {
            double retval = ((double)field.LongitudeDegrees) + field.LongitudeMinutes / 60.0;
            if (field.LongitudeEastWest == Nmea_Longitude_Fields.EastWestType.West)
            {
                retval = -retval;
            }
            if (retval < -180 || retval > 180)
            {
                RuntimeConversionErrors++;
                Log($"Error: convert lat returns {retval} which is out-of-range. Input {field}");
            }
            return retval;
        }


        // Online calculator: https://latitudeandlongitude.net/en/converter
        public static int Test()
        {
            int nerror = 0;
            nerror += TestOneLongitude("14700.0000", "E", 147.0);
            nerror += TestOneLongitude("14700.0000", "W", -147.0);

            nerror += TestOneLongitude("14753.4738", "E", 147.89123);
            nerror += TestOneLongitude("14753.4738", "W", -147.89123);

            nerror += TestOneLatitude("4700.0000", "N", 47.0);
            nerror += TestOneLatitude("4700.0000", "S", -47.0);

            nerror += TestOneLatitude("4753.4738", "N", 47.89123); // 47.89123=47.89123° N AKA 47° 53' 28.428'' N (DMS) AKA 47° 53.4738' N (DDM)
            nerror += TestOneLatitude("4753.4738", "S", -47.89123); // 47.89123=47.89123° N 
            nerror += RuntimeConversionErrors;
            return nerror;
        }

        private static int TestOneLatitude(string latitude, string ns, double expected)
        {
            int nerror = 0;
            Nmea_Latitude_Fields nmea = new Nmea_Latitude_Fields();
            var ok = nmea.Parse(latitude, ns);
            if (ok != Nmea_Gps_Parser.ParseResult.Ok)
            {
                Log($"Error: Latitude({latitude}, {ns}) expected={expected} parse error {ok}");
                nerror++;
            }
            double actual = ConvertToDecimal(nmea);
            if (actual != expected)
            {
                Log($"Error: Latitude({latitude}, {ns}) expected={expected} actual={actual} delta={expected - actual}");
                nerror++;
            }
            return nerror;
        }
        private static int TestOneLongitude(string longitude, string ew, double expected)
        {
            int nerror = 0;
            Nmea_Longitude_Fields nmea = new Nmea_Longitude_Fields();
            var ok = nmea.Parse(longitude, ew);
            if (ok != Nmea_Gps_Parser.ParseResult.Ok)
            {
                Log($"Error: Longitude({longitude}, {ew}) expected={expected} parse error {ok}");
                nerror++;
            }
            double actual = ConvertToDecimal(nmea);
            if (actual != expected)
            {
                Log($"Error: Longitude({longitude}, {ew}) expected={expected} actual={actual} delta={expected - actual}");
                nerror++;
            }
            return nerror;
        }
        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}
