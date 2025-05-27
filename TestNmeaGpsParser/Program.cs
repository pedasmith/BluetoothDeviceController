
using Parsers.Nmea;

namespace TestNmeaGpsParser
{
    internal class Program
    {
        static int Test()
        {
            int nerror = 0;
            nerror += Nmea_Data.Test();

            return nerror;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Testing Nmea Gps parser");
            Test();

            var example = Parsers.Nmea.Nmea_Gps_Parser.Example_02;
            var parser = new Nmea_Gps_Parser();
            parser.OnNmeaUnknown += Parser_OnNmeaUnknown;
            parser.OnGpggaOk += Parser_OnGpggaOk;
            parser.OnGpggaParseError += Parser_OnGpggaParseError;
            parser.OnGpgllOk += Parser_OnGpgllOk;
            parser.OnGpgllParseError += Parser_OnGpgllParseError;
            parser.OnGpgsaOk += Parser_OnGpgsaOk;
            parser.OnGpgsaParseError += Parser_OnGpgsaParseError;
            parser.OnGppwrOk += Parser_OnGppwrOk;
            parser.OnGppwrParseError += Parser_OnGppwrParseError;
            parser.OnGprmcOk += Parser_OnGprmcOk;
            parser.OnGprmcParseError += Parser_OnGprmcParseError;
            parser.OnGpvtgOk += Parser_OnGpvtgOk;
            parser.OnGpvtgParseError += Parser_OnGpvtgParseError;
            parser.OnGpzdaOk += Parser_OnGpzdaOk;
            parser.OnGpzdaParseError += Parser_OnGpzdaParseError;

            var lines = example.Split(new char[] { '\n', '\r' }); 
            foreach (var line in lines)
            {
                var trim = line.Trim();
                if (!string.IsNullOrEmpty(trim))
                {
                    var result = parser.Parse(trim);
                }
            }
        }



        private static void Parser_OnGpggaParseError(object? sender, GPGGA_Data e)
        {
            Console.WriteLine($"GPGGA:Error: {e}");
        }
        private static void Parser_OnGpggaOk(object? sender, GPGGA_Data e)
        {
            Console.WriteLine($"GPGGA:OK: {e}");
        }

        private static void Parser_OnGpgllParseError(object? sender, GPGLL_Data e)
        {
            Console.WriteLine($"GPGLL:Error: {e}");
        }
        private static void Parser_OnGpgllOk(object? sender, GPGLL_Data e)
        {
            Console.WriteLine($"GPGLL:OK: {e}");
        }
        private static void Parser_OnGpgsaParseError(object? sender, GPGSA_Data e)
        {
            Console.WriteLine($"GPGSA:Error: {e}");
        }
        private static void Parser_OnGpgsaOk(object? sender, GPGSA_Data e)
        {
            Console.WriteLine($"GPGSA:OK: {e}");
        }

        private static void Parser_OnGppwrOk(object? sender, GPPWR_Data e)
        {
            Console.WriteLine($"GPPWR:OK: {e}");
        }
        private static void Parser_OnGppwrParseError(object? sender, GPPWR_Data e)
        {
            Console.WriteLine($"GPPWR:Error: {e}");
        }


        private static void Parser_OnGprmcOk(object? sender, GPRMC_Data e)
        {
            Console.WriteLine($"GPRMC:OK: {e}");
        }
        private static void Parser_OnGprmcParseError(object? sender, GPRMC_Data e)
        {
            Console.WriteLine($"GPRMC:Error: {e}");
        }

        private static void Parser_OnGpvtgOk(object? sender, GPVTG_Data e)
        {
            Console.WriteLine($"GPVTG:OK: {e}");
        }
        private static void Parser_OnGpvtgParseError(object? sender, GPVTG_Data e)
        {
            Console.WriteLine($"GPVTG:ERROR: {e}");
        }

        private static void Parser_OnGpzdaOk(object? sender, GPZDA_Data e)
        {
            Console.WriteLine($"GPZDA:OK: {e}");
        }
        private static void Parser_OnGpzdaParseError(object? sender, GPZDA_Data e)
        {
            Console.WriteLine($"GPZDA:ERROR: {e}");
        }

        private static void Parser_OnNmeaUnknown(object? sender, Nmea_Data e)
        {
            Console.WriteLine($"Unknown: {e}");
        }
    }
}
