using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SampleServerXaml
{
    public sealed partial class CurrentTimeServer : UserControl
    {
        FillBtUnits _fillBtUnits = null;
        public FillBtUnits FillBtUnits
        {
            get { return _fillBtUnits; }
            set
            {
                _fillBtUnits = value;
                _fillBtUnits.OnPreferredUnitsChanged += _fillBtUnits_OnPreferredUnitsChanged;
            }
        }

        private async void _fillBtUnits_OnPreferredUnitsChanged(object sender, EventArgs e)
        {
            await DoUpdateUserUnitsPreferences(null);
        }

        public CurrentTimeServer()
        {
            this.InitializeComponent();
            this.Loaded += CurrentTimeServer_Loaded;
        }

        private async void CurrentTimeServer_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            uiClear.Visibility = Visibility.Visible;
#endif
            await DoStartServer(); // Really start it? Or wait until the units are filled in?
        }

        // Thread UpdateTimeThread = null;
        Task UpdateTimeTask = null;

        GattServiceProvider ServiceProvider = null;



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

        GattLocalCharacteristic CurrentTimeCharacteristic = null;
        GattLocalCharacteristic UserUnitPreferencesCharacteristic = null;
        private async Task DoStartServer()
        {
            var timeServiceUuid = Guid.Parse("00001805-0000-1000-8000-00805f9b34fb");
            var currentTimeUuid = Guid.Parse("00002A2B-0000-1000-8000-00805f9b34fb");
            var userUnitPreferencesUuid = BtUnits.UserUnitPreferenceGuid; // FYI: most likely 0x8020

            GattServiceProviderResult result = await GattServiceProvider.CreateAsync(timeServiceUuid);
            if (result.Error != Windows.Devices.Bluetooth.BluetoothError.Success)
            {
                Log($"Error: GattServiceProvider.CreateAsync failed with {result.Error}");
                return;
            }
            ServiceProvider = result.ServiceProvider;

            //
            // Make the CurrentTime characteristic
            //
            var currentTimeParameters = new GattLocalCharacteristicParameters();
            currentTimeParameters.WriteProtectionLevel = GattProtectionLevel.Plain;
            currentTimeParameters.UserDescription = "Current time (year/month/day/h/m/s and more)";
            currentTimeParameters.ReadProtectionLevel = GattProtectionLevel.Plain;
            currentTimeParameters.CharacteristicProperties = GattCharacteristicProperties.Read | GattCharacteristicProperties.Notify;
            var cresult = await ServiceProvider.Service.CreateCharacteristicAsync(currentTimeUuid, currentTimeParameters);
            if (cresult.Error != Windows.Devices.Bluetooth.BluetoothError.Success)
            {
                Log($"Error: GattServiceProvider.CreateAsync for time failed with {cresult.Error}");
                return;
            }
            CurrentTimeCharacteristic = cresult.Characteristic;
            CurrentTimeCharacteristic.ReadRequested += CurrentTimeCharacteristic_ReadRequested;
            CurrentTimeCharacteristic.SubscribedClientsChanged += CurrentTimeCharacteristic_SubscribedClientsChanged;

            //
            // Make the UserUnitPreferences characteristic
            //
            var userUnitPreferencesParameters = new GattLocalCharacteristicParameters();
            userUnitPreferencesParameters.WriteProtectionLevel = GattProtectionLevel.Plain;
            userUnitPreferencesParameters.UserDescription = "User display preferences (meter/foot, etc.)";
            userUnitPreferencesParameters.ReadProtectionLevel = GattProtectionLevel.Plain;
            userUnitPreferencesParameters.CharacteristicProperties = GattCharacteristicProperties.Read | GattCharacteristicProperties.Notify;
            cresult = await ServiceProvider.Service.CreateCharacteristicAsync(userUnitPreferencesUuid, userUnitPreferencesParameters);
            if (cresult.Error != Windows.Devices.Bluetooth.BluetoothError.Success)
            {
                Log($"Error: GattServiceProvider.CreateAsync for unit preferences failed with {cresult.Error}");
                return;
            }
            UserUnitPreferencesCharacteristic = cresult.Characteristic;
            UserUnitPreferencesCharacteristic.ReadRequested += UserUnitPreferencesCharacteristic_ReadRequested;
            UserUnitPreferencesCharacteristic.SubscribedClientsChanged += UserUnitPreferencesCharacteristic_SubscribedClientsChanged;


            //
            // Now start advertising!
            //
            var advParameters = new GattServiceProviderAdvertisingParameters
            {
                IsDiscoverable = true,
                IsConnectable = true
            };
            ServiceProvider.StartAdvertising(advParameters);

            if (UpdateTimeTask == null)
            {
                UpdateTimeTask = Task.Run(UpdateTimeOnThread);
            }
        }

        // Track the open clients for each characteristic. These
        // will be used by the Notify call.

        IReadOnlyList<GattSubscribedClient> TimeClientList = null;
        IReadOnlyList<GattSubscribedClient> UserUnitsPreferencesClientList = null;

        //
        // Everything for the UserUnitPreferencesCharacteristic
        //

        private void UserUnitPreferencesCharacteristic_SubscribedClientsChanged(GattLocalCharacteristic sender, object args)
        {
            UserUnitsPreferencesClientList = sender.SubscribedClients;
        }

        private async void UserUnitPreferencesCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            GattReadRequest request = await args.GetRequestAsync();
            await DoUpdateUserUnitsPreferences(request);
            deferral.Complete();
        }


        private async Task DoUpdateUserUnitsPreferences(GattReadRequest readRequest)
        {
            if (FillBtUnits == null) return; // shouldn't actually happen

            var units = FillBtUnits.FillBtUnits(new BtUnits());
            var buffer = units.WriteToBuffer();
            if (readRequest != null)
            {
                try
                {
                    readRequest.RespondWithValue(buffer);
                }
                catch (Exception)
                {
                    ;
                }
            }
            if (UserUnitsPreferencesClientList != null && UserUnitsPreferencesClientList.Count > 0)
            {
                try
                {
                    await UserUnitPreferencesCharacteristic.NotifyValueAsync(buffer);
                }
                catch (Exception)
                {
                    ;
                }
            }
        }


        //
        // UX methods for updating time
        //

        private async Task UpdateTimeOnThread()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(1000); // Wait 1 second
                    if (ServiceProvider != null)
                    {
                        // Don't update if we're not actually providing a service.
                        await DoUpdateTime(null); // not in response to a read. But will update notify.
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: UpdateTimeOnThread: e={e.Message}");
                }
            }
        }

        //
        // Everything for the CurrentTimeCharacteristic
        //


        private static byte ConvertToIsoDayOfWeek(DayOfWeek dow)
        {
            if ((int)dow > 7) return 0; // error or unknown
            switch (dow)
            {
                case DayOfWeek.Sunday: return 7;
                default: return (byte)dow;
            }
        }
        private enum AdjustReason : byte { ManualTimeUpdate = 0, ExternalReferenceTimeUpdate = 1, ChangeOfTimeZone = 2, ChangeOfDst = 3 };
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
        private void CurrentTimeCharacteristic_SubscribedClientsChanged(GattLocalCharacteristic sender, object args)
        {
            TimeClientList = sender.SubscribedClients;
        }

        private async void CurrentTimeCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            GattReadRequest request = await args.GetRequestAsync();
            await DoUpdateTime(request);
            deferral.Complete();
        }

        private async Task DoUpdateTime(GattReadRequest readRequest)
        {
            var now = DateTime.Now;
            var buffer = CreateCurrentTimeBuffer(now);
            bool updatedBT = false; // Used when updating display to show the last update time
            if (readRequest != null)
            {
                updatedBT = true;
                try
                {
                    readRequest.RespondWithValue(buffer);
                }
                catch (Exception)
                {
                    ;
                }
            }
            if (TimeClientList != null && TimeClientList.Count > 0)
            {
                try
                {
                    updatedBT = true;
                    await CurrentTimeCharacteristic.NotifyValueAsync(buffer);
                }
                catch (Exception)
                {
                    ;
                }
            }

            UIThreadHelper.CallOnUIThread(() =>
            {
                // See https://xkcd.com/1179/ for ISO 8601 with space, not T
                var time = $"{now.Year}-{now.Month:D2}-{now.Day:D2} {now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";
                uiCurrentTime.Text = time;
                if (updatedBT)
                {
                    uiAllReadValues.Visibility = Visibility.Visible;
                    uiLastReadTime.Text = time;
                }
            });

        }

        //
        // UX calls
        //

        private void OnStopServer(object sender, RoutedEventArgs e)
        {
            if (ServiceProvider == null) return;
            TimeClientList = null;
            UserUnitsPreferencesClientList = null;
            ServiceProvider.StopAdvertising();
            ServiceProvider = null;
        }
    }
}
