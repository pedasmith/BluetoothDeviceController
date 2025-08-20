using BluetoothDeviceController.SerialPort;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TestNmeaGpsParserWinUI;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.System.Display;
using static WinUI3Controls.SimpleMapControl;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{
    public interface IDoGpsMap
    {
        //Task AddNmea(GPRMC_Data gprc, LogLevel logLevel = LogLevel.Normal);
        Task MapDataUpdatedGroup(MapDataItem prev, LogLevel logLevel = LogLevel.Normal);
        Task MapDataAddedFirstItem(MapDataItem data, LogLevel logLevel = LogLevel.Normal);
        Task MapDataAddedNewItem(MapDataItem data, LogLevel logLevel = LogLevel.Normal);

        /// <summary>
        /// Clears the current line of GPS locations. There shouldn't be much "data", but 
        /// </summary>
        Task MapDataClear(LogLevel logLevel = LogLevel.Normal);
        Task PrivacyUpdated();
    }

    public sealed partial class GpsControl : UserControl, ITerminal
    {
        public ObservableCollection<NmeaMessageSummary> MessageHistory { get; } = new ObservableCollection<NmeaMessageSummary>();
        private Nmea_Gps_Parser NmeaParser { get; }  = new Nmea_Gps_Parser();
        public GpsControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += GpsControl_Loaded;

            NmeaParser.OnGpggaOk += NmeaParser_OnGpggaOk;
            NmeaParser.OnGpgllOk += NmeaParser_OnGpgllOk;
            NmeaParser.OnGppwrOk += NmeaParser_OnGppwrOk;
            NmeaParser.OnGprmcOk += NmeaParser_OnGprmcOk;
            NmeaParser.OnGpvtgOk += NmeaParser_OnGpvtgOk;
            NmeaParser.OnGpzdaOk += NmeaParser_OnGpzdaOk;

            NmeaParser.OnNmeaAll += NmeaParser_OnNmeaAll;
        }
        /// <summary>
        /// The list of all maps is set up in the Loaded event. It's essentially a static list; it's not intended to be dynamic.
        /// </summary>
        List<IDoGpsMap> AllMaps = new List<IDoGpsMap>();

        /// <summary>
        /// Global variable for handing the display request (i.e., to keep the display on while the app is running).
        /// </summary>
        DisplayRequest CurrDisplayRequest = null;
        bool DisplayRequestActive = false;

        private void GpsControl_Loaded(object sender, RoutedEventArgs e)
        {
            UserPreferences.LogTextBlock = uiLog;

            // Testing the Save code
            App.UP.Restore();

            uiSimpleMapV1.MapData = MapData;
            uiSimpleMapLeaflet.MapData = MapData;
            AllMaps.Add(uiSimpleMapV1);
            AllMaps.Add(uiSimpleMapLeaflet);
            uiSimpleMapLeaflet.UserMapPrivacyPreferences = App.UP.UserMapPrivacyPreferences;

            // Pick the "simple map"
            uiMapSelectionComboBox.SelectedIndex = 1;
        }

        private string CurrHistoryNmeaName = "";
        public void HistoryOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            var item = e.AddedItems[0] as NmeaMessageSummary;
            if (item == null) return;
            uiHistoryMessageDetail.Visibility = Visibility.Visible;
            uiHistoryMessageDetail.DataContext = item.MostRecentData;
            CurrHistoryNmeaName = item.Name;
        }

        private void NmeaParser_OnNmeaAll(object sender, Nmea_Data e)
        {
            var name = e.GetFirstPart();
            var summary = FindSummary(name);
            summary.Add(e); // It's an observablecollection, so everything updates automatically.

            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiNmeaCommands.Text += e.OriginalNmeaString + "\r\n";
            });


            if (name == CurrHistoryNmeaName)
            {
                uiHistoryMessageDetail.Visibility = Visibility.Visible;
                uiHistoryMessageDetail.DataContext = e;
            }
        }

        private NmeaMessageSummary FindSummary(string name)
        {
            NmeaMessageSummary summary;
            for (int i = 0; i < MessageHistory.Count; i++)
            {
                summary = MessageHistory[i];
                if (summary.Name == name)
                {
                    return summary;
                }
            }
            summary = new NmeaMessageSummary(name);
            MessageHistory.Add(summary);

            return summary;
        }

        #region Nmea_Message
        private void NmeaParser_OnGpggaOk(object sender, GPGGA_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiLatLongUpdateTime.Text = $"{e.Time}";
                uiLatitude.Text = $"{e.Latitude}";
                uiLongitude.Text = $"{e.Longitude}";

                uiUpdateSource.Text = $"{e.OpcodeString}";
            });
        }

        private void NmeaParser_OnGpgllOk(object sender, GPGLL_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiMode.Text = $"{e.Mode}";
                uiLatLongUpdateTime.Text = $"{e.Time}";
                uiLatitude.Text = $"{e.Latitude}";
                uiLongitude.Text = $"{e.Longitude}";
                uiUpdateSource.Text = $"{e.OpcodeString}";
            });
        }


        private void NmeaParser_OnGppwrOk(object sender, GPPWR_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiChargingStatus.Text = e.ChargingStatus == GPPWR_Data.ChargingStatusType.NotCharging ? "Not charging" : "Charging";
                uiVoltage.Text = e.Voltage.ToString("F2");
            });
        }

        private void NmeaParser_OnGprmcOk(object sender, GPRMC_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiMode.Text = $"{e.Mode}";
                uiLatLongUpdateDate.Text = $"{e.Date}";
                uiLatLongUpdateTime.Text = $"{e.Time}";
                uiLatitude.Text = $"{e.Latitude}";
                uiLongitude.Text = $"{e.Longitude}";
                uiUpdateSource.Text = $"{e.OpcodeString}";

                uiVelocityKnots.Text = $"{e.VelocityKnots}";
                uiHeadingTrue.Text = $"{e.HeadingDegreesTrue}";
                uiMagneticVariation.Text = $"{e.MagneticVariation}";

                var task = AddNmeaCombined(e);
            });
        }

        private void NmeaParser_OnGpvtgOk(object sender, GPVTG_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiMode.Text = $"{e.Mode}";
                uiUpdateSource.Text = $"{e.OpcodeString}";

                uiVelocityKnots.Text = $"{e.SpeedKnots}";
                uiVelocityKph.Text = $"{e.SpeedKph}";
                uiHeadingTrue.Text = $"{e.CourseTrue}";
                uiMagneticVariation.Text = $"{e.CourseMagnetic}";
            });
        }

        private void NmeaParser_OnGpzdaOk(object sender, GPZDA_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiLatLongUpdateTime.Text = $"{e.Time}";
                uiUpdateSource.Text = $"{e.OpcodeString}";
                uiLocalZoneHours.Text = $"{e.LocalZoneHours}";
                uiLocalZoneMinutes.Text = $"{e.LocalZoneMinutes}";
            });
        }

        #endregion

        private void Log(string message)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiLog.Text += message + "\n";
            });
        }

        private void LogClear()
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == true) return;

                uiLog.Text = "";
            });
        }

        public event TerminalSendDataEventHandler OnSendData;

        public void ErrorFromDevice(string error)
        {
            Log(error);
        }
        void OnNmeaCommandsClear(object sender, RoutedEventArgs e)
        {
            uiNmeaCommands.Text = "";
        }
        void OnNmeaCommandsCopy(object sender, RoutedEventArgs e)
        {
            var text = uiNmeaCommands.Text;
            var dp = new DataPackage();
            dp.RequestedOperation = DataPackageOperation.Copy;
            dp.SetText(text);
            Clipboard.SetContent(dp);
        }

        bool PrivacyOptionsBeingInitialized = true;
        private async void OnMenuFilePrivacyOptions(object sender, RoutedEventArgs e)
        {
            var start = App.UP.ToJson();

            PrivacyOptionsBeingInitialized = true;
            uiPrivacyOptionAllow3rdParty.IsChecked = App.UP.UserMapPrivacyPreferences.Allow3rdPartyServices;
            uiPrivacyOptionAllowOpenStreetMaps.IsChecked = App.UP.UserMapPrivacyPreferences.AllowOpenStreetMapUnderlyingValue;
            uiPrivacyOptionsIndividualExpander.IsExpanded = !App.UP.UserMapPrivacyPreferences.AllThirdPartyServicesAreAllowed;
            var whatToDo = "";
            if (!App.UP.UserMapPrivacyPreferences.Allow3rdPartyServices) whatToDo = "You must check the checkbox to use any 3rd party maps";
            else if (App.UP.UserMapPrivacyPreferences.AllThirdPartyServicesAreAllowed) whatToDo = "You are allowing all 3rd party maps";
            else whatToDo = "You must allow at least one map if you want to use a 3rd party map";

            uiSetPrivacyOptionsWhatToDo.Text = whatToDo;
            PrivacyOptionsBeingInitialized = false;


            var result = await uiSetPrivacyOptions.ShowAsync();
            App.UP.Save();
            var end = App.UP.ToJson();
            if (start != end)
            {
                // There has been a critical update
                foreach (var map in AllMaps)
                {
                    await map.PrivacyUpdated();
                }
            }
        }

        private void OnPrivacyCheckChange(object sender, RoutedEventArgs e)
        {
            if (PrivacyOptionsBeingInitialized) return;

            App.UP.UserMapPrivacyPreferences.AllowOpenStreetMapUnderlyingValue = uiPrivacyOptionAllowOpenStreetMaps.IsChecked ?? false;
            App.UP.UserMapPrivacyPreferences.Allow3rdPartyServices = uiPrivacyOptionAllow3rdParty.IsChecked ?? false; // ?? false is never the case; it's always properly set.
            ;
            if (!App.UP.UserMapPrivacyPreferences.UserHasPickedPrivacySettings)
            {
                App.UP.UserMapPrivacyPreferences.UserHasPickedPrivacySettings = true;
            }
        }

        public void ReceivedData(string message)
        {
            if (message == "") return;

            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                // Must do all this on a UI thread.
                NmeaParser.Parse(message); // all messages from a GPS device will be an NMEA one.
                uiLastMessage.Text = message;
            });
        }

        public void SetDeviceStatus(string status)
        {
            Log(status);
        }
        private int VdcCachedServiceCount = -1;
        private int VdcUncachedServiceCount = -1;
        private string CtdHostName = "";
        private string CtdServiceName = "";

        public void SetDeviceStatusEx(ConnectionState status, ConnectionSubstate substate, string text, double value)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed) return;

                bool handled = true;
                switch (status)
                {
                    case ConnectionState.UX:
                        switch (substate)
                        {
                            case ConnectionSubstate.UXReset:
                                uiStatus.Text = "";
                                uiSubstatus.Text = "";
                                uiIcon.Text = " ";
                                uiReadIcon.Text = " ";

                                // Reset saved values
                                VdcCachedServiceCount = -1;
                                VdcUncachedServiceCount = -1;
                                CtdHostName = "";
                                CtdServiceName = "";

                                //LogClear();
                                break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                    case ConnectionState.ScanningForDevices:
                        switch(substate)
                        {
                            case ConnectionSubstate.SfdStarted:
                                uiStatus.Text = "Scanning...";
                                uiSubstatus.Text = "";
                                uiIcon.Text = "🗲";
                                uiReadIcon.Text = " ";
                                break;
                            case ConnectionSubstate.SfdCompletedOk:
                                uiStatus.Text = "Scanning OK";
                                break;
                            case ConnectionSubstate.SfdException:
                                uiStatus.Text = "Unable to scan";
                                uiIcon.Text = "🛑";
                                uiSubstatus.Text = text;
                                break;
                            case ConnectionSubstate.SfdNoDeviceFound:
                                uiStatus.Text = "No devices found";
                                uiIcon.Text = "🛑";
                                break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                    case ConnectionState.VerifyDeviceCapabilities:
                        switch (substate)
                        {
                            case ConnectionSubstate.VdcStarted:
                                uiStatus.Text = "Checking...";
                                uiSubstatus.Text = "";
                                uiIcon.Text = "🖄";
                                uiReadIcon.Text = " ";
                                break;
                            case ConnectionSubstate.VdcGettingDevice:
                                uiStatus.Text = "Checking...getting device";
                                break;
                            case ConnectionSubstate.VdcGotDevice:
                                uiStatus.Text = "Checking...got device";
                                break;
                            case ConnectionSubstate.VdcReusingDevice:
                                uiStatus.Text = "Checking...reusing device";
                                break;
                            case ConnectionSubstate.VdcCachedServiceCount:
                                VdcCachedServiceCount = (int)value;
                                break;
                            case ConnectionSubstate.VdcUncachedServiceCount:
                                VdcUncachedServiceCount = (int)value;
                                break;
                            case ConnectionSubstate.VdcCompletedOk:
                                uiStatus.Text = "Device OK";
                                break;

                            case ConnectionSubstate.VdcNoDevice:
                                uiStatus.Text = "Device is not a device";
                                uiIcon.Text = "🛑";
                                break;
                            case ConnectionSubstate.VdcNoServices:
                                uiStatus.Text = "Device has no COM services";
                                uiIcon.Text = "🛑";
                                break;
                            case ConnectionSubstate.VdcException:
                                uiStatus.Text = "Unable to check";
                                uiIcon.Text = "🛑";
                                uiSubstatus.Text = text;
                                break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                    case ConnectionState.ConnectingToDevice:
                        switch (substate)
                        {
                            case ConnectionSubstate.CtdStarted:
                                uiStatus.Text = "Connecting...";
                                uiSubstatus.Text = "";
                                uiIcon.Text = "🗱";
                                uiReadIcon.Text = " ";
                                break;
                            case ConnectionSubstate.CtdCompletedOk:
                                uiStatus.Text = "Connected";
                                break;
                            case ConnectionSubstate.CtdHostName:
                                CtdHostName = text;
                                break;
                            case ConnectionSubstate.CtdServiceName:
                                CtdServiceName = text;
                                break;
                            case ConnectionSubstate.CtdException:
                                uiStatus.Text = "Unable to connect";
                                uiSubstatus.Text = text;
                                uiIcon.Text = "🛑";
                                break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                    case ConnectionState.SendingAndReceiving:
                        switch (substate)
                        {
                            case ConnectionSubstate.SRStarted:
                                uiSubstatus.Text = "";
                                uiStatus.Text = "Ready";
                                uiIcon.Text = "✔";
                                uiReadIcon.Text = " ";
                                break;
                            case ConnectionSubstate.SRWaitingForData:
                                uiReadIcon.Text = "⚪";
                                break;
                            case ConnectionSubstate.SRGotData:
                                uiReadIcon.Text = "⚫";
                                break;
                            case ConnectionSubstate.SRException:
                                uiReadIcon.Text = "⛒";
                                uiSubstatus.Text = text;
                                break;
                            case ConnectionSubstate.SRCancelled:
                                uiReadIcon.Text = "⦵";
                                break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                    default:
                        handled = false;
                        break;
                }

                if (!handled)
                {
                    var icon = TerminalSupport.StateAsIcon(status, substate);
                    uiIcon.Text = icon;
                    uiSubstatus.Text = TerminalSupport.StateAsString(status, substate, value);
                    uiStatus.Text = status.ToString();
                }
            });
        }

        private void OnMapSelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            FrameworkElement visible = null;
            FrameworkElement[] list = { uiSimpleMapV1, uiSimpleMapLeaflet };
            var tag = (e.AddedItems[0] as FrameworkElement).Tag as string;
            switch (tag)
            {
                default:
                case "simpleV1": visible = uiSimpleMapV1; break;
                case "leaflet": visible = uiSimpleMapLeaflet; break;
            }
            if (visible == null) return;
            foreach (var item in list)
            {
                item.Visibility = item == visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }


        private void OnMenuDeveloperSaveOptions(object sender, RoutedEventArgs e)
        {
            App.UP.Save();
        }

        /// <summary>
        /// This method exists so that the compiler doesn't complain about the OnSendData event which is 
        /// otherwise unused. The OnSendData is designed so that a user can send a message to the GPS unit. 
        /// These are all customized to the particular chipset, and aren't always documented.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuDeveloperSendData(object sender, RoutedEventArgs e)
        {
            var text = "#Fake NMea Command\r\n";
            Log($"Sending: {text}");
            OnSendData?.Invoke(this, text);
        }

        private void OnMenuDeveloperScreenOn(object sender, RoutedEventArgs e)
        {
            var isActive = (sender as ToggleMenuFlyoutItem).IsChecked; // this will be the new state
            if (isActive)
            {
                EnsureDisplayRequestActive();
            }
            else
            {
                EnsureDisplayRequestRelease();
            }
        }

        private void EnsureDisplayRequestActive()
        {
            if (DisplayRequestActive) return;
            if (CurrDisplayRequest == null)
            {
                CurrDisplayRequest = new DisplayRequest();
            }
            CurrDisplayRequest.RequestActive();
            DisplayRequestActive = true;
        }

        private void EnsureDisplayRequestRelease()
        {
            if (!DisplayRequestActive) return;
            if (CurrDisplayRequest == null)
            {
                CurrDisplayRequest = new DisplayRequest();
            }
            CurrDisplayRequest.RequestRelease();
            DisplayRequestActive = false;
        }

        private async void OnMenuDeveloperAddSamplePoints(object sender, RoutedEventArgs e)
        {
            var lines = SampleNmea[CurrSettings.CurrSampleNmea % SampleNmea.Length].Split("\r\n");
            foreach (var line in lines)
            {
                var gprc = new GPRMC_Data(line);
                gprc.Latitude.LatitudeMinutesDecimal += (CurrSettings.CurrSampleNmea * 50);
                await AddNmeaCombined(gprc, LogLevel.None);
            }

            // Boop the index so next time the button is pressed we get the next set of data.
            Log($"Add sample points: {CurrSettings.CurrSampleNmea}");
            CurrSettings.CurrSampleNmea += 1;
        }

        const double GROUPING_DISTANCE = 0.0005 / 60.0; // about 5*1.8 meters
        List<MapDataItem> MapData = new List<MapDataItem>();
        /// <summary>
        /// Add an NMEA GPRMS data item to the map.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AddNmeaCombined(GPRMC_Data gprc, LogLevel logLevel = LogLevel.Normal)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            switch (gprc.ParseStatus)
            {
                case Nmea_Gps_Parser.ParseResult.Ok:
                    var data = new MapDataItem(gprc);
                    if (MapData.Count >= 1)
                    {
                        // The 0.0001 is a ten-thousandth of a minute. But the distances
                        // are in DD, so I need something a good bit smaller.
                        var prev = MapData[MapData.Count - 1];
                        var distance = prev.Distance(data);
                        Log($"AddNmea: calculating new point distance={distance} (grouping distance is {GROUPING_DISTANCE})");
                        if (distance < GROUPING_DISTANCE)
                        {
                            prev.GroupedNmea.Add(gprc); // and I'll just abandon the new MapDataItem
                            foreach (var map in AllMaps)
                            {
                                await map.MapDataUpdatedGroup(prev, logLevel);
                            }
                        }
                        else // new last point
                        {
                            MapData.Add(data);
                            foreach (var map in AllMaps)
                            {
                                await map.MapDataAddedNewItem(data, logLevel);
                            }

                        }
                    }
                    else // is first point
                    {
                        MapData.Add(data);
                        foreach (var map in AllMaps)
                        {
                            await map.MapDataAddedFirstItem(data, logLevel);
                        }
                    }
                    break;
                default:
                    Log($"Error: AddNmea: Sample point parse {gprc.ParseStatus} data {gprc.OriginalNmeaString}");
                    break;
            }
        }

        private void OnMenuMapClearPoints(object sender, RoutedEventArgs e)
        {
            foreach (var map in AllMaps)
            {
                map.MapDataClear();
            }
        }



        class UserSettings
        {
            public int CurrSampleNmea{ get; set; } = 0; // Index into SampleNmea
        }
        UserSettings CurrSettings = new UserSettings();

        private string[] SampleNmea = new string[] {
            @"$GPRMC,184446.000,A,4321.6036,N,12345.4008,W,000.0,338.7,200625,,,A",

            @"$GPRMC,184446.000,A,4321.6036,N,12345.4008,W,000.0,338.7,200625,,,A
$GPRMC,184451.000,A,4321.6033,N,12345.4012,W,000.9,125.1,200625,,,A
$GPRMC,184504.000,A,4321.6030,N,12345.4008,W,000.0,126.8,200625,,,A
$GPRMC,184508.000,A,4321.6033,N,12345.4004,W,001.0,069.3,200625,,,A
$GPRMC,184509.000,A,4321.6036,N,12345.4008,W,001.1,067.8,200625,,,A
$GPRMC,184510.000,A,4321.6032,N,12345.4003,W,001.4,076.4,200625,,,A
$GPRMC,184511.000,A,4321.6035,N,12345.4078,W,001.8,050.0,200625,,,A
$GPRMC,184512.000,A,4321.6039,N,12345.4067,W,001.2,062.0,200625,,,A
$GPRMC,184513.000,A,4321.6041,N,12345.4063,W,000.9,065.0,200625,,,A
$GPRMC,184514.000,A,4321.6040,N,12345.4062,W,001.2,121.7,200625,,,A
$GPRMC,184515.000,A,4321.6035,N,12345.4057,W,002.5,070.8,200625,,,A
$GPRMC,184516.000,A,4321.6035,N,12345.4045,W,001.5,128.2,200625,,,A
$GPRMC,184517.000,A,4321.6032,N,12345.4040,W,001.5,137.7,200625,,,A
$GPRMC,184518.000,A,4321.6029,N,12345.4036,W,001.5,151.7,200625,,,A
$GPRMC,184519.000,A,4321.6025,N,12345.4033,W,000.0,151.7,200625,,,A
$GPRMC,184520.000,A,4321.6023,N,12345.4032,W,000.0,151.7,200625,,,A
$GPRMC,184521.000,A,4321.6021,N,12345.4033,W,000.0,151.7,200625,,,A
$GPRMC,184522.000,A,4321.6020,N,12345.4033,W,001.2,306.5,200625,,,A
$GPRMC,184523.000,A,4321.6022,N,12345.4037,W,000.0,306.5,200625,,,A",

            @"$GPRMC,184446.000,A,4321.4036,N,12345.4008,W,000.0,338.7,200625,,,A
$GPRMC,184451.000,A,4321.4033,N,12345.4012,W,000.9,125.1,200625,,,A
$GPRMC,184504.000,A,4321.4030,N,12345.4008,W,000.0,126.8,200625,,,A
$GPRMC,184508.000,A,4321.4033,N,12345.4004,W,001.0,069.3,200625,,,A
$GPRMC,184509.000,A,4321.4036,N,12345.4008,W,001.1,067.8,200625,,,A
$GPRMC,184510.000,A,4321.4032,N,12345.4003,W,001.4,076.4,200625,,,A
$GPRMC,184511.000,A,4321.4035,N,12345.4078,W,001.8,050.0,200625,,,A
$GPRMC,184512.000,A,4321.4039,N,12345.4067,W,001.2,062.0,200625,,,A
$GPRMC,184513.000,A,4321.4041,N,12345.4063,W,000.9,065.0,200625,,,A
$GPRMC,184514.000,A,4321.4040,N,12345.4062,W,001.2,121.7,200625,,,A
$GPRMC,184515.000,A,4321.4035,N,12345.4057,W,002.5,070.8,200625,,,A
$GPRMC,184516.000,A,4321.4035,N,12345.4045,W,001.5,128.2,200625,,,A
$GPRMC,184517.000,A,4321.4032,N,12345.4040,W,001.5,137.7,200625,,,A
$GPRMC,184518.000,A,4321.4029,N,12345.4036,W,001.5,151.7,200625,,,A
$GPRMC,184519.000,A,4321.4025,N,12345.4033,W,000.0,151.7,200625,,,A
$GPRMC,184520.000,A,4321.4023,N,12345.4032,W,000.0,151.7,200625,,,A
$GPRMC,184521.000,A,4321.4021,N,12345.4033,W,000.0,151.7,200625,,,A
$GPRMC,184522.000,A,4321.4020,N,12345.4033,W,001.2,306.5,200625,,,A
$GPRMC,184523.000,A,4321.4022,N,12345.4037,W,000.0,306.5,200625,,,A",

            // Same figure, but to the left
            @"$GPRMC,184446.000,A,4321.4036,N,12345.9008,W,000.0,338.7,200625,,,A
$GPRMC,184451.000,A,4321.4033,N,12345.9012,W,000.9,125.1,200625,,,A
$GPRMC,184504.000,A,4321.4030,N,12345.9008,W,000.0,126.8,200625,,,A
$GPRMC,184508.000,A,4321.4033,N,12345.9004,W,001.0,069.3,200625,,,A
$GPRMC,184509.000,A,4321.4036,N,12345.9008,W,001.1,067.8,200625,,,A
$GPRMC,184510.000,A,4321.4032,N,12345.9003,W,001.4,076.4,200625,,,A
$GPRMC,184511.000,A,4321.4035,N,12345.9078,W,001.8,050.0,200625,,,A
$GPRMC,184512.000,A,4321.4039,N,12345.9067,W,001.2,062.0,200625,,,A
$GPRMC,184513.000,A,4321.4041,N,12345.9063,W,000.9,065.0,200625,,,A
$GPRMC,184514.000,A,4321.4040,N,12345.9062,W,001.2,121.7,200625,,,A
$GPRMC,184515.000,A,4321.4035,N,12345.9057,W,002.5,070.8,200625,,,A
$GPRMC,184516.000,A,4321.4035,N,12345.9045,W,001.5,128.2,200625,,,A
$GPRMC,184517.000,A,4321.4032,N,12345.9040,W,001.5,137.7,200625,,,A
$GPRMC,184518.000,A,4321.4029,N,12345.9036,W,001.5,151.7,200625,,,A
$GPRMC,184519.000,A,4321.4025,N,12345.9033,W,000.0,151.7,200625,,,A
$GPRMC,184520.000,A,4321.4023,N,12345.9032,W,000.0,151.7,200625,,,A
$GPRMC,184521.000,A,4321.4021,N,12345.9033,W,000.0,151.7,200625,,,A
$GPRMC,184522.000,A,4321.4020,N,12345.9033,W,001.2,306.5,200625,,,A
$GPRMC,184523.000,A,4321.4022,N,12345.9037,W,000.0,306.5,200625,,,A",

        };

    }
}
