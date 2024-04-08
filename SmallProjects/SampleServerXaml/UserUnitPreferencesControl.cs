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
    }
    public sealed partial class UserUnitPreferencesControl : UserControl, FillBtUnits
    {
        public UserUnitPreferencesControl()
        {
            this.InitializeComponent();
            this.Loaded += UserUnitPreferencesControl_Loaded;
        }

        private void UserUnitPreferencesControl_Loaded(object sender, RoutedEventArgs e)
        {
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

        string TemperatureUnits = "";
        string TimeUnits = "";
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
            System.Console.WriteLine($"DBG: temp units {TemperatureUnits}");
            switch (TemperatureUnits)
            {
                case "celcius": retval.TemperaturePref = BtUnits.Temperature.celsius; break;
                case "fahrenheit": retval.TemperaturePref = BtUnits.Temperature.fahrenheit; break;
                default:
                    System.Console.WriteLine($"ERROR: unknown temp units {TemperatureUnits}");
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
