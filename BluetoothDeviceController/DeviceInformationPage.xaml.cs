using BluetoothDefinitionLanguage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace BluetoothDeviceController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceInformationPage : Page
    {
        BluetoothLEDevice ble;

        public DeviceInformationPage()
        {
            this.InitializeComponent();
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

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformationWrapper;
            uiProgress.IsActive = true;
            var raw = $"Name:{di.di.Name} Id:{di.di.Id}\nCanPair:{di.di.Pairing.CanPair} IsPaired:{di.di.Pairing.IsPaired}\n";
            uiRawData.Text = raw;
            raw = "";
            //var cache = BluetoothCacheMode.Uncached;

            ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
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
                    foreach (var service in result.Services)
                    {
                        var serviceHeader = $"Service:{service.Uuid}\n";
                        raw += serviceHeader;
                        try
                        {
                            var cresult = await service.GetCharacteristicsAsync();
                            if (cresult.Status != GattCommunicationStatus.Success)
                            {
                                raw += GetStatusString(cresult.Status, cresult.ProtocolError);
                            }
                            else
                            {
                                foreach (var characteristic in cresult.Characteristics)
                                {
                                    var dc = DeviceCharacteristic.Create(characteristic);
                                    raw += $"    Characteristic:{characteristic.Uuid} {characteristic.UserDescription}\n";

                                    //NOTE: does any device have a presentation format?
                                    //AFAICT, the answer is "no"
                                    foreach (var format in characteristic.PresentationFormats)
                                    {
                                        raw += $"    Fmt: Description:{format.Description} Namespace:{format.Namespace} Type:{format.FormatType} Unit: {format.Unit} Exp:{format.Exponent}\n";
                                    }

                                    //The descriptors don't seem to be actually interesting...
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
                                        raw += $"    Methods: {dc.PropertiesAsString}\n";
                                        if (dc.CanRead)
                                        {
                                            var vresult = await characteristic.ReadValueAsync();
                                            if (vresult.Status != GattCommunicationStatus.Success)
                                            {
                                                raw += GetStatusString(vresult.Status, vresult.ProtocolError);
                                            }
                                            else
                                            {
                                                var dt = BluetoothLEStandardServices.GetDisplayInfo(service, characteristic);
                                                var str = dt.AsString(vresult.Value);
                                                raw += $"    {str}\n";
                                            }
                                        }
                                        
                                    }
                                    catch (Exception e)
                                    {
                                        raw += $"    Exception reading value: {e.HResult} {e.Message}\n";
                                    }
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
            uiProgress.IsActive = false;
            uiRawData.Text += raw;
            return;

        }
    }
}
