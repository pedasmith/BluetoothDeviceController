using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.BleEditor
{


    public sealed partial class BleCharacteristicControl : UserControl, IWriteCharacteristic
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
        enum ValueShowMethod { Overwrite, Append };
        ValueShowMethod CurrValueShowMethod = BleCharacteristicControl.ValueShowMethod.Overwrite;
        public ObservableCollection<Command> Commands { get; } = new ObservableCollection<Command>();
        public bool AutomaticallyReadData { get; internal set; } = true;

        public BleCharacteristicControl(NameDevice device, GattDeviceService service, GattCharacteristic characteristic, bool automaticallyReadData)
        {
            AutomaticallyReadData = automaticallyReadData;
            this.DataContext = this;
            this.InitializeComponent();
            if (device == null)
            {
                ;
            }
            Loaded += async (s, e) => await SetupAsync(device, service, characteristic);
        }



        // Sets up all of the flags, values, etc.
        private async Task SetupAsync(NameDevice device, GattDeviceService service, GattCharacteristic characteristic)
        {
            Service = service;
            Characteristic = characteristic;
            NC = BleNames.Get(device, service, characteristic);
            if (characteristic.Uuid.ToString().Contains("aaf3f6d5"))
            {
                ; // Handy place to hang a debugger
            }

            var props = characteristic.CharacteristicProperties;
            uiNotifyFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Notify) ? Visibility.Visible : Visibility.Collapsed;
            uiNotifyOnFlag.Visibility = Visibility.Collapsed;
            uiReadFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Read) ? Visibility.Visible : Visibility.Collapsed;
            uiWriteFlag.Visibility = props.HasFlag(GattCharacteristicProperties.WriteWithoutResponse) ? Visibility.Visible : Visibility.Collapsed;
            uiIncrementWriteFlag.Visibility = uiWriteFlag.Visibility;
            uiWriteWithResponseFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Write) ? Visibility.Visible : Visibility.Collapsed;
            uiIncrementWriteWithResponseFlag.Visibility = uiWriteWithResponseFlag.Visibility;
            uiIndicateFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Indicate) ? Visibility.Visible : Visibility.Collapsed;
            uiIndicateOnFlag.Visibility = Visibility.Collapsed;

            // The less common flags
            uiBroadcastFlag.Visibility = props.HasFlag(GattCharacteristicProperties.Broadcast) ? Visibility.Visible : Visibility.Collapsed;
            uiAuthenticatedSignedWritesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.AuthenticatedSignedWrites) ? Visibility.Visible : Visibility.Collapsed;
            uiExtendedPropertiesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.ExtendedProperties) ? Visibility.Visible : Visibility.Collapsed;
            uiReliableWritesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.ReliableWrites) ? Visibility.Visible : Visibility.Collapsed;
            uiWritableAuxilariesFlag.Visibility = props.HasFlag(GattCharacteristicProperties.WritableAuxiliaries) ? Visibility.Visible : Visibility.Collapsed;

            // Set the little edit marker
            uiEditFlag.Visibility = (props.HasFlag(GattCharacteristicProperties.Write)
                || props.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
                 ? Visibility.Visible : Visibility.Collapsed;

            if (AutomaticallyReadData)
            {
                await DoReadAsync(true); // update the value field even if it's not readable
            }
            else
            {

            }

#if NEVER_EVER_DEFINED
            if (characteristic.Uuid == GoveeKeepaliveCharacteristicUuid)
            {
                System.Diagnostics.Debug.WriteLine("DBG: got the one Govee Keepalive / humi_cmd characteristic");
                ConstantlyWriteGoveeKeepalive (characteristic);
            }
#endif

            var namestr = "";
            if (!String.IsNullOrEmpty(NC?.Name))
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
                if (infostr.Contains("humi"))
                {
                    ; // Handy line to hang a debugger on.
                }
                // What the heck -- as long as we're filling up the space with the long GUID,
                // let's throw in the description (if it exists)
                if (!String.IsNullOrWhiteSpace(characteristic.UserDescription))
                {
                    infostr += " " + characteristic.UserDescription;
                }
                uiInfo.Text = infostr;
                uiInfo.Visibility = Visibility.Visible;
            }
            if (namestr.Contains("humi"))
            {
                ; // Handy line to hang a debugger on.
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
            try
            {
                SetupUI(); // sets up the fancy UI with sliders and buttons.
            }
            catch (Exception)
            {
                ;
            }

            if (NC != null && NC.Commands != null)
            {
                foreach (var (_, command) in NC.Commands)
                {
                    command.InitVariables();
                    command.WriteCharacteristic = this;
                }
            }
        }

#if NEVER_EVER_DEFINED
        // NOTE: The keepalive here does absolutely nothing useful. This code should be removed.
        //static Guid GoveeKeepaliveCharacteristicUuid = Guid.Parse("494e5445-4c4c-495f-524f-434b535f2012"); // From OpenHab
        static Guid GoveeKeepaliveCharacteristicUuid = Guid.Parse("494e5445-4c4c-495f-524f-434b535f2014"); // Use the heartbeat one?
        Task GoveeTask = null;
        private void ConstantlyWriteGoveeKeepalive(GattCharacteristic characteristic)
        {
            GoveeTask = Task.Run(async () =>
          {
              while (true)
              {
                  await Task.Delay(1000); // delay 2 seconds 
                  GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
                  try
                  {
                      byte[] cmd = new byte[20] { 0x33, 0x01, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, 0x32 }; // 0x32 is the CRC for the command
                      var buffer = Windows.Security.Cryptography.CryptographicBuffer.CreateFromByteArray(cmd); // UWP has these really dumb aspects.
                      status = await characteristic.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse);
                      System.Diagnostics.Debug.WriteLine($"DBG: TODO: wrote keepalive; result is {status}");
                  }
                  catch (Exception ex)
                  {
                      switch ((uint)ex.HResult)
                      {
                          case 0x80000013:
                              System.Diagnostics.Debug.WriteLine($"ERROR: failed to write keepalive; object is closed");
                              break;
                          default:
                              System.Diagnostics.Debug.WriteLine($"ERROR: TODO: Exception while writing keepalive; result is {ex.Message}");
                              break;
                      }
                  }
              }

          });
        }

#endif

        const double ElementMinWidth = 150;
        Thickness ElementMargin = new Thickness(2);
        /// <summary>
        /// Set up the uiUI list based on the UIList of SimpleUI
        /// </summary>
        private void SetupUI()
        {
            Panel panel = uiUI;
            panel.Children.Clear();
            Stack<Panel> stack = new Stack<Panel>();
            var SPACE = new char[] { ' ' };

            if (NC == null || NC.UIList == null) return;

            foreach (var simple in NC.UIList)
            {
                // Find the target
                Command command = null;
                var tlist = string.IsNullOrEmpty (simple.Target) ? new string[] { "" } : simple.Target.Split(SPACE);
                NC.Commands.TryGetValue(tlist[0], out command);


                switch (simple.UIType)
                {
                    default:
                        Console.WriteLine($"ERROR: unknown UIType {simple.UIType} for {NC.Name}");
                        break;
                    case "Blank":
                        {
                            var tb = new TextBlock() { Text = "" };
                            panel.Children.Add(tb);
                        }
                        break;
                    case "ButtonFor":
                        {
                            var b = new Button()
                            {
                                Content = simple.Label ?? command?.Label,
                                Tag = simple,
                                MinWidth = ElementMinWidth,
                                Margin = ElementMargin,
                            };
                            b.Click += B_Click;
                            panel.Children.Add(b);
                        }
                        break;
                    case "ComboBoxFor":
                        {
                            var param = command?.Parameters[tlist[1]];
                            var cb = new ComboBox()
                            {
                                Header = simple.Label ?? command?.Label,
                                MinWidth = ElementMinWidth,
                                Tag = simple,
                            };
                            ComboBoxItem selected = null;
                            foreach (var (name, value) in param.ValueNames)
                            {
                                var cbi = new ComboBoxItem()
                                {
                                    Content = name,
                                    Tag = (simple, param, name, value)
                                };
                                if (value == param.Init)
                                {
                                    selected = cbi;
                                }
                                cb.Items.Add(cbi);
                            }
                            cb.SelectionChanged += Cb_SelectionChanged;
                            panel.Children.Add(cb);
                        }
                        break;
                    case "RadioFor":
                        {
                            var param = command?.Parameters[tlist[1]];
                            var sp = new StackPanel();
                            foreach (var (name, value) in param.ValueNames)
                            {
                                var rb = new RadioButton()
                                {
                                    Content = name,
                                    Tag = (simple, param, name, value)
                                };
                                if (value == param.Init)
                                {
                                    rb.IsChecked = true;
                                }
                                rb.Checked += Rb_Checked;
                                sp.Children.Add(rb);
                            }
                            panel.Children.Add(sp);
                        }
                        break;
                    case "RowEnd":
                        panel = stack.Pop();
                        break;
                    case "RowStart":
                        {
                            var n = simple.GetN();
                            var grid = new VariableSizedWrapGrid()
                            {
                                Orientation = Orientation.Horizontal,
                                MaximumRowsOrColumns = n,
                            };
                            panel.Children.Add(grid);
                            stack.Push(panel);
                            panel = grid;
                        }
                        break;
                    case "SliderFor":
                        {
                            var param = command?.Parameters[tlist[1]];
                            var s = new Slider()
                            {
                                Width = ElementMinWidth,
                                Header = simple.Label ?? param.Label,
                                Minimum = param.Min,
                                Maximum = param.Max,
                                Value = param.Init,
                                Tag = simple,
                                Margin = ElementMargin
                            };
                            s.ValueChanged += S_ValueChanged;
                            panel.Children.Add(s);
                        }
                        break;
                }
            }
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return; // should never happen.
            var cbi = e.AddedItems[0] as ComboBoxItem;
            var (simple, variable, name, value) = ((SimpleUI, VariableDescription, String, Double))(cbi.Tag);
            variable.CurrValue = value;
        }

        private void Rb_Checked(object sender, RoutedEventArgs e)
        {
            var (simple, variable, name, value) = ((SimpleUI, VariableDescription, String, Double))((sender as FrameworkElement).Tag);
            variable.CurrValue = value;
        }

        private async void S_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var simple = (sender as FrameworkElement).Tag as SimpleUI;
            if (simple == null) return;

            var target = simple.Target.Split(new char[] { ' ' });
            if (target.Length != 2) return;

            Command command = null;
            NC.Commands.TryGetValue(target[0], out command);
            if (command != null)
            {
                command.Parameters[target[1]].CurrValue = e.NewValue;
            }
            NC.Commands.TryGetValue(simple.ComputeTarget ?? "", out command);
            if (command != null)
            {
                await command.DoCommand();
            }
        }

        private async void B_Click(object sender, RoutedEventArgs e)
        {
            var simple = (sender as FrameworkElement).Tag as SimpleUI;
            if (simple == null) return;

            Command command = null;

            foreach (var setter in simple.Set)
            {
                // e.g. Sport Direction Forward
                // set the direction parameter to the named Forward value
                var target = setter.Split(new char[] { ' ' });
                if (target.Length == 3)
                {
                    NC.Commands.TryGetValue(target[0], out command);
                    if (command != null && command.Parameters.ContainsKey(target[1]))
                    {
                        var variable = command.Parameters[target[1]];
                        var value = variable.ValueNames[target[2]];
                        variable.CurrValue = value;
                    }
                }
            }

            NC.Commands.TryGetValue(simple.Target, out command);
            if (command != null)
            {
                await command.DoCommand();
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
            try
            {
                await dlg.ShowAsync();
            }
            catch (Exception)
            {
                // Sigh. Some devices like the LEDDMX take a really long time to pop up, allowing the user to 
                // click multiple times and therefore trying to show two dialogs at once (which isn't allowed)
            }

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

        public async Task<GattWriteResult> DoWriteString (string str)
        {
            GattWriteOption writeOption = PreferredWriteOption();
            var bytes = Encoding.UTF8.GetBytes(str);
            const int MAXBYTES = 20;
            if (bytes.Length > MAXBYTES)
            {
                GattWriteResult status = null;

                for (int i = 0; i < bytes.Length; i += MAXBYTES)
                {
                    // So many calculations and copying just to get a slice
                    var maxCount = Math.Min(MAXBYTES, bytes.Length - i);
                    var subcommand = new ArraySegment<byte>(bytes, i, maxCount).ToArray().AsBuffer();
                    status = await Characteristic.WriteValueWithResultAsync(subcommand, writeOption);
                }
                return status;
            }

            else
            {
                var data = bytes.AsBuffer();
                var status = await Characteristic.WriteValueWithResultAsync(data, writeOption);
                return status;
            }
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
                var cvc = new CharacteristicEditorControl(NC, writeOption, AutomaticallyReadData);
                await cvc.InitAsync(Service, Characteristic);
                var dlg = new ContentDialog()
                {
                    Title = "Edit Value",
                    Content = cvc,
                    CloseButtonText = "Done",
                };
                await dlg.ShowAsync();
                prevBytesWritten = cvc.BytesWritten;

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
        byte[] prevBytesWritten = null;
        private async Task DoIncrementWriteTapped(GattWriteOption WriteOption)
        {
            var oldValue = uiValueShow.Text;
            var characteristic = Characteristic;

            try
            {
                if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                {
                    var buffer = await characteristic.ReadValueAsync();
                    if (buffer.Status == GattCommunicationStatus.Success)
                    {
                        var data = buffer.Value.ToArray();
                        await DoIncrementWriteData(WriteOption, data);
                    }
                }
                else
                {
                    await DoIncrementWriteData(WriteOption, prevBytesWritten);
                }
            }
            catch (Exception ex)
            {
                uiValueShow.Text = $"Error: {ex.Message}";
            }
        }

        private async Task DoIncrementWriteData(GattWriteOption WriteOption, byte[] data)
        {
            for (int i = data.Length - 1; i >= 0; i--)
            {
                if (data[i] < 0xFF)
                {
                    data[i]++;
                    break;
                }
                else
                {
                    data[i] = 0;
                    // And carry the one to the next byte.
                }
            }
            var dataBuffer = data.AsBuffer();
            var newAsString = ValueParser.ConvertToStringHex(dataBuffer);
            uiValueShow.Text = newAsString;
            var status = await Characteristic.WriteValueWithResultAsync(dataBuffer, WriteOption);
        }

        private async void OnIncrementWriteTapped(object sender, TappedRoutedEventArgs e)
        {
            await DoIncrementWriteTapped(GattWriteOption.WriteWithoutResponse);
        }

        private async void OnIncrementWriteWithResponseTapped(object sender, TappedRoutedEventArgs e)
        {
            await DoIncrementWriteTapped(GattWriteOption.WriteWithResponse);
        }
    }
}
