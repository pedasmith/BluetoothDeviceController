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
        /// <summary>
        /// Name of the device. This will be used to match actual devices and should not include e.g., the MAC address of the device
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// User-visible name of the device. Often not set, but for some devices (like the Lionel trains) where
        /// the BT name is LC-0-1-0494-B69B but the user knows it as a Lionel Train.
        /// </summary>
        public string UserName { get; set; }
        public string GetUserName() { return String.IsNullOrEmpty(UserName) ? Name : UserName; }

        public enum CompletionStatusEnum {  Usable, Unusable};
        public CompletionStatusEnum CompletionStatus { get; set; } = CompletionStatusEnum.Usable;
        public string CompletionStatusExplanation { get; set; } = "";
        /// <summary>
        /// Should be [maker]_[device]. Be consistant with other devices. This is used directly to make C# class names, so it must be a valid C# class name.
        /// </summary>
        public string ClassName { get; set; }
        public string GetClassName() { return String.IsNullOrEmpty(ClassName) ? (Name ?? "") : ClassName; }
        /// <summary>
        /// Additions to the class definitions; it generally blank or "partial". See the TI SensorTag 1350 for examples.
        /// </summary>
        public string ClassModifiers { get; set; }
        /// <summary>
        /// User-supplied description of the device
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// Documentation only string for how to use the device. Will be placed in the # Using section of the help file.
        /// </summary>
        public string UsingTheDevice { get; set; } = "";
        /// <summary>
        /// The default PIN for the device. Many devices don't require PIN values.
        /// </summary>
        /// 
        public string DefaultPin { get; set; } = "";
        /// <summary>
        /// Used in conjuction with the MD template files. The MD file will have a "SupressFile=":SuppressXAML:".
        /// If that SuppressFile contains the SuppressFile here, then the file will be suppressed.
        /// Note that a single JSON device file can list a bunch of different templates that get suppressed.
        /// </summary>
        public string SuppressFile { get; set; } = "";
        /// <summary>
        /// List of aliases for the device; used by the Triones bulb.
        /// </summary>
        public List<string> Aliases { get; set; } = new List<string>(); // Must not be null
        public IList<String> Links { get; } = new List<String>();
        public IList<NameService> Services { get; } = new List<NameService>();

        public NameService GetService (string uuid)
        {
            foreach (NameService service in Services)
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
