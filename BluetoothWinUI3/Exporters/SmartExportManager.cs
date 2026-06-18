using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace Exporters
{
    /// <summary>
    /// Manager class for the SmartExport system. Is intended to be a singleton.
    /// 
    /// Not implemented yet:
    /// - user should be able to create a SmartExporter based on their needs (e.g., 
    ///     "name matches" or "device is" or "device has method". Maybe later if I have types
    ///     of supported devices, "supported device is sensor" or "supported device is exercise")
    /// </summary>
    internal class SmartExportManager
    {
        SmartExportRunner Runner = new SmartExportRunner();
        SmartExporter AllKnownDevices;

        public SmartExportManager()
        {
            AllKnownDevices = new SmartExporter();
            AllKnownDevices.Exporter = new ExportHtmlForExcel();
            Runner.Add(AllKnownDevices);
        }

        /// <summary>
        /// From IHandleBTAdvertisements and called from MainWindow AdvertisementWatcher_WatcherEvent
        /// when a new known device is created.
        /// </summary>
        /// <param name="data"></param>
        public void HandleNewKnownDevice(KnownDevice knownDevice)
        {
            // TODO: here!here pick which SmartExporters will handle this
            AllKnownDevices.AddDevice(knownDevice);
            if (!Runner.IsStarted)
            {
                Runner.Start();
            }
        }

        /// <summary>
        /// Fake routine for right now: just does the export for the AllDevices
        /// </summary>
        /// <returns></returns>
        public string Export()
        {
            var retval = AllKnownDevices.Exporter.Export("All device data export!");
            return retval;
        }

    }
}
