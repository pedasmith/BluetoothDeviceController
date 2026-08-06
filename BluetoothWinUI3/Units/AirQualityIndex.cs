using HtmlAgilityPack;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Background;

namespace BluetoothWinUI3.Units
{
    /// <summary>
    /// IMPORTANT NOTE: calculating an AQI that matches the official definition is complex. It requires
    /// multiple sensors which most home air quality sensors don't include (the Ruuvi Air, for example,
    /// includes 2 of the 5 sensors for AQI). 
    /// 
    /// The calculations require precises math rounding which as of 2026-08-06 is not implemented
    /// 
    /// The calculations require a series of measures over time which as of 2026-08-06 is not implemented.
    /// 
    /// The calculations include special information for an AQI of 500 which isn't present in the tables
    /// 
    /// The calculations include multiple footnotes which are not implemented. And some of the footnotes are puzzling.
    /// </summary>
    public static class AirQualityIndex
    {
        /// <summary>
        /// Calculate an AQI value given the only data we have. Note that calculations don't include
        /// special cases for ozone (8 versus 1 hour), special cases for hazardous, special cases for SO2
        /// per all the footnotes on page 14
        /// https://document.airnow.gov/technical-assistance-document-for-the-reporting-of-daily-air-quailty.pdf
        /// See also the information for citizen science at https://www.epa.gov/air-sensor-toolbox
        /// This covers low-cost sensors and how to interprete the results.
        /// </summary>
        public static double Calculate(double pm25, double pm100, double nox)
        {
            pm25 = Truncate(Pollutant.PM25, pm25);
            pm100 = Truncate(Pollutant.PM100, pm100);
            nox = Truncate(Pollutant.NOX, nox);

            var aqiPM25 = InterpolateAqi(PM25_24HourList, pm25);
            var aqiPM100 = InterpolateAqi(PM100_24HourList, pm100);
            var aqiNox = InterpolateAqi(NOX_1HourList, nox);
            var aqi = Math.Max(aqiPM25, aqiPM100);
            aqi = Math.Max(aqi, aqiNox);
            return aqi;
        }

        public static string AqiMeaning(double aqi)
        {
            if (aqi < 0) return "Missing data";
            if (aqi <= 50) return "Good";
            if (aqi <= 100) return "Moderate";
            if (aqi <= 150) return "USG"; // unhealthy for sensitive groups
            if (aqi <= 200) return "Unhealthy";
            if (aqi <= 300) return "Very unhealthy";
            return "Hazardous";
        }


        /// <summary>
        /// Returns the official color for an AQI value
        /// Exception: negative values map to a gray
        /// Color can be converted to OxyColor
        /// </summary>
        public static uint AqiToColor(double aqi)
        {
            // Page 4: https://document.airnow.gov/technical-assistance-document-for-the-reporting-of-daily-air-quailty.pdf
            if (aqi < 0) return RGBToUInt(128, 128, 128); // Gray-ish. Not an official color
            if (aqi <= 50) return RGBToUInt(0, 228, 0); // Green
            if (aqi <= 100) return RGBToUInt(255, 255, 0); // Yellow
            if (aqi <= 150) return RGBToUInt(255, 126, 0); // Orange unhealthy for sensitive groups
            if (aqi <= 200) return RGBToUInt(255, 0, 0); // Red
            if (aqi <= 300) return RGBToUInt(137, 9, 131); //Purple
            return RGBToUInt(126, 0, 35); // Maroon
        }

        /// <summary>
        /// Convert RGB as bytes to an integer. 
        /// </summary>
        private static uint RGBToUInt(byte r, byte g, byte b)
        {
            uint retval = (uint)((r<<16) + (g<<8) + b);
            return retval;
        }

        static List<AqiBreakpoint> Ozone_8HourList = new()
        {
            new AqiBreakpoint(Pollutant.Ozone, 8, 0.000, 0.054,   0,  50),
            new AqiBreakpoint(Pollutant.Ozone, 8, 0.055, 0.070,  51, 100),
            new AqiBreakpoint(Pollutant.Ozone, 8, 0.071, 0.085, 101, 150),
            new AqiBreakpoint(Pollutant.Ozone, 8, 0.086, 0.105, 151, 200),
            new AqiBreakpoint(Pollutant.Ozone, 8, 0.106, 0.200, 201, 300),
            new AqiBreakpoint(Pollutant.Ozone, 8, 0.201, 99999, 301, 999),
        };

        static List<AqiBreakpoint> Ozone_1HourList = new()
        {
            new AqiBreakpoint(Pollutant.Ozone, 1, 0.000, 0.124,   0,  0), // Note a valid value
            new AqiBreakpoint(Pollutant.Ozone, 1, 0.125, 0.164, 101, 150),
            new AqiBreakpoint(Pollutant.Ozone, 1, 0.165, 0.204, 151, 200),
            new AqiBreakpoint(Pollutant.Ozone, 1, 0.205, 0.404, 201, 300),
            new AqiBreakpoint(Pollutant.Ozone, 1, 0.405, 99999, 301, 999),
        };

        static List<AqiBreakpoint> PM25_24HourList = new() // 2.5 ug/m3
        {
            new AqiBreakpoint(Pollutant.PM25, 24,   0.0,   9.0,   0,  50),
            new AqiBreakpoint(Pollutant.PM25, 24,   9.1,  35.4,  51, 100),
            new AqiBreakpoint(Pollutant.PM25, 24,  35.5,  55.4, 101, 150),
            new AqiBreakpoint(Pollutant.PM25, 24,  55.5, 125.4, 151, 200),
            new AqiBreakpoint(Pollutant.PM25, 24, 125.5, 225.4, 201, 300),
            new AqiBreakpoint(Pollutant.PM25, 24, 225.5, 99999, 301, 999),
        };


        static List<AqiBreakpoint> PM100_24HourList = new() // 10.0 ug/m3
        {
            new AqiBreakpoint(Pollutant.PM100, 24,   0.0,  54.0,   0,  50),
            new AqiBreakpoint(Pollutant.PM100, 24,  55.0, 154.0,  51, 100),
            new AqiBreakpoint(Pollutant.PM100, 24, 155.0, 254.0, 101, 150),
            new AqiBreakpoint(Pollutant.PM100, 24, 255.0, 354.0, 151, 200),
            new AqiBreakpoint(Pollutant.PM100, 24, 355.0, 424.0, 201, 300),
            new AqiBreakpoint(Pollutant.PM100, 24, 425.0, 99999, 301, 999),
        };

        static List<AqiBreakpoint> CO_8HourList = new() // PPM
        {
            new AqiBreakpoint(Pollutant.CO, 8,  0.0,  4.4,   0,  50),
            new AqiBreakpoint(Pollutant.CO, 8,  4.5,  9.5,  51, 100),
            new AqiBreakpoint(Pollutant.CO, 8,  9.5, 12.4, 101, 150),
            new AqiBreakpoint(Pollutant.CO, 8, 12.5, 15.4, 151, 200),
            new AqiBreakpoint(Pollutant.CO, 8, 15.5, 30.4, 201, 300),
            new AqiBreakpoint(Pollutant.CO, 8, 30.5, 9999, 301, 999),
        };

        static List<AqiBreakpoint> SO2_1HourList = new() // PPB
        {
            new AqiBreakpoint(Pollutant.SO2, 1,    0.0,  35.0,   0,  50),
            new AqiBreakpoint(Pollutant.SO2, 1,   36.0,  75.0,  51, 100),
            new AqiBreakpoint(Pollutant.SO2, 1,   76.0, 185.0, 101, 150),
            new AqiBreakpoint(Pollutant.SO2, 1,  186.0, 304.0, 151, 200),
            new AqiBreakpoint(Pollutant.SO2, 1,  305.0, 604.0, 201, 300),
            new AqiBreakpoint(Pollutant.SO2, 1,  605.0, 99999, 301, 999),
        };


        static List<AqiBreakpoint> NOX_1HourList = new() // PPB
        {
            new AqiBreakpoint(Pollutant.NOX, 1,    0.0,   53.0,   0,  50),
            new AqiBreakpoint(Pollutant.NOX, 1,   54.0,  100.0,  51, 100),
            new AqiBreakpoint(Pollutant.NOX, 1,  101.0,  360.0, 101, 150),
            new AqiBreakpoint(Pollutant.NOX, 1,  361.0,  649.0, 151, 200),
            new AqiBreakpoint(Pollutant.NOX, 1,  650.0, 1249.0, 201, 300),
            new AqiBreakpoint(Pollutant.NOX, 1, 1250.0, 999999, 301, 999),
        };

        private static double Truncate(Pollutant pollutant, double value)
        {
            int ndecimal = 0;
            switch (pollutant)
            {
                case Pollutant.Ozone: ndecimal = 3; break;
                case Pollutant.PM25: ndecimal = 1; break;
                case Pollutant.PM100: ndecimal = 0; break;
                case Pollutant.CO: ndecimal = 1; break;
                case Pollutant.SO2: ndecimal = 0; break;
                case Pollutant.NOX: ndecimal = 0; break;
            }
            int mult = (int)Math.Pow(10, ndecimal);
            double retval = Math.Truncate(value * mult) / mult;
            return retval;
        }


        const double INTERPOLATE_VALUE_TOO_SMALL = -1;
        const double INTERPOLATE_BAD_LIST = -2;
        const double INTERPOLATE_VALUE_IN_UNUSED_RANGE = -3; // For example, ozone 1 hour under 0.125
        static double InterpolateAqi(List<AqiBreakpoint> list, double value)
        {
            // TODO: Math weirdness: the guidance is to truncate the incoming value
            // e.g., a value of 0.07853333 is truncated to 0.078

            // Note: there's some specialized interpolation going on. 
            // Example: Ozone, 8 hours
            // Value 0.054 is 50
            // Value 0.055 is 51
            // Values between 0.054 and 0.055 are ALSO 50. 
            if (value < 0) return INTERPOLATE_VALUE_TOO_SMALL;
            if (value >= list[^1].ValueMin) // Last item in list using Index-from-end operator (^) in C# 8
            {
                return list[^1].AqiMin; // Note: AQI values 301 and higher have special calculations
            }
            // At this point, the value is guaranteed to be in the list
            AqiBreakpoint? breakpoint = null;
            for (int i=0; i<list.Count-1 && breakpoint==null; i++)
            {
                if (value >= list[i].ValueMin && value < list[i+1].ValueMin)
                {
                    breakpoint = list[i];
                }
            }
            if (breakpoint == null)
            {
                Log($"AQI Interpolate error: value={value} for {list[0].Pollutant} hours={list[0].HourRange}");
                return INTERPOLATE_BAD_LIST;
            }
            if (breakpoint.AqiMin == 0 && breakpoint.AqiMax == 0) return INTERPOLATE_VALUE_IN_UNUSED_RANGE; // Example: Ozone 1 hour between 0 and 0.124

            double aqiRatio = (breakpoint.AqiMax - breakpoint.AqiMin) / (breakpoint.ValueMax - breakpoint.ValueMin);
            double aqi = aqiRatio * (value - breakpoint.ValueMin) + breakpoint.AqiMin;
            // handle the case of a number slightly larger than the ValueMax but lower than the next breakpoint's
            // ValueMin. Per EPA guidance, the result should fall into the lower bucket.
            if (aqi > breakpoint.AqiMax) aqi = breakpoint.AqiMax;
            aqi = Math.Round(aqi);
            return aqi;
        }

        private static int TestOne(List<AqiBreakpoint> list, double value, double expected)
        {
            int nerror = 0;
            double actual = InterpolateAqi(list, value);
            if (actual != expected) // always integers :-)
            {
                Log($"AirQualityIndex:Test {list[0].Pollutant} hours={list[0].HourRange} value={value} expected={expected} but actual={actual}");
                nerror++;
            }
            return nerror;
        }
        private static int TestOneAqi(double pm25, double pm100, double nox, double expected)
        {
            int nerror = 0;
            double actual = Calculate(pm25, pm100, nox);
            if (actual != expected) // always integers :-)
            {
                Log($"AirQualityIndex:Test Calculate({pm25},{pm100},{nox}) expected={expected} but actual={actual}");
                nerror++;
            }
            return nerror;
        }

        private static int TestOneTruncate(Pollutant pollutant, double value, double expected)
        {
            int nerror = 0;
            double actual = Truncate(pollutant, value);
            if (actual != expected) 
            {
                Log($"AirQualityIndex:Test Truncate({pollutant}, {value}) expected={expected} but actual={actual}");
                nerror++;
            }
            return nerror;
        }
        public static int Test()
        {
            int nerror = 0;

            nerror += TestOneTruncate(Pollutant.Ozone, 0.07853333, 0.078);

            nerror += TestOneAqi(55.5, 200, 50, 151);
            nerror += TestOneAqi(50.0, 255, 50, 151);
            nerror += TestOneAqi(50.0, 200, 361, 151);

            nerror += TestOne(Ozone_8HourList, 0.078, 126);
            nerror += TestOne(Ozone_1HourList, 0.123, INTERPOLATE_VALUE_IN_UNUSED_RANGE);
            nerror += TestOne(Ozone_1HourList, 0.165, 151);
            nerror += TestOne(PM25_24HourList, 225.5, 301);
            nerror += TestOne(PM100_24HourList, 900.9, 301);
            nerror += TestOne(CO_8HourList, 15.5, 201);
            nerror += TestOne(SO2_1HourList, 604, 300);
            nerror += TestOne(NOX_1HourList, 53, 50);
            return nerror;
        }

        private static void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
    public enum Pollutant { Ozone, PM25, PM100, CO, SO2, NOX,  };



    class AqiBreakpoint
    {
        public AqiBreakpoint(Pollutant pollutant, int hours, double valueMin, double valueMax, double aqiMin, double aqiMax)
        {
            Pollutant = pollutant;
            HourRange = hours;
            ValueMin = valueMin;
            ValueMax = valueMax;
            AqiMin = aqiMin;
            AqiMax = aqiMax;
        }
        public Pollutant Pollutant;
        public int HourRange;
        public double ValueMin;
        public double ValueMax;
        public double AqiMin;
        public double AqiMax;
    }
}
