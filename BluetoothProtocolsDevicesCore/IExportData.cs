using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols
{
    /// <summary>
    /// Interface implemented by e.g. the ExportHtmlForExcel and ExportCsv classes. When the user wants
    /// to export data, MainWindows will make a specific IExportData (like the ExportHtmlForExcel or ExportCsv) 
    /// and then loop through the HistoricalData, calling ExportHeaders and ExportRow for each row. 
    /// </summary>
    public interface IExportData
    {
        public void HeadersSet(string[] headers);
        public void RowStart();
        public void RowEnd();
        public void CellSet(string value);
        public void CellSet(double value);

        /// <summary>
        /// Get the final data
        /// </summary>
        /// <returns></returns>
        public string Export(string description); 
    }

    /// <summary>
    /// Interface implemented by, e.g., Nordic_Thingy's different data classes.
    /// </summary>
    public interface IExportDataSource
    {
        void ExportHeaders(IExportData exporter);
        void ExportRow(IExportData exporter);
    }
}
