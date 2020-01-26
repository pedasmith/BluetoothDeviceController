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
    public sealed partial class BleCharacteristicControl : UserControl
    {
        //string On = "⌁"; // Electrical arrow u+2301
        //string Notify = "🄽"; // Squared latin capital letter n u+1f13d
        //string Indicate = "🅸"; // Negative Squared latin capital letter I
        //string Read = "🅁";// Squared Latin Capital Letter R U+1f181
        //string Write = "🅆";// Squared Latin Capital Letter W U+1f146
        //string WriteWithResponse = "🆆"; // Negative Squared Latin Capital Letter W U+1f186

        //string Hexadecimal = "䷞"; // Haxagram for Influence
        //string Text = "🖎"; // Left writing hand u+1f58e
        //string Decimal = "⏨"; // decimal exponent symbol u+23e8 might not support decimal input...

        //string Refresh = "🗘"; // clockwise right and left semicircle arrows u+1f5d8
        // Refresh is not needed because I can just wait for the read to be clicked

        GattDeviceService Service = null;
        GattCharacteristic Characteristic = null;
        bool IndicateSetup = false;
        bool NotifySetup = false;
        NameCharacteristic NC = null;
        IList<string> ArchivedData = new List<String>();
        string DefaultFormat = "BYTES|HEX";
        string PreferredFormat = null;
        enum ValueShowMethod {Overwrite, Append };
        ValueShowMethod CurrValueShowMethod = BleCharacteristicControl.ValueShowMethod.Overwrite;

        public BleCharacteristicControl(NameDevice device, GattDeviceService service, GattCharacteristic characteristic)
        {
            this.InitializeComponent();
            if (device == null)
            {
                ;
            }
            Loaded += async (s,e) => await SetupAsync(device, service, characteristic);
        }


        // Sets up all of the flags, values, etc.
        private async Task SetupAsync(NameDevice device, GattDeviceService service, GattCharacteristic characteristic)
        {
            Service = service;
            Characteristic = characteristic;

            NC = BleNames.Get(device, service, characteristic);

            var props = characteristic.CharacteristicProperties;
            uiNotifyFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Notify) ? Visibility.Visible : Visibility.Collapsed;
            uiNotifyOnFlag.Visibility = Visibility.Collapsed;
            uiReadFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Read) ? Visibility.Visible : Visibility.Collapsed;
            uiWriteFlag.Visibility = props.HasFlag(GattCharacteristicProperties.WriteWithoutResponse) ? Visibility.Visible : Visibility.Collapsed;
            uiWriteWithResponseFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Write) ? Visibility.Visible : Visibility.Collapsed;
            uiIndicateFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Indicate) ? Visibility.Visible : Visibility.Collapsed;
            uiIndicateOnFlag.Visibility = Visibility.Collapsed;

            // The less common flags
            uiBroadcastFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Broadcast) ? Visibility.Visible : Visibility.Collapsed;
            uiAuthenticatedSignedWritesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.AuthenticatedSignedWrites) ? Visibility.Visible : Visibility.Collapsed;
            uiExtendedPropertiesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.ExtendedProperties) ? Visibility.Visible : Visibility.Collapsed;
            uiReliableWritesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.ReliableWrites) ? Visibility.Visible : Visibility.Collapsed;
            uiWritableAuxilariesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.WritableAuxiliaries) ? Visibility.Visible : Visibility.Collapsed;

            // Set the little edit marker
            uiEditFlag.Visibility = (props.HasFlag (GattCharacteristicProperties.Write)
                || props.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
                 ? Visibility.Visible : Visibility.Collapsed;

            await DoReadAsync(true); // update the value field even if it's not readable

            var namestr = "";
            if (!String.IsNullOrEmpty (NC?.Name))
            {
                namestr = NC.Name;
            }
            else
            {
                var uidname = service.Uuid.ToString().GetCommon(characteristic.Uuid.ToString());
                if (uidname.Length < 8)
                {
                    namestr = uidname;
                    if (namestr == "ffa2")
                    {
                        ; // something handy to put a debugger on!
                    }
                    if (namestr == "ffa3")
                    {
                        ; // a handy line to put a debugger on!
                    }
                    if (!String.IsNullOrWhiteSpace(characteristic.UserDescription))
                    {
                        namestr += " " + characteristic.UserDescription;
                    }
                    else
                    {
                        ;
                    }
                }
            }



            // Either we have a good name (yay!) or we use the alternate route of using the
            // uuid info area.
            if (namestr != "")
            {
                // Don't need the UUID area or the line break
                uiName.Text = namestr;
                uiInfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                var infostr = "";
                if (!string.IsNullOrWhiteSpace(NC?.Name))
                {
                    infostr = NC.Name + " ";
                }
                infostr += characteristic.Uuid.ToString();
                uiInfo.Text = infostr;
                uiInfo.Visibility = Visibility.Visible;
            }

            // If there's a readable value with displayFormatPrimary=ASCII and secondary of LONG (ASCII^LONG)
            // then make the text area multi-line
            var vps = ValueParserSplit.ParseLine(NC?.Type ?? "");
            if (vps.Count == 1)
            {
                var displayFormat = vps[0].DisplayFormatPrimary;
                var displayFormatSecondary = vps[0].Get(1, 1);
                switch (displayFormat)
                {
                    case "ASCII":
                        switch (displayFormatSecondary)
                        {
                            case "LONG":
                                var h = uiValueShow.ActualHeight;
                                uiValueShowScroll.MinHeight = h * 3;
                                uiValueShowScroll.MaxHeight = h * 7;
                                CurrValueShowMethod = ValueShowMethod.Append;
                                break;
                        }
                        break;
                }
            }
        }

        private async Task DoReadAsync(bool ignoreUnreadableValues)
        {
            var props = Characteristic.CharacteristicProperties;
            bool isReadable = false;

            var valuestr = "";
            if (props.HasFlag(GattCharacteristicProperties.Read))
            {
                try
                {
                    var vresult = await Characteristic.ReadValueAsync();
                    if (vresult.Status != GattCommunicationStatus.Success)
                    {
                        valuestr = GetStatusString(vresult.Status, vresult.ProtocolError);
                    }
                    else
                    {
                        var decode = NC?.Type ?? "BYTES|HEX"; // default is hex
                        var result = ValueParser.Parse(vresult.Value, decode);
                        valuestr = result.AsString;
                    }
                }
                catch (Exception e)
                {
                    valuestr = $"Exception: {Characteristic.ToString()} {e.Message}";
                }
                isReadable = true;
            }
            else if (props.HasFlag(GattCharacteristicProperties.Indicate))
            {
                valuestr = "Tap the 🅸 to get data indications";
            }
            else if (props.HasFlag(GattCharacteristicProperties.Notify))
            {
                valuestr = "Tap the 🄽 to get data notifications";
            }
            else
            {
                valuestr = "Not a readable characteristic";
            }
            if (isReadable || ignoreUnreadableValues)
            {
                AddString(valuestr);
            }
        }

        private void AddString(string valuestr)
        {
            switch (CurrValueShowMethod)
            {
                case ValueShowMethod.Overwrite:
                    uiValueShow.Text = valuestr;
                    break;
                case ValueShowMethod.Append:
                    {
                        var currLen = uiValueShow.Text.Length;
                        const int maxLen = 1000;
                        if (currLen <= maxLen)
                        {
                            uiValueShow.Text += valuestr;
                        }
                        else
                        {
                            // Chop down the old value ruthlessly!
                            var newtext = (uiValueShow.Text + valuestr);
                            newtext = newtext.Substring(newtext.Length - maxLen);
                            uiValueShow.Text = newtext;
                        }
                    }
                    break;
            }
        }

        private string GetStatusString(GattCommunicationStatus status, byte? protocolError)
        {
            string raw = "";
            switch (status)
            {
                case GattCommunicationStatus.AccessDenied:
                    raw = $"BLE: ERROR: Access is denied";
                    break;
                case GattCommunicationStatus.ProtocolError:
                    if (protocolError.HasValue)
                    {
                        raw = $"BLE: Protocol error {protocolError.Value}\n";
                    }
                    else
                    {
                        raw = $"BLE: Protocol error (no protocol error value)\n";
                    }
                    break;
                case GattCommunicationStatus.Unreachable:
                    raw = $"BLE: ERROR: device is unreachable\n";
                    break;
            }
            return raw;
        }


        private async void OnIndicateTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!IndicateSetup)
            {
                IndicateSetup = true;
                try
                {
                    Characteristic.ValueChanged += Characteristic_ValueChanged;
                    await Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                }
                catch (Exception ex)
                {
                    uiValueShow.Text = $"Unable to set indicate callback {ex.Message}"; ;
                }
                uiIndicateOnFlag.Visibility = Visibility.Visible;
            }
            else
            {
                IndicateSetup = false;
                try
                {
                    Characteristic.ValueChanged -= Characteristic_ValueChanged;
                    await Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                }
                catch (Exception ex)
                {
                    uiValueShow.Text = $"Unable to remove indicate callback {ex.Message}"; ;
                    ; // NOTE: for example, we've long lost the connection. Should have a single info bar per device that can report that we've lost connection.
                }
                uiIndicateOnFlag.Visibility = Visibility.Collapsed;
            }
        }


        private async void OnNotifyTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!NotifySetup)
            {
                NotifySetup = true;
                try
                {
                    Characteristic.ValueChanged += Characteristic_ValueChanged;
                    await Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                }
                catch (Exception ex)
                {
                    uiValueShow.Text = $"Unable to add value changed callback {ex.Message}"; ;
                }
                uiNotifyOnFlag.Visibility = Visibility.Visible;
            }
            else
            {
                NotifySetup = false;
                try
                {
                    Characteristic.ValueChanged -= Characteristic_ValueChanged;
                    await Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                }
                catch (Exception ex)
                {
                    uiValueShow.Text = $"Unable to remove value changed callback {ex.Message}"; ;
                    ; // NOTE: for example, we've long lost the connection. Should have a single info bar per device that can report that we've lost connection.
                }
                uiNotifyOnFlag.Visibility = Visibility.Collapsed;
            }
        }
        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            string valuestr = "";
            lock (this)
            {
                var decode = PreferredFormat ?? NC?.Type ?? DefaultFormat; // default is hex
                var result = ValueParser.Parse(args.CharacteristicValue, decode);
                valuestr = result.AsString;
            }
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                AddString(valuestr);
           });
        }

        private async void OnShowMoreTapped(object sender, TappedRoutedEventArgs e)
        {
            var decode = PreferredFormat ?? NC?.Type ?? DefaultFormat; // default is hex

            var cvc = new ChacteristicDetailViewerControl();
            await cvc.InitAsync(Service, Characteristic, decode);
            var dlg = new ContentDialog()
            {
                Title = "Characteristic Details",
                Content = cvc,
                CloseButtonText = "OK"
            };
            await dlg.ShowAsync();

            if (cvc.PreferredFormatChanged)
            {
                PreferredFormat = cvc.PreferredFormat;
            }
        }

        private GattWriteOption PreferredWriteOption()
        {
            GattWriteOption writeOption = GattWriteOption.WriteWithResponse;
            if (Characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
            {
                writeOption = GattWriteOption.WriteWithoutResponse;
            }
            return writeOption;
        }

        private async void OnEditTapped(object sender, TappedRoutedEventArgs e)
        {
            GattWriteOption writeOption = PreferredWriteOption();
            await DoWriteAsync(writeOption);
        }

        private async Task DoWriteAsync(GattWriteOption writeOption)
        {
            try
            {
                var cvc = new CharacteristicEditorControl(NC, writeOption);
                await cvc.InitAsync(Service, Characteristic);
                var dlg = new ContentDialog()
                {
                    Title = "Edit Value",
                    Content = cvc,
                    CloseButtonText = "Done",
                };
                await dlg.ShowAsync();

                // Update the value if possible. Some writable values are not readable at all.
                await DoReadAsync(false); // not a readable item? then just move on with life!
            }
            catch (Exception ex)
            {
                uiValueShow.Text = "Unable to write";
                System.Diagnostics.Debug.WriteLine($"Exception: unable to write to characteristic ({ex.Message})");
            }
        }

        private async void OnReadTapped(object sender, TappedRoutedEventArgs e)
        {
            await DoReadAsync(true);
        }

        private async void OnWriteTapped(object sender, TappedRoutedEventArgs e)
        {
            await DoWriteAsync(GattWriteOption.WriteWithoutResponse);
        }

        private async void OnWriteWithResponseTapped(object sender, TappedRoutedEventArgs e)
        {
            await DoWriteAsync(GattWriteOption.WriteWithResponse);
        }
    }
}
