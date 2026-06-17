using BluetoothProtocols;
using System.Globalization;
using System.Text;

namespace Exporters
{
    /// <summary>
    /// Exports a table in CSV format.
    /// </summary>
    public class ExportCsv : IExportData
    {
        StringBuilder currHeaders = new StringBuilder();
        StringBuilder currRow = new StringBuilder();
        StringBuilder allRows = new StringBuilder();
        public ExportCsv() { }

        /// <summary>
        /// // RFC 4180 has a complete description of the CSV format
        /// </summary>
        public static string Escape(string data)
        {
            string Retval = data;
            if (Retval == null) return "";
            // double-quote is escaped by doubling it.
            bool mustQuote = false;
            if (Retval.Contains('"'))
            {
                mustQuote = true;
                Retval = Retval.Replace("\"", "\"\""); // replace each individual double-quote with two double-quotes
            }
            if (Retval.Contains(',') || Retval.Contains('\r') || Retval.Contains('\n'))
            {
                mustQuote = true;
            }

            if (mustQuote)
            {
                Retval = "\"" + Retval + "\"";
            }
            return Retval;
        }

        public void CellSet(string value)
        {
            if (currRow.Length > 0)
            {
                currRow.Append(",");
            }
            currRow.Append($"{Escape(value)}");
        }

        public void CellSet(double value)
        {
            // Languanges like French will use a comma as the decimal separator.
            string strval = value.ToString("F2", CultureInfo.InvariantCulture);
            CellSet(strval); // on the off change the the float needs to be escaped...
        }

        public void CellSet(byte[] value)
        {
            StringBuilder cellsb = new();
            foreach (byte b in value ?? new byte[] { })
            {
                cellsb.Append($"{b:X2} ");
            }
            string strval = cellsb.ToString().TrimEnd();
            CellSet(strval); // on the off change the the float needs to be escaped...
        }

        public string Export(string description)
        {
            var retval = MakeInitial(description)
                + currHeaders.ToString()
                + allRows.ToString()
                + MakeFinal();
            return retval;
        }

        public void HeadersStart()
        {
            currHeaders = new StringBuilder();
        }
        public void HeadersAppend(string[] headers)
        {
            foreach (var header in headers)
            {
                if (currHeaders.Length > 0)
                {
                    currHeaders.Append(",");
                }
                currHeaders.Append($"{Escape(header)}");
            }
        }
        public void HeadersEnd()
        {
            currHeaders.Append("\n");
        }
        public string MakeFinal()
        {
            return "";
        }
        public string MakeInitial(string description)
        {
            return "";
        }

        public void RowEnd()
        {
            currRow.Append("\n");
            allRows.Append(currRow.ToString());
        }

        public void RowStart()
        {
            currRow = new StringBuilder();
        }
    }
}
