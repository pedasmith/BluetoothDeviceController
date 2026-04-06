using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Display;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public class UserPreferences
    {
        bool AutostartAdvertisementWatcher { get; set; } = true;
    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher AdvertisementWatcher = new BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher();
        int NAdvertisements = 0;
        UserPreferences CurrUserPrefs = new UserPreferences();
        public MainWindow()
        {

            UIThreadHelper.DQueue = this.DispatcherQueue;
            AdvertisementWatcher.WatcherEvent += AdvertisementWatcher_WatcherEvent;
            InitializeComponent();
            this.Activated += MainWindow_Activated;
        }



        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            AdvertisementWatcher.Start();
        }

        /// <summary>
        /// The main way we show notices to the user. This is a little method so that it can be changed when we change
        /// how notices are shown. It's only for things that the user must acknowlege but they don't have to do anything.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="notice"></param>
        /// <returns></returns>
        private async Task ShowNotice(string title, string notice)
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = rootPanel.XamlRoot,
                Title = title,
                Content = notice,
                CloseButtonText = "OK",
            };
            await dialog.ShowAsync();
        }

        StringBuilder AllAdvertisements = new StringBuilder();
        Dictionary<string, string> UniqueAdvertisements = new Dictionary<string, string>();
        Dictionary<string, string> UniqueBTAddresses = new Dictionary<string, string>();

        private void AdvertisementWatcher_WatcherEvent(BluetoothLEAdvertisementWatcher sender, BluetoothWatcher.AdvertismentWatcher.WatcherData e)
        {
            // A little bit of logging and storing stuff for debugging
            NAdvertisements++;
            AllAdvertisements.AppendLine($"{NAdvertisements}, " + e.ToStringFull());
            var fmt = BluetoothWatcher.AdvertismentWatcher.WatcherData.AdvertisementStringFormat.CanCompare;
            UniqueAdvertisements[e.ToStringFull(fmt)] = e.ToStringFull();
            fmt = BluetoothWatcher.AdvertismentWatcher.WatcherData.AdvertisementStringFormat.AddressOnly;
            UniqueBTAddresses[e.ToStringFull(fmt)] = e.ToStringFull();


            // Update the UI as needed and create the next device
            UIThreadHelper.CallOnUIThread(() =>
            {
                uiAdvertCount.Text = NAdvertisements.ToString();
                uiAdvertRaw.Text = e.ToString();



                // If this is a new advert, see if it's a known type
                var supportedDevice = BluetoothWinUI3.BluetoothWinUI3Registration.SupportedDevices.GetSupported(e);
                if (supportedDevice != null)
                {
                    var known = KnownDevices.Get(e);
                    if (known == null)
                    {
                        var control = Activator.CreateInstance(supportedDevice.FactoryInterface) as UserControl;

                        uiKnownDevices.Items.Add(control);
                        known = KnownDevices.Add(e, control, supportedDevice);
                        control.DataContext = known;
                    }
                }
            });
        }

        private void OnAdvertisementStart(object sender, RoutedEventArgs e)
        {
            AdvertisementWatcher.Start();
        }
        private void OnAdvertisementStop(object sender, RoutedEventArgs e)
        {
            AdvertisementWatcher.Stop();
        }

        private void OnAdvertisementCopy(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + AllAdvertisements.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }
        private void OnAdvertisementCopyUnique(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var item in UniqueAdvertisements.Values)
            {
                sb.AppendLine(item);
            }

            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + sb.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }
        private void OnAdvertisementCopyUniqueAddresses(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var item in UniqueBTAddresses.Values)
            {
                sb.AppendLine(item);
            }

            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + sb.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }
        private void OnFileExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void OnHelpAbout(object sender, RoutedEventArgs e)
        {
            // TODO: make this work
        }

        private void OnDebugLoadDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Restore();

        }

        private void OnDebugSaveDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Save();
        }



        private async void OnBTRename(object sender, RoutedEventArgs e)
        {
            // TODO: notice how much boilerplate there is here. Simplify it when I make the next change.

            var selected = uiKnownDevices.SelectedItem as IDeviceControl;
            if (selected == null && uiKnownDevices.Items.Count == 1)
            {
                // If there is only one device, we can be nice and just rename that one without forcing the user to select it first.
                uiKnownDevices.SelectedIndex = 0;
                selected = uiKnownDevices.Items[0] as IDeviceControl;
            }
            if (selected == null)
            {
                await ShowNotice("No device selected", "You must select a device to rename it");
                return;
            }
            var knownDevice = selected.GetKnownDevice();
            if (knownDevice == null)
            {
                await ShowNotice("Can't rename that device", "This device cannot be renamed");
                return;
            }
            if (knownDevice.Id == "")
            {
                await ShowNotice("Can't rename that device", "This device cannot be renamed; it has no Windows device ID");
                return;
            }

            // Get the SaveDevice based on the ID
            var saveData = AllSaveData.FindWithId(knownDevice.Id);
            if (saveData == null)
            {
                // Must create a new SaveData.
                saveData = new SaveData(knownDevice);
                AllSaveData.Insert(saveData);
                AllSaveData.Save(); // quick update
            }


            var dlg = uiDialogRenameDevice;
            uiDialogRenameDeviceName.Text = saveData.GetUserName();
            var result = await dlg.ShowAsync();

            string newname = uiDialogRenameDeviceName.Text;
            saveData.SetUserName(newname);
            AllSaveData.Save();
            ;
            selected.UpdateUX(saveData);
        }

        // Keep screen on so that we constantly monitor devices
        // https://learn.microsoft.com/en-us/uwp/api/windows.system.display.displayrequest?view=winrt-28000
        DisplayRequest g_DisplayRequest = null;
        private async void OnFileKeepScreenOn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (g_DisplayRequest == null)
                {
                    g_DisplayRequest = new DisplayRequest();
                }
            }
            catch (Exception ex)
            {
                await ShowNotice("Unable to start", $"Unable to create the DisplayRequest to keep the screen on. Reason={ex.Message}");
            }
            if (g_DisplayRequest == null) return; // just give up

            try
            {
                // This call activates a display-required request. If successful,  
                // the screen is guaranteed not to turn off automatically due to user inactivity. 
                if ((sender as ToggleButton)?.IsChecked ?? true)
                {
                    g_DisplayRequest.RequestActive();
                }
                else
                {
                    g_DisplayRequest.RequestRelease();
                }
            }
            catch (Exception ex)
            {
                await ShowNotice ("Unable to request", $"Error: {ex.Message}");
            }
        }
    }
}
