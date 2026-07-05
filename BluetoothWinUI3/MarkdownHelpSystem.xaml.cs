using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using Windows.ApplicationModel;
using Windows.Devices.Power;
using Windows.Storage;
using Windows.System;

#if NET8_0_OR_GREATER
#nullable disable
#endif


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3
{
    public sealed partial class MarkdownHelpSystem : UserControl, IImageProvider
    {
        private List<string> History = new List<string>();
        public MarkdownHelpSystem()
        {
            InitializeComponent();
            this.Loaded += MarkdownHelpSystem_Loaded;
        }

        private async void MarkdownHelpSystem_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await InitializeMarkdownAsync();
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }


        public enum HelpFilePathType {  Relative, Absolute };
        /// <summary>
        /// The filename is relative to the Assets\HelpFiles directory (regardless of the BaseUrl settings)
        /// </summary>
        public async Task<bool> ShowHelpAsync(string filename, HelpFilePathType pathType = HelpFilePathType.Relative)
        {
            try
            {
                string fcontents = $"** Unable to open file {filename} **";
                switch (pathType)
                {
                    case HelpFilePathType.Absolute:
                        fcontents = await PathIO.ReadTextAsync(filename);
                        break;
                    case HelpFilePathType.Relative:
                        StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                        string fname = @"Assets\HelpFiles\" + filename;
                        var f = await InstallationFolder.GetFileAsync(fname);
                        fcontents = File.ReadAllText(f.Path);
                        break;
                }

                //
                // Fill in macros. The only macro is [[version]] and [[version_full]]
                //
                var version = Package.Current.Id.Version;
                fcontents = fcontents.Replace("[[version_full]]", $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}");
                fcontents = fcontents.Replace("[[version]]", $"{version.Major}.{version.Minor}");

                // Update history
                History.Add(filename);
                uiNavigation.Visibility = (History.Count > 1) ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;

                uiHelp.Text = fcontents;
                return true;
            }
            catch (Exception ex)
            {
                Log($"ShowHelp: unable to open file {filename} exception={ex.Message}");
            }
            return false;
        }

        private async Task InitializeMarkdownAsync()
        {
            var folder = Windows.ApplicationModel.Package.Current.EffectivePath;
            var config = new MarkdownConfig()
            {
                BaseUrl = folder + "\\Assets\\HelpFiles",
                ImageProvider = this,
                Themes = new MarkdownThemes()
                {
                },
            };
            uiHelp.Config = config;
            Log($"Markdown: BaseUrl={config.BaseUrl}");
        }


        // MARKDOWN:
        private async void OnHelpLinkClicked(object sender, CommunityToolkit.WinUI.Controls.LinkClickedEventArgs e)
        {
            if (e == null)
            {
                Log("Markdown: user clicked on a link, but the Url is null");
                return;
            }
            var link = e.Uri;

            if (link.Scheme == "file")
            {
                Log($"Markdown: click={link.AbsolutePath}");
                Log($"Markdown: click={link.LocalPath}");
                Log($"Markdown: click={link.OriginalString}");

                var path = link.LocalPath;
                var text = await PathIO.ReadTextAsync(path);
                e.Handled = true;

                await ShowHelpAsync(link.LocalPath, HelpFilePathType.Absolute);

                return;
            }
            e.Handled = true;
            await Launcher.LaunchUriAsync(link);
        }

        Regex SizeRegex = new Regex (@"([^)\s]+)\s*\&size_\s*(\d+)x(\d+)\s*");

        // MARKDOWN: part of IImageProvider
        public async Task<Image> GetImage(string url)
        {
            Image retval = new Image(); // Microsoft.UI.Xaml.Controls image
            double h = -1;
            double w = -1;
            var path = url;

            try
            {
                var fileUri = new Uri(url);
                byte[] bytes;
                if (fileUri.Scheme == "file")
                {
                    Log($"Markdown: image={url}");
                    Log($"Markdown: click={fileUri.AbsolutePath} [apath]");
                    Log($"Markdown: click={fileUri.LocalPath} [lpath]");
                    Log($"Markdown: click={fileUri.OriginalString} [orig]");

                    var sizematch = SizeRegex.Match(fileUri.LocalPath);
                    if (sizematch.Success) 
                    {
                        path = sizematch.Groups[1].Value;
                        var wstr = sizematch.Groups[2].Value;
                        Double.TryParse(wstr, out w);
                        var hstr = sizematch.Groups[3].Value;
                        Double.TryParse(hstr, out h);
                        ;
                    }
                    var ampIndex = path.IndexOf("&");
                    if (ampIndex >= 0)
                    {
                        path = path.Substring(0, ampIndex);
                    }

                    var buffer = await PathIO.ReadBufferAsync(path);
                    bytes = buffer.ToByteArray();
                }
                else
                {
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(fileUri);
                    bytes = await file.ReadBytesAsync();
                }
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    BitmapImage bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(ms.AsRandomAccessStream());
                    retval.Source = bitmap;
                    //retval.MaxHeight = 50; // TODO: check this out

                    if (h >= 0) retval.Height = h;
                    if (w >= 0) retval.Width = w;
                    // See https://github.com/CommunityToolkit/Labs-Windows/blob/f437e525e57d33c56315ae68577d12d80d21dfb0/components/MarkdownTextBlock/src/Extensions.cs
                    // They are doing extra work to recognize URL like "../DevicePictures/Nordic_Thingy.png=widthxheight
                }
            }
            catch (Exception ex)
            {
                Log($"Error: Markdown: unable to open image {path} (from {url}) exception {ex.Message}");
            }
            return retval;
        }

        // MARKDOWN: part of IImageProvider
        public bool ShouldUseThisProvider(string url)
        {
            var isMSAppX = url.StartsWith("ms-appx:") || url.StartsWith("file:");
            return isMSAppX;
        }


        private async void OnBackParent(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (History.Count < 2) return;
            History.RemoveAt(History.Count - 1); // the last one
            var newfilename = History[History.Count - 1];
            History.RemoveAt(History.Count - 1); // the last one
            await ShowHelpAsync(newfilename);
        }
    }
}
