using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothDefinitionLanguage
{
    class DeviceCharacteristic
    {
        public string UserDescription { get; internal set; }
        public GattCharacteristicProperties Properties { get; internal set; }
        public string PropertiesAsString {
            get
            {
                var str = "";
                if (Properties.HasFlag(GattCharacteristicProperties.Broadcast)) str += "Broadcast ";
                if (Properties.HasFlag(GattCharacteristicProperties.Read)) str += "Read ";
                if (Properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse)) str += "WriteWithoutResponse ";
                if (Properties.HasFlag(GattCharacteristicProperties.Write)) str += "Write ";
                if (Properties.HasFlag(GattCharacteristicProperties.Notify)) str += "Notify ";
                if (Properties.HasFlag(GattCharacteristicProperties.Indicate)) str += "Indicate ";
                if (Properties.HasFlag(GattCharacteristicProperties.AuthenticatedSignedWrites)) str += "AuthenticatedSignedWrites ";
                if (Properties.HasFlag(GattCharacteristicProperties.ExtendedProperties)) str += "ExtendedProperties ";
                if (Properties.HasFlag(GattCharacteristicProperties.ReliableWrites)) str += "ReliableWrites ";
                if (Properties.HasFlag(GattCharacteristicProperties.WritableAuxiliaries)) str += "WritableAuxiliaries ";
                return str;
            }
        }
        public bool CanRead {  get { return Properties.HasFlag (GattCharacteristicProperties.Read); } }
        public void Init (GattCharacteristic characteristic)
        {
            Properties = characteristic.CharacteristicProperties;
            UserDescription = characteristic.UserDescription;
        }
        public static DeviceCharacteristic Create (GattCharacteristic characteristic)
        {
            var dc = new DeviceCharacteristic();
            dc.Init(characteristic);
            return dc;
        }
    }
    class DeviceService
    {
        public ObservableCollection<GattCharacteristic> GattCharacteristics { get; } = new ObservableCollection<GattCharacteristic>();

        //public IAsyncOperation<GattCharacteristicsResult> GetCharacteristicsAsync(BluetoothCacheMode cacheMode);
        public async Task Init(GattDeviceService service)
        {
            var characteristics = await service.GetCharacteristicsAsync(); //NOTE: cache mode?
            foreach (var characteristic in characteristics.Characteristics)
            {

            }

        }
    }
    class Device
    {
        public string DeviceName { get; internal set; } = null;
        public string EditedName { get; internal set; } = null;
        public string Name { get { return EditedName ?? DeviceName; } }

        public ObservableCollection<GattDeviceService> GattServices { get; } = new ObservableCollection<GattDeviceService>();

        public async Task Init (Windows.Devices.Bluetooth.BluetoothLEDevice ble)
        {
            DeviceName = ble.Name;
            var services = await ble.GetGattServicesAsync();
            foreach (var service in services.Services)
            {

            }
        }
    }
}
