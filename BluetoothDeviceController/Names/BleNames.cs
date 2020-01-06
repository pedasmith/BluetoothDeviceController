using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage;

namespace BluetoothDeviceController.Names
{
    public class BleNames
    {
        private static NameAllDevices AllDevices { get; set; } = null;
        public static NameAllDevices AllRawDevices { get; set; } = null;
        static NameDevice DefaultDevice = null;
        public static NameDevice GetDevice(string name, NameAllDevices allDevices = null)
        {
            if (allDevices == null) allDevices = AllDevices;
            if (allDevices == null) return null;

            // Find the matching characteristic ID
            foreach (var device in allDevices.AllDevices)
            {
                if (name.StartsWith(device.Name ?? ""))
                {
                    return device;
                }
                foreach (var alias in device.Aliases)
                {
                    if (name.StartsWith(alias))
                    {
                        return device; // e.g. the TI SensorTag 2.0 can also show up as the CC1350
                    }
                }
            }
            return DefaultDevice;
        }


        public static NameCharacteristic Get(NameDevice device, GattDeviceService service, GattCharacteristic characteristic)
        {
            if (device == null) return null;

            foreach (var s in device.Services)
            {
                if (string.Compare(s.UUID, service.Uuid.ToString("D"), StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    foreach (var c in s.Characteristics)
                    {
                        if (string.Compare(c.UUID, characteristic.Uuid.ToString("D"), StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            return c;
                        }
                    }
                }
            }
            return null;
        }

        // Read in data from the Assets\CharacteristicsData.json file. Note the mis-spelling Assets\Chacteristics!!
        public async Task InitAsync()
        {
            string path = "";
            AllDevices = new NameAllDevices();
            AllRawDevices = new NameAllDevices();
            try
            {
                string dname = @"Assets\ChacteristicsData\";

                // Read in the Default device. 
                DefaultDevice = await InitDefault(dname);

                // Read in the full set of devices
                StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var dir = await InstallationFolder.GetFolderAsync(dname);
                var files = await dir.GetFilesAsync();
                foreach (var file in files)
                {
                    path = file.Path;
                    InitSingleFile(AllDevices, file, DefaultDevice);
                    InitSingleFile(AllRawDevices, file, null); // read in a device without adding in default services
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: BLE NAMES: {e.Message} with path {path}");
            }
        }

        private async Task<NameDevice> InitDefault(string dname)
        {
            string path = "";
            try
            {
                // Read in the Default device. 
                StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string fname = $"{dname}DefaultBluetooth.json";
                var f = await InstallationFolder.GetFileAsync(fname);
                var fcontents = File.ReadAllText(f.Path);
                path = f.Path;
                var defaultlist = Newtonsoft.Json.JsonConvert.DeserializeObject<NameAllDevices>(fcontents);
                foreach (var item in defaultlist.AllDevices)
                {
                    if (item.Name == "##DEFAULT##")
                    {
                        return item;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: DEFAULT BLE NAMES: {e.Message} path {path}");
            }
            return null;
        }

        private void InitSingleFile(NameAllDevices allDevices, StorageFile file, NameDevice defaultDevice)
        {
            if (File.Exists(file.Path))
            {
                var contents = File.ReadAllText(file.Path);
                var newlist = Newtonsoft.Json.JsonConvert.DeserializeObject<NameAllDevices>(contents);
                foreach (var item in newlist.AllDevices)
                {
                    // Check to make sure that at least one of IsRead IsNotify IsIndicate IsWrite IsWriteWithoutResponse is used
                    // OK for the defaults to not have things specified.
                    if (item.Name != "##DEFAULT##")
                    {
                        foreach (var service in item.Services)
                        {
                            foreach (var characteristic in service.Characteristics)
                            {
                                if (!characteristic.IsIndicate && !characteristic.IsNotify
                                    && !characteristic.IsRead
                                    && !characteristic.IsWrite && !characteristic.IsWriteWithoutResponse)
                                {
                                    System.Diagnostics.Debug.WriteLine($"JSON ERROR: {file.Name} service {service.Name} characteristic {characteristic.Name} has no 'verb' like IsRead:true etc. ");
                                }
                            }
                        }
                    }
                    // Add in all of the values from the default
                    if (defaultDevice != null)
                    {
                        foreach (var nameService in DefaultDevice.Services)
                        {
                            item.Services.Add(nameService);
                        }
                    }

                    // And now either replace or update.
                    var index = allDevices.GetIndex(item.Name);
                    if (index < 0) allDevices.AllDevices.Add(item);
                    else allDevices.AllDevices[index] = item; // replace or add.
                }
            }
        }
    }
}
