using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemonstateBluetoothGattServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        GattServiceProvider ServiceProvider = null;

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await DoStartServer();
        }

        private void Log(string text)
        {
            Console.WriteLine(text);
            System.Diagnostics.Debug.WriteLine(text);
            uiLog.Text += text + "\n";
        }
        private void OnClearScreen(object sender, RoutedEventArgs e)
        {
            uiLog.Text = "";
        }

        private async void OnStartServer(object sender, RoutedEventArgs e)
        {
            await DoStartServer();
        }

        private async Task DoStartServer()
        { 
            var timeServiceUuid = Guid.Parse("00001805-0000-1000-8000-00805f9b34fb");
            var currentTimeUuid = Guid.Parse("00002A2B-0000-1000-8000-00805f9b34fb");
            GattServiceProviderResult result = await GattServiceProvider.CreateAsync(timeServiceUuid);
            if (result.Error != Windows.Devices.Bluetooth.BluetoothError.Success)
            {
                Log($"Error: GattServiceProvider.CreateAsync failed with {result.Error}");
                return;
            }

            ServiceProvider = result.ServiceProvider;
            var p = new GattLocalCharacteristicParameters();
            p.WriteProtectionLevel = GattProtectionLevel.Plain;
            p.UserDescription = "Current time (year/month/day/h/m/s and more)";
            p.ReadProtectionLevel = GattProtectionLevel.Plain;
            p.CharacteristicProperties = GattCharacteristicProperties.Read | GattCharacteristicProperties.Notify;
            var cresult = await ServiceProvider.Service.CreateCharacteristicAsync(currentTimeUuid, p);
            if (cresult.Error != Windows.Devices.Bluetooth.BluetoothError.Success)
            {
                Log($"Error: GattServiceProvider.CreateAsync failed with {cresult.Error}");
                return;
            }
            var currentTimeCharacteristic = cresult.Characteristic;
            currentTimeCharacteristic.ReadRequested += CurrentTimeCharacteristic_ReadRequested;
            currentTimeCharacteristic.SubscribedClientsChanged += CurrentTimeCharacteristic_SubscribedClientsChanged;


            var advParameters = new GattServiceProviderAdvertisingParameters
            {
                IsDiscoverable = true,
                IsConnectable = true
            };
            ServiceProvider.StartAdvertising(advParameters);
        }

        private void CurrentTimeCharacteristic_SubscribedClientsChanged(GattLocalCharacteristic sender, object args)
        {
            ;
        }
        private static byte ConvertToIsoDayOfWeek(DayOfWeek dow)
        {
            if ((int)dow > 7) return 0; // error or unknown
            switch (dow)
            {
                case DayOfWeek.Sunday: return 7;
                default: return (byte)dow;
            }
        }
        private enum AdjustReason : byte{  ManualTimeUpdate=0, ExternalReferenceTimeUpdate=1, ChangeOfTimeZone=2, ChangeOfDst=3 };
        private IBuffer CreateCurrentTimeBuffer(DateTime now)
        {
            byte[] buffer = new byte[10];
            var ms = new MemoryStream(buffer);
            var writer = new DataWriter(ms.AsOutputStream());
            writer.ByteOrder = ByteOrder.LittleEndian;

            // DateTime section 3.70
            writer.WriteUInt16((ushort)now.Year); // 1582 to 9999
            writer.WriteByte((byte)now.Month); // 1 to 12
            writer.WriteByte((byte)now.Day); // 1 to 31
            writer.WriteByte((byte)now.Hour); // 0 to 23
            writer.WriteByte((byte)now.Minute); // 0 to 59
            writer.WriteByte((byte)now.Second); // 0 to 59 -- this means no leap seconds
            // Add in the Day of Week 3.73 to make this a Day Date Time 3.27
            writer.WriteByte(ConvertToIsoDayOfWeek(now.DayOfWeek));
            // Add in the Fractions to make an Exact Time 256 (3.90)
            writer.WriteByte((byte)(256 * (now.Millisecond / 1000)));
            // Add in the adjust reason to make a Current Time (3.62)
            writer.WriteByte((byte)AdjustReason.ExternalReferenceTimeUpdate);

            return writer.DetachBuffer();
        }
        private async void CurrentTimeCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();

            // Our familiar friend - DataWriter.

            var request = await args.GetRequestAsync();
            var now = DateTime.Now;
            var buffer = CreateCurrentTimeBuffer(now);
            request.RespondWithValue(buffer);

            // See https://xkcd.com/1179/ for ISO 8601 with space, not T
            var time = $"{now.Year}-{now.Month}-{now.Day} {now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";
            uiCurrentTime.Text = time;

            deferral.Complete();
        }



        private void OnStopServer(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider == null) return;
            ServiceProvider.StopAdvertising();
            ServiceProvider = null;
        }
    }
}
