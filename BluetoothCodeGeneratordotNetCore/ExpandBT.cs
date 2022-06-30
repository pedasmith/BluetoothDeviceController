using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothCodeGenerator
{
    class ExpandBt
    {
        private NameDevice GetDefaultBle()
        {
            string path = "";
            try
            {
                // Read in the Default device. 
                //StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                //string fname = $"{dname}DefaultBluetooth.json";
                //var f = await InstallationFolder.GetFileAsync(fname);
                var fcontents = ""; // File.ReadAllText(f.Path);
                path = ""; //  f.Path;
                var defaultlist = Newtonsoft.Json.JsonConvert.DeserializeObject<NameAllBleDevices>(fcontents);
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
    }
}
