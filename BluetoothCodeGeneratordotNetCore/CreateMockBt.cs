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
            var services = new TemplateSnippet();
            retval.AddChild("Services", services);

            var service = new TemplateSnippet();
            service.Macros.Add("Name", "Common Configuration");
            service.Macros.Add("UUID", "00001800-0000-1000-8000-00805f9b34fb");
            services.AddChildViaMacro(service); // have to wait until the UUID macro is added

            var ch = new TemplateSnippet();
            ch.Macros.Add("UUID", "00002a00-0000-1000-8000-00805f9b34fb");
            ch.Macros.Add("Name", "Device Name");
            ch.Macros.Add("Type", "STRING|ASCII|Device_Name");
            service.AddChildViaMacro(ch); // uses the UUID to add correctly


            var links = new TemplateSnippet();
            links.AddMacroNumber("https://www.ti.com/product/CC1350");
            links.AddMacroNumber("https://www.ti.com/tool/CC1350STK");
            return retval;
        }
    }
}
