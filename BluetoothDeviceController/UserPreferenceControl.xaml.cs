using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using enumUtilities;
using Windows.ApplicationModel.DataTransfer;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController
{
    public class DisplayPreferenceConverter : EnumValueConverter<UserPreferences.DisplayPreference> { }
    public class SearchScopeConverter : EnumValueConverter<UserPreferences.SearchScope> { }
    public class ReadSelectionConverter : EnumValueConverter<UserPreferences.ReadSelection> { }

    public sealed partial class UserPreferenceControl : UserControl
    {
        public UserPreferences Preferences { get; internal set; }
        public IDoSearch Search { get; set; } = null;
        public UserPreferenceControl()
        {
            this.InitializeComponent();
            this.DataContext = Preferences;
            this.Loaded += UserPreferenceControl_Loaded;
        }

        private void UserPreferenceControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSearchUI();
            if (Search != null)
            {
                Search.DeviceEnumerationChanged += Search_DeviceEnumerationChanged;
            }
        }

        // Whenever the device enumeration changes (which doesn't include merely finding a new device), update the UI.
        private async void Search_DeviceEnumerationChanged(object sender, EventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                UpdateSearchUI();
            });
        }

        private void UpdateSearchUI()
        {
            if (Search == null) return;
            uiSearchStart.IsEnabled = !Search.GetSearchActive();
            uiSearchCancel.IsEnabled = !uiSearchStart.IsEnabled;
        }

        public void SetPreferences (UserPreferences pref)
        {
            Preferences = pref;
            this.DataContext = Preferences;
        }

        private void OnSearchStart(object sender, RoutedEventArgs e)
        {
            if (Search == null) return;
            Search.StartSearch(Preferences.DeviceReadSelection);
            UpdateSearchUI();
        }

        private void OnSearchCancel(object sender, RoutedEventArgs e)
        {
            if (Search == null) return;
            Search.CancelSearch();
            UpdateSearchUI();
        }


        private void OnSearchScopeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            var scope = (UserPreferences.SearchScope)e.AddedItems[0];

            switch (scope)
            {
                case UserPreferences.SearchScope.All_bluetooth_devices:
                    ReadSelectionComboBox.IsEnabled = true;
                    break;
                default:
                    ReadSelectionComboBox.IsEnabled = false;
                    break;
            }
        }

        private void OnCopyJson(object sender, RoutedEventArgs e)
        {
            var value = Search?.GetCurrentSearchResults();
            if (String.IsNullOrEmpty(value)) return;
            var dp = new DataPackage();
            // Convert the list of individual items into a proper list.
            value = @"{
  ""AllDevices"": [
" + value + @"
  ]
}";

            dp.SetText(value);
            dp.Properties.Title = "JSON Bluetooth data";
            Clipboard.SetContent(dp);
        }

        static Windows.System.Display.DisplayRequest CurrDisplayRequest = null;

        private void OnKeepScreenOnChecked(object sender, RoutedEventArgs e)
        {
            if (CurrDisplayRequest == null)
            {
                CurrDisplayRequest = new Windows.System.Display.DisplayRequest();
            }
            var ischeck = (sender as CheckBox).IsChecked.GetValueOrDefault();
            if (ischeck)
            {
                CurrDisplayRequest.RequestActive();
            }
            else
            {
                CurrDisplayRequest.RequestRelease();
            }
        }

    }
}
