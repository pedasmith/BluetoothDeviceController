using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilities;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{
    public sealed partial class SimpleMapLeafletControl : UserControl
    {
        public SimpleMapLeafletControl()
        {
            this.InitializeComponent();
            this.Loaded += SimpleMapLeafletControl_Loaded;

        }

        private void SimpleMapLeafletControl_Loaded(object sender, RoutedEventArgs e)
        {
            uiWebView.NavigationStarting += UiWebView_NavigationStarting;
            uiWebView.NavigationCompleted += UiWebView_NavigationCompleted;
            uiWebView.WebMessageReceived += UiWebView_WebMessageReceived;

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

        private async void OnLoadWebViaHost(object sender, RoutedEventArgs e)
        {
            await uiWebView.EnsureCoreWebView2Async();
            uiWebView.CoreWebView2.SetVirtualHostNameToFolderMapping("msappxreplacement", "Assets/SimpleMapLeaflet", Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

            uiWebView.Source = new Uri(HostRootPath);
        }

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

        private void Log(string message)
        {
            uiLog.Text = message;
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
