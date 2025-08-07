using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        // Input URL: ms-appx:///Assets/SimpleMapLeaflet/SimpleMapLeaflet.html
        // Updated URL: http://msappxreplacement/SimpleMapLeaflet.html

        String RealMapHtml = "ms-appx:///SimpleMapLeaflet.html"; // Site that reports browser info: "https://www.whatismybrowser.com/"; //
        String UserMustInitializePrivacyPage = "ms-appx:///UserMustInitializePrivacy.html";
        String LoadingMapPage = "ms-appx:///LoadingMap.html";
        String UserHasBlockedThisMapPage = "ms-appx:///UserHasBlockedThisMap.html";
        String UserHasBlockedAllMapsPage = "ms-appx:///UserHasBlockedAllMaps.html";
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
            // These event calls are not needed, but might be useful for debugging.
            //uiWebView.NavigationCompleted += UiWebView_NavigationCompleted;
            //uiWebView.WebMessageReceived += UiWebView_WebMessageReceived;

            await uiWebView.EnsureCoreWebView2Async();
            uiWebView.CoreWebView2.SetVirtualHostNameToFolderMapping("msappxreplacement", AssetLocation,
                Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

            uiWebView.CoreWebView2.Settings.UserAgent = "SimpleGpsExplorer (ShipwreckSoftware; library=leaflet; control=WebView2)";
            TraceNavigation($"OSM: Initialize: just set UserAgent get={uiWebView.CoreWebView2.Settings.UserAgent}");


            // Can't set these events until after the EnsureCoreWebView2Async() call.
            // These event calls are not needed, but might be useful for debugging.
            //uiWebView.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            //uiWebView.CoreWebView2.ContentLoading += CoreWebView2_ContentLoading;
            //uiWebView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
            //uiWebView.CoreWebView2.AddWebResourceRequestedFilter("*", Microsoft.Web.WebView2.Core.CoreWebView2WebResourceContext.All, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestSourceKinds.All);

            // Add in my event handling
            // See https://github.com/MicrosoftEdge/WebView2Feedback/issues/457
            // less useful: See https://learn.microsoft.com/en-us/microsoft-edge/webview2/how-to/communicate-btwn-web-native
            // less useful: See https://learn.microsoft.com/en-us/microsoft-edge/webview2/how-to/hostobject?tabs=win32
            uiWebView.WebMessageReceived += UiWebView_WebMessageReceived;

            await InitializeWithLoadingPage();
        }

        private void CoreWebView2_WebResourceRequested(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebResourceRequestedEventArgs args)
        {
            TraceNavigation($"OSM: Navigate: web resource requested: {args.Request.Uri}");

            var headers = args.Request.Headers;
            foreach (var (key, value) in headers)
            {
                TraceNavigation($"    Navigate: web resource: header key={key} value={value}");
            }
        }

        private void CoreWebView2_SourceChanged(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs args)
        {
            TraceNavigation($"OSM: Navigate: source change: {args.IsNewDocument}");
        }

        private void CoreWebView2_ContentLoading(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2ContentLoadingEventArgs args)
        {
            TraceNavigation($"OSM: Navigate: content loading: {args.NavigationId}");
        }

        class WebEvent
        {
            public string eventName { get; set; }
        }
        class WebClickEvent: WebEvent
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        class WebKeyDownEvent: WebEvent
        {
            public string key { get; set; }
        }
        private async void UiWebView_WebMessageReceived(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            var webevent = JsonSerializer.Deserialize<WebEvent>(args.WebMessageAsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            switch (webevent.eventName)
            {
                case "OnClick":
                    var clickEvent = JsonSerializer.Deserialize<WebClickEvent>(args.WebMessageAsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Log($"OSM: Web click event: lat={clickEvent.lat} lng={clickEvent.lng}");

                    if (uiWebView.CoreWebView2 != null)
                    {
                        var closestIndex = MapDataItem.GetClosestIndex(MapData, clickEvent.lat, clickEvent.lng); 
                        if (closestIndex >= 0)
                        {
                            var closest = MapData[closestIndex];
                            string summary = closestIndex + " " + closest.SummaryString;
                            string script = $"showPopup({closest.Latitude.AsDecimal}, {closest.Longitude.AsDecimal}, \"{summary}\");";
                            // Log($"Leaflet: script={script}");
                            await uiWebView.CoreWebView2.ExecuteScriptAsync(script);
                        }
                        else
                        {
                            string summary = "Can't find the closet point";
                            string script = $"showPopup({clickEvent.lat}, {clickEvent.lng}, \"{summary}\");";
                            // Log($"Leaflet: script={script}");
                            await uiWebView.CoreWebView2.ExecuteScriptAsync(script);
                        }
                    }
                    break;

                case "OnKeyDown":
                    var keyDownEvent = JsonSerializer.Deserialize<WebKeyDownEvent>(args.WebMessageAsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Log($"OSM: Web key down event: key={keyDownEvent.key}");
                    break;

                default:
                    Log($"OSM: Web event: Other type={webevent.eventName}");
                    break;
            }
        }

        private void UiWebView_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            TraceNavigation($"OSM: Navigate complete: http={args.HttpStatusCode} succ={args.IsSuccess}");
        }

        private void UiWebView_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            TraceNavigation($"OSM: Navigate started: kind={args.NavigationKind} uri={args.Uri} nheaders={args.RequestHeaders.Count()}");

            // Input URL: ms-appx:///SimpleMapLeaflet/SimpleMapLeaflet.html
            // Updated URL: http://msappxreplacement/SimpleMapLeaflet.html
            var originalUri = args.Uri;
            var extraStr = "";
            if (originalUri.StartsWith("ms-appx:///"))
            {
                var replacementUri = originalUri.Replace("ms-appx:///", "http://msappxreplacement/");
                TraceNavigation($"OSM: Navigate: replacing original URI {originalUri} with {replacementUri}");
                args.Cancel = true; // Cancel the original navigation.
                sender.Source = new Uri(replacementUri);
                extraStr = " (cancelling):"; // Note: this doesn't get actually displayed because ms-appx:// URLs seemingly have no headers.
            }

            var headers  = args.RequestHeaders;
            foreach (var (key,value) in headers)
            {
                TraceNavigation($"    Navigate: {extraStr}header key={key} value={value}");
                if (key == "User-Agent")
                {
                    // The correct user-agent can only be found in the WebResourceRequested event.
                    // See https://sunriseprogrammer.blogspot.com/2025/07/wierd-issues-in-winui3-webview2-solving.html
                    TraceNavigation($"    Navigate: Note: User-Agent values are reported as the default values at this point, not a correct values");
                }
            }
        }



        //This code should work, but because ms-appx: doesn't work, the loading fails. 
        //var htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(RootPath));
        //var content = await FileIO.ReadTextAsync(htmlFile);

        public async Task PrivacyUpdated()
        {
            await InitializeWithLoadingPage();
        }

        /// <summary>
        /// Set the uiWebView "Source" to the correct page based on the user's privacy preferences.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeWithLoadingPage()
        {
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
            else if (!UserMapPrivacyPreferences.Allow3rdPartyServices)
            {
                uiWebView.Source = new Uri(UserHasBlockedAllMapsPage);
            }
            else if (UserMapPrivacyPreferences.AllowOpenStreetMap)
            {
                uiWebView.Source = new Uri(LoadingMapPage);
                await Task.Delay(2_000); // Give the user a chance to see the loading page.
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

        private void TraceNavigation(string message)
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
                        
                        //var prev = MapData[MapData.Count - 1];
                        //var distance = prev.Distance(data);
                        //Log($"AddNmea: calculating new point distance={distance} ");
                        if (uiWebView.CoreWebView2 != null)
                        {
                            MapData.Add(data);

                            string script = $"AddNmea({data.Latitude.AsDecimal}, {gprc.Longitude.AsDecimal}, '{gprc.SummaryString}')";
                            Log($"Leaflet: script={script}");
                            await uiWebView.CoreWebView2.ExecuteScriptAsync(script);
                        }
                    }
                    else
                    {
                        if (uiWebView.CoreWebView2 != null)
                        {
                            MapData.Add(data);

                            string script = $"AddNmea({data.Latitude.AsDecimal}, {gprc.Longitude.AsDecimal}, '{gprc.SummaryString}')";
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
