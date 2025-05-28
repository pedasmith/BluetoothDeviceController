
using Parsers.Nmea;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace TestNmeaGpsParser
{
    class UserOptions
    {
        public string Match = "*";
        public bool Matches(string devicename)
        {
            if (Match == "*") return true;

            var nameup = devicename.ToUpper();
            var matchup = Match.ToUpper();
            if (nameup.Contains(matchup)) return true;
            return false;
        }
    }
    internal class Program
    {
        /// <summary>
        /// Calls all of the internal static self-test methods.
        /// </summary>
        /// <returns>Number of errors; should always be 0</returns>
        static int Test()
        {
            int nerror = 0;
            nerror += Nmea_Data.Test();

            return nerror;
        }
        private static void Log(String str)
        {
            Console.WriteLine(str);
        }
        [STAThread]
        static void Main(string[] args)
        {
            UserOptions options = new UserOptions();
            Log("Nmea Gps Program");
            Test();

            for (int i=0; i<args.Length; i++)
            {
                var arg = args[i];
                bool hasParam = i < (args.Length - 1);
                var argParam = hasParam ? args[i + 1] : "";
                switch (arg)
                {
                    default:
                        Log($"ERROR: Unknown argument {arg}");
                        break;
                    case "-example":
                        {
                            i++;
                            string text = "";
                            switch (argParam)
                            {
                                default:
                                case "01":
                                    text = File.ReadAllText("Assets\\ExampleNmeaFiles\\Example_01.nmea");
                                    break;
                                case "02":
                                    text = File.ReadAllText("Assets\\ExampleNmeaFiles\\Example_02.nmea");
                                    break;
                            }
                            Log($"Using example NMEA text {argParam}");
                            Demonstrate_Callbacks(text);
                        }
                        break;
                    case "-file":
                        i++;
                        try
                        {
                            Log($"Reading file {argParam}");
                            var text = File.ReadAllText(argParam);
                            Demonstrate_Callbacks(text);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case "-listcom":
                        ListBluetooth(options);
                        break;
                    case "-match":
                        i++;
                        options.Match = argParam;
                        break;
                }
            }
        }

        #region Demonstrate_Windows
        // Note: to make this work, the TargetFramework was set to     <TargetFramework>net9.0-windows10.0.17763.0</TargetFramework>
        // The completely magical string -windows10.0.17763.0 makes it work, but isn't documented in any
        // logical spot. See https://blogs.windows.com/windowsdeveloper/2020/09/03/calling-windows-apis-in-net5/
        // for details.
        private static async void ListBluetooth(UserOptions options)
        {
            int nNotMatch = 0;
            int nMatch = 0;
            DeviceInformation? firstMatch = null;
            DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            foreach (DeviceInformation? device in PairedBluetoothDevices)
            {
                if (options.Matches(device.Name))
                {
                    nMatch++;
                    Log($"Info: device name={device.Name}");
                    if (firstMatch == null) firstMatch = device;
                }
                else
                {
                    nNotMatch++;
                }
            }
            Log($"List complete. N. Match={nMatch} Not matching={nNotMatch}");

            // Now let's try to connect
            if (firstMatch == null) return;

            var accessStatus = DeviceAccessInformation.CreateFromId(firstMatch.Id);
            if (accessStatus.CurrentStatus != DeviceAccessStatus.Allowed)
            {
                Log($"Can't connect: access status={accessStatus.CurrentStatus}");
                return;
            }

            BluetoothDevice? bt = null;
            try
            {
                Log($"About to get device from id={firstMatch.Id}");
                bt = await BluetoothDevice.FromIdAsync(firstMatch.Id);
                Log($"Result: {bt}");
                if (bt == null)
                {
                    Log($"Unable to get BT device: FromId returned null");
                    return;
                }
                Log($"Got device: connection status={bt.ConnectionStatus} address={bt.BluetoothAddress}");
            }
            catch (Exception ex)
            {
                Log($"Can't get BT device: reason={ex.Message}");
                return;
            }

            try
            {
                var rfcommServices = await bt.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);
                Log($"RfcommServices count={rfcommServices.Services.Count}");
            }
            catch (Exception ex)
            {
                Log($"Can't get BT comm services: reason={ex.Message}");
                return;
            }
        }

        #endregion

        #region Demonstrate_Callbacks
        static void Demonstrate_Callbacks(string example)
        {
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
        #endregion
    }
}
