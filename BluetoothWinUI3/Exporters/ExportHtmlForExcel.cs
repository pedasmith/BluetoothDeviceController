using BluetoothProtocols;
using System.Text;
using System.Threading;

namespace Exporters
{
    /// <summary>
    /// Exports a table in HTML format. This can be directly pasted into Excel (unlike CSV clipboard formats)
    /// </summary>
    public class ExportHtmlForExcel : IExportData
    {
        StringBuilder currHeaders = new StringBuilder();
        StringBuilder currRow = new StringBuilder();
        StringBuilder allRows = new StringBuilder();

        public ExportHtmlForExcel() { }

        private string Escape(string value)
        {
            // From e.g.,https://stackoverflow.com/questions/7381974/which-characters-need-to-be-escaped-in-html 
            var retval = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            return retval;
        }

        public void CellSet(string value)
        {
            currRow.Append($"<td>{Escape(value)}</td>");
        }

        public void CellSet(double value)
        {
            currRow.Append($"<td>{Escape(value.ToString("F2"))}</td>");
        }

        public void CellSet(byte[] value)
        {
            StringBuilder cellsb = new();
            foreach (byte b in value ?? new byte[] { })
            {
                cellsb.Append($"{b:X2} ");
            }
            string strval = cellsb.ToString().TrimEnd();
            currRow.Append($"<td>{Escape(strval)}</td>");
        }

        public string Export(string description)
        {
            var retval = MakeInitial(description)
                + currHeaders.ToString()
                + allRows.ToString()
                + MakeFinal();
            return retval;
        }

        private void HeadersSet(string[] headers)
        {
            currHeaders = new StringBuilder();
            currHeaders.Append("<tr>");
            foreach (var header in headers)
            {
                currHeaders.Append($"<th><b>{Escape(header)}</b></th>");
            }
        }

        public void HeadersStart()
        {
            currHeaders = new StringBuilder();
            currHeaders.Append("<tr>");
        }
        public void HeadersAppend(string[] headers)
        {
            foreach (var header in headers)
            {
                currHeaders.Append($"<th><b>{Escape(header)}</b></th>");
            }
        }
        public void HeadersEnd()
        {
            currHeaders.Append("</tr>\n");
        }

        public string MakeFinal()
        {
            // The last row will already have the \n
            return "</table></body></html>";
        }
        public string MakeInitial (string description)
        {
            return "<!DOCTYPE html>\n<html><head>\n<title>" + Escape(description) + "</title>\n</head>\n<body><table>\n";
        }

        public void RowEnd()
        {
            currRow.Append("</tr>\n");
            allRows.Append(currRow);
        }

        public void RowStart()
        {
            currRow = new StringBuilder();
            currRow.Append("<tr>");
        }
    }
}
