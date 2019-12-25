using BluetoothDefinitionLanguage;
using BluetoothDeviceController.Names;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BleEditorPage : Page, HasId
    {
        BluetoothLEDevice ble;
        public string JsonAsList { get; internal set; } = "";
        public string JsonAsSingle { get; internal set; } = "";
        public bool NavigationComplete = false;
        public BleEditorPage()
        {
            this.InitializeComponent();
        }
        public string GetId()
        {
            return BleDeviceId;
        }

        private string DeviceName = "Device_Outline";
        private string DeviceNameUser = "Bluetooth Device";
        public string GetPicturePath()
        {
            return $"/Assets/DevicePictures/{DeviceName}-175.PNG";
        }
        public string GetDeviceNameUser()
        {
            return $"{DeviceNameUser}";
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
        private bool ServiceShouldBeEdited(GattDeviceService service)
        {
            var serviceuuid = service.Uuid.ToString();
            if (serviceuuid.StartsWith("000018"))
            {
                return false; // remove all the common services like 00001800-0000-1000-8000-00805f9b34fb
            }
            return true;
        }

        NameDevice WireDevice;
        String BleDeviceId = "";
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformation;
            uiProgress.IsActive = true;

            try
            {
                ble = await BluetoothLEDevice.FromIdAsync(di.Id);
                if (ble != null)
                {
                    var knownDevice = BleNames.GetDevice(ble.Name);
                    BleDeviceId = ble.DeviceId;
                    await DisplayBluetooth(knownDevice, di, ble);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: unable to navigate {ex.Message}");
                // I don't know of any exceptions. But if there are any, supress them completely.
            }
            NavigationComplete = true ;
        }

        // From https://stackoverflow.com/questions/11320968/can-newtonsoft-json-net-skip-serializing-empty-lists
        public class IgnoreEmptyEnumerableResolver : DefaultContractResolver
        {
            public static readonly IgnoreEmptyEnumerableResolver Instance = new IgnoreEmptyEnumerableResolver();

            protected override JsonProperty CreateProperty(MemberInfo member,
                MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);

                if (property.PropertyType != typeof(string) &&
                    typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    property.ShouldSerialize = instance =>
                    {
                        IEnumerable enumerable = null;
                        // this value could be in a public field or public property
                        switch (member.MemberType)
                        {
                            case MemberTypes.Property:
                                enumerable = instance
                                    .GetType()
                                    .GetProperty(member.Name)
                                    ?.GetValue(instance, null) as IEnumerable;
                                break;
                            case MemberTypes.Field:
                                enumerable = instance
                                    .GetType()
                                    .GetField(member.Name)
                                    .GetValue(instance) as IEnumerable;
                                break;
                        }

                        return enumerable == null ||
                               enumerable.GetEnumerator().MoveNext();
                        // if the list is null, we defer the decision to NullValueHandling
                    };
                }

                return property;
            }
        }
        private async Task DisplayBluetooth(NameDevice knownDevice, DeviceInformation di, BluetoothLEDevice ble)
        {
            var jsonFormat = Newtonsoft.Json.Formatting.Indented;
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ContractResolver = IgnoreEmptyEnumerableResolver.Instance,
            };

            var wireAllDevices = new NameAllDevices();
            WireDevice = new NameDevice();
            WireDevice.Name = di.Name;
            WireDevice.Details += $"Id:{di.Id}\nCanPair:{di.Pairing.CanPair} IsPaired:{di.Pairing.IsPaired}";
            wireAllDevices.AllDevices.Add(WireDevice);

            WireDevice.ClassModifiers = knownDevice.ClassModifiers;
            WireDevice.ClassName = knownDevice.ClassName;
            WireDevice.Description = knownDevice.Description;


            uiRawData.Text = Newtonsoft.Json.JsonConvert.SerializeObject(WireDevice, jsonFormat);
            string raw = null;

            if (ble == null)
            {
                // Happens if another program is trying to use the device!
                raw = $"BLE: ERROR: Another app is using this device.\n";
            }
            else
            {
                var result = await ble.GetGattServicesAsync();
                if (result.Status != GattCommunicationStatus.Success)
                {
                    raw += GetStatusString(result.Status, result.ProtocolError);
                }
                else
                {
                    var header = new BleDeviceHeaderControl();
                    await header.InitAsync(ble);

                    int serviceCount = 0;

                    foreach (var service in result.Services)
                    {
                        await header.AddServiceAsync(ble, service);
                        var shouldDisplay = ServiceShouldBeEdited(service);

                        var defaultService = knownDevice.GetService(service.Uuid.ToString("D"));
                        var wireService = new NameService(service, defaultService, serviceCount++);
                        WireDevice.Services.Add(wireService);

                        try
                        {
                            var cresult = await service.GetCharacteristicsAsync();
                            if (cresult.Status != GattCommunicationStatus.Success)
                            {
                                raw += GetStatusString(cresult.Status, cresult.ProtocolError);
                            }
                            else
                            {
                                var characteristicCount = 0;

                                foreach (var characteristic in cresult.Characteristics)
                                {
                                    var defaultCharacteristic = defaultService?.GetChacteristic(characteristic.Uuid.ToString("D"));
                                    var wireCharacteristic = new NameCharacteristic(characteristic, defaultCharacteristic, characteristicCount++);
                                    wireService.Characteristics.Add(wireCharacteristic);

                                    //
                                    // Here are each of the editor children items
                                    //
                                    var edit = new BleCharacteristicControl(knownDevice, service, characteristic);
                                    uiEditor.Children.Add(edit);

                                    var dc = DeviceCharacteristic.Create(characteristic);

                                    //NOTE: does any device have a presentation format?
                                    foreach (var format in characteristic.PresentationFormats)
                                    {
                                        raw += $"    Fmt: Description:{format.Description} Namespace:{format.Namespace} Type:{format.FormatType} Unit: {format.Unit} Exp:{format.Exponent}\n";
                                    }

                                    //The descriptors don't seem to be actually interesting. it's how we read and write.
                                    var descriptorStatus = await characteristic.GetDescriptorsAsync();
                                    if (descriptorStatus.Status == GattCommunicationStatus.Success)
                                    {
                                        foreach (var descriptor in descriptorStatus.Descriptors)
                                        {
                                            ;
                                        }
                                    }


                                    try
                                    {
                                        if (wireCharacteristic.IsRead)
                                        {
                                            var vresult = await characteristic.ReadValueAsync();
                                            if (vresult.Status != GattCommunicationStatus.Success)
                                            {
                                                raw += GetStatusString(vresult.Status, vresult.ProtocolError);
                                            }
                                            else
                                            {
                                                var dt = BluetoothLEStandardServices.GetDisplayInfo(service, characteristic);
                                                var hexResults = dt.AsString(vresult.Value);
                                                wireCharacteristic.ExampleData.Add (hexResults);

                                                // And add as a converted value
                                                var NC = BleNames.Get(knownDevice, service, characteristic);
                                                var decode = NC?.Type;
                                                if (decode != null && !decode.StartsWith("BYTES|HEX"))
                                                {
                                                    var decoded = ValueParser.Parse(vresult.Value, decode);
                                                    wireCharacteristic.ExampleData.Add(decoded.UserString);
                                                }
                                            }
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        raw += $"    Exception reading value: {e.HResult} {e.Message}\n";
                                    }

                                    // Update the UI with the latest discovery
                                    // Later: or not. The raw data isn't super useful.
                                    //uiRawData.Text = Newtonsoft.Json.JsonConvert.SerializeObject(WireDevice, jsonFormat);

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            raw += $"    Exception reading characteristic: {e.HResult} {e.Message}\n";
                        }
                    }
                }
            }
            JsonAsList = Newtonsoft.Json.JsonConvert.SerializeObject(wireAllDevices, jsonFormat, jsonSettings);
            JsonAsSingle = Newtonsoft.Json.JsonConvert.SerializeObject(WireDevice, jsonFormat, jsonSettings);

            uiProgress.IsActive = false;
            uiRawData.Text += raw;
            return;

        }



        private void OnCopyData_Json(object sender, RoutedEventArgs e)
        {
            var dp = new DataPackage();
            dp.SetText(JsonAsList);
            dp.Properties.Title = "JSON Bluetooth data";
            Clipboard.SetContent(dp);
        }

        private void OnCopyData_NetProtocol(object sender, RoutedEventArgs e)
        {
            var dp = new DataPackage();
            if (WireDevice == null)
            {
                dp.SetText("No data and therefore no class");
            }
            else
            {
                var str = GenerateCSharpClass.GenerateProtocol(WireDevice);
                dp.SetText(str);
            }
            dp.Properties.Title = "Class to read and write Bluetooth data";
            Clipboard.SetContent(dp);
        }

        private void OnCopyData_PageXaml(object sender, RoutedEventArgs e)
        {
            var dp = new DataPackage();
            if (WireDevice == null)
            {
                dp.SetText("No data and therefore no class");
            }
            else
            {
                var str = GenerateCSharpClass.GeneratePageXaml(WireDevice);
                dp.SetText(str);
            }
            dp.Properties.Title = "Class to read and write Bluetooth data";
            Clipboard.SetContent(dp);
        }

        private void OnCopyData_PageNet(object sender, RoutedEventArgs e)
        {
            var dp = new DataPackage();
            if (WireDevice == null)
            {
                dp.SetText("No data and therefore no class");
            }
            else
            {
                var str = GenerateCSharpClass.GeneratePageCSharp(WireDevice);
                dp.SetText(str);
            }
            dp.Properties.Title = "Class to read and write Bluetooth data";
            Clipboard.SetContent(dp);
        }
    }
}
