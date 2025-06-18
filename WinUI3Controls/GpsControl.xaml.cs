using BluetoothDeviceController.SerialPort;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TestNmeaGpsParserWinUI;
using Utilities;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;

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
                                break;
                            case ConnectionSubstate.SfdCompletedOk:
                                uiStatus.Text = "Scanning OK";
                                break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                    case ConnectionState.VerifyDeviceCapabilities:
                        switch (substate)
                        {
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
                                break;
                            case ConnectionSubstate.CtdCompletedOk:
                                uiStatus.Text = "Connected";
                                break;
                            default:
                                handled = false;
                                break;
                        }
                        break;
                    case ConnectionState.SendingAndReceiving:
                        switch (substate)
                        {
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
    }
}
