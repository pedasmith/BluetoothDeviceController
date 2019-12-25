using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.BleEditor
{
    public sealed partial class BleDeviceHeaderControl : UserControl
    {
        public BleDeviceHeaderControl()
        {
            this.InitializeComponent();
        }

        public async Task InitAsync(BluetoothLEDevice ble)
        {
            
            await Task.Delay(0); //NOTE: Remove...
        }
        public async Task AddServiceAsync(BluetoothLEDevice ble, GattDeviceService service)
        {
            if (service.Uuid.ToString() == "")
            {

            }
            await Task.Delay(0); //NOTE: Remove...
        }
    }
}
