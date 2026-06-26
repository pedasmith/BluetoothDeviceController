using BluetoothWinUI3;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using WinUI.TableView;

namespace UtilitiesWinUI3;

internal static class UtilitiesWinUI3
{
    /// <summary>
    /// Used to, e.g., find all lines with the right Tag and recolor them
    /// </summary>
    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj == null) yield return (T)Enumerable.Empty<T>();
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
            if (ithChild == null) continue;
            if (ithChild is T t) yield return t;
            foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
        }
    }

    public delegate void Log(string str);
    public static async Task ExportGraphAsPngAsync(Control uiOxyPlot, Brush rootPanel_Background, Log log)
    {
        try
        {
            var exporter = new Exporters.ExportControlAsPng();
            var outputStream = await exporter.ExportAsync(uiOxyPlot, rootPanel_Background);
            var dataPackage = new DataPackage()
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            outputStream.Seek(0);
            var streamref = RandomAccessStreamReference.CreateFromStream(outputStream);
            dataPackage.SetBitmap(streamref);
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }
        catch (Exception ex)
        {
            log($"Error: unable to make PNG data for the clipboard; {ex.Message}");
        }
    }

    public static uint GetGraphColor(string name, PlotModel oxyPlotModel)
    {
        foreach (var series in oxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                if (lineSeries.DataFieldY == name)
                {
                    return UtilitiesOxyColor.OxyColorToUint(lineSeries.Color);
                }
            }
        }
        return UtilitiesOxyColor.OxyColorToUint(OxyColors.Undefined);
    }

    /// <summary>
    /// Updates the OxyPlot line with a given name (e.g., "Temperature"). Is called from MainWindow when the
    /// user picks a new color.
    /// </summary>
    /// <param name="lineName"></param>
    /// <param name="color"></param>
    public static void UpdateGraphColor(PlotModel OxyPlotModel, Border rootPanel, string lineName, uint color)
    {
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lseries)
            {
                if (lseries.DataFieldY == lineName)
                {
                    lseries.Color = OxyColor.FromUInt32((uint)color);
                    OxyPlotModel.InvalidatePlot(false); //DOC: false is just the axes, true is everything.
                    break;
                }
            }
        }

        // Set the line key color
        foreach (Line line in UtilitiesWinUI3.FindVisualChildren<Line>(rootPanel))
        {
            if ((line.Tag as string) == lineName + "Color") // e.g., Tag="TemperatureColor"
            {
                byte a = 0xFF;
                byte r = (byte)((color >> 16) & 0xFF);
                byte g = (byte)((color >> 8) & 0xFF);
                byte b = (byte)((color >> 0) & 0xFF);
                line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
        }
        SetLineKeyColor(rootPanel, lineName, color);

    }

    /// <summary>
    /// The "line" here are the key lines that are generally placed below the data. For example, there's a block 
    /// with the most recent Temperature data, plus a little title, plus a line that gets colored with the same
    /// color as the graph line.
    /// </summary>
    /// <param name="lineName"></param>
    /// <param name="color"></param>
    public static void SetLineKeyColor(Border rootPanel, string lineName, uint color)
    {
        foreach (Line line in UtilitiesWinUI3.FindVisualChildren<Line>(rootPanel))
        {
            if ((line.Tag as string) == lineName + "Color") // e.g., Tag="TemperatureColor"
            {
                byte a = 0xFF;
                byte r = (byte)((color >> 16) & 0xFF);
                byte g = (byte)((color >> 8) & 0xFF);
                byte b = (byte)((color >> 0) & 0xFF);
                line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
        }
    }

    /// <summary>
    /// Sets the line colors for the keys based on the OxyPlotModel values.
    /// </summary>
    public static void InitializeKeyLineColorsFromDefaultOxyPlot(PlotModel OxyPlotModel, Border rootPanel)
    {
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                var name = lineSeries.DataFieldY;
                if (!String.IsNullOrEmpty(name))
                {
                    if (lineSeries.Color != OxyColors.Automatic
                        && lineSeries.Color != OxyColors.Undefined)
                    {
                        var color = UtilitiesOxyColor.OxyColorToUint(lineSeries.Color);
                        SetLineKeyColor(rootPanel, name, color);
                    }
                }
            }
        }
    }

    public static void UpdateUXWindowSize(MainWindow.WindowSize windowSize, Windows.Foundation.Size largeActualSize, Border rootPanel, PlotModel OxyPlotModel, PlotView uiOxyPlot)
    {
        switch (windowSize)
        {
            default:
            case MainWindow.WindowSize.Normal:
                rootPanel.Width = 380;
                rootPanel.Height = 380;
                OxyPlotModel.SetAxesVisibility(uiOxyPlot, false);
                break;
            case MainWindow.WindowSize.Large:
                rootPanel.Width = largeActualSize.Width;
                rootPanel.Height = largeActualSize.Height;
                OxyPlotModel.SetAxesVisibility(uiOxyPlot, true);
                break;
        }
    }

    public static void TableView_AutoGeneratingColumn_FixupDateTime(object sender, TableViewAutoGeneratingColumnEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "TimestampMostRecent":
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
                break;
            case "TimestampMostRecentDT":
                e.Cancel = true; // Don't generate a column for this property because it's not user friendly. 
                break;
        }
    }

    public static void SetDataGridVisibility(PlotView uiOxyPlot, Grid uiDataGridPanel, IDeviceControlBasic.Visibility visibility)
    {
        switch (visibility)
        {
            case IDeviceControlBasic.Visibility.Collapsed:
                uiOxyPlot.Visibility = Visibility.Visible;
                uiDataGridPanel.Visibility = Visibility.Collapsed;
                break;
            case IDeviceControlBasic.Visibility.Visible:
            default:
                uiOxyPlot.Visibility = Visibility.Collapsed;
                uiDataGridPanel.Visibility = Visibility.Visible;
                break;
        }
    }
}
