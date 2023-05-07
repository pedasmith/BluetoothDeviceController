using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BTControls
{
    public enum AnnunciatorActivity { Initial, 
        ScanStarted, ScanStopped, ScanComplete, ScanItemRemoved, ScanItemUpdated, ScanItemAdded, ScanItemFound, ScanTimerTick,
        ConnectionStarted, ConnectionStoppedFailed, ConnectionSucceeded
    }
    public enum AnnunciatorStatus { Other, NoDeviceFound, Scanning, Connecting, Connected, CantConnect, ConnectionLost };
    public interface IBTAnnunciator
    {
        void Activity(AnnunciatorActivity value);
        void SetText(string str);
        void SetStatus(AnnunciatorStatus status, string text="");
    }
    public sealed partial class BTAnnunciator : UserControl, IBTAnnunciator
    {
        public string DeviceType = "keyboard";
        public string DeviceName = "keyboard";

        public static string[] TimerList = { "🁫", "🁳", "🁻", "🂃", "🂋", "🂓" };
        public static BTAnnunciator Singleton = null;
        public BTAnnunciator()
        {
            this.InitializeComponent();
            if (Singleton != null)
            {
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR: TWO SINGLETONS FOR BTAnnunciator! This is a programming error");
            }
            Singleton = this;
        }
        // State variables
        int nextTimerAnimation = 0;
        int nItemsFound = 0;
        int nItemsAdded = 0;
        int nItemsConnected = 0;
        public void Activity (AnnunciatorActivity value)
        {
            switch (value)
            {
                case AnnunciatorActivity.Initial:
                    SetStatus(AnnunciatorStatus.Other, "Starting");
                    nextTimerAnimation = 0;
                    break;
                case AnnunciatorActivity.ScanStarted:
                    SetStatus(AnnunciatorStatus.Scanning, $"Scanning for {DeviceType}");
                    nextTimerAnimation = 0;
                    break;
                case AnnunciatorActivity.ScanItemAdded:
                    nItemsAdded++;
                    TimerBump();
                    break;
                case AnnunciatorActivity.ScanItemRemoved:
                case AnnunciatorActivity.ScanItemUpdated:
                case AnnunciatorActivity.ScanTimerTick:
                    TimerBump();
                    break;
                case AnnunciatorActivity.ScanItemFound:
                    nItemsFound++;
                    break;

                case AnnunciatorActivity.ScanStopped: // subtle differences between these two ? :-)
                case AnnunciatorActivity.ScanComplete:
                    if (nItemsFound > 0)
                    {
                        // Good news: found it!
                    }
                    else
                    {
                        // Bad new: did not find keyboard
                        SetStatus(AnnunciatorStatus.NoDeviceFound, $"Unable to find {DeviceType}");
                        TimerClear();
                    }
                    break;

                case AnnunciatorActivity.ConnectionStarted:
                    SetStatus(AnnunciatorStatus.Connecting, "Connecting");
                    break;
                case AnnunciatorActivity.ConnectionSucceeded:
                    nItemsConnected++;
                    TimerClear();
                    SetStatus(AnnunciatorStatus.Connected, $"Connected to {DeviceName}");
                    break;
                case AnnunciatorActivity.ConnectionStoppedFailed:
                    TimerClear();
                    if (nItemsConnected == 0)
                    {
                        SetStatus(AnnunciatorStatus.CantConnect, $"Unable to connect to {DeviceName}");
                    }
                    else
                    {
                        SetStatus(AnnunciatorStatus.ConnectionLost, "Connection lost");
                    }
                    break;
            }
        }

        private void TimerBump()
        {
            Utilities.UIThreadHelper.CallOnUIThread(() =>
            {
                uiTimer.Text = TimerList[nextTimerAnimation];
                nextTimerAnimation = (nextTimerAnimation + 1) % TimerList.Length;
            });
        }
        private void TimerClear ()
        {
            Utilities.UIThreadHelper.CallOnUIThread(() =>
            {
                uiTimer.Text = "";
                nextTimerAnimation = 0;
            });
        }

        public void SetText(string str)
        {
            Utilities.UIThreadHelper.CallOnUIThread(() =>
            {
                uiText.Text = str;
            });
        }
        SolidColorBrush BackgroundRed = new SolidColorBrush(Colors.LightPink);
        SolidColorBrush BackgroundYellow = new SolidColorBrush(Colors.LightYellow);
        SolidColorBrush BackgroundGreen = new SolidColorBrush(Colors.White);
        SolidColorBrush Red = new SolidColorBrush(Colors.DarkRed);
        SolidColorBrush Yellow = new SolidColorBrush(Colors.DarkGoldenrod);
        SolidColorBrush Green = new SolidColorBrush(Colors.DarkGreen);
        public void SetStatus(AnnunciatorStatus status, string text = "")
        {
            switch (status)
            {
                case AnnunciatorStatus.Other: SetStatus("", Green); break; // APPBAR GLYPH MOBBLUETOOTH
                case AnnunciatorStatus.NoDeviceFound: SetStatus("", Yellow); break; // APPBAR GLYPH MOBBLUETOOTH
                case AnnunciatorStatus.Scanning: SetStatus("𝇛", Yellow); break; // SCANDICUS FLEXUS
                case AnnunciatorStatus.Connecting: SetStatus("⟳", Yellow); break; // APPBAR GLYPH MOBBLUETOOTH
                case AnnunciatorStatus.Connected: SetStatus("", Green); break; // APPBAR GLYPH NETWORKCONENCTEDCHECKMARK
                case AnnunciatorStatus.CantConnect: SetStatus("", Red); break; // APPBAR GLYPH SIGNALNOTCONNECTED
                case AnnunciatorStatus.ConnectionLost: SetStatus("", Red); break; // APPBAR GLYPH SIGNALNOTCONNECTED
            }
            SetText(text);
        }
        private void SetStatus(string str, Brush brush, Brush backgroundBrush = null)
        {
            if (backgroundBrush == null)
            {
                if (brush == Red) backgroundBrush = BackgroundRed;
                else if (brush == Yellow) backgroundBrush = BackgroundYellow;
                else backgroundBrush = BackgroundGreen;
            }
            Utilities.UIThreadHelper.CallOnUIThread(() =>
            {
                uiStatus.Text = str;
                uiStatus.Foreground = brush;
                uiBorder.BorderBrush = brush;
                uiBorder.Background = backgroundBrush;
            });
        }
    }
}
