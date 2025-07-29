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
        /// 
        /// RootPath is what we would use if the ms-apps: scheme was actually supported.
        /// Instead HostRootPath is needed because WebView2 doesn't support the clearly documented ms-appx: scheme that we were all
        /// told to use to load local assets. ms-appx isn't supported because the WebView2 developers and PMs are idiots,
        /// and don't mind that us developers have to waste hours of time researching why something doesn't
        /// work instead of just documented it and making a better error message.
        //String RootPath = "ms-appx:///Assets/SimpleMapLeaflet/SimpleMapLeaflet.html";
        String RealMapHtml = "http://msappxreplacement/SimpleMapLeaflet.html";
        String UserMustInitializePrivacyPage = "http://msappxreplacement/UserMustInitializePrivacy.html";
        String LoadingMapPage = "http://msappxreplacement/LoadingMap.html";
        String UserHasBlockedThisMapPage = "http://msappxreplacement/UserHasBlockedThisMap.html";
        String UserHasBlockedAllMapsPage = "http://msappxreplacement/UserHasBlockedAllMaps.html";
        String AssetLocation = "Assets/SimpleMapLeaflet";

        List<MapDataItem> MapData = new List<MapDataItem>();

        public UserMapPrivacyPreferences UserMapPrivacyPreferences { get; set; } = null;
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
            Log($"DBG: Navigate started: kind={args.NavigationKind} uri={args.Uri}");
        }



        //This code should work, but because ms-appx: doesn't work, the loading fails. 
        //var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(RootPath));
        //var content = await FileIO.ReadTextAsync(htmlFile);

        public async Task PrivacyUpdated()
        {
            await InitializeWithLoadingPage();
        }

        private async Task InitializeWithLoadingPage()
        {
            await uiWebView.EnsureCoreWebView2Async();
            uiWebView.CoreWebView2.SetVirtualHostNameToFolderMapping("msappxreplacement", AssetLocation, Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

            if (UserMapPrivacyPreferences == null)
            {
                uiWebView.Source = new Uri(UserMustInitializePrivacyPage);
            }
            else if (!UserMapPrivacyPreferences.UserHasPickedPrivacySettings)
            {
                uiWebView.Source = new Uri(UserMustInitializePrivacyPage);
            }
            else if (UserMapPrivacyPreferences.AllowOpenStreetMapIsBlocked)
            {
                // User allows this page, but they have disallowed all maps.
                uiWebView.Source = new Uri(UserHasBlockedAllMapsPage);
            }
            else if (UserMapPrivacyPreferences.DisableAll3rdPartyServices)
            {
                uiWebView.Source = new Uri(UserHasBlockedAllMapsPage);
            }
            else if (UserMapPrivacyPreferences.AllowOpenStreetMap)
            {
                uiWebView.Source = new Uri(LoadingMapPage);
                await Task.Delay(1000); // Give the user a chance to see the loading page.
                uiWebView.Source = new Uri(RealMapHtml);
            }
            else // This isn't really possible.
            {
                Log("Error: Leaflet: the UserMapPrivacyPreferences is not set up correctly. ");
                uiWebView.Source = new Uri(UserHasBlockedThisMapPage);
            }
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
