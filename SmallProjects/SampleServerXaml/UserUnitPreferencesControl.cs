using BluetoothCurrentTimeServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SampleServerXaml
{
    public interface FillBtUnits
    {
        BtUnits FillBtUnits(BtUnits retval = null);
        event EventHandler OnPreferredUnitsChanged;
    }
    public sealed partial class UserUnitPreferencesControl : UserControl, FillBtUnits
    {
        public event EventHandler OnPreferredUnitsChanged;
        public UserUnitPreferencesControl()
        {
            this.InitializeComponent();
            this.Loaded += UserUnitPreferencesControl_Loaded;
        }

        private void UserUnitPreferencesControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the initial values from the App.SavedBtUnits 
            string savedtemp = "";
            switch (App.SavedBtUnits.TemperaturePref)
            {
                case BtUnits.Temperature.fahrenheit: savedtemp = "fahrenheit"; break;
                case BtUnits.Temperature.celsius: savedtemp = "celcius"; break;
                case BtUnits.Temperature.kelvin: savedtemp = "kelvin"; break; // NOTE: not actually functional 2024-04-08
            }
            foreach (var child in uiTemp.Items)
            {
                var cbi = child as ComboBoxItem;
                if (cbi == null) continue;
                if (cbi.Tag as string == savedtemp)
                {
                    uiTemp.SelectedItem = cbi;
                }
            }
        }
        // Wrap event invocations inside a protected virtual method
        // to allow derived classes to override the event invocation behavior
        private void RaisePreferredUnitsChanged()
        {
            // Link: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            var raiseEvent = OnPreferredUnitsChanged;

            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                raiseEvent(this, null);
            }
        }

        private void Log(string text)
        {
            Console.WriteLine(text);
            System.Diagnostics.Debug.WriteLine(text);
            uiLog.Text += text + "\n";
        }
        private void OnClearScreen(object sender, RoutedEventArgs e)
        {
            uiLog.Text = "";
        }

        private void OnChangePreference(object sender, SelectionChangedEventArgs args)
        {
            RaisePreferredUnitsChanged();
        }

        //
        // Everything for the UserUnitPreferencesCharacteristic
        //

        /// <summary>
        /// Fill in the BtUnits from the UX. If no BtUnits provided, will make one. Will only 
        /// update values are that filled in -- if there's no prefernce in the UX, the BtUnits
        /// that was passed in will be unchanged.
        /// </summary>
        /// <param name="retval"></param>
        /// <returns></returns>
        public BtUnits FillBtUnits(BtUnits retval = null)
        {
            if (retval == null) retval = new BtUnits();
            //var tempunits = (uiTemp.SelectedItem as ComboBoxItem).Tag as string;
            var TemperatureUnits = (uiTemp.SelectedItem as ComboBoxItem)?.Tag as string ?? "";
            var TimeUnits = (uiTime.SelectedItem as ComboBoxItem)?.Tag as string ?? "";


            System.Console.WriteLine($"DBG: savedtemp units {TemperatureUnits}");
            switch (TemperatureUnits)
            {
                case "celcius": retval.TemperaturePref = BtUnits.Temperature.celsius; break;
                case "fahrenheit": retval.TemperaturePref = BtUnits.Temperature.fahrenheit; break;
                default:
                    System.Console.WriteLine($"ERROR: unknown savedtemp units {TemperatureUnits}");
                    break;
            }
            //var timeunits = (uiTime.SelectedItem as ComboBoxItem).Tag as string;
            System.Console.WriteLine($"DBG: time units {TimeUnits}");
            switch (TimeUnits)
            {
                case "ampm": retval.TimePref = BtUnits.Time.hour12ampm; break;
                case "24hr": retval.TimePref = BtUnits.Time.hour24; break;
                default:
                    System.Console.WriteLine($"ERROR: unknown time units {TimeUnits}");
                    break;
            }
            return retval;
        }
    }
}
