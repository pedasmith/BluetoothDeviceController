using BluetoothDefinitionLanguage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
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

namespace BluetoothDeviceController.BleEditor
{
    public sealed partial class ChacteristicDetailViewerControl : UserControl
    {
        public string PreferredFormat {  get { return uiPreferredFormat.Text; } }
        public bool PreferredFormatChanged = false;

        public ChacteristicDetailViewerControl()
        {
            this.InitializeComponent();
        }

        public async Task InitAsync(GattDeviceService service, GattCharacteristic characteristic, string preferredFormat)
        {
            uiPreferredFormat.Text = preferredFormat;

            string str = null;
            try
            {
                AddData("Service", characteristic.Service.Uuid.ToString());
                AddData("ID", characteristic.Uuid.ToString());
            }
            catch (Exception e1)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: CharacteristicDetailViewer: Adding data {e1.Message}");
            }
            try
            {
                str = characteristic.UserDescription;
            }
            catch (Exception e2)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: CharacteristicDetailViewer: Getting user description {e2.Message}");
            }
            if (!String.IsNullOrWhiteSpace(str)) AddData("Description", str);
            try
            {
                str = "";
                foreach (var format in characteristic.PresentationFormats)
                {
                    str = $"{format.Description} type={format.FormatType} namespace={format.Namespace} unit={format.Unit} exponent={format.Exponent}";
                    AddData("Format", str);
                }
            }
            catch (Exception e)
            {
                AddData("Format", $"Exception: {e.Message}");
            }
            str = "";

            // Don't bother with descriptors
            try
            {
                var dc = DeviceCharacteristic.Create(characteristic);
                AddData("Methods", dc.PropertiesAsString); // e.g. Read Write etc.
            }
            catch (Exception e)
            {
                AddData("Methods", $"Exception: {e.Message}");
            }

            try
            {
                AddData("Protection Level", characteristic.ProtectionLevel.ToString());
            }
            catch (Exception e)
            {
                AddData("Protection Level", $"Exception: {e.Message}");
            }

            try
            {
                if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                {
                    var buffer = await characteristic.ReadValueAsync();
                    if (buffer.Status == GattCommunicationStatus.Success)
                    {
                        var data = ValueParser.ConvertToStringHex(buffer.Value);
                        AddData("Data", data);
                    }
                    else
                    {
                        AddData("Data", $"Error: {buffer.Status}");
                    }
                }
            }
            catch (Exception e)
            {
                AddData("Data", $"Exception: {e.Message}");
            }
        }

        private void AddData(string name, string value)
        {
            var tab = "\t";
            if (name.Length < 8) tab += "\t";
            uiData.Text += $"{name}{tab}{value}\n";
        }

        private void OnPreferredFormatKeyDown(object sender, KeyRoutedEventArgs e)
        {
            PreferredFormatChanged = true;
        }
    }
}
