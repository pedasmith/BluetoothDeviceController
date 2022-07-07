using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateExpander;

namespace BluetoothCodeGenerator
{
    internal static class BtJsonToMacro
    {
        public static TemplateSnippet Convert(NameDevice bt)
        {
            TemplateSnippet retval = new TemplateSnippet();
            retval.Macros.Add("CLASSNAME", bt.ClassName);
            retval.Macros.Add("DESCRIPTION", bt.Description);
            retval.Macros.Add("CURRTIME", DateTime.Now.ToString("yyyy-MM-dd::hh:mm"));
            retval.Macros.Add("CLASSMODIFIERS", bt.ClassModifiers);

            //Services
            var services = new TemplateSnippet();
            retval.AddChild("Services", services);

            foreach (var btService in bt.Services)
            {
                var service = new TemplateSnippet();
                service.Macros.Add("Name", btService.Name);
                service.Macros.Add("UUID", btService.UUID);
                services.AddChildViaMacro(service); // have to wait until the UUID macro is added

                var chs = new TemplateSnippet();
                service.AddChild("Characteristics", chs);

                foreach (var btCharacteristic in btService.Characteristics)
                {
                    var ch = new TemplateSnippet();
                    ch.Macros.Add("UUID", btCharacteristic.UUID);
                    ch.Macros.Add("Name", btCharacteristic.Name);
                    ch.Macros.Add("Type", btCharacteristic.Type);
                    chs.AddChildViaMacro(ch); // uses the UUID to add correctly
                }
            }

            // The LINKS
            var links = new TemplateSnippet();
            retval.AddChild("LINKS", links);
            foreach (var link in bt.Links)
            {
                links.AddMacroNumber(link);
            }

            return retval;

        }
    }
}
