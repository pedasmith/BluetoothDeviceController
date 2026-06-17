using BluetoothConversions;
using BluetoothProtocols;
using BluetoothWinUI3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            var headers = data[0].ExportGetHeaders(exporter).ToList();
            headers.Insert(0, "Date");
            headers.Insert(1, "Time");
            headers.Insert(2, "Name");
            exporter.HeadersSet(headers.ToArray());
            foreach (var row in data)
            {
                exporter.RowStart();
                exporter.CellSet(row.TimestampMostRecentDT.ToString("yyyy-MM-dd"));
                exporter.CellSet(row.TimestampMostRecentDT.ToString("HH:mm:ss"));
                exporter.CellSet(row.Name);
                row.ExportRow(exporter);
                exporter.RowEnd();
            }
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            retval = exporter.Export($"Data from TODO: at {now}");
            return retval;

        }
    }
}
