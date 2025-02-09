using BluetoothDefinitionLanguage;
using BluetoothDeviceController.Names;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BleEditorPage : Page, HasId
    {
        public static BleEditorPage GlobalEditorPage = null;
        public static IReactToEditor MainReactToEditor = null;

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
            var di = args.Parameter as DeviceInformationWrapper;
            uiProgress.IsActive = true;
            GlobalEditorPage = this;
            MainReactToEditor.EditorStarted();
            try
            {
                if (di.di == null)
                {
                    ; // can happen when switching from one search type to another.
                    var advert = di.BleAdvert.AdvertisementType == BleAdvertisementWrapper.BleAdvertisementType.BluetoothLE ? $"({di.BleAdvert.BleAdvert.BluetoothAddress}) " : "";
                    var dlg = new ContentDialog() { 
                        Title = "Unable to switch to device", 
                        Content = $"The selected device {advert} cannot be displayed",
                        CloseButtonText = "Ok"
                    };
                    await dlg.ShowAsync();
                    uiProgress.IsActive = false;
                }
                else
                {
                    ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
                    if (ble != null)
                    {
                        var knownDevice = BleNames.GetDevice(ble.Name);
                        BleDeviceId = ble.DeviceId;
                        var upref = di.UserPreferences ?? UserPreferences.MainUserPreferences;
                        await DisplayBluetooth(knownDevice, di, ble, upref.AutomaticallyReadData);
                    }
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
        //TODO: make this settable. Right now, we sometimes get old cached data when
        // we would strongly prefer uncached data. For now, set to be always uncached
        // because that's my current problem.
        BluetoothCacheMode CurrCacheMode = BluetoothCacheMode.Uncached;
        private async Task DisplayBluetooth(NameDevice knownDevice, DeviceInformationWrapper di, BluetoothLEDevice ble, bool automaticallyReadData)
        {
            var jsonFormat = Newtonsoft.Json.Formatting.Indented;
            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ContractResolver = IgnoreEmptyEnumerableResolver.Instance,
            }; 


            var wireAllDevices = new NameAllBleDevices();
            WireDevice = new NameDevice();
            WireDevice.Name = di.di.Name;
            WireDevice.Details += $"Id:{di.di.Id}\nCanPair:{di.di.Pairing.CanPair} IsPaired:{di.di.Pairing.IsPaired}";
            wireAllDevices.AllDevices.Add(WireDevice);

            WireDevice.ClassModifiers = knownDevice.ClassModifiers;
            WireDevice.ClassName = knownDevice.ClassName;
            WireDevice.Description = knownDevice.Description;


            uiRawData.Text = Newtonsoft.Json.JsonConvert.SerializeObject(WireDevice, jsonFormat); //, jsonSettings);
            string raw = "";

            if (ble == null)
            {
                // Happens if another program is trying to use the device!
                raw = $"BLE: ERROR: Another app is using this device.\n";
            }
            else
            {
                // TODO: remove this code which is only here while I investigate the WESCALE scale
#if NEVER_EVER_DEFINED
                {
                    // WESCALE: no gatt services
                    var services = ble.GattServices;
                    var count = services.Count;
                    var devacc = ble.DeviceAccessInformation;
                    var devaccstatus = devacc.CurrentStatus;

                    foreach (var service in services)
                    {
                        var chars = service.GetAllCharacteristics();
                    }
                    try
                    {
                        var guid = Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb");
                        var request = await ble.RequestAccessAsync();

                        var allservice = await ble.GetGattServicesForUuidAsync(guid);
                        var servicefff0 = ble.GetGattService(guid);
                        var charsfff0 = servicefff0.GetAllCharacteristics();
                        var countfff0 = charsfff0.Count;
                        foreach (var ch in charsfff0)
                        {
                            ;
                        }
                    }
                    catch (Exception ex)
                    {
                        ;
                    }
                }
#endif
                GattDeviceServicesResult result = null;
                try
                {
                    result = await ble.GetGattServicesAsync(CurrCacheMode);
                }
                catch (Exception ex)
                {
                    if (CurrCacheMode == BluetoothCacheMode.Uncached)
                    {
                        // TODO: BUG: DBG: weird BT issue: trying to get uncached services
                        // will sometimes result in massive failures.
                        raw += $"EXCEPTION: while getting uncached Gatt services; will retry. Connection status={ble.ConnectionStatus}: {ex.Message}\n";
                        var diinfo = ble.DeviceAccessInformation.CurrentStatus;
                        var requestStatus = await ble.RequestAccessAsync();
                        raw += $"EXCEPTION (cont): while getting uncached Gatt services; device access info={diinfo} request access status={requestStatus}";
                        result = await ble.GetGattServicesAsync(BluetoothCacheMode.Cached);
                    }
                    else
                    {
                        raw += $"EXCEPTION: while getting Gatt services: {ex.Message}";
                    }
                }
                if (result == null)
                {
                    raw += $"Major error: while reading Gatt services, unable to read ";
                }
                else if (result.Status != GattCommunicationStatus.Success)
                {
                    int nservice = result.Services.Count;
                    raw += GetStatusString(result.Status, result.ProtocolError);
                }
                else
                {
                    var header = new BleDeviceHeaderControl();
                    await header.InitAsync(ble);

                    int serviceCount = 0;
                    int descriptorCount = 0;

                    foreach (var service in result.Services)
                    {
                        await header.AddServiceAsync(ble, service);
                        var shouldDisplay = ServiceShouldBeEdited(service);
                        //TODO: if we're gather data for the JSON, we should include all services. However, shouldDisplay is actually never used.
                        shouldDisplay = true;

                        var defaultService = knownDevice.GetService(service.Uuid.ToString("D"));
                        var wireService = new NameService(service, defaultService, serviceCount++);
                        WireDevice.Services.Add(wireService);

                        try
                        {
                            var cresult = await service.GetCharacteristicsAsync(CurrCacheMode);
                            if (cresult.Status != GattCommunicationStatus.Success)
                            {
                                raw += GetStatusString(cresult.Status, cresult.ProtocolError);
                            }
                            else
                            {
                                var characteristicCount = 0;

                                foreach (var characteristic in cresult.Characteristics)
                                {
                                    //The descriptors don't seem to be actually interesting. it's how we read and write.
                                    var descriptorStatus = await characteristic.GetDescriptorsAsync();
                                    if (descriptorStatus.Status == GattCommunicationStatus.Success)
                                    {
                                        descriptorCount++;
                                        foreach (var descriptor in descriptorStatus.Descriptors)
                                        {
                                            Console.WriteLine($"INFO: DESCRIPTOR! device {di.di.Id} service {service.Uuid} char {characteristic.Uuid} descriptor {descriptor.Uuid}");
                                            ;
                                        }
                                    }

                                    var defaultCharacteristic = defaultService?.GetChacteristic(characteristic.Uuid.ToString("D"));
                                    var wireCharacteristic = new NameCharacteristic(characteristic, defaultCharacteristic, characteristicCount++);
                                    wireService.Characteristics.Add(wireCharacteristic);

                                    if (wireCharacteristic.Suppress)
                                    {
                                        continue; // don't show a UI for a supressed characteristic.
                                    }
                                    //
                                    // Here are each of the editor children items
                                    //

                                    // Useful while debugging the Govee H5074 temperature + humidity device
                                    // The device is really "twitchy": it turns off very fast after you connect
                                    // to it. This will make the display come up much faster, allowing
                                    // the user to interact (very briefly) with it.
                                    if (String.IsNullOrEmpty (characteristic.UserDescription))
                                    {
                                        ; // just for debugging
                                    }
                                    var edit = new BleCharacteristicControl(knownDevice, service, characteristic, automaticallyReadData);
                                    uiEditor.Children.Add(edit);

                                    var dc = DeviceCharacteristic.Create(characteristic);

                                    //NOTE: does any device have a presentation format?
                                    foreach (var format in characteristic.PresentationFormats)
                                    {
                                        raw += $"    Fmt: Description:{format.Description} Namespace:{format.Namespace} Type:{format.FormatType} Unit: {format.Unit} Exp:{format.Exponent}\n";
                                    }


                                    try
                                    {
                                        if (automaticallyReadData && wireCharacteristic.IsRead)
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

            DoCopyData_Json();
        }
        public void DoCopyData_Json()
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
