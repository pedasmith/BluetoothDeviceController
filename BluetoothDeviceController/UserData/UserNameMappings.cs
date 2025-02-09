using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BluetoothDeviceController.UserData
{
    /// <summary>
    /// Static class to map ids and names. The ID is generally a DeviceInformation.Id
    /// </summary>
    public class UserNameMappings
    {
        private static IList<NameMapping> AllMappings = null;  

        /// <summary>
        /// Primary function; gets the name mapping associated with a given Id. Will return NULL if the ID isn't found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static NameMapping Get (string id)
        {
            if (AllMappings == null) return null;
            foreach (var mapping in AllMappings)
            {
                if (mapping.Id == id)
                {
                    return mapping;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds or updates the given id+name to the list of name mappings AND saves
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task AddOrUpdateAsync(string id, string name)
        {
            if (AllMappings == null)
            {
                await InitAsync();
                if (AllMappings == null) return;// Should never happen
            }
            var original = Get(id);
            if (original == null)
            {
                original = new NameMapping() { Id = id, Name = name };
                AllMappings.Add(original);
                await SaveAsync();
            }
            else
            {
                if (original.Name == name)
                {
                    return;
                }
                else
                {
                    original.Name = name;
                    await SaveAsync();
                }
            }
        }

        private const string fname = @"NameMapping.json";

        public static async Task InitAsync()
        {
            if (AllMappings != null) return;

            try
            {
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var f = await folder.TryGetItemAsync(fname);
                //var f = await folder.GetFileAsync(fname);
                if (f != null)
                {
                    var fcontents = File.ReadAllText(f.Path);
                    AllMappings = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<NameMapping>>(fcontents);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: User NameMappings Read: {e.Message}");
            }
            if (AllMappings == null)
            {
                AllMappings = new List<NameMapping>(); // make some kind of array
            }
        }

        public static async Task SaveAsync()
        {
            if (AllMappings == null) return;

            try
            {
                StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                //var f = await folder.GetFileAsync(fname);

                var text = Newtonsoft.Json.JsonConvert.SerializeObject(AllMappings);
                var file = await folder.CreateFileAsync(fname, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, text);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: User NameMappings Write: {e.Message}");
            }
        }
    }
}
