using System;
using System.Collections.Generic;
using System.Text;
using WinUI.TableView;

namespace BluetoothWinUI3
{
    internal class TableViewColumnCustomization
    {
        public List<string> TableColumns = new();
        public List<string> TableColumnsToExclude = new();
        public bool FixupTimestamps = true;
        public void TableView_AutoGeneratingColumn_UseCustomization(object sender, TableViewAutoGeneratingColumnEventArgs e)
        {
            if (FixupTimestamps && e.PropertyName == "TimestampMostRecent")
            {
                var col = e.Column as TableViewDateColumn;
                if (col == null)
                {
                    // Log($"Error: TimestampMostRecent is not a TableViewDateColumn.");
                    return;
                }
                col.IsReadOnly = true;
                // DateFormat is from the DateTimeFormatter which is completely different from the 
                // normal format from DateTimeOffset.ToString().
                // https://learn.microsoft.com/en-us/uwp/api/windows.globalization.datetimeformatting.datetimeformatter?view=winrt-28000
                col.DateFormat = "{hour.integer}:{minute.integer(2)}:{second.integer(2)}";
                col.Header = "Time";
            }
            else if (FixupTimestamps && e.PropertyName == "TimestampMostRecentDT")
            {
                e.Cancel = true; // Don't generate a column for this property because it's not user friendly. 
            }
            else if (TableColumns.Count > 0 && !TableColumns.Contains(e.PropertyName))
            {
                // The sensor has a bunch of fields (e.g., "TagType") which should not be part of the data grid.
                e.Cancel = true;
            }
            else if (TableColumnsToExclude.Count > 0 && TableColumnsToExclude.Contains(e.PropertyName))
            {
                e.Cancel = true;
            }
        }
    }
}
