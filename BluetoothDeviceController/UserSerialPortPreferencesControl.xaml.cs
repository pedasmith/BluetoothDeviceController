using BluetoothDeviceController.SerialPort;
using enumUtilities;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController
{
    public class TerminalLineEndConverter : EnumValueConverter<UserSerialPortPreferences.TerminalLineEnd> { }
    public sealed partial class UserSerialPortPreferencesControl : UserControl
    {
        public UserSerialPortPreferencesControl()
        {
            this.InitializeComponent();
            this.Loaded += UserSerialPortPreferencesControl_Loaded;
        }

        private void UserSerialPortPreferencesControl_Loaded(object sender, RoutedEventArgs e)
        {
            int selectedIndex = 0;
            ShortcutIdComboBox.Items.Add(new ComboBoxItem()
            { 
                Content="Based on device",
                Tag="",
            });
            AllShortcuts.Init();
            foreach (var item in AllShortcuts.All)
            {
                ShortcutIdComboBox.Items.Add(new ComboBoxItem()
                {
                    Content = item.Name,
                    Tag = item.Id,
                });
                if (item.Id == SerialPortPreferences.ShortcutId)
                {
                    selectedIndex = ShortcutIdComboBox.Items.Count-1; // e.g. item[1] has count of 2 ([0] and [1])
                }
            }
            ShortcutIdComboBox.SelectedIndex = selectedIndex;
        }

        public UserSerialPortPreferences SerialPortPreferences { get; set; }
        public void SetPreferences(UserSerialPortPreferences pref, bool userCanSetLineEndings)
        {
            TerminalLineEndConverterComboBox.Visibility = userCanSetLineEndings ? Visibility.Visible : Visibility.Collapsed;
            SerialPortPreferences = pref;
            this.DataContext = SerialPortPreferences;
        }

        private void OnLineEndChanged(object sender, SelectionChangedEventArgs e)
        {
            ; // don't actually have to do anything here.
        }

        private void OnMacroChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            var item = e.AddedItems[0] as ComboBoxItem;
            SerialPortPreferences.ShortcutId = item.Tag as string;
            SerialPortPreferences.SaveToLocalSettings();
        }
    }
}
