using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols
{
    public interface IExportData
    {
        public void HeadersSet(string[] headers);
        public void RowStart();
        public void RowEnd();
        public void CellSet(string value);
        public void CellSet(double value);
    }
}
