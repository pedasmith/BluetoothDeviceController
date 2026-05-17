using BluetoothProtocols;
using System.Text;

namespace Exporters
{
    /// <summary>
    /// Exports a table in CSV format.
    /// </summary>
    public class ExportCsv : IExportData
    {
        StringBuilder sb = new StringBuilder();
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

        bool isFirstCellInRow = true;
        public void CellSet(string value)
        {
            if (!isFirstCellInRow)
            {
                sb.Append(",");
            }
            isFirstCellInRow = false;
            sb.Append($"{Escape(value)}");
        }

        public void CellSet(double value)
        {
            if (!isFirstCellInRow)
            {
                sb.Append(",");
            }
            isFirstCellInRow = false;
            sb.Append($"{value.ToString("F2")}");
        }

        public string Export()
        {
            return sb.ToString();
        }

        public void HeadersSet(string[] headers)
        {
            bool isFirst = true;
            foreach (var header in headers)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }
                isFirst = false;
                sb.Append($"{Escape(header)}");
            }
            sb.Append("\n");
        }

        public void RowEnd()
        {
            sb.Append("\n");
        }

        public void RowStart()
        {
            isFirstCellInRow = true;
        }
    }
}
