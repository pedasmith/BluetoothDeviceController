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
        StringBuilder sb = new();
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
            // Languanges like French will use a comma as the decimal separator.
            string strval = value.ToString("F2", CultureInfo.InvariantCulture);
            CellSet(strval); // on the off change the the float needs to be escaped...
        }

        public void CellSet(byte[] value)
        {
            StringBuilder sb = new();
            foreach (byte b in value)
            {
                sb.Append($"{b:X2} ");
            }
            string strval = sb.ToString().TrimEnd();
            CellSet(strval); // on the off change the the float needs to be escaped...
        }

        public string Export(string _) // description is not used in CSV exports
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
