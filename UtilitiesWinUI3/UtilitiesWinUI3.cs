using BluetoothWinUI3;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI;

#if NET8_0_OR_GREATER
#nullable disable
#endif

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

    public static uint GetGraphColor(PlotModel oxyPlotModel, string axisTitle)
    {
        var propertyName = PlotModelAxisTitleToPropertyName(oxyPlotModel, axisTitle);
        foreach (var series in oxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                if (lineSeries.DataFieldY == propertyName)
                {
                    return UtilitiesOxyColor.OxyColorToUint(lineSeries.Color);
                }
            }
        }
        return UtilitiesOxyColor.OxyColorToUint(OxyColors.Undefined);
    }

    /// <summary>
    /// Updates the OxyPlot line with a given name (e.g., "Temperature" or "Heart Rate"). Is called from MainWindow when the
    /// user picks a new color.
    /// </summary>
    public static void UpdateGraphColor(PlotModel OxyPlotModel, Border rootPanel, string axisTitle, uint color)
    {
        var propertyName = UtilitiesWinUI3.PlotModelAxisTitleToPropertyName(OxyPlotModel, axisTitle);
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lseries)
            {
                if (lseries.DataFieldY == propertyName)
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
            if ((line.Tag as string) == propertyName + "Color") // e.g., Tag="TemperatureColor"
            {
                byte a = 0xFF;
                byte r = (byte)((color >> 16) & 0xFF);
                byte g = (byte)((color >> 8) & 0xFF);
                byte b = (byte)((color >> 0) & 0xFF);
                line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
        }
        SetLineKeyColor(rootPanel, propertyName, color);
    }

    /// <summary>
    /// The user knows a name like "Heart Rate". But internally, it's "HeartRate"
    /// </summary>
    public static string PlotModelAxisTitleToPropertyName(PlotModel OxyPlotModel, string lineName)
    {
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lseries)
            {
                if (lseries.Title == lineName)
                {
                    return lseries.DataFieldY;
                }
            }
        }
        return null;
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

    /// <summary>
    /// Given a uint color, make a Color struct with A, R, G, B fields. The uint has A as the high byte and B as the low byte.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ConvertIgnoreA(uint color)
    {
        Color retval = new Color()
        {
            A = 0xFF, // ignore the A value completely: (byte)((color >> 24) & 0xff),
            R = (byte)((color >> 16) & 0xff),
            G = (byte)((color >> 8) & 0xff),
            B = (byte)((color >> 0) & 0xff),
        };
        return retval;
    }

    /// <summary>
    /// Given a Color struct with A, R, G, B values make an int ARGB where A is in the high byte and B in the low byte.
    /// </summary>
    public static uint ConvertBackIgnoreA(Color color)
    {
        byte a = 0xFF; // would be color.A, but that's set to 0???
        uint retval = ((uint)a << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | ((uint)color.B << 0);
        return retval;
    }

    /// <summary>
    /// Given a SaveData type color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static SolidColorBrush GetBrush(uint color)
    {
        if (color == DeviceColors.ColorIsDefault) return default;
        return new SolidColorBrush(ConvertIgnoreA(color));
    }

    public static void Recolor(this WriteableBitmap bmp, Color value)
    {
        var stream = bmp.PixelBuffer.AsStream();
        byte[] pixels = new byte[stream.Length]; // Must copy all the bytes for no good reason.
        var realLength = stream.Read(pixels, 0, pixels.Length);
        if (realLength != pixels.Length)
        {
            // Can never fail in reality
            return;
        }
        byte transparentA = 0;
        for (int i = 0; i < realLength; i += 4)
        {
            byte b = pixels[i + 0];
            byte g = pixels[i + 1];
            byte r = pixels[i + 2];
            byte a = pixels[i + 3];
            if (i == 0)
            {
                transparentA = a;
            }
            if (a != transparentA)
            {
                pixels[i + 0] = value.B;
                pixels[i + 1] = value.G;
                pixels[i + 2] = value.R;
            }
        }
        // Write it back.
        stream.Position = 0;
        stream.Write(pixels, 0, realLength);
    }


}
