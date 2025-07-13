using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using static WinUI3Controls.SimpleMapControl;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{
    public sealed partial class SimpleMapLeafletControl : UserControl, IDoGpsMap
    {
        List<MapDataItem> MapData = new List<MapDataItem>();
        public SimpleMapLeafletControl()
        {
            this.InitializeComponent();
            this.Loaded += SimpleMapLeafletControl_Loaded;
        }

        private async void SimpleMapLeafletControl_Loaded(object sender, RoutedEventArgs e)
        {
            uiWebView.NavigationStarting += UiWebView_NavigationStarting;
            uiWebView.NavigationCompleted += UiWebView_NavigationCompleted;
            uiWebView.WebMessageReceived += UiWebView_WebMessageReceived;

            await InitializeWithLoadingPage();
        }

        private void UiWebView_WebMessageReceived(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            Log($"DBG: Web message: {args.WebMessageAsJson}");
        }

        private void UiWebView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            Log($"DBG: Navigate complete: http={args.HttpStatusCode} succ={args.IsSuccess}");
        }

        private void UiWebView_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Log($"DBG: Navigate startd: kind={args.NavigationKind} uri={args.Uri}");
        }

        String RootPath = "ms-appx:///Assets/SimpleMapLeaflet/SimpleMapLeaflet.html";
        String HostRootPath = "http://msappxreplacement/SimpleMapLeaflet.html";

        /// <summary>
        /// Because WebView2 doesn't support the clearly documented ms-appx: scheme that we were all
        /// told to use to load local assets. That's because the WebView2 developers and PMs are idiots,
        /// and don't mind that us developers have to waste hours of time researching why something doesn't
        /// work instead of just documented it and making a better error message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnLoadWebViaHost(object sender, RoutedEventArgs e)
        {
            await uiWebView.EnsureCoreWebView2Async();
            uiWebView.CoreWebView2.SetVirtualHostNameToFolderMapping("msappxreplacement", "Assets/SimpleMapLeaflet", Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

            uiWebView.Source = new Uri(HostRootPath);
        }

#if NEVER_EVER_DEFINED
        private async void OnLoadWebNavigate(object sender, RoutedEventArgs e)
        {
            await uiWebView.EnsureCoreWebView2Async();

            var fileUri = new Uri(RootPath).AbsoluteUri;
            uiWebView.CoreWebView2.Navigate(fileUri);
        }

        private async void OnLoadWebSource(object sender, RoutedEventArgs e)
        {
            await uiWebView.EnsureCoreWebView2Async();

            var fileUri = new Uri(RootPath).AbsoluteUri;

            //
            //var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
            //var content = await FileIO.ReadTextAsync(htmlFile);
            uiWebView.Source = new Uri(RootPath);
        }

        private async void OnLoadWebFromString(object sender, RoutedEventArgs e)
        {
            await uiWebView.EnsureCoreWebView2Async();

            var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(RootPath));
            var content = await FileIO.ReadTextAsync(htmlFile);
            uiWebView.NavigateToString(content);
        }
        private async void OnLoadWebFromStringClear(object sender, RoutedEventArgs e)
        {
            await uiWebView.EnsureCoreWebView2Async();

            var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(RootPath));
            var content = await FileIO.ReadTextAsync(htmlFile);
            uiWebView.NavigateToString("<b>This page intentionally left blank</b>");
        }

#endif

        private async Task InitializeWithLoadingPage()
        {
            await uiWebView.EnsureCoreWebView2Async();

            var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(RootPath));
            var content = await FileIO.ReadTextAsync(htmlFile);
            uiWebView.NavigateToString("<marquee>...Loading map...</marquee>");

            // Now load for realsies
            uiWebView.CoreWebView2.SetVirtualHostNameToFolderMapping("msappxreplacement", "Assets/SimpleMapLeaflet", Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            uiWebView.Source = new Uri(HostRootPath);
        }

        private void Log(string message)
        {
            uiLog.Text = message;
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Add an NMEA GPRMS data item to the map.
        /// </summary>
        public async Task<int> AddNmea(GPRMC_Data gprc, LogLevel logLevel = LogLevel.Normal)
        {
            int nsegments = 0;
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
                        Log($"AddNmea: calculating new point distance={distance} ");
                        MapData.Add(data);

                        // TODO: Update Drawing
                    }
                    else
                    {
                        if (uiWebView.CoreWebView2 != null)
                        {
                            string script = $"AddNmea({gprc.Latitude.AsDecimal}, {gprc.Longitude.AsDecimal}, '{gprc.SummaryString}')";
                            Log($"Leaflet: script={script}");
                            await uiWebView.CoreWebView2.ExecuteScriptAsync(script);
                        }
                    }
                        break;
                default:
                    Log($"Error: AddNmea: Sample point parse {gprc.ParseStatus} data {gprc.OriginalNmeaString}");
                    break;
            }
            return nsegments;
        }
    }
}
