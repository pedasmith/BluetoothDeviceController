using BluetoothDefinitionLanguage;
using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BluetoothDeviceController.BleEditor;
using Windows.Devices.Bluetooth.Advertisement;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.BleEditor
{
    public sealed partial class CharacteristicEditorControl : UserControl
    {
        public CharacteristicEditorControl(NameCharacteristic nc, GattWriteOption writeOption, bool automaticallyReadData)
        {
            NC = nc;
            WriteOption = writeOption;
            AutomaticallyReadData = automaticallyReadData; 
            this.InitializeComponent();
            this.Loaded += CharacteristicEditorControl_Loaded;
        }
        public byte[] BytesWritten = null;
        public bool AutomaticallyReadData { get; internal set; } = true;
        private GattWriteOption WriteOption;
        private string BytesFormat = "BYTES"; // can also be STRING
        private string DisplayFormat;
        private string DisplayFormatSecondary;

        private void CharacteristicEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (NC != null)
            {
                SetDisplayType(NC.Type);
                foreach (var item in uiConvertType.Items)
                {
                    var tag = (item as ComboBoxItem).Tag as string;
                    if (tag == DisplayFormat)
                    {
                        uiConvertType.SelectedItem = item;
                    }
                }
            }
        }

        private void SetDisplayType(string type)
        {
            var vps = ValueParserSplit.ParseLine(type);
            if (vps.Count != 1) return; // if there are multiple values, give up and let the user struggle with HEX
            BytesFormat = vps[0].ByteFormatPrimary;
            DisplayFormat = vps[0].DisplayFormatPrimary;
            DisplayFormatSecondary = vps[0].Get(1, 1);

            if (DisplayFormat == "ASCII" && DisplayFormatSecondary == "LONG")
            {
                var h = uiEditBox.ActualHeight;
                uiEditBox.MinHeight = h * 3;
                uiEditBox.MinWidth = 300;
                uiEditBox.AcceptsReturn = true;
            }
        }

        NameCharacteristic NC = null;
        GattDeviceService Service;
        GattCharacteristic Characteristic;
        IBuffer CurrReadBuffer = null;

        private void UpdateDisplayWithCurrReadBuffer(bool addData = true)
        {
            if (CurrReadBuffer != null)
            {
                var bf = (string.IsNullOrEmpty(BytesFormat)) ? "BYTES" : BytesFormat;
                var df = (string.IsNullOrEmpty(DisplayFormat)) ? "HEX" : DisplayFormat;
                var df2 = (string.IsNullOrEmpty(DisplayFormatSecondary)) ? "" : DisplayFormatSecondary;
                var displayType = $"{bf}|{df}^{df2}";
                var result = ValueParser.Parse(CurrReadBuffer, displayType);
                if (result.Result == ValueParserResult.ResultValues.Ok)
                {
                    var data = result.UserString;
                    if (addData) AddData("Data", data);
                    uiEditBox.Text = data; // NOTE: what if I don't want hex?
                }
            }
            else
            {
                uiEditBox.Text = ""; // TODO: maybe save the last value and display that?
            }
        }
        public async Task InitAsync(GattDeviceService service, GattCharacteristic characteristic)
        {
            Service = service;
            Characteristic = characteristic;

            string str;
            AddData("Service", characteristic.Service.Uuid.ToString());
            AddData("ID", characteristic.Uuid.ToString());
            str = characteristic.UserDescription;
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
                // NOTE: used to always try to read; this is a mistake because some devices 
                // say they are readable but aren't. That's why there's an "When editing, automatically read from device" checkbox.
                // here!here
                if (AutomaticallyReadData && characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                {
                    var buffer = await characteristic.ReadValueAsync();
                    if (buffer.Status == GattCommunicationStatus.Success)
                    {
                        //NOTE: also get the data as the nice version?
                        CurrReadBuffer = buffer.Value;
                        UpdateDisplayWithCurrReadBuffer();
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

            AddButtons(); // e.g. for the Skootbot robot controls
        }

        private void AddButtons()
        {
            // e.g. the WilliamWeillerEngineering Skoobot has "buttons" -- there are enums
            // defined for specific commands like forward and reverse. When buttonType is set to
            // "standard" then we add buttons for those specific commands.
            //OTHERWISE we just have the string command to send.
            if (NC?.UI?.buttonType != "standard") return;
            uiButtons.Children.Clear();

            // What's the name of the value?
            var parse = ValueParserSplit.ParseLine(NC.Type);
            if (parse.Count != 1) return; // We can only do buttons when there is only one choice.
            var value1 = parse[0];
            if (value1.ByteFormatPrimary != "U8") return; // can only handle enums for now
            var name = value1.NamePrimary;
            // Get the corresponding enums
            if (!NC.EnumValues.ContainsKey(name)) return; // no enums means no buttons.
            var enums = NC.EnumValues[name];
            var margin = new Thickness(2);
            foreach (var (enumName, enumValue) in enums)
            {
                var b = new Button()
                {
                    Content = enumName,
                    Tag = enumValue,
                    MinWidth = 120,
                    Margin = margin,
                };
                b.Click += OnEnum_Click;
                uiButtons.Children.Add(b);
                if (enumName.Length >= 15)
                {
                    // Spill to two columns
                    VariableSizedWrapGrid.SetColumnSpan(b, 2);
                }
            }
        }

        /// <summary>
        /// For now, an enum is strictly a one-byte value; that makes it easier to call DoWrite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnEnum_Click(object sender, RoutedEventArgs e)
        {
            var value = (int)(sender as FrameworkElement).Tag;
            var buffer = new byte[] { (byte)value };
            BytesWritten = buffer;
            await DoWriteAsync(buffer.AsBuffer());
        }

        private void AddData(string name, string value)
        {
            var tab = "\t";
            if (name.Length < 8) tab += "\t";
            uiData.Text += $"{name}{tab}{value}\n";
        }

        private async void OnWrite(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            if (Characteristic == null) return;
            uiStatus.Text = "";

            var str = uiEditBox.Text;
            var convertType = ((uiConvertType.SelectedItem as ComboBoxItem)?.Tag as string) ?? "HEX";
            if (convertType == "ASCII")
            {
                str = str.Replace("\\r", "\r").Replace("\\n", "\n");
            }
            var result = str.ConvertToBuffer(convertType); // e.g. HEX DEC ASCII
            if (result.Result == ValueParserResult.ResultValues.Ok)
            {
                // In the case of a STRING, we are more careful. The string has to be broken up
                // into the correct size pieces to be sent.
                // Why look at DisplayFormat here even though the original conversion used the
                // convertType? Because the user might have to enter a long set of HEX values.
                // But the long hex value still needs to be chopped up as appropriate.
                if (DisplayFormat == "ASCII" && DisplayFormatSecondary == "LONG")
                {
                    var command = result.ByteResult.ToArray();
                    const int MAXBYTES = 20;
                    for (int i=0; i<command.Length; i+= MAXBYTES)
                    {
                        // So many calculations and copying just to get a slice
                        var maxCount = Math.Min(MAXBYTES, command.Length - i);
                        var subcommand = new ArraySegment<byte>(command, i, maxCount);
                        var data = subcommand.ToArray().AsBuffer();
                        BytesWritten = subcommand.ToArray();
                        await DoWriteAsync(data);
                    }
                }
                else
                {
                    var data = result.ByteResult.ToArray().AsBuffer();
                    BytesWritten = result.ByteResult.ToArray();
                    await DoWriteAsync(data);
                }
            }
            else
            {
                uiStatus.Text = $"ERROR: unable to parse {str}\n{result.ErrorString}";
                return;
            }
        }

        private async Task DoWriteAsync(IBuffer data)
        {
            uiStatus.Text = "";
            uiProgress.IsActive = true;

            try
            {
                uiStatus.Text = "Writing data...";
                var status = await Characteristic.WriteValueWithResultAsync(data, WriteOption);

                switch (status.Status)
                {
                    case GattCommunicationStatus.Success:
                        uiStatus.Text = "Write OK";
                        break;
                    default:
                        uiStatus.Text = $"ERROR: unable to write data\nError is {status.Status.ToString()}";
                        break;
                }
            }
            catch (Exception ex)
            {
                uiStatus.Text = $"ERROR: exception while writing\nException is {ex.Message}";
            }

            uiProgress.IsActive = false;
        }

        private void OnConvertTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            // Tags are DEC HEX ASCII
            //if (!IsLoadedLocal) return;
            var nremoved = e.RemovedItems.Count;
            if (nremoved == 0) return;

            var newtype = ((sender as ComboBox).SelectedItem as ComboBoxItem).Tag as string;
            switch (newtype)
            {
                case "DEC":
                    SetDisplayType("BYTES|DEC");
                    UpdateDisplayWithCurrReadBuffer(false);
                    break;
                default:
                case "HEX":
                    SetDisplayType("BYTES|HEX");
                    UpdateDisplayWithCurrReadBuffer(false);
                    break;
                case "ASCII":
                    SetDisplayType("STRING|ASCII");
                    UpdateDisplayWithCurrReadBuffer(false);
                    break;
            }

        }
    }
}
