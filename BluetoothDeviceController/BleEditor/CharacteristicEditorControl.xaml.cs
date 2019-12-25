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
using Windows.UI.Popups;
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
    public sealed partial class CharacteristicEditorControl : UserControl
    {
        public CharacteristicEditorControl()
        {
            this.InitializeComponent();
        }

        GattDeviceService Service;
        GattCharacteristic Characteristic;
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
                if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                {
                    var buffer = await characteristic.ReadValueAsync();
                    if (buffer.Status == GattCommunicationStatus.Success)
                    {
                        //NOTE: also get the data as the nice version?
                        var data = ValueParser.ConvertToStringHex(buffer.Value);
                        AddData("Data", data);
                        uiEditBox.Text = data; // NOTE: what if I don't want hex?
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

        private async void OnWrite(object sender, RoutedEventArgs e)
        {
            if (Service == null) return;
            if (Characteristic == null) return;
            uiStatus.Text = "";
            uiProgress.IsActive = true;

            var str = uiEditBox.Text;
            var convertType = ((uiConvertType.SelectedItem as ComboBoxItem)?.Tag as string) ?? "HEX";
            var result = str.ConvertToBuffer(convertType);
            if (result.Result == ValueParserResult.ResultValues.Ok)
            {
                {
                    try
                    {
                        GattWriteOption writeOption = GattWriteOption.WriteWithResponse;
                        if (Characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
                        {
                            writeOption = GattWriteOption.WriteWithoutResponse;
                        }
                        uiStatus.Text = "Writing data...";
                        var status = await Characteristic.WriteValueWithResultAsync(result.ByteResult.ToArray().AsBuffer(), writeOption);

 
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
                }
            }
            else
            {
                uiStatus.Text = $"ERROR: unable to parse {str}\n{result.ErrorString}";
                return;
            }
            uiProgress.IsActive = false;
        }
    }
}
