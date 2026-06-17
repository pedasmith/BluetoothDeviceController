using BluetoothProtocols;
using System.Text;

namespace Exporters
{
    /// <summary>
    /// Exports a table in HTML format. This can be directly pasted into Excel (unlike CSV clipboard formats)
    /// </summary>
    public class ExportHtmlForExcel : IExportData
    {
        StringBuilder sb = new StringBuilder();
        public ExportHtmlForExcel() { }

        private string Escape(string value)
        {
            // From e.g.,https://stackoverflow.com/questions/7381974/which-characters-need-to-be-escaped-in-html 
            var retval = value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            return retval;
        }

        public void CellSet(string value)
        {
            sb.Append($"<td>{Escape(value)}</td>");
        }

        public void CellSet(double value)
        {
            sb.Append($"<td>{Escape(value.ToString("F2"))}</td>");
        }

        public void CellSet(byte[] value)
        {
            StringBuilder cellsb = new();
            foreach (byte b in value ?? new byte[] { })
            {
                cellsb.Append($"{b:X2} ");
            }
            string strval = cellsb.ToString().TrimEnd();
            sb.Append($"<td>{Escape(strval)}</td>");
        }

        public string Export(string description)
        {
            return "<!DOCTYPE html>\n<html><head>\n<title>" + Escape(description) + "</title>\n</head>\n<body><table>\n" + sb.ToString() + "\n</table></body></html>";
        }

        public void HeadersSet(string[] headers)
        {
            sb.Append("<tr>");
            foreach (var header in headers)
            {
                sb.Append($"<th><b>{Escape(header)}</b></th>");
            }
            sb.Append("</tr>\n");
        }

        public void RowEnd()
        {
            sb.Append("</tr>\n");
        }

        public void RowStart()
        {
            sb.Append("<tr>");
        }
    }
}
