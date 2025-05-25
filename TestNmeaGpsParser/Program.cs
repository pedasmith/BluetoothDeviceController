
using Parsers.Nmea;

namespace TestNmeaGpsParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing Nmea Gps parser");

            var example = Parsers.Nmea.Nmea_Gps_Parser.Example_01;
            var parser = new Nmea_Gps_Parser();
            var lines = example.Split(new char[] { '\n' }); 
            foreach (var line in lines)
            {
                var result = parser.Parse(line);
            }
        }
    }
}
