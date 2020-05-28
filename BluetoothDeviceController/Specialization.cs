using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace BluetoothDeviceController
{
    public class Specialization
    {
        public enum ParentScrollType {  ParentShouldScroll, ChildHandlesScrolling};
        public Specialization (Type page, string[] names, string icon, string shortDescription, string description, ParentScrollType parentShouldScroll=ParentScrollType.ParentShouldScroll)
        {
            Page = page;
            Icon = icon;
            ShortDescription = shortDescription;
            Description = description;

            foreach (var name in names)
            {
                Names.Add(name);
            }
            ParentShouldScroll = parentShouldScroll;
        }
        public override string ToString()
        {
            return $"{ShortDescription} -- {Page.ToString()} -- {Names[0]}";
        }
        private IList<string> Names = new List<string>();
        public Type Page { get; internal set; }
        public string Icon { get; internal set; }
        public string Description { get; internal set; }
        public string ShortDescription { get; internal set; }
        public ParentScrollType ParentShouldScroll { get; internal set; }
        public bool Match (string deviceName)
        {
            foreach (var name in Names)
            {
                if (deviceName.StartsWith(name)) return true;
            }
            return false;
        }

        public static Specialization Get (IList<Specialization> list, string deviceName)
        {
            foreach (var item in list)
            {
                if (item.Match(deviceName)) return item;
            }
            return null;
        }
    }
}
