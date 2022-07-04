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
            var retval = new TemplateSnippet();
            retval.Macros.Add("CLASSNAME", "TI1350");
            retval.Macros.Add("DESCRIPTION", "The TI 1350 and 2650 are the latest in the TI range of Sensor...");
            retval.Macros.Add("CURRTIME", DateTime.Now.ToString("yyyy-MM-dd::hh:mm"));
            retval.Macros.Add("CLASSMODIFIERS", "partial");

            //Services
            var services = new TemplateSnippet();
            retval.AddChild("Services", services);

            var service = new TemplateSnippet();
            service.Macros.Add("Name", "Common Configuration");
            service.Macros.Add("UUID", "00001800-0000-1000-8000-00805f9b34fb");
            services.AddChildViaMacro(service); // have to wait until the UUID macro is added

            var chs = new TemplateSnippet();
            service.AddChild("Characteristics", chs);

            var ch = new TemplateSnippet();
            ch.Macros.Add("UUID", "00002a00-0000-1000-8000-00805f9b34fb");
            ch.Macros.Add("Name", "Device Name");
            ch.Macros.Add("Type", "STRING|ASCII|Device_Name");
            chs.AddChildViaMacro(ch); // uses the UUID to add correctly

            ch = new TemplateSnippet();
            ch.Macros.Add("UUID", "00002a01-0000-1000-8000-00805f9b34fb");
            ch.Macros.Add("Name", "Appearance");
            ch.Macros.Add("Type", "U16|Speciality^Appearance|Appearance");
            chs.AddChildViaMacro(ch); // uses the UUID to add correctly

            // Second Service with one characteristic
            service = new TemplateSnippet();
            service.Macros.Add("Name", "Device Info");
            service.Macros.Add("UUID", "0000180a-0000-1000-8000-00805f9b34fb");
            services.AddChildViaMacro(service); // have to wait until the UUID macro is added

            chs = new TemplateSnippet();
            service.AddChild("Characteristics", chs);

            ch = new TemplateSnippet();
            ch.Macros.Add("UUID", "00002a23-0000-1000-8000-00805f9b34fb");
            ch.Macros.Add("Name", "System ID");
            ch.Macros.Add("Type", "STRING|ASCII");
            chs.AddChildViaMacro(ch); // uses the UUID to add correctly

            // The LINKS
            var links = new TemplateSnippet();
            links.AddMacroNumber("https://www.ti.com/product/CC1350");
            links.AddMacroNumber("https://www.ti.com/tool/CC1350STK");
            retval.AddChild("LINKS", links);
            return retval;
        }
    }
}
