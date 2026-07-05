using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TemplateExpander;

namespace BluetoothCodeGenerator
{
    internal class CreateMockBt
    {
        public static TemplateSnippet Create()
        {
            var retval = new TemplateSnippet("TI1350");
            retval.MacrosAdd("CLASSNAME", "TI1350");
            retval.MacrosAdd("DESCRIPTION", "The TI 1350 and 2650 are the latest in the TI range of Sensor...");
            retval.MacrosAdd("CURRTIME", DateTime.Now.ToString("yyyy-MM-dd::hh:mm"));
            retval.MacrosAdd("CLASSMODIFIERS", "partial");

            //Services
            var services = new TemplateSnippet("Services");
            retval.AddChild("Services", services);

            var service = new TemplateSnippet("Common Configuration");
            service.MacrosAdd("Name", "Common Configuration");
            service.MacrosAdd("UUID", "00001800-0000-1000-8000-00805f9b34fb");
            services.AddChildViaMacro(service); // have to wait until the UUID macro is added

            var chs = new TemplateSnippet("Characteristics");
            service.AddChild("Characteristics", chs);

            var ch = new TemplateSnippet("Device Name");
            ch.MacrosAdd("UUID", "00002a00-0000-1000-8000-00805f9b34fb");
            ch.MacrosAdd("Name", "Device Name");
            ch.MacrosAdd("Type", "STRING|ASCII|Device_Name");
            chs.AddChildViaMacro(ch); // uses the UUID to add correctly

            ch = new TemplateSnippet("Appearance");
            ch.MacrosAdd("UUID", "00002a01-0000-1000-8000-00805f9b34fb");
            ch.MacrosAdd("Name", "Appearance");
            ch.MacrosAdd("Type", "U16|Speciality^Appearance|Appearance");
            chs.AddChildViaMacro(ch); // uses the UUID to add correctly

            // Second Service with one characteristic
            service = new TemplateSnippet("Device Info");
            service.MacrosAdd("Name", "Device Info");
            service.MacrosAdd("UUID", "0000180a-0000-1000-8000-00805f9b34fb");
            services.AddChildViaMacro(service); // have to wait until the UUID macro is added

            chs = new TemplateSnippet("Characteristics");
            service.AddChild("Characteristics", chs);

            ch = new TemplateSnippet("System ID");
            ch.MacrosAdd("UUID", "00002a23-0000-1000-8000-00805f9b34fb");
            ch.MacrosAdd("Name", "System ID");
            ch.MacrosAdd("Type", "STRING|ASCII");
            chs.AddChildViaMacro(ch); // uses the UUID to add correctly

            // The LINKS
            var links = new TemplateSnippet("LINKS");
            links.AddMacroNumber("https://www.ti.com/product/CC1350");
            links.AddMacroNumber("https://www.ti.com/tool/CC1350STK");
            retval.AddChild("LINKS", links);
            return retval;
        }
    }
}
