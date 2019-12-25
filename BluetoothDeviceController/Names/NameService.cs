using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothDeviceController.Names
{
    public class NameService
    {
        public NameService()
        {

        }
        public NameService (GattDeviceService service, NameService defaultService, int count = -1)
        {
            UUID = service.Uuid.ToString("D"); // documented at https://docs.microsoft.com/en-us/dotnet/api/system.guid.tostring?view=netframework-4.8#System_Guid_ToString_System_String_
            if (defaultService != null)
            {
                Name = defaultService.Name;
                Suppress = defaultService.Suppress;
                Description = defaultService.Description;
                Priority = defaultService.Priority;
            }
            else
            {
                Name = count >= 0 ? $"Unknown{count}" : "Unknown";
            }
        }
        public string UUID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// When true, the UI for the service will be entirely suppressed.
        /// </summary>
        public bool Suppress { get; set; } = false;
        public string Description { get; set; } = null;

        /// <summary>
        /// Display priority. Default is 0; max should be 10. Services with higher priorities are displayed first.
        /// </summary>
        public int Priority { get; set; } = 0;
        public NameCharacteristic GetChacteristic (string uuid)
        {
            foreach (var characteristic in Characteristics)
            {
                if (string.Compare(characteristic.UUID, uuid, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return characteristic;
                }
            }
            return null;
        }

        public IList<NameCharacteristic> Characteristics { get; } = new List<NameCharacteristic>();

        public override string ToString()
        {
            return $"{Name}:{UUID}";
        }
    }
}
