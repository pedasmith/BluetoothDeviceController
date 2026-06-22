using BluetoothProtocols;
using BluetoothWinUI3;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Exporters;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace Exporters
{
    internal class SmartExporter
    {
        public IExportData Exporter { get; set; } = new ExportCsv();
        /// <summary>
        /// List of devices that we will export from. All devices in this list have already been
        /// checked; device.Control is an IDeviceControlBasic and the the GetUXCapabilities()
        /// has the CanGetData flag turned on.
        /// </summary>
        private List<KnownDevice> Devices=new List<KnownDevice>();
        /// <summary>
        /// Technically, a device that supports CanGetData() might not actually have any
        /// data yet (most commonly as a race condition between adding the device to this
        /// exporter and the device actually having gotten a data notification. There can  
        /// easily be a gap of several seconds.
        /// </summary>
        private List<KnownDevice> DevicesToSkip = new List<KnownDevice>();
        public void AddDevice(KnownDevice device)
        {
            var basic = device.Control as IDeviceControlBasic;
            if (basic == null)
            {
                // Should never happen. All controls that handle a real BT device
                // should be IDeviceControlBasic.
                Log($"Error: SmartExporter: device {device} is not an IDeviceControlBasic");
                return;
            }
            if (!basic.GetUXCapabilities().HasFlag(IDeviceControlBasic.UXCapabilities.CanGetData))
            {
                Log($"Error: SmartExporter: device {device} does not export data");
                return;
            }
            Devices.Add(device);
            BuildHeaders();
        }

        private void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }

        /// <summary>
        /// build the headers for this set of devices. Will also reset the DevicesToSkip,
        /// adding in all the devices that don't have data yet (no data means we can't get
        /// the headers)
        /// </summary>
        private void BuildHeaders()
        {
            DevicesToSkip.Clear();

            Exporter.HeadersStart();
            foreach (var device in Devices)
            {
                var basic = device.Control as IDeviceControlBasic;
                var data = basic.GetDataMostRecent();
                if (data == null)
                {
                    DevicesToSkip.Add(device);
                }
                else
                {
                    var headers = data.ExportGetHeaders(Exporter);
                    // SmartExport always add these three fields
                    Exporter.HeadersAppend(["Date", "Time", "Name"]);
                    Exporter.HeadersAppend(headers);
                }
            }
            Exporter.HeadersEnd();
        }

        /// <summary>
        /// Calls the exporter's Export method and returns it. 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public string Export(string description)
        {
            var retval = Exporter.Export(description);
            return retval;
        }

        public void ExportRowMostRecentData()
        {
            if (DevicesToSkip.Count > 0)
            {
                BuildHeaders(); // after this, DevicesToSkip might still have elements; that's OK
            }
            if (DevicesToSkip.Count == Devices.Count)
            {
                return;
            }
            Exporter.RowStart();
            foreach (var device in Devices)
            {
                if (DevicesToSkip.Contains(device))
                {
                    continue; // skip this device
                }
                var basic = device.Control as IDeviceControlBasic;
                var row = basic.GetDataMostRecent(); // row is guaranteed to exist per check in BuildHeaders
                if (row == null) continue;
                // These have to match the header we added earier
                Exporter.CellSet(row.TimestampMostRecentDT.ToString("yyyy-MM-dd"));
                Exporter.CellSet(row.TimestampMostRecentDT.ToString("HH:mm:ss"));
                Exporter.CellSet(row.Name);
                row.ExportRow(Exporter);
            }
            Exporter.RowEnd();
        }
    }
}  
