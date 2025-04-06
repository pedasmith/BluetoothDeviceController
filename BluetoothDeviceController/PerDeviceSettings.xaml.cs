using BluetoothDeviceController.Names;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController
{
    public sealed partial class PerDeviceSettings : UserControl
    {
        public PerDeviceSettings(DeviceInformation di, string oldName)
        {
            var id = di.Id;
            var mac = GuidGetCommon.NiceId(id, "");
            this.InitializeComponent();
            this.Loaded += (s, e) =>
                {
                    uiName.Text = oldName;
                    uiDeviceName.Text = di.Name;
                    uiDeviceMacAddress.Text = mac;
                    uiDeviceId.Text = di.Id;
                };
        }
        public string UserName {  get { return uiName.Text; } }
        public bool NameChanged { get; internal set; } = false;

        private void UiName_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            NameChanged = true;
        }
    }
}
