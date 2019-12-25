using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static BluetoothDeviceController.BleEditor.BleEditorPage;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController
{
    public sealed partial class GenerateCodeControl : UserControl
    {
        public GenerateCodeControl()
        {
            this.InitializeComponent();
            this.Loaded += GenerateCodeControl_Loaded;
        }

        private void GenerateCodeControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Add all JSON files to the list
            foreach (var nameDevice in BleNames.AllRawDevices.AllDevices)
            {
                uiJsonNames.Items.Add(new ComboBoxItem() { Content = nameDevice.Name });
            }
        }

        NameDevice SelectedNameDevice = null;
        private void OnSelectJsonFile(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;

            var name = (e.AddedItems[0] as ComboBoxItem).Content as string;
            SelectedNameDevice = BleNames.GetDevice(name, BleNames.AllRawDevices);

            DisplaySelection();
        }



        string SelectedOutput = "JSON";
        private void OnSelectOutput(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            var tag = (e.AddedItems[0] as ComboBoxItem).Tag as string;
            SelectedOutput = tag;
            DisplaySelection();
        }

        private void DisplaySelection()
        {
            string str;
            switch (SelectedOutput)
            {
                case "JSON":
                    var jsonFormat = Newtonsoft.Json.Formatting.Indented;
                    var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
                        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                        ContractResolver = IgnoreEmptyEnumerableResolver.Instance,
                    };
                    str = Newtonsoft.Json.JsonConvert.SerializeObject(SelectedNameDevice, jsonFormat, jsonSettings);
                    uiJsonText.Text = str;
                    CopyToClipboard(str, "JSON device description");
                    break;
                case "Protocol_C#":
                    str = GenerateCSharpClass.GenerateProtocol(SelectedNameDevice);
                    uiJsonText.Text = str;
                    CopyToClipboard(str, "C# device protocol");
                    break;
                case "Page_XAML":
                    str = GenerateCSharpClass.GeneratePageXaml(SelectedNameDevice);
                    uiJsonText.Text = str;
                    CopyToClipboard(str, "C# device protocol");
                    break;
                case "Page_C#":
                    str = GenerateCSharpClass.GeneratePageCSharp(SelectedNameDevice);
                    uiJsonText.Text = str;
                    CopyToClipboard(str, "C# device protocol");
                    break;
            }
        }

        private void CopyToClipboard(string str, string title)
        {
            var dp = new DataPackage();
            dp.SetText(str);
            dp.Properties.Title = title;
            Clipboard.SetContent(dp);
        }
    }
}
