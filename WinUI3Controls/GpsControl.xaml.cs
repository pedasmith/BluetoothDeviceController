using BluetoothDeviceController.SerialPort;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TestNmeaGpsParserWinUI;
using Utilities;
using Windows.ApplicationModel.DataTransfer;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{
    public sealed partial class GpsControl : UserControl, ITerminal
    {
        public ObservableCollection<NmeaMessageSummary> MessageHistory { get; } = new ObservableCollection<NmeaMessageSummary>();
        private Nmea_Gps_Parser NmeaParser { get; }  = new Nmea_Gps_Parser();
        public GpsControl()
        {
            this.InitializeComponent();
            this.DataContext = this;

            NmeaParser.OnGpggaOk += NmeaParser_OnGpggaOk;
            NmeaParser.OnGpgllOk += NmeaParser_OnGpgllOk;
            NmeaParser.OnGppwrOk += NmeaParser_OnGppwrOk;
            NmeaParser.OnGprmcOk += NmeaParser_OnGprmcOk;
            NmeaParser.OnGpvtgOk += NmeaParser_OnGpvtgOk;
            NmeaParser.OnGpzdaOk += NmeaParser_OnGpzdaOk;

            NmeaParser.OnNmeaAll += NmeaParser_OnNmeaAll;
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
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiNmeaCommands.Text += e.OriginalNmeaString + "\r\n";
                }
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


        private void NmeaParser_OnGpggaOk(object sender, GPGGA_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiLatLongUpdateTime.Text = $"{e.Time}";
                    uiLatitude.Text = $"{e.Latitude}";
                    uiLongitude.Text = $"{e.Longitude}";
                    
                    uiUpdateSource.Text = $"{e.OpcodeString}";
                }
            });
        }

        private void NmeaParser_OnGpgllOk(object sender, GPGLL_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiMode.Text = $"{e.Mode}";
                    uiLatLongUpdateTime.Text = $"{e.Time}";
                    uiLatitude.Text = $"{e.Latitude}";
                    uiLongitude.Text = $"{e.Longitude}";
                    uiUpdateSource.Text = $"{e.OpcodeString}";
                }
            });
        }


        private void NmeaParser_OnGppwrOk(object sender, GPPWR_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiChargingStatus.Text = e.ChargingStatus == GPPWR_Data.ChargingStatusType.NotCharging ? "Not charging" : "Charging";
                    uiVoltage.Text = e.Voltage.ToString("F2");
                }
            });
        }

        private void NmeaParser_OnGprmcOk(object sender, GPRMC_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiMode.Text = $"{e.Mode}";
                    uiLatLongUpdateDate.Text = $"{e.Date}";
                    uiLatLongUpdateTime.Text = $"{e.Time}";
                    uiLatitude.Text = $"{e.Latitude}";
                    uiLongitude.Text = $"{e.Longitude}";
                    uiUpdateSource.Text = $"{e.OpcodeString}";

                    uiVelocityKnots.Text = $"{e.VelocityKnots}";
                    uiHeadingTrue.Text = $"{e.HeadingDegreesTrue}";
                    uiMagneticVariation.Text = $"{e.MagneticVariation}";

                    uiSimpleMapV1.AddNmea(e);
                }
            });
        }




        private void NmeaParser_OnGpvtgOk(object sender, GPVTG_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiMode.Text = $"{e.Mode}";
                    uiUpdateSource.Text = $"{e.OpcodeString}";

                    uiVelocityKnots.Text = $"{e.SpeedKnots}";
                    uiVelocityKph.Text = $"{e.SpeedKph}";
                    uiHeadingTrue.Text = $"{e.CourseTrue}";
                    uiMagneticVariation.Text = $"{e.CourseMagnetic}";
                }
            });
        }

        private void NmeaParser_OnGpzdaOk(object sender, GPZDA_Data e)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiLatLongUpdateTime.Text = $"{e.Time}";
                    uiUpdateSource.Text = $"{e.OpcodeString}";
                    uiLocalZoneHours.Text = $"{e.LocalZoneHours}";
                    uiLocalZoneMinutes.Text = $"{e.LocalZoneMinutes}";
                }
            });
        }


        private void Log(string message)
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
                if (MainWindow.MainWindowWindow.MainWindowIsClosed == false)
                {
                    uiLog.Text += message + "\n";
                }
            });
        }

        private void LogClear()
        {
            UIThreadHelper.CallOnUIThread(MainWindow.MainWindowWindow, () =>
            {
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

                                LogClear();
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
    }
}
