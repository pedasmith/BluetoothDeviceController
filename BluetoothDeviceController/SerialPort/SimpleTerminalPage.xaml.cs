using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SerialPort
{
    public delegate void TerminalSendDataEventHandler(object sender, string data);
    public interface ITerminal
    {
        /// <summary>
        /// The device has gotten some data and calls this method to tell the terminal to display it.
        /// </summary>
        /// <param name="data"></param>
        void ReceivedData(string data);

        /// <summary>
        /// The terminal has gotten data from the user which needs to be send to the device.
        /// </summary>
        event TerminalSendDataEventHandler OnSendData;
        /// <summary>
        /// The device has errored out in some way and calls this method to tell the terminal to display it.
        /// </summary>
        /// <param name="error"></param>
        void ErrorFromDevice(string error);

        void SetDeviceStatus(string status);
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimpleTerminalPage : Page, ITerminal, SpecialtyPages.ISetHandleStatus
    {
        public enum TerminalLineEnd { None, CR, LF, CRLF };
        TerminalLineEnd LineEnd = TerminalLineEnd.CRLF; // Needed for Slant LittleBot

        BluetoothCommTerminalAdapter TerminalAdapter;

        // UI setting to distinguish send receive and error text.
        FontWeight SendWeight = FontWeights.Bold;
        FontWeight ReceiveWeight = FontWeights.Normal;
        Brush ErrorBrush = new SolidColorBrush(Colors.DarkRed);
        Thickness macroButtonMargin = new Thickness(2);


        public SimpleTerminalPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformationWrapper;
            ParentStatusHandler?.SetStatusActive(true);
            AddShortcutButtons(di.di.Name);

            TerminalAdapter = new BluetoothCommTerminalAdapter(this, di);
            await TerminalAdapter.InitAsync();

            ParentStatusHandler?.SetStatusActive(false);
        }


        private SpecialtyPages.IHandleStatus ParentStatusHandler = null;
        public void SetHandleStatus(SpecialtyPages.IHandleStatus handleStatus)
        {
            ParentStatusHandler = handleStatus;
        }

        private void AddShortcutButtons(string deviceName)
        {
            var list = AllShortcuts.GetShortcuts(deviceName);
            uiMacroButtonList.Children.Clear();
            foreach (var shortcuts in list)
            {
                foreach (var shortcut in shortcuts.List)
                {
                    var b = new Button()
                    {
                        Content = shortcut.Label,
                        Tag = shortcut.Replace,
                        Width = 100,
                        Margin = macroButtonMargin,
                    };
                    b.Click += OnMacroClick;
                    uiMacroButtonList.Children.Add(b);
                }
            }
        }

        private void OnMacroClick(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            var text = b?.Tag as string;
            if (text == null) return;

            AddInputTextToUI(text);
            OnSendData?.Invoke(this, text);
        }

        public event TerminalSendDataEventHandler OnSendData;

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
        public async void SetDeviceStatus(string text)
        {
            ;
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => {
                    ParentStatusHandler?.SetStatusText(text);
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

            var sendText = ConvertString(text);
            if (!sendText.EndsWith('\r') && !sendText.EndsWith('\n'))
            {
                switch (LineEnd)
                {
                    case TerminalLineEnd.CR: sendText += "\r"; break;
                    case TerminalLineEnd.LF: sendText += "\n"; break;
                    case TerminalLineEnd.CRLF: sendText += "\r\n"; break;
                }
            }
            AddInputTextToUI(sendText);
            tb.Text = "";
            OnSendData?.Invoke (this, sendText);
        }

        private string ConvertString(string str)
        {
            var last = str.Length - 1;
            if (last < 0) return "";

            var retval = str;
            while (retval.Length > 0 && retval[retval.Length-1] == '\r') retval = retval.Substring(0, retval.Length - 1);
            while (retval.Length > 0 && retval[retval.Length-1] == '\n') retval = retval.Substring(0, retval.Length - 1);

            retval = retval.Replace("\\n", "\n");
            retval = retval.Replace("\\r", "\r");
            retval = retval.Replace("\\t", "\t");
            retval = retval.Replace("\\0", "\0");
            return retval;
        }
    }
}
