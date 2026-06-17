using BluetoothConversions;
using BluetoothProtocols;
using BluetoothWinUI3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Exporters
{
    public class ExportDeviceData
    {
        public ExportDeviceData()
        {

        }

        public static string ExportData(IDeviceControlBasic control, IExportData exporter)
        {
            string retval = "";
            var data = control.GetData();
            if (data == null || data.Count == 0)
            {
                //Log("No data to export.");
                return retval;
            }
            data[0].ExportHeaders(exporter);
            foreach (var row in data)
            {
                row.ExportRow(exporter);
            }
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            retval = exporter.Export($"Data from TODO: at {now}");
            return retval;

        }
    }
}
