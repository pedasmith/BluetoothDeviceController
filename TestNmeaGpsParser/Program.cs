
using Parsers.Nmea;
using System.Linq.Expressions;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Storage.Pickers;

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

        public bool AllowSlowLists = false;
        public bool ShowMatchingDevices = true;
        public bool ShowAqsQuery = false;
        public bool TraceEachQuery = false;
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
            var p = new Program();
            p.DoMain(args);
        }

        void DoMain(string[] args)
        { 
            UserOptions options = new UserOptions();
            Log("Nmea Gps Program");
            Test();

            // Initialize the folder picker with the window handle (HWND).
            //var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            //WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);

            List<SelectorInfo> selectors = new List<SelectorInfo>();
            Task? task;


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
                    case "-aqs":
                        i++;
                        selectors.Clear();
                        task = FillSelectorsFromAqsQuery(selectors, "AQS", argParam, options, false);
                        task.Wait();
                        DisplaySelectors(selectors, options);
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
                    case "-listdeviceselectors":
                        selectors.Clear();
                        options.TraceEachQuery = true;
                        task = FillSelectorsFromDevicesAsync(selectors, options);
                        task.Wait();
                        options.TraceEachQuery = false;
                        DisplaySelectors(selectors, options);
                        break;
                    case "-match":
                        i++;
                        options.Match = argParam;
                        break;
                    case "-slowlist":
                        options.AllowSlowLists = true;
                        break;
                    case "-noslowlist":
                        options.AllowSlowLists = false;
                        break;
                    case "-showaqs":
                        options.ShowAqsQuery = true;
                        break;
                    case "-noshowaqs":
                        options.ShowAqsQuery = false;
                        break;
                    case "-showmatching":
                        options.ShowMatchingDevices = true;
                        break;
                    case "-noshowmatching":
                        options.ShowMatchingDevices = false;
                        break;

                }
            }
        }

        #region Demonstrate_Windows
        // Note: to make this work, the TargetFramework was set to     <TargetFramework>net9.0-windows10.0.17763.0</TargetFramework>
        // The completely magical string -windows10.0.17763.0 makes it work, but isn't documented in any
        // logical spot. See https://blogs.windows.com/windowsdeveloper/2020/09/03/calling-windows-apis-in-net5/
        // for details.
        private async void ListBluetooth(UserOptions options)
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

        #region List_DeviceSelectors

        public class SelectorInfo
        {
            public SelectorInfo(string name, string selector, DeviceInformation? di)
            {
                Name = name;
                Selector = selector;
                DI = di;
            }
            public string Name = "";
            public string Selector = "";
            public DeviceInformation? DI;

        }
        public async Task FillSelectorsFromDevicesAsync(List<SelectorInfo> selectors, UserOptions options)
        {
            const bool IsSlow = true;

            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.BluetoothAdapter", Windows.Devices.Bluetooth.BluetoothAdapter.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.BluetoothDevice", Windows.Devices.Bluetooth.BluetoothDevice.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.BluetoothLEDevice", Windows.Devices.Bluetooth.BluetoothLEDevice.GetDeviceSelector(), options);

            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.Rfcomm.ObexFileTransfer", RfcommDeviceService.GetDeviceSelector(RfcommServiceId.ObexFileTransfer), options);
            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.Rfcomm.PhoneBookAccessPce", RfcommDeviceService.GetDeviceSelector(RfcommServiceId.PhoneBookAccessPce), options);
            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.Rfcomm.PhoneBookAccessPse", RfcommDeviceService.GetDeviceSelector(RfcommServiceId.PhoneBookAccessPse), options);
            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.Rfcomm.SerialPort", RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort), options);
            await FillSelectorsFromAqsQuery(selectors, "Bluetooth.Rfcomm.GenericFileTransfer", RfcommDeviceService.GetDeviceSelector(RfcommServiceId.GenericFileTransfer), options);

            // All the sensors are together
            await FillSelectorsFromAqsQuery(selectors, "Accelerometer(Gravity)", Windows.Devices.Sensors.Accelerometer.GetDeviceSelector(Windows.Devices.Sensors.AccelerometerReadingType.Gravity), options);
            await FillSelectorsFromAqsQuery(selectors, "Acceleromter(Linear)", Windows.Devices.Sensors.Accelerometer.GetDeviceSelector(Windows.Devices.Sensors.AccelerometerReadingType.Linear), options);
            await FillSelectorsFromAqsQuery(selectors, "Acceleromter(Standard)", Windows.Devices.Sensors.Accelerometer.GetDeviceSelector(Windows.Devices.Sensors.AccelerometerReadingType.Standard), options);
            await FillSelectorsFromAqsQuery(selectors, "Activity", Windows.Devices.Sensors.ActivitySensor.GetDeviceSelector(), options);
            //await ListDeviceSelectorOne(selectors, "Activity", Windows.Devices.Sensors.Altimeter.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Barometer", Windows.Devices.Sensors.Barometer.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Compass", Windows.Devices.Sensors.Compass.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Gyrometer", Windows.Devices.Sensors.Gyrometer.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "HingeAngle", Windows.Devices.Sensors.HingeAngleSensor.GetDeviceSelector(), options);
            //await ListDeviceSelectorOne(selectors, "HumanPresence", Windows.Devices.Sensors.HumanPresenseSensor.GetDeviceSelector(), options);

            await FillSelectorsFromAqsQuery(selectors, "Inclinometer(Absolute)", Windows.Devices.Sensors.Inclinometer.GetDeviceSelector(Windows.Devices.Sensors.SensorReadingType.Absolute), options);
            await FillSelectorsFromAqsQuery(selectors, "Inclinomter(Relative)", Windows.Devices.Sensors.Inclinometer.GetDeviceSelector(Windows.Devices.Sensors.SensorReadingType.Relative), options);
            await FillSelectorsFromAqsQuery(selectors, "Light", Windows.Devices.Sensors.LightSensor.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Magnetometer", Windows.Devices.Sensors.Magnetometer.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Orientation(Absolute)", Windows.Devices.Sensors.OrientationSensor.GetDeviceSelector(Windows.Devices.Sensors.SensorReadingType.Absolute), options);
            await FillSelectorsFromAqsQuery(selectors, "Orientation(Relative)", Windows.Devices.Sensors.OrientationSensor.GetDeviceSelector(Windows.Devices.Sensors.SensorReadingType.Relative), options);
            await FillSelectorsFromAqsQuery(selectors, "Pedometer", Windows.Devices.Sensors.Pedometer.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Proximity", Windows.Devices.Sensors.ProximitySensor.GetDeviceSelector(), options);
            //await ListDeviceSelectorOne(selectors, "Activity", Windows.Devices.Sensors.SimpleOrientation.GetDeviceSelector(), options);

            // All of the POS devices together
            await FillSelectorsFromAqsQuery(selectors, "BarcodeScanner", Windows.Devices.PointOfService.BarcodeScanner.GetDeviceSelector(), options, IsSlow);
            await FillSelectorsFromAqsQuery(selectors, "CashDrawer", Windows.Devices.PointOfService.CashDrawer.GetDeviceSelector(), options, IsSlow);
            await FillSelectorsFromAqsQuery(selectors, "LineDisplay", Windows.Devices.PointOfService.LineDisplay.GetDeviceSelector(), options, IsSlow);
            await FillSelectorsFromAqsQuery(selectors, "MagneticStripeReader", Windows.Devices.PointOfService.MagneticStripeReader.GetDeviceSelector(), options, IsSlow);
            await FillSelectorsFromAqsQuery(selectors, "PosPrinter", Windows.Devices.PointOfService.PosPrinter.GetDeviceSelector(), options, IsSlow);


            // Everything else alphabetical
            await FillSelectorsFromAqsQuery(selectors, "3DPrinter", Windows.Devices.Printers.Print3DDevice.GetDeviceSelector(), options);
            //does not exist?await ListDeviceSelectorOne(selectors, "DisplayMuxDevice", Windows.Devices.Display.Core.DisplayMuxDevice.GetDeviceSelector());
            await FillSelectorsFromAqsQuery(selectors, "DIAL(WiDi)", Windows.Media.DialProtocol.DialDevice.GetDeviceSelector("WiDi"), options, IsSlow);
            await FillSelectorsFromAqsQuery(selectors, "DIAL(org.smarttv-alliance)", Windows.Media.DialProtocol.DialDevice.GetDeviceSelector("org.smarttv-alliance"), options, IsSlow);

            await FillSelectorsFromAqsQuery(selectors, "I2C", Windows.Devices.I2c.I2cDevice.GetDeviceSelector(), options);

            await FillSelectorsFromAqsQuery(selectors, "Lamp", Windows.Devices.Lights.Lamp.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "LampArray", Windows.Devices.Lights.LampArray.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "MediaFrameSourceGroup", Windows.Media.Capture.Frames.MediaFrameSourceGroup.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Midi(In)", Windows.Devices.Midi.MidiInPort.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Midi(Out)", Windows.Devices.Midi.MidiOutPort.GetDeviceSelector(), options);

            await FillSelectorsFromAqsQuery(selectors, "MobileBroadband", Windows.Networking.NetworkOperators.MobileBroadbandModem.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableCalendar", Windows.Devices.Portable.ServiceDevice.GetDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.CalendarService), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableContacts", Windows.Devices.Portable.ServiceDevice.GetDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.ContactsService), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableDeviceStatus", Windows.Devices.Portable.ServiceDevice.GetDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.DeviceStatusService), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableNotes", Windows.Devices.Portable.ServiceDevice.GetDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.NotesService), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableRingtones", Windows.Devices.Portable.ServiceDevice.GetDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.RingtonesService), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableSms", Windows.Devices.Portable.ServiceDevice.GetDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.SmsService), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableTasks", Windows.Devices.Portable.ServiceDevice.GetDeviceSelector(Windows.Devices.Portable.ServiceDeviceType.TasksService), options);
            await FillSelectorsFromAqsQuery(selectors, "PortableStorage", Windows.Devices.Portable.StorageDevice.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "ProjectionManager", Windows.UI.ViewManagement.ProjectionManager.GetDeviceSelector(), options, IsSlow);
            await FillSelectorsFromAqsQuery(selectors, "PWM", Windows.Devices.Pwm.PwmController.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Radio", Windows.Devices.Radios.Radio.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Scanner", Windows.Devices.Scanners.ImageScanner.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "SerialDevice", Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Sms", Windows.Devices.Sms.SmsDevice.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Sms(2)", Windows.Devices.Sms.SmsDevice2.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "SPI", Windows.Devices.Spi.SpiDevice.GetDeviceSelector(), options);
            //await ListDeviceSelectorOne(selectors, "USB", Windows.Devices.Usb.UsbDevice.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "Vibrator", Windows.Devices.Haptics.VibrationDevice.GetDeviceSelector(), options);

            await FillSelectorsFromAqsQuery(selectors, "WiFi", Windows.Devices.WiFi.WiFiAdapter.GetDeviceSelector(), options);
            await FillSelectorsFromAqsQuery(selectors, "WiFiDirect", Windows.Devices.WiFiDirect.WiFiDirectDevice.GetDeviceSelector(), options);
        }

        public void DisplaySelectors(List<SelectorInfo> selectors, UserOptions options)
        { 
            // Potential
            // HidDevice takes two weird parameters

            if (options.ShowMatchingDevices)
            {
                Log("Device\tName\tId\tKind");
                foreach (var item in selectors)
                {
                    if (item.DI == null)
                    {
                        Log($"{item.Name}\t\t\t");
                    }
                    else
                    {
                        Log($"{item.Name}\t{item.DI.Name}\t{item.DI.Id}\t{item.DI.Kind}");
                    }
                }
            }
            if (options.ShowAqsQuery)
            {
                Log("Device,AQS");
                var oldname = "---no-a-real-name";
                foreach (var item in selectors)
                {
                    if (item.Name == oldname) continue;
                    oldname = item.Name;

                    Log($"{item.Name},{item.Selector}");
                }
            }
        }

        private async Task FillSelectorsFromAqsQuery(IList<SelectorInfo> selectors, string name, string aqsQuery, UserOptions options, bool isSlow=false)
        {
            if (options.TraceEachQuery)
            {
                Log($"{name}");
            }
            try
            {
                if (options.ShowAqsQuery && !options.ShowMatchingDevices)
                {
                    selectors.Add(new SelectorInfo(name, aqsQuery, null));
                }
                else if (isSlow && !options.AllowSlowLists)
                {
                    selectors.Add(new SelectorInfo(name+"(not queried)", aqsQuery, null));
                }
                else
                {
                    DeviceInformationKind kind = DeviceInformationKind.DeviceInterface;
                    DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(aqsQuery, null, kind);
                    var n = devices.Count;
                    if (n == 0)
                    {
                        selectors.Add(new SelectorInfo(name, aqsQuery, null));
                    }
                    else
                    {
                        int nadded = 0;
                        foreach (DeviceInformation? device in devices)
                        {
                            if (device != null)
                            {
                                selectors.Add(new SelectorInfo(name, aqsQuery, device));
                                nadded++;
                            }
                            if (nadded == 0)
                            {
                                selectors.Add(new SelectorInfo(name, aqsQuery, null));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"    Error: exception {ex.Message}");
                selectors.Add(new SelectorInfo(name + "EXCEPTION" + ex.Message, aqsQuery, null));
            }
        }
        #endregion
    }
}
