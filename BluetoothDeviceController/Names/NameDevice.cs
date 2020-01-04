using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Names
{
    public class NameDevice
    {
        public bool HasReadDevice_Name()
        {
            foreach (var service in Services)
            {
                if (service.Name == "Common Configuration")
                {
                    foreach (var characteristic in service.Characteristics)
                    {
                        if (characteristic.Name == "Device Name")
                        {
                            if (characteristic.IsRead)
                            {
                                return true;
                            }
                            return false; // it's the right characteristic, but isn't actually readable.
                        }
                    }
                    return false; // it's the right service but there isn't a device name
                }
            }
            return false; // nothing matches; return false.
        }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public string GetClassName() { return String.IsNullOrEmpty(ClassName) ? (Name ?? "") : ClassName; }
        public string ClassModifiers { get; set; }
        public string Description { get; set; }
        public string DefaultPin { get; set; }
        public List<string> Aliases { get; set; } = new List<string>(); // Must not be null
        public IList<NameService> Services { get; } = new List<NameService>();
        public IList<String> Links { get; } = new List<String>();

        public NameService GetService (string uuid)
        {
            foreach (var service in Services)
            {
                if (string.Compare(service.UUID, uuid, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    return service;
                }
            }
            return null;
        }
        public string Details { get; set; } = "";
        public override string ToString()
        {
            return $"{Name ?? ""}:[{Services.Count}]";
        }

        // Used by NewtonSoft: https://www.newtonsoft.com/json/help/html/ConditionalProperties.htm
        public bool ShouldSerializeName() { return !string.IsNullOrWhiteSpace(Name); }
        public bool ShouldSerializeClassName() { return !string.IsNullOrWhiteSpace(ClassName); }
        public bool ShouldSerializeClassModifiers() { return !string.IsNullOrWhiteSpace(ClassModifiers); }
        public bool ShouldSerializeDescription() { return !string.IsNullOrWhiteSpace(Description); }
        public bool ShouldSerializeAliases() { return Aliases != null && Aliases.Count > 0; }
        public bool ShouldSerializeServices() { return Services != null && Services.Count > 0; ; }
        public bool ShouldSerializeDetails() { return !string.IsNullOrWhiteSpace(Details); }

    }
}
