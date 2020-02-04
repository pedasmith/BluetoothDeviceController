using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static BluetoothDeviceController.UserSerialPortPreferences;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.SerialPort
{
    public sealed partial class SimpleTerminalControl : UserControl, ITerminal
    {
        // Must set all of these
        public ITerminal ParentTerminal = null;
        public DeviceInformationWrapper DI = null;
        public IDoTerminalSendData DoSendData = null;
        UserSerialPortPreferences SerialPortPreferences { get { return DI.SerialPortPreferences; } }

        // UI setting to distinguish send receive and error text.
        FontWeight SendWeight = FontWeights.Bold;
        FontWeight ReceiveWeight = FontWeights.Normal;
        Brush ErrorBrush = new SolidColorBrush(Colors.DarkRed);
        Thickness shortcutButtonMargin = new Thickness(2);


        public SimpleTerminalControl()
        {
            this.InitializeComponent();
        }


        IList<SerialConfig> CurrConfigList = null;
        Thickness SliderMargin = new Thickness(2);
        public void UpdateShortcutButtons()
        {
            if (DI == null)
            {
                return;
            }

            // Getting the list of buttons.
            var deviceName = DI.di.Name;
            CurrConfigList = AllShortcuts.GetShortcuts(deviceName);
            if (!String.IsNullOrEmpty(SerialPortPreferences?.ShortcutId))
            {
                var preflist = AllShortcuts.GetShortcuts("", SerialPortPreferences.ShortcutId);
                if (preflist.Count > 0)
                {
                    CurrConfigList = preflist;
                }
            }

            uiShortcutButtonList.Children.Clear();
            PrevValues.Clear();
            CurrValues.Clear();

            uiShortcutButtonList.ItemWidth = 120;
            uiShortcutButtonList.ItemHeight = 60;

            foreach (var shortcuts in CurrConfigList)
            {
                // Add in all values
                foreach (var (name, setting) in shortcuts.Settings)
                {
                    setting.CmdName = name;
                    switch (setting.InputType)
                    {
                        case SerialConfigSetting.UiType.Hide:
                            // Hiden values do nothing.
                            break;

                        case SerialConfigSetting.UiType.TextBox:
                            {
                                var numberScope = new InputScope();
                                numberScope.Names.Add(new InputScopeName() { NameValue = InputScopeNameValue.Number });
                                var tb = new TextBox()
                                {
                                    Header = setting.Label ?? setting.Name ?? name,
                                    InputScope = numberScope,
                                    Text = setting.Init.ToString(),
                                    Tag = setting,
                                };
                                tb.TextChanged += SettingValueTextChanged;
                                uiShortcutButtonList.Children.Add(tb);
                                VariableSizedWrapGrid.SetColumnSpan(tb, 1);
                            }
                            break;
                        case SerialConfigSetting.UiType.Slider:
                            {
                                var slider = new Slider()
                                {
                                    Header = setting.Label ?? setting.Name ?? name,
                                    Value = setting.Init,
                                    Margin = SliderMargin,
                                    Minimum = setting.Min,
                                    Maximum = setting.Max,
                                    Tag = setting,
                                };
                                slider.ValueChanged += SliderValueChanged;
                                uiShortcutButtonList.Children.Add(slider);
                                VariableSizedWrapGrid.SetColumnSpan(slider, 1);
                            }
                            break;

                    }
                    PrevValues[name] = setting.Init;
                    CurrValues[name] = setting.Init;
                }

                FontFamily ff = new FontFamily("Segoe UI,Segoe MDL2 Assets");
                // Add in the button proper
                foreach (var (name, shortcut) in shortcuts.Commands)
                {
                    if (!shortcut.IsHidden)
                    {
                        var b = new Button()
                        {
                            Content = shortcut.Label,
                            FontFamily = ff,
                            Tag = shortcut,
                            Width = 100,
                            Margin = shortcutButtonMargin,
                            VerticalAlignment = VerticalAlignment.Bottom,
                        };
                        b.Click += OnShortcutClick;
                        uiShortcutButtonList.Children.Add(b);
                    }
                }
            }
        }

        private async void SliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var slider = sender as Slider;
            var scs = slider.Tag as SerialConfigSetting;
            double newValue = slider.Value;
            CurrValues[scs.CmdName] = newValue;
            await DoCommandAsync(scs.OnChange);
        }

        private async void SettingValueTextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            var scs = tb.Tag as SerialConfigSetting;
            double newValue;
            bool convertOk = Double.TryParse(tb.Text, out newValue);
            if (convertOk)
            {
                CurrValues[scs.CmdName] = newValue;
                await DoCommandAsync(scs.OnChange);
            }
        }

        private async Task DoCommandAsync(string cmdname)
        {
            if (String.IsNullOrEmpty(cmdname)) return;
            if (CurrCommandState != CommandState.Ready) return;

            foreach (var shortcuts in CurrConfigList)
            {
                foreach (var (name, shortcut) in shortcuts.Commands)
                {
                    if (name == cmdname)
                    {
                        await DoCommandAsync(shortcut, true);
                    }
                }
            }
        }

        Dictionary<string, double> CurrValues = new Dictionary<string, double>();
        Dictionary<string, double> PrevValues = new Dictionary<string, double>();

        private async void OnShortcutClick(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var command = b?.Tag as Command;
            await DoCommandAsync(command, false);
        }

        enum CommandState { Ready, Busy };
        CommandState CurrCommandState = CommandState.Ready;

        public event TerminalSendDataEventHandler OnSendData;
        private void SendDataToListener(string data)
        {
            DoSendData?.DoInvoke(this, data);
            OnSendData?.Invoke(this, data);
        }

        private async Task DoCommandAsync(Command command, bool wait)
        {
            if (command == null) return;
            CurrCommandState = CommandState.Busy;

            foreach (var (name, newValue) in command.Set)
            {
                CurrValues[name] = newValue;
            }

            var cmd = command.Replace;
            if (!string.IsNullOrEmpty(command.Compute))
            {
                var commandlist = command.Compute.Split(new char[] { ' ' });
                cmd = "";
                foreach (var strcommand in commandlist)
                {
                    var calculateResult = BleEditor.ValueCalculate.Calculate(strcommand, double.NaN, null, PrevValues, CurrValues);
                    cmd += calculateResult.S ?? calculateResult.D.ToString();
                }

                // Actually calculate the value and then move new-->old
                foreach (var (name, value) in CurrValues)
                {
                    PrevValues[name] = value;
                }
            }
            if (!string.IsNullOrEmpty(command.ReplaceFile))
            {
                // Is like SerialFiles\\Espruino_Modules_Smartibot.js in the Assets directory
                var filename = command.ReplaceFile;
                try
                {
                    StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                    string fname = @"Assets\" + filename;
                    var f = await InstallationFolder.GetFileAsync(fname);
                    var fcontents = File.ReadAllText(f.Path);
                    cmd = fcontents;
                }
                catch (Exception)
                {
                }
            }
            if (!String.IsNullOrEmpty(cmd))
            {
                AddInputTextToUI(cmd);
                SendDataToListener(cmd);
                if (wait) await Task.Delay(100); // wait until the device has the chance to handle the command.
            }
            CurrCommandState = CommandState.Ready;

            // And do the follow-on command, if any.
            if (!string.IsNullOrEmpty(command.OnChange))
            {
                await DoCommandAsync(command.OnChange);
            }
        }

        /// <summary>
        /// Helper method for AddRun
        /// </summary>
        /// <returns></returns>
        private Paragraph EnsureParagraph()
        {
            if (uiOutput.Blocks.Count == 0)
            {
                uiOutput.Blocks.Add(new Paragraph());
            }
            if (!(uiOutput.Blocks[uiOutput.Blocks.Count - 1] is Paragraph))
            {
                uiOutput.Blocks.Add(new Paragraph());
            }
            return uiOutput.Blocks[uiOutput.Blocks.Count - 1] as Paragraph;
        }
        /// <summary>
        ///  Helper method for everyone who wants to add text to the existing terminal. Must be called on the foreground thread.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="addLineBreak"></param>
        /// <returns></returns>
        private void AddRun(Run run, bool addLineBreak)
        {
            var paragraph = EnsureParagraph();
            paragraph.Inlines.Add(run);
            if (addLineBreak)
            {
                // // //paragraph.Inlines.Add(new LineBreak());
            }
            ScrollToBottom();
        }
        private void ScrollToBottom()
        {
            var v = uiOutputScroller.ScrollableHeight;
            if (v > 0)
            {
                uiOutputScroller.ChangeView(null, v, null);
            }
        }
        public async void ReceivedData(string text)
        {
            //TODO: handle the case of getting e.g. one\rtwo\r\nthree\nfour\r\nlines and then more_words\r
            ;
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => {
                    var addLineBreak = text.EndsWith('\n') || text.EndsWith('\r');
                    var run = new Run(); // Note that you can't make a new 'run' on the non-UI thread.
                    run.Text = text;
                    run.FontWeight = ReceiveWeight;
                    AddRun(run, addLineBreak);
                });
        }
        public async void ErrorFromDevice(string text)
        {
            ;
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => {
                    var addLineBreak = text.EndsWith('\n') || text.EndsWith('\r');
                    var run = new Run();
                    run.Text = text;
                    run.FontWeight = ReceiveWeight;
                    run.Foreground = ErrorBrush;
                    AddRun(run, addLineBreak);
                });
        }
        private void AddInputTextToUI(string text)
        {
            var addLineBreak = text.EndsWith('\n') || text.EndsWith('\r');

            var run = new Run();
            run.Text = text;
            run.FontWeight = SendWeight;
            AddRun(run, addLineBreak);
        }


        private void OnInputText(object sender, TextChangedEventArgs e)
        {

            var tb = sender as TextBox;
            if (tb == null) return;
            var text = tb.Text;
            if (!text.EndsWith('\r')) return;

            if (DI == null) // when DI is null, DI.(stuff) won't work...
            {
                // Reject the input entirely if there's no device!
                ErrorFromDevice($"Can't connect to the device; input rejected.\n");
                return;
            }

            var sendText = ConvertString(text);
            if (!sendText.EndsWith('\r') && !sendText.EndsWith('\n'))
            {
                switch (SerialPortPreferences.LineEnd)
                {
                    case TerminalLineEnd.CR: sendText += "\r"; break;
                    case TerminalLineEnd.LF: sendText += "\n"; break;
                    case TerminalLineEnd.CRLF: sendText += "\r\n"; break;
                }
            }
            AddInputTextToUI(sendText);
            tb.Text = "";
            SendDataToListener(sendText);
        }

        private string ConvertString(string str)
        {
            var last = str.Length - 1;
            if (last < 0) return "";

            var retval = str;
            while (retval.Length > 0 && retval[retval.Length - 1] == '\r') retval = retval.Substring(0, retval.Length - 1);
            while (retval.Length > 0 && retval[retval.Length - 1] == '\n') retval = retval.Substring(0, retval.Length - 1);

            retval = retval.Replace("\\n", "\n");
            retval = retval.Replace("\\r", "\r");
            retval = retval.Replace("\\t", "\t");
            retval = retval.Replace("\\0", "\0");
            return retval;
        }

        public async void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            if (DI == null)
            {
                // Reject the input entirely if there's no device!
                ErrorFromDevice($"Can't connect to the device; input rejected.\n");
                return;
            }

            var oldShortcutId = DI.SerialPortPreferences.ShortcutId;
            var settings = new UserSerialPortPreferencesControl();
            settings.SetPreferences(SerialPortPreferences);
            var dlg = new ContentDialog()
            {
                Content = settings,
                PrimaryButtonText = "OK",
                Title = "Serial Terminal Settings"
            };

            // There are two different dialogs; 
            ContentDialogResult result = ContentDialogResult.None;
            try
            {
                await (App.Current as App).WaitForAppLockAsync("SerialPortSettingsDlg");
                result = await dlg.ShowAsync();
                SerialPortPreferences.SaveToLocalSettings(); // Always save the old values.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: SerialPortSettingsDlg: {ex.Message}");
            }
            finally
            {
                (App.Current as App).ReleaseAppLock("SerialPortSettingsDlg");
            }
            if (DI.SerialPortPreferences.ShortcutId != oldShortcutId)
            {
                UpdateShortcutButtons();
            }
        }

        public void SetDeviceStatus(string status)
        {
            ParentTerminal.SetDeviceStatus(status);
        }
    }
}
