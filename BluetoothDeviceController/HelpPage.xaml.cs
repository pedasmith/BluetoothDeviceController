using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController
{
    class PageHistory
    {
        private Stack<string> Breadcrumbs = new Stack<string>();

        /// <summary>
        /// Call this when you navigate to a new page
        /// </summary>
        /// <param name="place"></param>
        public void NavigatedTo(string place)
        {
            if (Breadcrumbs.Count >= 1 && place == Breadcrumbs.Peek()) return;
            Breadcrumbs.Push(place);
        }

        /// <summary>
        /// Call this to get the right page to "pop" to.
        /// </summary>
        /// <returns></returns>
        public string PopLastPage()
        {
            // If stack is help.md                          then return help.md and don't change the stack
            // If the stack is help.md thingy.md            then pop thingy.md pop and return help.md
            // If the stack is help.md thingy.md excel.md   then pop excel.md pop and return thingy.md 
            if (Breadcrumbs.Count == 1) return Breadcrumbs.Peek();
            if (Breadcrumbs.Count == 2)
            {
                Breadcrumbs.Pop();
                return Breadcrumbs.Peek();
            }
            if (Breadcrumbs.Count > 2)
            {
                // Pop twice, not once. The next action is almost certainly going to be a push
                // of the thing I am now returning.
                Breadcrumbs.Pop();
                var retval = Breadcrumbs.Pop();
                return retval;
            }
            return "Help.md"; // emergency back-up plan!
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpPage : Page
    {
        public HelpPage()
        {
            this.InitializeComponent();
        }

        public static string NavigatedTo = "";
        private void SetNavigatedTo(string place)
        {
            History.NavigatedTo(place);
            NavigatedTo = place;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var pagename = e.Parameter as string;
            uiHelpText.UriPrefix = "ms-appx:///Assets/HelpFiles/";
            uiHelpText.LinkClicked += UiHelpText_LinkClicked;

            const string StartPage = "Help.md";
            //pagename = this.DataContext as string;
            if (String.IsNullOrEmpty(pagename))
            {
                pagename = StartPage;
            }
            await GotoAsync(pagename);

        }

        PageHistory History = new PageHistory();
        private async Task<bool> GotoAsync (string filename)
        {
            if (filename.StartsWith ("http://") || filename.StartsWith("https://"))
            {
                // Pop out to a browser window!
                try
                {
                    Uri uri = new Uri(filename);
                    var launched = await Windows.System.Launcher.LaunchUriAsync(uri);
                }
                catch (Exception)
                {
                    ; // do thing?
                }
                return true;
            }


            try
            {
                StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string fname = @"Assets\HelpFiles\" + filename;
                var f = await InstallationFolder.GetFileAsync(fname);
                var fcontents = File.ReadAllText(f.Path);
                uiHelpText.Text = fcontents;
                SetNavigatedTo(filename);
                return true;
            }
            catch (Exception)
            {
            }
            const string ErrorName = "Error.md";
            if (filename != ErrorName)
            {
                await GotoAsync(ErrorName);
            }
            return false; // If I'm showing the error, return false.
        }


        private async void UiHelpText_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            var ok = await GotoAsync(e.Link);
        }

        private void UiHelpText_ImageClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
        }

        private async void OnBack(object sender, RoutedEventArgs e)
        {
            var page = History.PopLastPage();
            await GotoAsync(page);
        }
    }
}
